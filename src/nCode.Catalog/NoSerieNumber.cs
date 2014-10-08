using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace nCode.Catalog
{
    /// <summary>
    /// Represents a Number of a Number Serie.
    /// </summary>
    public class NoSerieNumber
    {
        private static object lockObject = new object();

        /// <summary>
        /// Gets the next number given by the Code of the Number Serie.
        /// </summary>
        public static NoSerieNumber GetNext(string code)
        {
            lock (lockObject)
            {
                using (CatalogModel model = new CatalogModel())
                {
                    NoSerie noSerie = (from n in model.NoSeries where n.Code == code select n).SingleOrDefault();

                    if (noSerie == null)
                        throw new ApplicationException("No Series '" + code + "' not found.");

                    NoSerieNumber no = new NoSerieNumber(noSerie.LastNo + 1 ?? noSerie.StartNo, noSerie.Prefix, noSerie.Suffix);
                    noSerie.LastNo = no.No;
                    model.SubmitChanges();
                    return no;
                }
            }
        }

        /// <summary>
        /// Peeks at the next number given by the Code of the Number Serie.
        /// </summary>
        public static NoSerieNumber Peek(string code)
        {
            lock (lockObject)
            {
                using (CatalogModel model = new CatalogModel(SqlUtilities.ConnectionString))
                {
                    NoSerie noSerie = (from n in model.NoSeries where n.Code == code select n).SingleOrDefault();

                    if (noSerie == null)
                        throw new ApplicationException("No Series '" + code + "' not found.");

                    NoSerieNumber no = new NoSerieNumber(noSerie.LastNo + 1 ?? noSerie.StartNo, noSerie.Prefix, noSerie.Suffix);                    
                    return no;
                }
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="NoSerieNumber"/> class.
        /// </summary>
        public NoSerieNumber(int no, string prefix, string suffix)
        {
            No = no;
            Prefix = prefix;
            Suffix = suffix;
        }

        /// <summary>
        /// Gets the No.
        /// </summary>
        public int No { get; private set; }

        /// <summary>
        /// Gets the Prefix.
        /// </summary>
        public string Prefix { get; private set; }

        /// <summary>
        /// Gets the Suffix.
        /// </summary>
        public string Suffix { get; private set; }

        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            string s = No.ToString();
            if (Prefix != null)
                s = Prefix + s;
            if (Suffix != null)
                s = s + Suffix;
            return s;
        }
    }
}
