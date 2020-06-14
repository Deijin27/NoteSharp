using Android.Content;
using Xamarin.Forms;

[assembly: Dependency(typeof(Notes.Droid.FileSystemImplementation))]
namespace Notes.Droid
{
    public class FileSystemImplementation : IFileSystem
    {
        public string GetExternalStoragePath()
        {
            return Android.OS.Environment.ExternalStorageDirectory.AbsolutePath;
            //Context context = Android.App.Application.Context;
            //var filePath = context.GetExternalFilesDir("");
            //return filePath.Path;
        }
    }
}