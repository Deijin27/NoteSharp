using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Notes.Models;
using Notes.Resources;
using Rg.Plugins.Popup.Services;
using Notes.PopupPages;

namespace Notes.Data
{
    /// <summary>
    /// An object that contains notes, folders, and sheets; suitable to be 
    /// serialized to a single json file.
    /// </summary>
    class JsonBackupSerializable
    {
        public List<Note> Notes { get; set; }
        public List<Folder> Folders { get; set; }
        public List<CSS> Sheets { get; set; }
    }

    public enum BackupOption
    {
        SQLite3,
        JSON
    }

    static class Backup
    {
        public delegate void BackupRestoredEventHandler();
        static public BackupRestoredEventHandler BackupRestored;

        private static string BackupFolderPath => Path.Combine(App.ExternalStoragePath, "NoteSharp", "Backups");

        /// <summary>
        /// Generate a backup file path ensuring that a file doesn't already exist there.
        /// </summary>
        /// <param name="fileExtension"></param>
        /// <returns></returns>
        private static string NewBackupFilePath(string fileExtension)
        {
            string backupFolder = BackupFolderPath;
            string newBackupFileName = $"Backup {DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss")}";
            string newBackupFileTry = Path.Combine(backupFolder, newBackupFileName);
            string newBackupFile = newBackupFileTry + fileExtension;
            if (File.Exists(newBackupFile))
            {
                int count = 1;
                do
                {
                    newBackupFile = $"{newBackupFileTry} [{count++}]{fileExtension}";
                }
                while (File.Exists(newBackupFile));
            }
            
            return newBackupFile;
        }


        const string SqliteExt = ".sqlite3";

        private static async Task<string> CreateBackupDatabase()
        {
            Directory.CreateDirectory(BackupFolderPath);
            string newBackupFile = NewBackupFilePath(SqliteExt);
            await App.Database.BackupAsync(newBackupFile);
            return newBackupFile;
        }

        private static async void ChooseBackupFile()
        {
            
            var backupFiles = Directory.GetFiles(BackupFolderPath)
                                       .Select(Path.GetFileName)
                                       .Where(i => Path.GetExtension(i) == SqliteExt)
                                       .Select(i => new ListPopupPageItem { Name = Path.GetFileNameWithoutExtension(i), AssociatedObject = i})
                                       .ToList();

            if (backupFiles.Count == 0)
            {
                await PopupNavigation.Instance.PushAsync(new AlertPopupPage
                (
                    AppResources.Alert_BackupsFolderEmpty_Title, 
                    AppResources.Alert_BackupsFolderEmpty_Message, 
                    AppResources.AlertOption_OK
                ));
            }

            
            // I should use something other than an action sheet, but this works for now.
            var popup = new ListPopupPage
            (
                AppResources.ActionSheetTitle_ChooseBackupFile, 
                "Pick a backup file to restore.",
                AppResources.ActionSheetOption_Cancel, 
                backupFiles
            );
            popup.CancelClicked += CancelBackupRestore;
            popup.BackgroundClicked += CancelBackupRestore;
            popup.HardwareBackClicked += CancelBackupRestore;
            popup.ListOptionClicked += ProceedBackupRestore;

            await PopupNavigation.Instance.PushAsync(popup);
        }

        static async void CancelBackupRestore() { await PopupNavigation.Instance.PopAsync(); }

        static async void ProceedBackupRestore(ListPopupPageItem selectedItem)
        {
            await PopupNavigation.Instance.PopAsync();
            string chosenBackupPath = Path.Combine(BackupFolderPath, (string)selectedItem.AssociatedObject);
            bool restoreSuccessful = await RestoreBackupDatabase(chosenBackupPath);

            if (restoreSuccessful)
            {
                await PopupNavigation.Instance.PushAsync(new AlertPopupPage
                (
                    AppResources.Alert_BackupRestoreComplete_Title,
                    AppResources.Alert_BackupRestoreComplete_Message,
                    AppResources.AlertOption_OK
                ));
                BackupRestored?.Invoke();
            }
        }

        private static async Task<bool> RestoreBackupDatabase(string chosenBackupPath)
        {
            if (!NoteDatabase.IsValid(chosenBackupPath))
            {
                await PopupNavigation.Instance.PushAsync(new AlertPopupPage
                (
                    AppResources.Alert_BackupRestoreFailed_Title,
                    AppResources.Alert_BackupRestoreFailed_Message + $" ({chosenBackupPath})",
                    AppResources.AlertOption_OK
                ));

                return false;
            }

            string databasePath = App.DatabasePath;
            await App.Database.CloseAsync(); // first close database connections (maybe unnecessary idk)
            File.Delete(databasePath); // delete existing database
            File.Copy(chosenBackupPath, databasePath); // replace with chosen database
            App.ResetDatabase(); // then reload the database path
            return true;
        }

        

        public static async Task<bool> GetPermissionAndCreateBackup()
        {
            try
            {
                PermissionStatus status = await CheckAndRequestStorageWritePermission();
                if (status != PermissionStatus.Granted)
                {
                    await PopupNavigation.Instance.PushAsync(new AlertPopupPage
                    (
                        AppResources.Alert_CreateBackupPermissionDenied_Title,
                        AppResources.Alert_CreateBackupPermissionDenied_Message,
                        AppResources.AlertOption_OK
                    ));
                    return false;
                }

                // new stuff
                string backupPath = await CreateBackupDatabase();

                await PopupNavigation.Instance.PushAsync(new AlertPopupPage
                (
                    AppResources.Alert_BackupComplete_Title,
                    AppResources.Alert_BackupComplete_Message + "\n" + backupPath,
                    AppResources.AlertOption_OK
                ));

                return true;
            }
            catch (Exception e)
            {
                await PopupNavigation.Instance.PushAsync(new AlertPopupPage("Error Encountered", e.Message, "OK"));
                return false;
            }

        }

        public static async void GetPermissionAndRestoreBackup()
        {
            var popup = new TwoOptionPopupPage
            (
                "Backup Existing?",
                "If you don't, any notes, folders or style sheets you have currently will be lost.",
                "Backup",
                "Delete"
            );
            popup.BackgroundClicked += CancelBackupRestore;
            popup.HardwareBackClicked += CancelBackupRestore;
            popup.LeftOptionClicked += ProceedBackupExistingThenRestore;
            popup.RightOptionClicked += ProceedRestoreWithoutBackupExisting;

            await PopupNavigation.Instance.PushAsync(popup);
        }

        public static async void ProceedBackupExistingThenRestore()
        {
            await PopupNavigation.Instance.PopAsync();
            try
            {
                PermissionStatus status = await CheckAndRequestStorageWritePermission();
                if (status != PermissionStatus.Granted)
                {
                    await PopupNavigation.Instance.PushAsync(new AlertPopupPage
                    (
                        AppResources.Alert_CreateBackupPermissionDenied_Title,
                        AppResources.Alert_CreateBackupPermissionDenied_Message,
                        AppResources.AlertOption_OK
                    ));
                }
                else
                {
                    await CreateBackupDatabase();
                    ProceedRestore();
                }
            }
            catch (Exception e)
            {
                await PopupNavigation.Instance.PushAsync(new AlertPopupPage("Error Encountered", e.Message, "OK"));
            }
        }

        public static async void ProceedRestoreWithoutBackupExisting()
        {
            await PopupNavigation.Instance.PopAsync();
            ProceedRestore();
        }

        public static async void ProceedRestore()
        {
            
            PermissionStatus status = await CheckAndRequestStorageReadPermission();
            if (status != PermissionStatus.Granted)
            {
                await PopupNavigation.Instance.PushAsync(new AlertPopupPage
                (
                    AppResources.Alert_RestoreBackupPermissionDenied_Title,
                    AppResources.Alert_RestoreBackupPermissionDenied_Message,
                    AppResources.AlertOption_OK
                ));
            }
            else
            {
                ChooseBackupFile();
            }
        }

        public static async Task<PermissionStatus> CheckAndRequestStorageWritePermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            }

            // Additionally could prompt the user to turn on in settings

            return status;
        }

        public static async Task<PermissionStatus> CheckAndRequestStorageReadPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageRead>();
            }

            // Additionally could prompt the user to turn on in settings

            return status;
        }

    }
}