using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.SqlClient;


namespace nCode.Data
{
    public class Index
    {
        public string Name { get; set; }
        public string Table { get; set; }
        public IEnumerable<string> Columns { get; set; }
        public bool Drop { get; set; }

        public Index()
        {

        }

        public Index(string table, IEnumerable<string> columns)
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
                "FROM sys.indexes " +
                "WHERE name = @Name", conn, trans);
            cmdExists.Parameters.AddWithValue("@Name", Name);

            bool exists = ((int?)cmdExists.ExecuteScalar() ?? 0) == 1;

            /* Drop Index */
            if (exists && Drop)
            {
                StringBuilder commandBuilder = new StringBuilder();
                commandBuilder.Append("DROP INDEX [" + Name + "] ON [dbo].[" + Table + "]");
                commandBuilder.AppendLine(";");
                SqlCommand createCommand = new SqlCommand(commandBuilder.ToString(), conn, trans);
                createCommand.ExecuteNonQuery();
                return;
            }

            /* Create Index */
            if (!exists && !Drop)
            {
                StringBuilder commandBuilder = new StringBuilder();
                commandBuilder.Append("CREATE NONCLUSTERED INDEX [" + Name + "] ON [dbo].[" + Table + "] (");
                commandBuilder.Append(string.Join(", ", Columns.Select(x => "[" + x.Trim() + "]").ToArray()));
                commandBuilder.Append(")");
                commandBuilder.AppendLine(";");
                SqlCommand createCommand = new SqlCommand(commandBuilder.ToString(), conn, trans);
                createCommand.ExecuteNonQuery();
            }
        }
    }
}