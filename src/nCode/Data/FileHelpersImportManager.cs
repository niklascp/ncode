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
    /// <typeparam name="B">FileHelpers boddy-class.</typeparam>
    public abstract class FileHelpersImportManager<B> : ImportManager, IFileImportManager
    {
        public FileHelpersImportManager()
        {
            ShowDelimiterOption = false;
            ShowHeaderOption = false;
        }

        protected FileHelperEngine<B> Engine { get; private set; }

        public override void Import(IDictionary<string, object> options)
        {
            /* Validate Options. */
            if (!options.ContainsKey("dataStream") || !(options["dataStream"] is Stream))
                throw new ArgumentException("Options is expected to contain a 'dataStream' property.", "options");

            if (!options.ContainsKey("encoding") || !(options["encoding"] is Encoding))
                throw new ArgumentException("Options is expected to contain a 'encoding' property.", "options");

            var dataStream = (Stream)options["dataStream"];
            var encoding = (Encoding)options["encoding"];

            //Delimiter delimiter;
            //bool firstColumnIsHeaders;

            Engine = new FileHelperEngine<B>();
            IEnumerable<B> data;

            /* Parse the stream. */
            using (StreamReader reader = new StreamReader(dataStream, encoding)) 
            {
                try
                {
                    data = Engine.ReadStream(reader);
                }
                catch (ConvertException ex)
                {
                    Log.Warn(string.Format("FileHelpersImportManager Type: '{0}' failed during import.", GetType().AssemblyQualifiedName), ex);
                    throw new ImportFormatException(ex.Message, lineNumber: ex.LineNumber, data: ex.FieldStringValue, innerException: ex);
                }
                catch (FileHelpersException ex)
                {
                    Log.Warn(string.Format("FileHelpersImportManager Type: '{0}' failed during import.", GetType().AssemblyQualifiedName), ex);
                    throw new ImportFormatException(ex.Message, innerException: ex);
                }
            }

            ProccessData(data);
        }

        public abstract void ProccessData(IEnumerable<B> data);

        public virtual bool ShowDelimiterOption { get; protected set; }

        public virtual bool ShowHeaderOption { get; protected set; }
    }
}
