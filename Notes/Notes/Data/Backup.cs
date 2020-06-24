using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Notes.Models;
using Notes.Constants;
using System.Text.Json;
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
                await page.DisplayAlert("Backups Folder is Empty", "Try the \"Create Backup\" option from the settings page", "OK");
                return null;
            }

            // I should use something other than an action sheet, but this works for now.
            string result = await page.DisplayActionSheet("Choose Backup File", ActionSheetOption.Cancel, null, backupFiles);

            if (string.IsNullOrEmpty(result) || result == ActionSheetOption.Cancel) return null;

            return Path.Combine(backupFolderPath, result);
        }

        private static async Task<bool> RestoreBackupDatabase(string chosenBackupPath, Page page)
        {
            if (!NoteDatabase.IsValid(chosenBackupPath))
            {
                await page.DisplayAlert("Unable to Load Backup", $"An error was encountered loading chosen backup file", "OK");
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
            PermissionStatus status = await CheckAndRequestStorageWritePermission();
            if (status != PermissionStatus.Granted)
            {
                await page.DisplayAlert("Permission Denied", "Unable to create backup because permission to write to storage was denied.", "OK");
                return false;
            }
            
            string option = await page.DisplayActionSheet
            (
                "Backup to file format:",
                ActionSheetOption.Cancel,
                null,
                ActionSheetOption.SQLite3,
                ActionSheetOption.JSON
            );

            string backupPath;
            switch (option)
            {
                case ActionSheetOption.SQLite3:
                    backupPath = await CreateBackupDatabase();
                    break;
                case ActionSheetOption.JSON:
                    backupPath = await CreateBackupJson();
                    break;
                case ActionSheetOption.Cancel:
                default: // this includes the null when user clicks surroundings
                    return false;
            }
            await page.DisplayAlert("Backup Complete", $"It can be found at: {backupPath}", "OK");
            return true;
            
        }

        private static async Task<string> QueryBackupExisting(Page page)
        {
            string option;

            bool certain = false;
            do
            {
                option = await page.DisplayActionSheet
                (
                    "Do what with existing notes, folders and style sheets?",
                    ActionSheetOption.Cancel,
                    ActionSheetOption.DeletePermanently,
                    ActionSheetOption.CreateBackup
                );

                if (option == ActionSheetOption.DeletePermanently)
                {
                    certain = await page.DisplayAlert
                    (
                        "Are you sure?",
                        "All existing notes, folders, and style sheets will be deleted",
                        "Yes, Delete!",
                        "Wait, NO!"
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
            string backupExistingOption = await QueryBackupExisting(page);
            switch (backupExistingOption)
            {
                case ActionSheetOption.DeletePermanently:
                    break;
                case ActionSheetOption.CreateBackup:
                    bool backupSuccessful = await GetPermissionAndCreateBackup(page);
                    if (!backupSuccessful) return;
                    break;
                case ActionSheetOption.Cancel:
                default: // includes null when user clicks surroundings
                    return;
            }

            PermissionStatus status = await CheckAndRequestStorageReadPermission();
            if (status != PermissionStatus.Granted)
            {
                await page.DisplayAlert("Permission Denied", "Unable to restore backup because permission to read from storage was denied.", "OK");
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
                await page.DisplayAlert("Backup Restored", $"The backup was restored successfully.", "OK");
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