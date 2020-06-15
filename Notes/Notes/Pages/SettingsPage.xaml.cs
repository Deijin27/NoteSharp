using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using Notes.Themes;
using Xamarin.Essentials;
using System.IO;
using Notes.Models;
using System.Threading;
using System.Text.Json;
using System.Runtime.CompilerServices;
using Notes.Data;

namespace Notes.Pages
{
    public partial class SettingsPage : ContentPage
    {
        public SettingsPage()
        {
            InitializeComponent();

            if (App.Theme == AppTheme.Dark) ThemeSwitch.IsToggled = true;
            else ThemeSwitch.IsToggled = false;

            SpellCheckSwitch.IsToggled = App.IsSpellCheckEnabled;
        }

        private void ThemeSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            if (e.Value)
            {
                App.Theme = AppTheme.Dark;
            }
            else
            {
                App.Theme = AppTheme.Light;
            }
        }

        private void SpellCheckSwitch_Toggled(object sender, ToggledEventArgs e)
        {
            App.IsSpellCheckEnabled = e.Value;
        }

        /*
        private async Task<string> CreateBackupJson(string externalStoragePath)
        {
            string appFolder = "NoteSharp";
            string backupsSubFolder = "Backups";
            string backupFolderLocation = Path.Combine(externalStoragePath, appFolder, backupsSubFolder);
            Directory.CreateDirectory(backupFolderLocation); // creates unless it exists already

            string newBackupFolderName = $"backup {DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-fff")}";
            string newBackupFolderTry = Path.Combine(backupFolderLocation, newBackupFolderName);
            string newBackupFolder = string.Copy(newBackupFolderTry);
            int count = 1;
            while (Directory.Exists(newBackupFolder)) // it almost certainly won't, but i'll do this anyway
            {
                newBackupFolder = newBackupFolderTry + $"[{count++}]";
            }
            Directory.CreateDirectory(newBackupFolder);

            // should probably change the serialization to async using a file stream
            string noteBackupFile = Path.Combine(newBackupFolder, "Notes.json");
            List<Note> allNotes = await App.Database.GetAllNotesAsync();
            string serializedNotes = JsonSerializer.Serialize(allNotes);
            File.WriteAllText(noteBackupFile, serializedNotes);

            string folderBackupFile = Path.Combine(newBackupFolder, "Folders.json");
            List<Folder> allFolders = await App.Database.GetAllFoldersAsync();
            string serializedFolders = JsonSerializer.Serialize(allFolders);
            File.WriteAllText(folderBackupFile, serializedFolders);

            string styleSheetBackupFile = Path.Combine(newBackupFolder, "StyleSheets.json");
            List<CSS> allStyleSheets = await App.Database.GetAllSheetsAsync();
            string serializedStyleSheets = JsonSerializer.Serialize(allStyleSheets);
            File.WriteAllText(styleSheetBackupFile, serializedStyleSheets);

            return newBackupFolder;
        }
        */
        private async Task<string> CreateBackupDatabase(string externalStoragePath)
        {
            string appFolder = "NoteSharp";
            string backupsSubFolder = "Backups";
            string backupFolderLocation = Path.Combine(externalStoragePath, appFolder, backupsSubFolder);

            string newBackupFileName = $"backup {DateTime.UtcNow.ToString("yyyy-MM-dd-HH-mm-ss-fff")}.sqlite3";
            string newBackupFileTry = Path.Combine(backupFolderLocation, newBackupFileName);
            string newBackupFile = string.Copy(newBackupFileTry);
            int count = 1;
            while (File.Exists(newBackupFile)) // it almost certainly won't, but i'll do this anyway
            {
                newBackupFile = newBackupFileTry + $"[{count++}]";
            }

            await App.Database.BackupAsync(newBackupFile);

            return newBackupFile;
        }

        /*
        private async Task<bool> RestoreBackupJson(string externalStoragePath)
        {
            string appFolder = "NoteSharp";
            string backupsSubFolder = "Backups";
            string backupFolderLocation = Path.Combine(externalStoragePath, appFolder, backupsSubFolder);

            if (!Directory.Exists(backupFolderLocation))
            {
                await DisplayAlert("Backups Folder Doesn't Exist Yet", "Try the \"Create Backup\" option from the settings page", "OK");
                return false;
            }

            var backupFolders = Directory.GetDirectories(backupFolderLocation)
                                         .Select(Path.GetFileName)
                                         .Where(i => i != "Cancel")
                                         .ToArray();

            if (backupFolders.Length == 0)
            {
                await DisplayAlert("Backups Folder is Empty", "Try the \"Create Backup\" option from the settings page", "OK");
                return false;
            }

            // I should use something other than an aciton sheet, but this works for now.
            string result = await DisplayActionSheet("Choose Backup File", "Cancel", null, backupFolders);

            if (result == "Cancel")
            {
                return false;
            }
            
            string chosenBackupPath = Path.Combine(backupFolderLocation, result);

            string noteBackupFile = Path.Combine(chosenBackupPath, "Notes.json");
            string folderBackupFile = Path.Combine(chosenBackupPath, "Folders.json");
            string styleSheetBackupFile = Path.Combine(chosenBackupPath, "StyleSheets.json");

            if (!File.Exists(noteBackupFile))
            {
                await DisplayAlert("Unable to Load Backup", "The selected backup folder does not contain Notes.json", "OK");
                return false;
            }
            if (!File.Exists(folderBackupFile))
            {
                await DisplayAlert("Unable to Load Backup", "The selected backup folder does not contain Folders.json", "OK");
                return false;
            }
            if (!File.Exists(styleSheetBackupFile))
            {
                await DisplayAlert("Unable to Load Backup", "The selected backup folder does not contain StyleSheets.json", "OK");
                return false;
            }

            List<Note> allNotes;
            List<Folder> allFolders;
            List<CSS> allStyleSheets;
            // I probably shouldn't use error catching here, but I don't know how else to do it.
            string noteJson = File.ReadAllText(noteBackupFile);
            try 
            {
                allNotes = JsonSerializer.Deserialize<List<Note>>(noteJson);
            }
            catch (JsonException e)
            {
                await DisplayActionSheet("Unable to Load Backup", $"Error encountered in Notes.json; details: [{e.Message}]", "OK");
                return false;
            }
            string folderJson = File.ReadAllText(folderBackupFile);
            try
            {
                allFolders = JsonSerializer.Deserialize<List<Folder>>(folderJson);
            }
            catch (JsonException e)
            {
                await DisplayActionSheet("Unable to Load Backup", $"Error encountered in Folders.json; details: [{e.Message}]", "OK");
                return false;
            }
            string styleSheetJson = File.ReadAllText(styleSheetBackupFile);
            try
            {
                allStyleSheets = JsonSerializer.Deserialize<List<CSS>>(styleSheetJson);
            }
            catch (JsonException e)
            {
                await DisplayActionSheet("Unable to Load Backup", $"Error encountered in StyleSheets.json; details: [{e.Message}]", "OK");
                return false;
            }

            await App.Database.DeleteAllAsync();
            await App.Database.PopulateEmpty(allNotes, allFolders, allStyleSheets);
            return true;
        }
        */
        
        
        private async Task<bool> RestoreBackupDatabase(string externalStoragePath)
        {
            string appFolder = "NoteSharp";
            string backupsSubFolder = "Backups";
            string backupFolderLocation = Path.Combine(externalStoragePath, appFolder, backupsSubFolder);

            if (!Directory.Exists(backupFolderLocation))
            {
                await DisplayAlert("Backups Folder Doesn't Exist Yet", "Try the \"Create Backup\" option from the settings page", "OK");
                return false;
            }

            var backupFiles = Directory.GetFiles(backupFolderLocation)
                                         .Select(Path.GetFileName)
                                         .Where(i => i != "Cancel")
                                         .ToArray();

            if (backupFiles.Length == 0)
            {
                await DisplayAlert("Backups Folder is Empty", "Try the \"Create Backup\" option from the settings page", "OK");
                return false;
            }

            // I should use something other than an aciton sheet, but this works for now.
            string result = await DisplayActionSheet("Choose Backup File", "Cancel", null, backupFiles);

            if (result == "Cancel")
            {
                return false;
            }

            string chosenBackupPath = Path.Combine(backupFolderLocation, result);

            if (!NoteDatabase.IsValid(chosenBackupPath))
            {
                await DisplayAlert("Unable to Load File", $"An error was encountered loading chosen file", "OK");
                return false;
            }

            string databasePath = App.DatabasePath;
            // first close database connections
            await App.Database.CloseAsync();
            File.Delete(databasePath);
            File.Copy(chosenBackupPath, databasePath);
            // then reload the database path
            App.ResetDatabase();
            return true;
        }

        /*
        private async Task<bool> GetPermissionAndCreateBackupJson()
        {
            string externalStoragePath = DependencyService.Get<IFileSystem>().GetExternalStoragePath();
            if (externalStoragePath == null)
            {
                await DisplayAlert("Backup Not Available", "Sorry, backup is currently unavailable on this device", "OK");
                return false;
            }
            else
            {
                var status = await CheckAndRequestStorageWritePermission();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permission Denied", "Unable to create backup because permission to write to storage was denied.", "OK");
                    return false;
                }
                else
                {
                    string path = await CreateBackupJson(externalStoragePath);
                    await DisplayAlert("Backup Complete", $"It can be found at: {path}", "OK");
                    return true;
                }
            }
        }
        */

        private async Task<bool> GetPermissionAndCreateBackupDatabase()
        {
            string externalStoragePath = DependencyService.Get<IFileSystem>().GetExternalStoragePath();
            if (externalStoragePath == null)
            {
                await DisplayAlert("Backup Not Available", "Sorry, backup is currently unavailable on this device", "OK");
                return false;
            }
            else
            {
                var status = await CheckAndRequestStorageWritePermission();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permission Denied", "Unable to create backup because permission to write to storage was denied.", "OK");
                    return false;
                }
                else
                {
                    string path = await CreateBackupDatabase(externalStoragePath);
                    await DisplayAlert("Backup Complete", $"It can be found at: {path}", "OK");
                    return true;
                }
            }
        }
        
        /*
        private async Task GetPermissionAndRestoreBackupJson()
        {
            string externalStoragePath = DependencyService.Get<IFileSystem>().GetExternalStoragePath();
            if (externalStoragePath == null)
            {
                await DisplayAlert("Backup Not Available", "Sorry, backup is currently unavailable on this device", "OK");
            }
            else
            {
                bool backupExisting = await DisplayAlert("Backup Existing First?",
                    "Restoring a backup will overwrite all your existing notes; would you like to back these up first?",
                    "Yes", "No");
                bool dontBackupExisting = false;
                if (!backupExisting)
                {
                    dontBackupExisting = await DisplayAlert("Are you sure?", 
                        "Just checking to make sure you don't accidentally delete your stuff", 
                        "Don't Backup Existing", "Backup Existing");
                }
                bool success = true;
                if (!dontBackupExisting)
                {
                   success = await GetPermissionAndCreateBackupJson();
                   if (!success) return;
                }
                
                var status = await CheckAndRequestStorageReadPermission();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permission Denied", "Unable to create backup because permission to read from storage was denied.", "OK");
                }
                else
                {
                    await RestoreBackupJson(externalStoragePath);
                    await DisplayAlert("Backup Restored", $"The backup was restored successfully.", "OK");
                }
            }
        }
        */
        
        private async Task GetPermissionAndRestoreBackupDatabase()
        {
            string externalStoragePath = DependencyService.Get<IFileSystem>().GetExternalStoragePath();
            if (externalStoragePath == null)
            {
                await DisplayAlert("Backup Not Available", "Sorry, backup is currently unavailable on this device", "OK");
            }
            else
            {
                bool backupExisting = await DisplayAlert("Backup Existing First?",
                    "Restoring a backup will overwrite all your existing notes; would you like to back these up first?",
                    "Yes", "No");
                bool dontBackupExisting = false;
                if (!backupExisting)
                {
                    dontBackupExisting = await DisplayAlert("Are you sure?",
                        "Just checking to make sure you don't accidentally delete your stuff",
                        "Don't Backup Existing", "Backup Existing");
                }
                bool success = true;
                if (!dontBackupExisting)
                {
                    success = await GetPermissionAndCreateBackupDatabase();
                    if (!success) return;
                }

                var status = await CheckAndRequestStorageReadPermission();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permission Denied", "Unable to create backup because permission to read from storage was denied.", "OK");
                }
                else
                {
                    bool restoreSuccessful = await RestoreBackupDatabase(externalStoragePath);
                    if (restoreSuccessful)
                        await DisplayAlert("Backup Restored", $"The backup was restored successfully.", "OK");
                }
            }
        }

        public async Task<PermissionStatus> CheckAndRequestStorageWritePermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            }

            // Additionally could prompt the user to turn on in settings

            return status;
        }

        public async Task<PermissionStatus> CheckAndRequestStorageReadPermission()
        {
            var status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (status != PermissionStatus.Granted)
            {
                status = await Permissions.RequestAsync<Permissions.StorageRead>();
            }

            // Additionally could prompt the user to turn on in settings

            return status;
        }

        private async void CreateBackupButton_Clicked(object sender, EventArgs e)
        {
            await GetPermissionAndCreateBackupDatabase();
        }

        private async void RestoreBackupButton_Clicked(object sender, EventArgs e)
        {
            await GetPermissionAndRestoreBackupDatabase();
        }
    }
}