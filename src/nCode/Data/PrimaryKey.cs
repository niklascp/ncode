using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;


namespace nCode.Data
{
    /// <summary>
    /// REpresents a Primary Key.
    /// </summary>
    public class PrimaryKey
    {
        public string Name { get; set; }
        public string Table { get; set; }
        public IEnumerable<string> Columns { get; set; }
        
        public PrimaryKey()
        {

        }

        public PrimaryKey(string table, IEnumerable<string> columns)
        {
            Table = table;
            Columns = columns;
            Name = "PK_" + table;
        }

        public void Update(SqlConnection conn, SqlTransaction trans)
        {
            /* Check for existance */
            SqlCommand cmdExists = new SqlCommand(
                "SELECT 1 " +
                "FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS " +
                "WHERE CONSTRAINT_NAME = @Name", conn, trans);
            cmdExists.Parameters.AddWithValue("@Name", Name);

            bool exists = ((int?)cmdExists.ExecuteScalar() ?? 0) == 1;

            /* Create Unique Key */
            if (!exists)
            {
                StringBuilder commandBuilder = new StringBuilder();
                commandBuilder.Append("ALTER TABLE [dbo].[" + Table + "] ADD CONSTRAINT [" + Name + "] UNIQUE (");
                commandBuilder.Append(string.Join(", ", Columns.Select(x => "[" + x.Trim() + "]").ToArray()));
                commandBuilder.AppendLine(")");
                SqlCommand createCommand = new SqlCommand(commandBuilder.ToString(), conn, trans);
                createCommand.ExecuteNonQuery();
            }
        }
    }
}