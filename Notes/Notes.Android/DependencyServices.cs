using Android.Content;
using Android.Content.PM;
using Xamarin.Forms;

[assembly: Dependency(typeof(Notes.Droid.FileSystemImplementation))]
namespace Notes.Droid
{
    public class FileSystemImplementation : IDeviceFileSystem
    {
        public string GetExternalStoragePath()
        {
            return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath; // depreceated, but this used to be what i needed for it to work

            //Context context = Android.App.Application.Context;
            //var filePath = context.getex//.GetExternalFilesDir(null); // null for root of files directory
            //return filePath.AbsolutePath;
        }
    }
}