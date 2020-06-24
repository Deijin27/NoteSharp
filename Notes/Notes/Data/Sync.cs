using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xamarin.Essentials;
using System.Text.Json;

namespace Notes.Data
{
    class SyncInfo
    {
        public Guid SyncID { get; set; }
        public bool SyncInProgress { get; set; }
        public DateTime StartedAt { get; set; }
    }

    static class Sync
    {
        #region Temporary stuff to emulate cloud storage prior to its implementation
        private static string CloudFolderPath => Path.Combine(App.ExternalStoragePath, "NoteSharp", "Cloud");

        private static string LocalFolderPath => Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        private static string LocalCloudDatabasePath => Path.Combine(LocalFolderPath, "SyncDatabase.db3");
        //private static string LocalSyncInfoPath => Path.Combine(LocalFolderPath, "SyncInfo");
        private static string CloudDatabasePath => Path.Combine(CloudFolderPath, "SyncDatabase.db3");
        private static string CloudSyncInfoPath => Path.Combine(CloudFolderPath, "SyncInfo.json");
        
        private static SyncInfo CloudSyncInfo
        {
            get
            {
                return JsonSerializer.Deserialize<SyncInfo>(File.ReadAllText(CloudSyncInfoPath));
            }
            set
            {
                File.WriteAllText(CloudSyncInfoPath, JsonSerializer.Serialize(value));
            }
        }

        private static async Task<SyncInfo> GetCloudDatabase()
        {
            PermissionStatus status = await Backup.CheckAndRequestStorageReadPermission();
            if (status != PermissionStatus.Granted)
            {
                return null;
            }
            PermissionStatus status2 = await Backup.CheckAndRequestStorageWritePermission();
            if (status2 != PermissionStatus.Granted)
            {
                return null;
            }

            SyncInfo initialSyncInfo = CloudSyncInfo;
            Thread.Sleep(1000); // simulate time to download sync info
            if (initialSyncInfo.SyncInProgress && (DateTime.Now - initialSyncInfo.StartedAt < TimeSpan.FromMinutes(5)))
            {
                return null;
            }
            initialSyncInfo = new SyncInfo
            {
                SyncID = Guid.NewGuid(),
                SyncInProgress = true,
                StartedAt = DateTime.Now
            };
            CloudSyncInfo = initialSyncInfo;
            Thread.Sleep(1000); // simulate time to upload sync info
            File.Copy(CloudDatabasePath, LocalCloudDatabasePath);

            Thread.Sleep(2000); // simulate time to download database

            return initialSyncInfo;
        }

        private static bool CommitCloudDatabase(SyncInfo initialSyncInfo)
        {
            SyncInfo finalSyncInfo = CloudSyncInfo;
            Thread.Sleep(1000); // simulate time to download sync info

            if (finalSyncInfo.SyncID != initialSyncInfo.SyncID) return false;

            File.Delete(CloudDatabasePath);
            File.Move(LocalCloudDatabasePath, CloudDatabasePath);
            Thread.Sleep(2000); // simulate time to download database

            finalSyncInfo.SyncInProgress = false;
            CloudSyncInfo = finalSyncInfo;

            return true;
        }

        #endregion 
    }
}
