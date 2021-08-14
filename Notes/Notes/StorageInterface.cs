using System;
using System.Collections.Generic;
using System.Text;

namespace Notes
{
    public interface IDeviceFileSystem
    {
        string GetExternalStoragePath();
    }
}
