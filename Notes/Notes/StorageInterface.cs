using System;
using System.Collections.Generic;
using System.Text;

namespace Notes
{
    public interface IFileSystem
    {
        string GetExternalStoragePath();
    }

    public interface IExternalStoragePermission
    {
        bool IsPermissionGranted();


        bool RequestPermission();
    }
}
