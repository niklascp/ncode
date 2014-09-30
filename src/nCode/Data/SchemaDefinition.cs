using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Linq;
using System.Data.SqlClient;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Reflection;

namespace nCode.Data
{
    public class SchemaDefinition
    {
        private List<Table> tables;
        private List<Index> indexes;
        private List<UniqueKey> uniqueKeys;
        private List<ForeignKey> foreignKeys;

        /// <summary>
        /// Initializes a new empty instance of a table.
        /// </summary>
        public SchemaDefinition()
        {
            tables = new List<Table>();
            indexes = new List<Index>();
            uniqueKeys = new List<UniqueKey>();
            foreignKeys = new List<ForeignKey>();
        }


        /* Logic for loading schema from Xml-files. */

        public Version Version { get; private set; }

        public void Load(string definitionFile)
        {
            XmlDocument schemaInfo = new XmlDocument();

            try
            {
                schemaInfo.Load(HttpContext.Current.Server.MapPath(definitionFile));
            }
            catch (Exception ex)
            {
                throw new SchemaConfigurationException("Failed to load schema definition file '" + definitionFile + "':" + Environment.NewLine + ex.Message);
            }

            try
            {
                Version = new Version(schemaInfo.SelectSingleNode("/schemaInfo").Attributes["version"].Value);
            }
            catch (Exception)
            {
                throw new SchemaConfigurationException("Failed to load schema definition file '" + definitionFile + "': Version is not well formatted or missing.");
            }
            
            LoadTables(schemaInfo);

            XmlNodeList schemaInfoForeignKeys = schemaInfo.SelectNodes("/schemaInfo/foreignKeys/foreignKey");
            foreach (XmlNode schemaInfoForeignKey in schemaInfoForeignKeys)
            {
                if (schemaInfoForeignKey.Attributes["from"] == null)
                    throw new SchemaConfigurationException("ForeignKey is missing a from value.");
                string from = schemaInfoForeignKey.Attributes["from"].Value;
                
                if (schemaInfoForeignKey.Attributes["to"] == null)
                    throw new SchemaConfigurationException("ForeignKey is missing a to value.");
                string to = schemaInfoForeignKey.Attributes["to"].Value;

                ForeignKey foreignKey;
                if (schemaInfoForeignKey.Attributes["name"] != null)
                    foreignKey = new ForeignKey(from, to, schemaInfoForeignKey.Attributes["name"].Value);
                else
                    foreignKey = new ForeignKey(from, to);

                if (schemaInfoForeignKey.Attributes["updateRule"] != null)
                    foreignKey.UpdateRule = schemaInfoForeignKey.Attributes["updateRule"].Value;

                if (schemaInfoForeignKey.Attributes["deleteRule"] != null)
                    foreignKey.DeleteRule = schemaInfoForeignKey.Attributes["deleteRule"].Value;

                foreignKeys.Add(foreignKey);

            }
        }

        private void LoadTables(XmlDocument schemaInfo)
        {

            XmlNodeList schemaInfoTables = schemaInfo.SelectNodes("/schemaInfo/tables/table");
            foreach (XmlNode schemaInfoTable in schemaInfoTables)
            {
                if (schemaInfoTable.Attributes["name"] == null)
                    throw new SchemaConfigurationException("Missing table name.");

                string primaryKey = null;
                if (schemaInfoTable.Attributes["primaryKey"] != null)
                    primaryKey = schemaInfoTable.Attributes["primaryKey"].Value;

                Table table = new Table(schemaInfoTable.Attributes["name"].Value);

                if (schemaInfoTable.Attributes["oldName"] != null)
                    table.OldName = schemaInfoTable.Attributes["oldName"].Value;

                XmlNodeList schemaInfoColumns = schemaInfoTable.SelectNodes("column");
                foreach (XmlNode schemaInfoColumn in schemaInfoColumns)
                {
                    if (schemaInfoColumn.Attributes["name"] == null)
                        throw new SchemaConfigurationException("Missing column name (in table '" + table.Name + "').");
                    string name = schemaInfoColumn.Attributes["name"].Value;

                    Column column = new Column(name);

                    if (schemaInfoColumn.Attributes["type"] != null)
                    {
                        try
                        {
                            column.Type = Type.GetType(schemaInfoColumn.Attributes["type"].Value);
                        }
                        catch
                        {
                            throw new SchemaConfigurationException("Unknown column type: '" + schemaInfoColumn.Attributes["Type"].Value + "' (in table '" + table.Name + "').");
                        }
                    }

                    if (schemaInfoColumn.Attributes["oldName"] != null)
                        column.OldName = schemaInfoColumn.Attributes["oldName"].Value;

                    if (schemaInfoColumn.Attributes["drop"] != null)
                        column.Drop = bool.Parse(schemaInfoColumn.Attributes["drop"].Value);

                    if (schemaInfoColumn.Attributes["length"] != null)
                        if (schemaInfoColumn.Attributes["length"].Value.Equals("max", StringComparison.OrdinalIgnoreCase))
                            column.Length = -1;
                        else
                            column.Length = int.Parse(schemaInfoColumn.Attributes["length"].Value);

                    if (schemaInfoColumn.Attributes["precision"] != null)
                        column.Precision = int.Parse(schemaInfoColumn.Attributes["precision"].Value);

                    if (schemaInfoColumn.Attributes["scale"] != null)
                        column.Scale = int.Parse(schemaInfoColumn.Attributes["scale"].Value);

                    if (schemaInfoColumn.Attributes["allowNull"] != null && schemaInfoColumn.Attributes["allowNull"].Value.Equals("True", StringComparison.OrdinalIgnoreCase))
                    {
                        column.AllowNull = true;
                    }
                    else
                    {
                        if (schemaInfoColumn.Attributes["autoIncrement"] != null)
                            column.AutoIncrement = true;
                        else if (schemaInfoColumn.Attributes["setExistingRows"] != null)
                            column.SetExistingRows = schemaInfoColumn.Attributes["setExistingRows"].Value;
                    }

                    /* We now allow the foreignKey attribute - NCP 20090318 */
                    if (schemaInfoColumn.Attributes["foreignKey"] != null)
                    {
                        ForeignKey foreignKey = new ForeignKey(table.Name + "." + column.Name, schemaInfoColumn.Attributes["foreignKey"].Value);
                        foreignKey.DeleteRule = "Cascade";
                        foreignKeys.Add(foreignKey);
                    }

                    table.Columns.Add(column);

                    if (name == primaryKey)
                        table.PrimaryKey = column;
                }

                tables.Add(table);

                /* Parse Index Key XML - NCP20091212 */
                foreach (XmlNode nodeIndex in schemaInfoTable.SelectNodes("index"))
                {
                    if (nodeIndex.Attributes["columns"] == null)
                        throw new SchemaConfigurationException("Index is missing a columns attribute.");

                    string[] columns = nodeIndex.Attributes["columns"].Value.Split(new char[] { ',', ';' });
                    Index index = new Index(table.Name, columns);

                    if (nodeIndex.Attributes["name"] != null)
                        index.Name = nodeIndex.Attributes["name"].Value;

                    if (nodeIndex.Attributes["drop"] != null)
                        index.Drop = bool.Parse(nodeIndex.Attributes["drop"].Value);

                    indexes.Add(index);
                }

                /* Parse Unique Key XML - NCP20090821 */
                foreach (XmlNode nodeIndex in schemaInfoTable.SelectNodes("uniqueKey"))
                {
                    if (nodeIndex.Attributes["columns"] == null)
                        throw new SchemaConfigurationException("Unique Key is missing a columns attribute.");

                    string[] columns = nodeIndex.Attributes["columns"].Value.Split(new char[] { ',', ';' });
                    UniqueKey index = new UniqueKey(table.Name, columns);

                    if (nodeIndex.Attributes["name"] != null)
                        index.Name = nodeIndex.Attributes["name"].Value;

                    if (nodeIndex.Attributes["drop"] != null)
                        index.Drop = bool.Parse(nodeIndex.Attributes["drop"].Value);

                    uniqueKeys.Add(index);
                }
            }
        }


        /* Logic for loading schema from DbContext-classes. (Entity Framework: Code First) */

        private static string GetTableName(Type entityClass)
        {
            var entityAttributes = entityClass.GetCustomAttributes(false);

            /* Get table name */
            string tableName = entityClass.Name;
            if (entityAttributes.OfType<TableAttribute>().Any())
            {
                var tableAttribute = entityAttributes.OfType<TableAttribute>().First();
                tableName = tableAttribute.Name;
            }
            return tableName;
        }

        private static string GetColumnName(PropertyInfo column)
        {
            /* Decide Column name, and Database Type Name. */
            var columnName = column.Name;
            var attributes = column.GetCustomAttributes(false);
            var columnAttribute = attributes.OfType<ColumnAttribute>().SingleOrDefault();

            if (columnAttribute != null && !string.IsNullOrEmpty(columnAttribute.Name))
                columnName = columnAttribute.Name;

            return columnName;
        }

        public void LoadFromDbContext(Type contextClass)
        {
            if (!contextClass.IsSubclassOf(typeof(DbContext)))
                throw new ArgumentException("Context Class must be a subclass of DbContext.");

            var ps = contextClass.GetProperties();

            foreach (var p in ps)
            {
                /* Skip any property not returning a DbSet. */
                if (!p.PropertyType.IsGenericType || p.PropertyType.GetGenericTypeDefinition() != typeof(DbSet<>))
                    continue;

                var attributes = p.GetCustomAttributes(false);

                /* Skip any property not marked as foreign. */
                if (attributes.OfType<ForeignEntityAtrribute>().Any())
                    continue;

                var arguments = p.PropertyType.GetGenericArguments();

                LoadFromEntityType(arguments[0]);
            }
        }

        private void LoadFromEntityType(Type entityClass)
        {
            var tableAttributes = entityClass.GetCustomAttributes(false);

            string tableName = GetTableName(entityClass);

            Table table = new Table(tableName);

            /* Optionally, set old name. */
            if (tableAttributes.OfType<RenameAttribute>().Any())
                table.OldName = tableAttributes.OfType<RenameAttribute>().First().OldName;

            var ps = entityClass.GetProperties();

            foreach (var p in ps)
            {
                /* Skip ICollection, and computed properties */
                if (p.PropertyType.IsInterface || !p.CanWrite)
                    continue;

                var attributes = p.GetCustomAttributes(false);

                /* This property is not mapped. */
                if (attributes.OfType<NotMappedAttribute>().Any())
                    continue;

                /* Decide Column name, and Database Type Name. */
                string columnName = GetColumnName(p);

                /* Handle Foreign key */
                if (attributes.OfType<ForeignKeyAttribute>().Any())
                {
                    /* Notice that the ForeignKey-annotation may be placed on the foreign key property and specify the
                     * associated navigation property name, or placed on a navigation property and specify the associated
                     * foreign key name. -- We only support the latter!
                     */

                    /* Find the real column name (in this table) of the foreign key. */
                    var foreignKeyAttribute = attributes.OfType<ForeignKeyAttribute>().First();
                    var foreignKeyColumnProperty = ps.SingleOrDefault(x => x.Name == foreignKeyAttribute.Name);

                    if (foreignKeyColumnProperty == null)
                        throw new SchemaConfigurationException(string.Format("The ForeignKey attribute on navigation property '{0}' indicates a key property '{1}' which cannot be found, in {2}.", p.Name, foreignKeyAttribute.Name, entityClass.FullName));

                    var foreignKeyColumnName = GetColumnName(foreignKeyColumnProperty);

                    /* Get Foreign Table Name. */
                    var foreignType = p.PropertyType;
                    var foreignTableName = GetTableName(foreignType);

                    /* Find Foreign Table key */
                    string foreignTableKeyName = null;
                    foreach (var foreignProperty in foreignType.GetProperties())
                    {
                        var foreignColumnAttributes = foreignProperty.GetCustomAttributes(false);
                        if (foreignColumnAttributes.OfType<KeyAttribute>().Any())
                        {
                            foreignTableKeyName = GetColumnName(foreignProperty);
                            break;
                        }
                    }

                    if (foreignTableKeyName == null)
                        throw new SchemaConfigurationException(string.Format("The ForeignKey attribute on navigation property '{0}' requires a property with a KeyAttribute in '{1}' which cannot be found, in {2}.", p.Name, foreignType.Name, entityClass.FullName));

                    var fromToken = string.Format("{0}.{1}", tableName, foreignKeyColumnName);
                    var toToken = string.Format("{0}.{1}", foreignTableName, foreignTableKeyName);
                    foreignKeys.Add(new ForeignKey(fromToken, toToken) { DeleteRule = "CASCADE" });
                    continue;
                }

                if (attributes.OfType<ComplexTypeAttribute>().Any())
                    throw new SchemaConfigurationException("Complex Types are currently not supported for schema update.");

                if (attributes.OfType<InversePropertyAttribute>().Any())
                    throw new SchemaConfigurationException("Inverse Properties are currently not supported for schema update.");

                Column column = new Column(columnName);
                
                if (Nullable.GetUnderlyingType(p.PropertyType) != null)
                {
                    column.Type = Nullable.GetUnderlyingType(p.PropertyType);
                    column.AllowNull = true;
                }
                else
                {
                    column.Type = p.PropertyType;
                    column.AllowNull = !p.PropertyType.IsValueType;
                }
                
                /* Decide if NULL is allowed. */
                if (attributes.OfType<RequiredAttribute>().Any())
                    column.AllowNull = false;

                /* Decide if this is the Primary Key. */
                if (string.Equals(columnName, "Id", StringComparison.InvariantCultureIgnoreCase) || attributes.OfType<KeyAttribute>().Any())
                {
                    table.PrimaryKey = column;
                }

                /* Should the column auto-increment */
                if (attributes.OfType<DatabaseGeneratedAttribute>().Any(x => x.DatabaseGeneratedOption == DatabaseGeneratedOption.Identity))
                {
                    column.AutoIncrement = true;
                }

                /* Resolve any specific database types. */
                string databaseTypeName = null;
                if (attributes.OfType<ColumnAttribute>().Any())
                {
                    var columnAttribute = attributes.OfType<ColumnAttribute>().First();
                    databaseTypeName = columnAttribute.TypeName;
                }

                /* Special case for ntext. Setting a string columns length to zero will trigger the ntext sql datatype. */
                if (string.Equals(databaseTypeName, "ntext", StringComparison.InvariantCultureIgnoreCase))
                {
                    column.Length = 0;
                }
                else if (attributes.OfType<MaxLengthAttribute>().Any())
                {
                    var maxLengthAttribute = attributes.OfType<MaxLengthAttribute>().First();
                    column.Length = maxLengthAttribute.Length;
                }
                
                /* Handle SetExistingRows attribute */
                if (attributes.OfType<SetExistingRowsAttribute>().Any())
                {
                    var setExistingRowsAttribute = attributes.OfType<SetExistingRowsAttribute>().First();
                    column.SetExistingRows = setExistingRowsAttribute.Value;
                }

                table.Columns.Add(column);
            }

            tables.Add(table);

            /* Indexes */
            foreach (var indexAttribute in tableAttributes.OfType<IndexAttribute>())
            {
                var columnNames = new List<string>();

                foreach (var columnName in indexAttribute.Columns)
                {
                    var columnProperty = ps.SingleOrDefault(x => x.Name == columnName);

                    if (columnProperty == null)
                        throw new SchemaConfigurationException(string.Format("The IndexAttribute attribute refers to a property '{0}' which cannot be found, in {1}.", columnName, entityClass.FullName));

                    columnNames.Add(GetColumnName(columnProperty));
                }

                var index = new Index(table.Name, columnNames);

                indexes.Add(index);
            }

            /* Unique Keys */
            foreach (var uniqueKeyAttribute in tableAttributes.OfType<UniqueKeyAttribute>())
            {
                var columnNames = new List<string>();

                foreach (var columnName in uniqueKeyAttribute.Columns)
                {
                    var columnProperty = ps.SingleOrDefault(x => x.Name == columnName);

                    if (columnProperty == null)
                        throw new SchemaConfigurationException(string.Format("The UniqueKeyAttribute attribute refers to a property '{0}' which cannot be found, in {1}.", columnName, entityClass.FullName));

                    columnNames.Add(GetColumnName(columnProperty));
                }

                var uniqueKey = new UniqueKey(table.Name, columnNames);

                uniqueKeys.Add(uniqueKey);
            }

        }



        /* Method for updating the schema. */

        public void UpdateDatabase(bool commit = true)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                conn.Open();

                SqlTransaction trans = conn.BeginTransaction();

                foreach (Table table in tables)
                {
                    table.UpdateStructure(conn, trans);
                }

                foreach (Index index in indexes)
                {
                    index.Update(conn, trans);
                }

                foreach (UniqueKey uniqueKey in uniqueKeys)
                {
                    uniqueKey.Update(conn, trans);
                }

                foreach (ForeignKey foreignKey in foreignKeys)
                {
                    foreignKey.Update(conn, trans);
                }

                if (commit)
                    trans.Commit();
            }
        }
    }
}