using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;

namespace nCode.Data
{
    /// <summary>
    /// Represent a Import Manager, that can Import the given Objects into the system.
    /// </summary>
    public abstract class ImportManager : IDisposable
    {
        public ImportManager()
        {

        }

        public string ObjectName { get; protected set; }
    
        public int Inserted { get; protected set; }

        public int Updated { get; protected set; }

        public int Deleted { get; protected set; }

        public abstract void Import(IDictionary<string, object> options);

        public abstract void Commit();

        public abstract void Dispose();
    }
}
