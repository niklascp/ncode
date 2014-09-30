using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.Data;

namespace nCode.Data
{
    public class Column
    {
        public Column(string name)
        {
            Name = name;
            AllowNull = false;
            Precision = 18;
            Scale = 2;
        }

        public string Name { get; private set; }

        public Type Type { get; set; }

        public bool AutoIncrement { get; set; }

        public int Length { get; set; }

        public int Precision { get; set; }

        public int Scale { get; set; }

        public bool AllowNull { get; set; }

        public string SetExistingRows { get; set; }

        public string OldName { get; set; }

        public bool Drop { get; set; }

        public SqlDbType GetSqlType()
        {
            Type dbType = Type;

            if (dbType == null)
                throw new SchemaConfigurationException("Missing column type for column '" + Name + "'.");

            if (dbType.IsEnum)
                return SqlDbType.Int;

            if (dbType == typeof(Guid))
                return SqlDbType.UniqueIdentifier;

            if (dbType == typeof(String))
                return SqlDbType.NVarChar;

            if (dbType == typeof(Int32))
                return SqlDbType.Int;

            if (dbType == typeof(Double))
                return SqlDbType.Float;

            if (dbType == typeof(Decimal))
                return SqlDbType.Decimal;

            if (dbType == typeof(DateTime))
                return SqlDbType.DateTime;

            if (dbType == typeof(Boolean))
                return SqlDbType.Bit;

            if (dbType == typeof(Byte[]))
                return SqlDbType.VarBinary;
            
            throw new Exception("Unable to resolve SqlDbType for '" + Type.ToString() + "'");
        }

        internal string GetSqlDeclarationString()
        {
            SqlDbType dbType = GetSqlType();

            if (dbType == SqlDbType.UniqueIdentifier)
                return "[uniqueidentifier]";

            if (dbType == SqlDbType.NVarChar)
                return "[nvarchar](" + (Length > 0 ? Length.ToString() : "max") + ")";

            if (dbType == SqlDbType.Int)
                return "[int]";
            
            if (dbType == SqlDbType.Float)
                return "[float]";

            if (dbType == SqlDbType.Decimal)
                return "[decimal](" + Precision + ", " + Scale + ")";

            if (dbType == SqlDbType.DateTime)
                return "[datetime]";

            if (dbType == SqlDbType.Bit)
                return "[bit]";

            if (dbType == SqlDbType.VarBinary && Length != 0)
                return "[varbinary](" + (Length > 0 ? Length.ToString() : "max") + ")";

            throw new Exception("Unable to resolve Sql declaration string for SqlDbType '" + dbType.ToString() + "'");
        }

        internal object GetSetExistingRowsValue()
        {
            try
            {
                if (SetExistingRows == null)
                    return DBNull.Value;
                else if (Type == typeof(Guid))
                    return new Guid(SetExistingRows);
                else if (Type == typeof(String))
                    return SetExistingRows;
                else if (Type == typeof(Int32) || Type.IsEnum)
                    return int.Parse(SetExistingRows);
                else if (Type == typeof(Decimal))
                    return decimal.Parse(SetExistingRows, CultureInfo.InvariantCulture);
                else if (Type == typeof(Double))
                    return double.Parse(SetExistingRows, CultureInfo.InvariantCulture);
                else if (Type == typeof(DateTime))
                    return DateTime.Parse(SetExistingRows, CultureInfo.InvariantCulture);
                else if (Type == typeof(Boolean))
                    return SetExistingRows.Equals("true", StringComparison.OrdinalIgnoreCase);
            }
            catch
            { }
            
            throw new Exception("Unable to convert the string '" + SetExistingRows + "' into a object of the type '" + Type.ToString() + "'");
        }
    }
}
