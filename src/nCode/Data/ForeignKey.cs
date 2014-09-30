using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;

namespace nCode.Data
{
    /// <summary>
    /// Represents a Foreign Key.
    /// </summary>
    public class ForeignKey
    {
        /// <summary>
        /// The name of the Foreign Key.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The name of the Foreign Table.
        /// </summary>
        public string FromTable { get; set; }

        /// <summary>
        /// The name of the Foreign Column.
        /// </summary>
        public string FromColumn { get; set; }

        /// <summary>
        /// The name of the Primary Table.
        /// </summary>
        public string ToTable { get; set; }

        /// <summary>
        /// The name of the Primary Column.
        /// </summary>
        public string ToColumn { get; set; }

        /// <summary>
        /// The Update Rule.
        /// </summary>
        public string UpdateRule { get; set; }

        /// <summary>
        /// The Delete Rule.
        /// </summary>
        public string DeleteRule { get; set; }

        /// <summary>
        /// Initializes a new Foreign Key.
        /// </summary>
        public ForeignKey()
        {
            UpdateRule = "No Action";
            DeleteRule = "No Action";
        }

        /// <summary>
        /// Initializes a new Foreign Key with the given relation and name.
        /// </summary>
        /// <param name="from">table.column</param>
        /// <param name="to">table.column</param>
        /// <param name="name">name</param>
        public ForeignKey(string from, string to, string name)
            : this()
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("ForeignKey name can't be null or empty.", "name");

            Name = name;
            ParseFromTo(from, to);
        }

        /// <summary>
        /// Initializes a new Foreign Key with the given relation and an automatically assigned name.
        /// </summary>
        /// <param name="from">table.column</param>
        /// <param name="to">table.column</param>
        public ForeignKey(string from, string to)
            : this()
        {
            ParseFromTo(from, to);
            Name = "FK_" + FromTable + "_" + ToTable;
        }

        private void ParseFromTo(string from, string to)
        {
            if (string.IsNullOrEmpty(from))
                throw new ArgumentException("ForeignKey (from) can not be null or empty", "from");

            if (string.IsNullOrEmpty(to))
                throw new ArgumentException("ForeignKey (to) can not be null or empty", "to");

            string[] fromParts = from.Split(new char[] { '.' });
            if (fromParts.Length != 2)
                throw new ArgumentException("ForeignKey (from) not well formatted: '" + from + "'", "from");
            FromTable = fromParts[0];
            FromColumn = fromParts[1];

            string[] toParts = to.Split(new char[] { '.' });
            if (toParts.Length != 2)
                throw new ArgumentException("ForeignKey (to) not well formatted: '" + to + "'", "to");
            ToTable = toParts[0];
            ToColumn = toParts[1];
        }

        internal void Update(SqlConnection conn, SqlTransaction trans)
        {
            SqlCommand cmdFK = new SqlCommand(
                "SELECT FK_Table = FK.TABLE_NAME, FK_Column = CU.COLUMN_NAME, PK_Table = PK.TABLE_NAME, PK_Column = PT.COLUMN_NAME, C.UPDATE_RULE, C.DELETE_RULE " +
                "FROM INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C " +
                "INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME " +
                "INNER JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME " +
                "INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME " +
                "INNER JOIN ( " +
                "SELECT i1.TABLE_NAME, i2.COLUMN_NAME " +
                "FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1 " +
                "INNER JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME " +
                "WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY' " +
                ") PT ON PT.TABLE_NAME = PK.TABLE_NAME " +
                "WHERE C.CONSTRAINT_NAME = @Name", conn);

            cmdFK.Transaction = trans;

            cmdFK.Parameters.AddWithValue("@Name", Name);

            SqlDataReader rdr = cmdFK.ExecuteReader();

            bool exists = false;
            bool drop = false;

            /* A ForeignKey with the name exists. Check if it is equal to this one. */
            if (rdr.Read())
            {
                exists = true;
                drop = (!FromTable.Equals((string)rdr["FK_Table"], StringComparison.InvariantCultureIgnoreCase)
                    || !FromColumn.Equals((string)rdr["FK_Column"], StringComparison.InvariantCultureIgnoreCase)
                    || !ToTable.Equals((string)rdr["PK_Table"], StringComparison.InvariantCultureIgnoreCase)
                    || !ToColumn.Equals((string)rdr["PK_Column"], StringComparison.InvariantCultureIgnoreCase)
                    || !UpdateRule.Equals((string)rdr["UPDATE_RULE"], StringComparison.InvariantCultureIgnoreCase)
                    || !DeleteRule.Equals((string)rdr["DELETE_RULE"], StringComparison.InvariantCultureIgnoreCase));
            }
            rdr.Close();

            /* Drop the foreign key. */
            if (drop)
            {
                string sqlDropFK =
                    "ALTER TABLE [dbo].[" + FromTable + "] " +
                    "DROP CONSTRAINT [" + Name + "]";
                SqlCommand cmdDropFK = new SqlCommand(sqlDropFK, conn);
                cmdDropFK.Transaction = trans;
                cmdDropFK.ExecuteNonQuery();
            }

            /* Create the foreign key. */
            if (drop || !exists)
            {
                string sqlAddFK =
                    "ALTER TABLE [dbo].[" + FromTable + "] " +
                    "ADD CONSTRAINT [" + Name + "] FOREIGN KEY([" + FromColumn + "]) REFERENCES [dbo].[" + ToTable + "]([" + ToColumn + "])";

                sqlAddFK += " ON UPDATE " + UpdateRule.ToUpper();

                sqlAddFK += " ON DELETE " + DeleteRule.ToUpper();

                SqlCommand cmdAddFK = new SqlCommand(sqlAddFK, conn);
                cmdAddFK.Transaction = trans;
                cmdAddFK.ExecuteNonQuery();
            }
        }
    }
}