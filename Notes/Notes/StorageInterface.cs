﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Notes
{
    public interface IFileSystem
    {
        string GetExternalStoragePath();
    }
}
