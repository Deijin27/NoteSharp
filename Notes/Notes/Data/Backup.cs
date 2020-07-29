using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Notes.Models;
using System.Text.Json;
using Notes.Resources;
using Rg.Plugins.Popup.Services;
using Notes.PopupPages;
//using Newtonsoft.Json;

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

        private static async Task<string> CreateBackupJson()
        {
            Directory.CreateDirectory(BackupFolderPath);
            string newBackupFile = NewBackupFilePath(".json");

            // Ideally I can in the future find some way to do this without having to load
            // everything into memory at once.
            var serializable = new JsonBackupSerializable
            {
                Sheets = await App.Database.GetAllSheetsAsync(),
                Folders = await App.Database.GetAllFoldersAsync(),
                Notes = await App.Database.GetAllNotesAsync()
            };

            using (FileStream fs = File.Create(newBackupFile))
            {
                await JsonSerializer.SerializeAsync(fs, serializable);
            }

            return newBackupFile;
        }

        /*private static async Task<string> CreateBackupJsonWithoutLoatingEverythingAtOnce()
        {
            string newBackupFile = NewBackupFilePath(".json");

            var db = App.Database;

            using (FileStream fs = File.Create(newBackupFile))
            using (StreamWriter writer = new StreamWriter(fs))
            {
                // Notes
                writer.Write("{\"Notes\":[");

                int noteCount = await db.CountNotesAsync();

                for (int i = 0; i < noteCount; i++)
                {
                    Note note = await db.NoteAtIndexAsync(i);
                    string snote = JsonSerializer.Serialize(note);
                    writer.Write(snote);
                    if (i != noteCount - 1)
                    {
                        writer.Write(",");
                    }
                }

                // Folders
                writer.Write("],\"Folders\":[");

                
            }

            return newBackupFile;
        }*/

        private static async Task<string> CreateBackupDatabase()
        {
            Directory.CreateDirectory(BackupFolderPath);
            string newBackupFile = NewBackupFilePath(".sqlite3");
            await App.Database.BackupAsync(newBackupFile);
            return newBackupFile;
        }


        private static List<string> JsonFileExtensions = new List<string>
        {
            ".json"
        };

        private static List<string> SQLite3FileExtensions = new List<string>
        {
            ".db",
            ".sqlite",
            ".sqlite3"
        };

        private static IEnumerable<string> ValidBackupFileExtensions => JsonFileExtensions.Concat(SQLite3FileExtensions);

        private static async Task<string> ChooseBackupFile(Page page)
        {
            string backupFolderPath = BackupFolderPath;

            var backupFiles = Directory.GetFiles(backupFolderPath)
                                       .Select(Path.GetFileName)
                                       .Where(i => ValidBackupFileExtensions.Contains(Path.GetExtension(i)))
                                       .ToArray();

            if (backupFiles.Length == 0)
            {
                await page.DisplayAlert
                (
                    AppResources.Alert_BackupsFolderEmpty_Title, 
                    AppResources.Alert_BackupsFolderEmpty_Message, 
                    AppResources.AlertOption_OK
                );
                return null;
            }

            string option_cancel = AppResources.ActionSheetOption_Cancel;
            // I should use something other than an action sheet, but this works for now.
            string result = await page.DisplayActionSheet
            (
                AppResources.ActionSheetTitle_ChooseBackupFile, 
                option_cancel, 
                null, 
                backupFiles
            );

            if (string.IsNullOrEmpty(result) || result == option_cancel) return null;

            return Path.Combine(backupFolderPath, result);
        }

        private static async Task<bool> RestoreBackupDatabase(string chosenBackupPath, Page page)
        {
            if (!NoteDatabase.IsValid(chosenBackupPath))
            {
                await page.DisplayAlert
                (
                    AppResources.Alert_BackupRestoreFailed_Title, 
                    AppResources.Alert_BackupRestoreFailed_Message, 
                    AppResources.AlertOption_OK
                );

                return false;
            }

            string databasePath = App.DatabasePath;
            await App.Database.CloseAsync(); // first close database connections (maybe unnecessary idk)
            File.Delete(databasePath); // delete existing database
            File.Copy(chosenBackupPath, databasePath); // replace with chosen database
            App.ResetDatabase(); // then reload the database path
            return true;
        }

        private static async Task<bool> RestoreBackupJson(string chosenBackupPath, Page page)
        {
            JsonBackupSerializable deserialized;
            try
            {
                using (FileStream fs = File.OpenRead(chosenBackupPath))
                {
                    deserialized = await JsonSerializer.DeserializeAsync<JsonBackupSerializable>(fs);
                }
            }
            catch (Exception)
            {
                return false;
            }

            NoteDatabase db = App.Database;
            await db.DeleteAllAsync();
            
            if (deserialized.Notes != null)
            {
                await db.InsertAllAsync(deserialized.Notes);
            }
            if (deserialized.Folders != null)
            {
                await db.InsertAllAsync(deserialized.Folders);
            }
            if (deserialized.Sheets != null)
            {
                await db.InsertAllAsync(deserialized.Sheets);
            }

            return true;
        }

        public static async Task<bool> GetPermissionAndCreateBackup(Page page)
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

                var popup = new ListPopupPage
                (
                    AppResources.ActionSheetTitle_ChooseBackupFileFormat,
                    "The database will be exported to your local storage in the selected file format",
                    AppResources.ActionSheetOption_Cancel,
                    new List<ListPopupPageItem>
                    {
                        new ListPopupPageItem { Name = "SQLite3", AssociatedObject = BackupOption.SQLite3 },
                        new ListPopupPageItem { Name = "JSON", AssociatedObject = BackupOption.JSON }
                    }
                );
                popup.CancelClicked += ClosePopupCancelBackup;
                popup.BackgroundClicked += ClosePopupCancelBackup;
                popup.HardwareBackClicked += ClosePopupCancelBackup;
                popup.ListOptionClicked += ClosePopupProceedWithBackup;

                await PopupNavigation.Instance.PushAsync(popup);

                return true;
            }
            catch (Exception e)
            {
                await PopupNavigation.Instance.PushAsync(new AlertPopupPage("Error Encountered", e.Message, "OK"));
                return false;
            }

        }

        private async static void ClosePopupProceedWithBackup(ListPopupPageItem selectedMode)
        {
            
            await PopupNavigation.Instance.PopAsync();

            // push loading popup here when i've created it

            string backupPath;
            switch ((BackupOption)selectedMode.AssociatedObject)
            {
                case BackupOption.JSON:
                    backupPath = await CreateBackupJson();
                    break;
                case BackupOption.SQLite3:
                    backupPath = await CreateBackupDatabase();
                    break;
                default:
                    throw new Exception("BackupOption not recognised (ClosePopupProceedWithBackup)");
            }

            
            await PopupNavigation.Instance.PushAsync(new AlertPopupPage
            (
                AppResources.Alert_BackupComplete_Title,
                AppResources.Alert_BackupComplete_Message + backupPath,
                AppResources.AlertOption_OK
            ));
        }

        private static void ClosePopupCancelBackup()
        {
            PopupNavigation.Instance.PopAsync();
        }

        private static async Task<string> QueryBackupExisting(Page page, string option_cancel, string option_deletePermanently, string option_createBackup)
        {
            string option;

            bool certain = false;
            do
            {
                option = await page.DisplayActionSheet
                (
                    AppResources.ActionSheetTitle_QueryBackupExisting,
                    option_cancel,
                    option_deletePermanently,
                    option_createBackup
                );

                if (option == option_deletePermanently)
                {
                    certain = await page.DisplayAlert
                    (
                        AppResources.Alert_ConfirmDeleteExisting_Title,
                        AppResources.Alert_ConfirmDeleteExisting_Message,
                        AppResources.Alert_ConfirmDeleteExisting_OptionDelete,
                        AppResources.Alert_ConfirmDeleteExisting_OptionCancel
                    );
                }
                else
                {
                    certain = true;
                }
            }
            while (!certain);

            return option;
        }

        public static async Task GetPermissionAndRestoreBackup(Page page)
        {
            string option_cancel = AppResources.ActionSheetOption_Cancel;
            string option_deletePermanently = AppResources.ActionSheetOption_LocalBackup_DeletePermanently;
            string option_createBackup = AppResources.ActionSheetOption_LocalBackup_CreateBackup;

            string backupExistingOption = await QueryBackupExisting(page, option_cancel, option_deletePermanently, option_createBackup);
            if (backupExistingOption == option_createBackup)
            {
                bool backupSuccessful = await GetPermissionAndCreateBackup(page);
                if (!backupSuccessful) return;
            }
            else if (backupExistingOption == option_cancel || string.IsNullOrEmpty(backupExistingOption))
            {
                return;
            }

            PermissionStatus status = await CheckAndRequestStorageReadPermission();
            if (status != PermissionStatus.Granted)
            {
                await page.DisplayAlert
                (
                    AppResources.Alert_RestoreBackupPermissionDenied_Title, 
                    AppResources.Alert_RestoreBackupPermissionDenied_Message, 
                    AppResources.AlertOption_OK
                );

                return;
            }

            string chosenBackupPath = await ChooseBackupFile(page);

            if (chosenBackupPath == null) return;

            bool restoreSuccessful;
            string fileExt = Path.GetExtension(chosenBackupPath);
            if (JsonFileExtensions.Contains(fileExt))
            {
                restoreSuccessful = await RestoreBackupJson(chosenBackupPath, page);
            }
            else
            {
                restoreSuccessful = await RestoreBackupDatabase(chosenBackupPath, page);
            }

            if (restoreSuccessful)
            {
                await page.DisplayAlert
                (
                    AppResources.Alert_BackupRestoreComplete_Title, 
                    AppResources.Alert_BackupRestoreComplete_Message, 
                    AppResources.AlertOption_OK
                );
            }
            return;
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