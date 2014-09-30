using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using FileHelpers;

namespace nCode.Data
{
    /// <summary>
    /// 
    /// </summary>
    public interface  IFileImportManager
    {
        bool ShowDelimiterOption { get; }
        bool ShowHeaderOption { get; }
    }
}
