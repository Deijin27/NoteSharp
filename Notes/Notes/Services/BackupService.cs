using System;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Notes.Models;
using Notes.Resources;

namespace Notes.Services
{
    public class BackupService : IBackupService
    {
        readonly IAppServiceProvider Services;
        public BackupService(IAppServiceProvider services)
        {
            Services = services;
        }

        private string BackupFolderPath => Services.FileSystem.Path.Combine(Services.ExternalStoragePath, "NoteSharp", "Backups");

        /// <summary>
        /// Generate a backup file path ensuring that a file doesn't already exist there.
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        private string NewBackupFilePath(string fileExtension)
        {
            string backupFolder = BackupFolderPath;
            string newBackupFileName = $"Backup {DateTime.UtcNow:yyyy-MM-dd-HH-mm-ss}";
            string newBackupFileTry = Services.FileSystem.Path.Combine(backupFolder, newBackupFileName);
            string newBackupFile = newBackupFileTry + fileExtension;
            if (Services.FileSystem.File.Exists(newBackupFile))
            {
                int count = 1;
                do
                {
                    newBackupFile = $"{newBackupFileTry} [{count++}]{fileExtension}";
                }
                while (Services.FileSystem.File.Exists(newBackupFile));
            }

            return newBackupFile;
        }


        const string SqliteExt = ".sqlite3";

        private async Task<string> CreateBackupDatabase()
        {
            Services.FileSystem.Directory.CreateDirectory(BackupFolderPath);
            string newBackupFile = NewBackupFilePath(SqliteExt);
            await Services.NoteDatabase.BackupAsync(newBackupFile);
            return newBackupFile;
        }

        private async void ChooseBackupFile()
        {

            var backupFiles = Services.FileSystem.Directory.GetFiles(BackupFolderPath)
                                       .Select(Services.FileSystem.Path.GetFileName)
                                       .Where(i => Services.FileSystem.Path.GetExtension(i) == SqliteExt)
                                       .Select(i => new ListPopupPageItem<string> { Name = Services.FileSystem.Path.GetFileNameWithoutExtension(i), AssociatedObject = i })
                                       .ToList();

            if (backupFiles.Count == 0)
            {
                await Services.Popups.AlertPopup
                (
                    AppResources.Alert_BackupsFolderEmpty_Title,
                    AppResources.Alert_BackupsFolderEmpty_Message,
                    AppResources.AlertOption_OK
                );
            }
            else
            {
                var result = await Services.Popups.ListPopup
                (
                    AppResources.ActionSheetTitle_ChooseBackupFile,
                    "Pick a backup file to restore.",
                    AppResources.ActionSheetOption_Cancel,
                    backupFiles
                );
                if (result.Choice == ListPopupResult.ListItem)
                {
                    string chosenBackupPath = Services.FileSystem.Path.Combine(BackupFolderPath, result.SelectedItem);
                    bool restoreSuccessful = await RestoreBackupDatabase(chosenBackupPath);

                    if (restoreSuccessful)
                    {
                        await Services.Popups.AlertPopup
                        (
                            AppResources.Alert_BackupRestoreComplete_Title,
                            AppResources.Alert_BackupRestoreComplete_Message,
                            AppResources.AlertOption_OK
                        );
                    }
                }
            }
            
        }


        private async Task<bool> RestoreBackupDatabase(string chosenBackupPath)
        {
            if (!Services.NoteDatabase.ValidateDatabaseAtPath(chosenBackupPath))
            {
                await Services.Popups.AlertPopup
                (
                    AppResources.Alert_BackupRestoreFailed_Title,
                    AppResources.Alert_BackupRestoreFailed_Message + $" ({chosenBackupPath})",
                    AppResources.AlertOption_OK
                );

                return false;
            }

            string databasePath = Services.DatabasePath;
            await Services.NoteDatabase.CloseAsync(); // first close database connections (maybe unnecessary idk)
            Services.FileSystem.File.Delete(databasePath); // delete existing database
            Services.FileSystem.File.Copy(chosenBackupPath, databasePath); // replace with chosen database
            Services.ResetDatabase(); // then reload the database path
            return true;
        }



        public async Task<bool> GetPermissionAndCreateBackup()
        {
            try
            {
                PermissionStatus status = await Services.Permissions.CheckAndRequestStorageWritePermission();
                if (status != PermissionStatus.Granted)
                {
                    await Services.Popups.AlertPopup
                    (
                        AppResources.Alert_CreateBackupPermissionDenied_Title,
                        AppResources.Alert_CreateBackupPermissionDenied_Message,
                        AppResources.AlertOption_OK
                    );
                    return false;
                }

                // new stuff
                string backupPath = await CreateBackupDatabase();

                await Services.Popups.AlertPopup
                (
                    AppResources.Alert_BackupComplete_Title,
                    AppResources.Alert_BackupComplete_Message + "\n" + backupPath,
                    AppResources.AlertOption_OK
                );

                return true;
            }
            catch (Exception e)
            {
                await Services.Popups.AlertPopup("Error Encountered", e.Message, "OK");
                return false;
            }
        }

        public async Task GetPermissionAndRestoreBackup()
        {
            var result = await Services.Popups.TwoOptionPopup
            (
                "Backup Existing?",
                "If you don't, any notes, folders or style sheets you have currently will be lost.",
                "Backup",
                "Delete"
            );

            bool backupSuccessfulOrSkipped = true;

            if (result == TwoOptionPopupResult.LeftButton)
            {
                backupSuccessfulOrSkipped = await GetPermissionAndCreateBackup();
            }

            if (backupSuccessfulOrSkipped)
            {
                PermissionStatus status = await Services.Permissions.CheckAndRequestStorageReadPermission();
                if (status != PermissionStatus.Granted)
                {
                    await Services.Popups.AlertPopup
                    (
                        AppResources.Alert_RestoreBackupPermissionDenied_Title,
                        AppResources.Alert_RestoreBackupPermissionDenied_Message,
                        AppResources.AlertOption_OK
                    );
                }
                else
                {
                    ChooseBackupFile();
                }
            }
        }
    }
}
