using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;


namespace nCode.Data
{
    public class UniqueKey
    {
        public string Name { get; set; }
        public string Table { get; set; }
        public IEnumerable<string> Columns { get; set; }
        public bool Drop { get; set; }

        public UniqueKey()
        {

        }

        public UniqueKey(string table, IEnumerable<string> columns)
        {
            Table = table;
            Columns = columns;
            Name = "IX_" + table;

            foreach (string c in columns)
                Name += "_" + c;
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

            /* Drop Unique Key */
            if (exists && Drop)
            {
                StringBuilder commandBuilder = new StringBuilder();
                commandBuilder.Append("ALTER TABLE [dbo].[" + Table + "] DROP CONSTRAINT [" + Name + "]");
                commandBuilder.AppendLine(";");
                SqlCommand createCommand = new SqlCommand(commandBuilder.ToString(), conn, trans);
                createCommand.ExecuteNonQuery(); 
                return;
            }

            /* Create Unique Key */
            if (!exists && !Drop)
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