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

        private async Task<string> CreateBackup(string externalStoragePath)
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

        private async Task GetPermissionAndCreateBackup()
        {
            string externalStoragePath = DependencyService.Get<IFileSystem>().GetExternalStoragePath();
            if (externalStoragePath == null)
            {
                await DisplayAlert("Backup Not Available", "Sorry, backup is currently unavailable on this device", "OK");
            }
            else
            {
                var status = await CheckAndRequestStorageWritePermission();
                if (status != PermissionStatus.Granted)
                {
                    await DisplayAlert("Permission Denied", "Unable to create backup because permission to write to storage was denied.", "OK");
                }
                else
                {
                    string path = await CreateBackup(externalStoragePath);
                    await DisplayAlert("Backup Complete", $"It can be found at: {path}", "OK");
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

        private async void CreateBackupButton_Clicked(object sender, EventArgs e)
        {
            await GetPermissionAndCreateBackup();
        }
    }
}