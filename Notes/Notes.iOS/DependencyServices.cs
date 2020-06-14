using Xamarin.Forms;

[assembly: Dependency(typeof(Notes.IOS.FileSystemImplementation))]
namespace Notes.IOS
{
    public class FileSystemImplementation : IFileSystem
    {
        public string GetExternalStoragePath() // TODO
        {
            return null;
            //Context context = Android.App.Application.Context;
            //var filePath = context.GetExternalFilesDir("");
            //return filePath.Path;
        }
    }
}