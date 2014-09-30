using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;
using System.Data.SqlClient;

namespace nCode.Data
{
    /// <summary>
    /// Represents information about a column on the server.
    /// </summary>
    public class ColumnInfo
    {
        public string Name { get; set; }
        public bool IsNullable { get; set; }
        public SqlDbType DataType { get; set; }
        public int? CharacterLength { get; set; }
        public int? NumericPrecision { get; set; }
        public int? NumericScale { get; set; }
    }

    /// <summary>
    /// Represents information about the schema for a table, and methods to enforce and update the server to match the schema.
    /// </summary>
    public class Table
    {
        private static PrimaryKey GetPrimaryKey(SqlConnection conn, SqlTransaction trans, string tableName)
        {
            var columnNames = new List<string>();
            var command = new SqlCommand("sp_pkeys", conn);
            command.Transaction = trans;
            command.CommandType = CommandType.StoredProcedure;
            command.Parameters.AddWithValue("@table_name", tableName);

            var primaryKey = new PrimaryKey();

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                primaryKey.Name = (string)reader["TABLE_NAME"];
                primaryKey.Table = (string)reader["PK_NAME"];
                columnNames.Add((string)reader["COLUMN_NAME"]);
            }

            reader.Close();

            primaryKey.Columns = columnNames;

            return primaryKey;
        }

        private static IEnumerable<ColumnInfo> GetColumnInfo(SqlConnection conn, SqlTransaction trans, string tableName)
        {
            var columnInfos = new List<ColumnInfo>();
            var command = new SqlCommand("SELECT * FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = @table_name", conn);
            command.Transaction = trans;
            command.Parameters.AddWithValue("@table_name", tableName);

            var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var ci = new ColumnInfo();
                ci.Name = (string)reader["COLUMN_NAME"];
                ci.IsNullable = ((string)reader["IS_NULLABLE"]).Equals("YES", StringComparison.OrdinalIgnoreCase);
                ci.DataType = (SqlDbType)Enum.Parse(typeof(SqlDbType), (string)reader["DATA_TYPE"], true);
                ci.CharacterLength = (reader["CHARACTER_MAXIMUM_LENGTH"] != DBNull.Value) ? Convert.ToInt32(reader["CHARACTER_MAXIMUM_LENGTH"]) : (int?)null;
                ci.NumericPrecision = (reader["NUMERIC_PRECISION"] != DBNull.Value) ? Convert.ToInt32(reader["NUMERIC_PRECISION"]) : (int?)null;
                ci.NumericScale = (reader["NUMERIC_SCALE"] != DBNull.Value) ? Convert.ToInt32(reader["NUMERIC_SCALE"]) : (int?)null;
                columnInfos.Add(ci);
            }

            reader.Close();

            return columnInfos;
        }

        /// <summary>
        /// Gets a list of all Foreign Keys.
        /// </summary>
        private static IEnumerable<ForeignKey> GetForeignKeys(SqlConnection conn, SqlTransaction trans, string tableName)
        {
            var list = new List<ForeignKey>();

            var sql = @"
SELECT
[Name] = C.CONSTRAINT_NAME, 
[ForeignTable] = FK.TABLE_NAME, 
[ForeignColumn] = CU.COLUMN_NAME,
[PrimaryTable] = PK.TABLE_NAME,
[PrimaryColumn] = PT.COLUMN_NAME,
[UpdateRule] = C.UPDATE_RULE,
[DeleteRule] = C.DELETE_RULE 
FROM
INFORMATION_SCHEMA.REFERENTIAL_CONSTRAINTS C 
JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS FK ON C.CONSTRAINT_NAME = FK.CONSTRAINT_NAME 
JOIN INFORMATION_SCHEMA.TABLE_CONSTRAINTS PK ON C.UNIQUE_CONSTRAINT_NAME = PK.CONSTRAINT_NAME 
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME 
JOIN (
	SELECT i1.TABLE_NAME, i2.COLUMN_NAME 
    FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS i1 
    JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE i2 ON i1.CONSTRAINT_NAME = i2.CONSTRAINT_NAME 
    WHERE i1.CONSTRAINT_TYPE = 'PRIMARY KEY' 
) PT ON PT.TABLE_NAME = PK.TABLE_NAME";

            var sqlCommand = new SqlCommand(sql, conn, trans);

            using (var rdr = sqlCommand.ExecuteReader())
            {
                while (rdr.Read())
                {
                    var fk = new ForeignKey()
                    {
                        Name = (string)rdr["Name"],
                        FromTable = (string)rdr["ForeignTable"],
                        FromColumn = (string)rdr["ForeignColumn"],
                        ToTable = (string)rdr["PrimaryTable"],
                        ToColumn = (string)rdr["PrimaryColumn"],
                        UpdateRule = (string)rdr["UpdateRule"],
                        DeleteRule = (string)rdr["DeleteRule"]
                    };

                    list.Add(fk);
                }
            }

            /* TODO: This filtering should be done on the Database Server-side. */
            return list.Where(x => x.FromTable.Equals(tableName, StringComparison.OrdinalIgnoreCase) || x.ToTable.Equals(tableName, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Gets a list of all Unique Keys.
        /// </summary>
        private static IEnumerable<UniqueKey> GetUniqueKeys(SqlConnection conn, SqlTransaction trans, string tableName)
    {
            var sql = @"
SELECT
[Name] = C.CONSTRAINT_NAME, 
[Table] = C.TABLE_NAME, 
[Column] = CU.COLUMN_NAME,
[Order] = CU.ORDINAL_POSITION
FROM
INFORMATION_SCHEMA.TABLE_CONSTRAINTS C 
JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE CU ON C.CONSTRAINT_NAME = CU.CONSTRAINT_NAME 
WHERE
C.[CONSTRAINT_TYPE] = 'UNIQUE'";

            var sqlCommand = new SqlCommand(sql, conn, trans);

            var temp = new List<dynamic>();

            using (var rdr = sqlCommand.ExecuteReader())
            {
                while (rdr.Read())
                {
                    temp.Add(new
                    {
                        Name = (string)rdr["Name"],
                        Table = (string)rdr["Table"],
                        Column = (string)rdr["Column"],
                        Order = (int)rdr["Order"]
                    });
                }
            }

            return temp
                /* TODO: This filtering should be done on the Database Server-side. */
                .Where(x => ((string)x.Table).Equals(tableName, StringComparison.OrdinalIgnoreCase))
                .GroupBy(x => new { Name = (string)x.Name, Table = (string)x.Table })
                .Select(x => new UniqueKey()
                {
                    Name = x.Key.Name,
                    Table = x.Key.Table,
                    Columns = x.OrderBy(y => (int)y.Order).Select(y => (string)y.Column)
                }).ToList();
        }

        /// <summary>
        /// Gets a list of all Indexes.
        /// </summary>
        private static IEnumerable<Index> GetIndexes(SqlConnection conn, SqlTransaction trans, string tableName)
        {
            var sql = @"
SELECT 
    [Name] = ind.name,
    [Table] = t.name,
    [Column] = col.name,
    [Order] = ic.index_column_id
FROM 
    sys.indexes ind 
	JOIN sys.index_columns ic ON  ind.object_id = ic.object_id and ind.index_id = ic.index_id 
	JOIN sys.columns col ON ic.object_id = col.object_id and ic.column_id = col.column_id 
	JOIN sys.tables t ON ind.object_id = t.object_id 
WHERE 
    ind.is_primary_key = 0 AND ind.is_unique = 0 AND ind.is_unique_constraint = 0 AND t.is_ms_shipped = 0 
ORDER BY 
    t.name, ind.name, ind.index_id, ic.index_column_id";

            var sqlCommand = new SqlCommand(sql, conn, trans);

            var temp = new List<dynamic>();

            using (var rdr = sqlCommand.ExecuteReader())
            {
                while (rdr.Read())
                {
                    temp.Add(new
                    {
                        Name = (string)rdr["Name"],
                        Table = (string)rdr["Table"],
                        Column = (string)rdr["Column"],
                        Order = (int)rdr["Order"]
                    });
                }
            }

            return temp
                /* TODO: This filtering should be done on the Database Server-side. */
                .Where(x => ((string)x.Table).Equals(tableName, StringComparison.OrdinalIgnoreCase))
                .GroupBy(x => new { Name = (string)x.Name, Table = (string)x.Table })
                .Select(x => new Index()
                {
                    Name = x.Key.Name,
                    Table = x.Key.Table,
                    Columns = x.OrderBy(y => (int)y.Order).Select(y => (string)y.Column)
                }).ToList();
        }

        private static bool TableExist(SqlConnection conn, SqlTransaction trans, string tableName)
        {
            SqlCommand command = new SqlCommand("SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE table_name = @table_name", conn);
            command.Transaction = trans;
            command.Parameters.AddWithValue("@table_name", tableName);

            if (command.ExecuteScalar() == null)
                return false;

            return true;
        }

        /// <summary>
        /// Compare a definition column against the current database schema. Used to check if a update is needed. 
        /// </summary>
        private static bool ColumnUpdateNeeded(Column column, ColumnInfo currentState)
        {
            if (currentState == null)
                return true;

            /* Check nullablility. */
            if (currentState.IsNullable != column.AllowNull)
                return true;

            /* Check datatype. */
            if (currentState.DataType != column.GetSqlType())
                return true;

            /* Check length */
            if (currentState.DataType == SqlDbType.NVarChar && (currentState.CharacterLength ?? 0) != column.Length)
                return true;

            /* Check precision and scale. */
            if (currentState.DataType == SqlDbType.Decimal &&
                (currentState.NumericPrecision != column.Precision ||
                currentState.NumericScale != column.Scale))
                return true;

            /* Nothing to update. */
            return false;
        }

        private string name;
        private List<Column> columns;
        private Column primaryKey;

        public Table(string name)
        {
            this.name = name;
            columns = new List<Column>();
        }

        public List<Column> Columns
        {
            get { return columns; }
        }

        public string Name
        {
            get { return name; }
        }

        public Column PrimaryKey
        {
            get { return primaryKey; }
            set { primaryKey = value; }
        }

        /// <summary>
        /// Gets or sets a old name for this table. If the old name exists, it will be renamed to the current name. For naming consistency all Foreign Keys using this table, Unique Keys and Indexes on the table will be droped and re-created.
        /// </summary>
        public string OldName { get; set; }

        /// <summary>
        /// Updates core table structure (no ix, fk etc.)
        /// </summary>
        internal void UpdateStructure(SqlConnection conn, SqlTransaction trans)
        {
            SqlCommand preCommand = new SqlCommand(string.Empty, conn);
            preCommand.Transaction = trans;
            preCommand.CommandTimeout = 60 * 5;

            SqlCommand schemaCommand = new SqlCommand(string.Empty, conn);
            schemaCommand.Transaction = trans;
            schemaCommand.CommandTimeout = 60 * 5;

            SqlCommand postCommand = new SqlCommand(string.Empty, conn);
            postCommand.Transaction = trans;
            postCommand.CommandTimeout = 60 * 5;

            /* Table dosn't exists - Create it. */
            if (!TableExist(conn, trans, name) && (OldName == null || !TableExist(conn, trans, OldName)))
            {
                schemaCommand.CommandText += "CREATE TABLE [dbo].[" + name + "] (";
                foreach (Column column in columns)
                {
                    if (column.Drop)
                        continue;

                    schemaCommand.CommandText += "[" + column.Name + "] " + column.GetSqlDeclarationString();
                    if (column.AllowNull)
                        schemaCommand.CommandText += " NULL";
                    else
                        schemaCommand.CommandText += " NOT NULL";

                    if (column.AutoIncrement)
                    {
                        schemaCommand.CommandText += " IDENTITY(1,1)";
                    }

                    schemaCommand.CommandText += ",";
                }

                /* Primary Key Constraint */
                /* TODO: Allow PK consisting of multiple columns. */
                schemaCommand.CommandText += "CONSTRAINT [PK_" + name + "] PRIMARY KEY ([" + primaryKey.Name + "])";
                schemaCommand.CommandText += ")";
            }
            /* Table already exists, update schema. */
            else
            {
                IEnumerable<ColumnInfo> currentStructure;
                PrimaryKey currentPrimaryKey;

                /* This is a rename. */
                if (!TableExist(conn, trans, name) && OldName != null && TableExist(conn, trans, OldName))
                {
                    currentStructure = GetColumnInfo(conn, trans, OldName);
                    currentPrimaryKey = GetPrimaryKey(conn, trans, OldName);

                    /* Drop all Foreign Keys. */
                    foreach (var fk in GetForeignKeys(conn, trans, OldName))
                        preCommand.CommandText += "ALTER TABLE [" + fk.FromTable + "] DROP CONSTRAINT [" + fk.Name + "];\r\n";

                    /* Drop all Unique Keys. */
                    foreach (var uq in GetUniqueKeys(conn, trans, OldName))
                        preCommand.CommandText += "ALTER TABLE [" + uq.Table + "] DROP CONSTRAINT [" + uq.Name + "];\r\n";

                    /* Drop all Indexes. */
                    foreach (var ix in GetIndexes(conn, trans, OldName))
                        preCommand.CommandText += "DROP INDEX [" + ix.Name + "] ON [" + ix.Table + "];\r\n";

                    /* Perform Rename */
                    preCommand.CommandText += "EXEC sp_rename '[dbo].[" + OldName + "]', '" + Name + "', 'OBJECT';\r\n";
                }
                else
                {
                    currentStructure = GetColumnInfo(conn, trans, name);
                    currentPrimaryKey = GetPrimaryKey(conn, trans, name);
                }

                foreach (Column column in columns)
                {
                    /* Check existence. */
                    ColumnInfo currentState = null;

                    if (!string.IsNullOrEmpty(column.OldName) && currentStructure.Any(x => x.Name.Equals(column.OldName, StringComparison.OrdinalIgnoreCase)))
                    {
                        currentState = currentStructure.Single(x => x.Name.Equals(column.OldName, StringComparison.OrdinalIgnoreCase));

                        /* Add Rename to Pre Command. */
                        if (!column.Drop)
                            preCommand.CommandText += "EXEC sp_rename '[dbo].[" + name + "].[" + column.OldName + "]', '" + column.Name + "', 'COLUMN';\r\n";
                    }

                    if (currentStructure.Any(x => x.Name.Equals(column.Name, StringComparison.OrdinalIgnoreCase)))
                    {
                        if (currentState != null)
                            throw new Exception(string.Format("The column '{1}' in table '{0}' already exists, and thus '{2}' cannot be renamed.", name, column.Name, column.OldName));

                        currentState = currentStructure.Single(x => x.Name.Equals(column.Name, StringComparison.OrdinalIgnoreCase));
                    }


                    /* Drop column and continue. */
                    if (column.Drop)
                    {
                        if (currentState != null)
                            schemaCommand.CommandText += "ALTER TABLE [dbo].[" + name + "] DROP COLUMN [" + currentState.Name + "]\r\n";
                        continue;
                    }

                    /* Should we alter/add this column. */
                    if (ColumnUpdateNeeded(column, currentState))
                    {
                        schemaCommand.CommandText += "ALTER TABLE [dbo].[" + name + "] ";

                        if (currentState != null)
                            schemaCommand.CommandText += "ALTER COLUMN ";
                        else
                            schemaCommand.CommandText += "ADD ";

                        schemaCommand.CommandText += "[" + column.Name + "] " + column.GetSqlDeclarationString();

                        if (column.AllowNull)
                        {
                            schemaCommand.CommandText += " NULL";
                        }
                        else
                        {
                            if (currentState != null)
                            {
                                // Do we need to update existing rows before update?
                                if (column.SetExistingRows != null && currentState.IsNullable)
                                {
                                    preCommand.CommandText += "UPDATE [" + name + "] SET [" + column.Name + "] = ";

                                    // Set value to another row 
                                    if (column.SetExistingRows.StartsWith("[") && column.SetExistingRows.EndsWith("]"))
                                    {
                                        preCommand.CommandText += column.SetExistingRows;
                                    }
                                    // Set value to fixed value
                                    else
                                    {
                                        preCommand.CommandText += "@" + column.Name;
                                        preCommand.Parameters.AddWithValue("@" + column.Name, column.GetSetExistingRowsValue());
                                    }

                                    preCommand.CommandText += " WHERE [" + column.Name + "] IS NULL\r\n";
                                }

                                schemaCommand.CommandText += " NOT NULL";
                            }
                            else
                            {
                                // We want to update existing items.
                                if (column.SetExistingRows != null)
                                {
                                    // Add the column and allow nulls for now.
                                    schemaCommand.CommandText += " NULL";

                                    // Add Update Statement and Change Column to Not Nullable in post query.
                                    postCommand.CommandText += "UPDATE [" + name + "] SET [" + column.Name + "] = ";

                                    // Set value to another row 
                                    if (column.SetExistingRows.StartsWith("[") && column.SetExistingRows.EndsWith("]"))
                                    {
                                        postCommand.CommandText += column.SetExistingRows;
                                    }
                                    // Set value to fixed value
                                    else
                                    {
                                        postCommand.CommandText += "@" + column.Name;
                                        postCommand.Parameters.AddWithValue("@" + column.Name, column.GetSetExistingRowsValue());
                                    }

                                    postCommand.CommandText += " WHERE [" + column.Name + "] IS NULL\r\n";
                                    postCommand.CommandText += "ALTER TABLE [dbo].[" + name + "] ALTER COLUMN [" + column.Name + "] " + column.GetSqlDeclarationString() + " NOT NULL\r\n";
                                }
                                else
                                {
                                    schemaCommand.CommandText += " NOT NULL";
                                }
                            }

                            if (column.AutoIncrement)
                                schemaCommand.CommandText += " IDENTITY(1,1)";
                        }

                        schemaCommand.CommandText += "\r\n";
                    }

                    if (column == primaryKey)
                    {
                        var match = currentPrimaryKey.Columns.Any() && string.Equals(currentPrimaryKey.Columns.First(), primaryKey.Name, StringComparison.OrdinalIgnoreCase);

                        /* Test if any primary key is defined for this column. */
                        if (!match)
                        {
                            if (currentPrimaryKey.Columns.Any())
                                schemaCommand.CommandText += "ALTER TABLE [dbo].[" + name + "] DROP CONSTRAINT [PK_" + currentPrimaryKey.Name + "];\r\n";

                            /* TODO: Allow PK consisting of multiple columns. */
                            schemaCommand.CommandText += "ALTER TABLE [dbo].[" + name + "] ADD CONSTRAINT [PK_" + name + "] PRIMARY KEY ([" + column.Name + "])\r\n";
                        }
                    }
                }
            }

            try
            {
                var hasChanges = false;

                // Execute Pre Command
                if (preCommand.CommandText != string.Empty)
                {
                    hasChanges |= true;
                    Log.Info("Executing Schema Pre-command:\r\n" + preCommand.CommandText);
                    preCommand.ExecuteNonQuery();
                }

                // Execute Schema Update Command
                if (schemaCommand.CommandText != string.Empty)
                {
                    hasChanges |= true;
                    Log.Info("Executing Schema Command:\r\n" + schemaCommand.CommandText);
                    schemaCommand.ExecuteNonQuery();
                }

                // Execute Post Command
                if (postCommand.CommandText != string.Empty)
                {
                    hasChanges |= true;
                    Log.Info("Executing Schema Post-Command:\r\n" + postCommand.CommandText);
                    postCommand.ExecuteNonQuery();
                }

                if (hasChanges)
                    Log.Info("Database Schema Update: Table " + name + ": Successfully updated Database Schema.");
            }
            catch (SqlException ex)
            {
                throw new ApplicationException(ex.Message + "\r\n\r\nPre Command:\r\n" + preCommand.CommandText + "\r\n\r\nSchema Command:\r\n" + schemaCommand.CommandText + "\r\n\r\nPost Command:\r\n" + postCommand.CommandText, ex);
            }
        }

    }


}
