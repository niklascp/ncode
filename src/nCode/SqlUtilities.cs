﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Configuration;
using System.Data.SqlClient;

using nCode.Data;

using Dapper;

namespace nCode
{
    public static class SqlUtilities
    {
        public static string ConnectionString
        {
            get
            {
                return ConfigurationManager.ConnectionStrings["Web"].ConnectionString;
            }
        }

        public static object FromDateTime(DateTime? d)
        {
            if (!d.HasValue)
                return DBNull.Value;
            return d.Value;
        }

        public static DateTime? ToDateTime(object o)
        {
            if (o == DBNull.Value)
                return null;
            return (DateTime?)o;
        }

        public static object FromString(string s)
        {
            if (string.IsNullOrEmpty(s))
                return DBNull.Value;
            return s;
        }

        public static string ToString(object o)
        {
            if (o == DBNull.Value)
                return null;
            return (string)o;
        }

        public static object FromGuid(Guid? g)
        {
            if (!g.HasValue)
                return DBNull.Value;
            return g.Value;
        }

        public static Guid? ToGuid(object o)
        {
            if (o == DBNull.Value)
                return null;
            return (Guid?)o;
        }

        public static bool TableExist(string name)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE table_name = @Name", conn);
                cmd.Parameters.AddWithValue("@Name", name);

                if (cmd.ExecuteScalar() == null)
                    return false;

                return true;
            }
        }

        public static bool IndexExist(string tableName, string indexName)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                return conn.ExecuteScalar<bool>(
                    "SELECT CAST(CASE WHEN EXISTS(SELECT 1 FROM sys.indexes WHERE name = @indexName AND object_id = OBJECT_ID(@tableName)) THEN 1 ELSE 0 END AS bit)",
                    new
                    {
                        tableName = tableName,
                        indexName = indexName
                    });
            }
        }

        public static bool ColumnExist(string tableName, string columnName)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();
                return conn.ExecuteScalar<bool>(
                    "SELECT CAST(CASE WHEN EXISTS(SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = @tableName AND column_name = @columnName) THEN 1 ELSE 0 END AS bit)",
                    new
                    {
                        tableName = tableName,
                        columnName = columnName
                    });
            }
        }

        public static void DropTableIfExist(string tableName)
        {
            using (SqlConnection conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.AppendLine("IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE table_name = '" + tableName + "')");
                sqlBuilder.AppendLine("BEGIN");
                sqlBuilder.AppendLine("  DROP TABLE [" + tableName + "]");
                sqlBuilder.AppendLine("END");

                conn.Execute(sqlBuilder.ToString());
            }
        }

        public static void DropColumnIfExist(string tableName, string columnName)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.AppendLine("IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS WHERE table_name = '" + tableName + "' AND column_name = '" + columnName + "')");
                sqlBuilder.AppendLine("BEGIN");
                sqlBuilder.AppendLine("  ALTER TABLE [" + tableName + "] DROP COLUMN [" + columnName + "]");
                sqlBuilder.AppendLine("END");

                conn.Execute(sqlBuilder.ToString());
            }
        }

        public static void DropForeignKeyIfExist(string tableName, string constraintName)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                StringBuilder sqlBuilder = new StringBuilder();
                sqlBuilder.AppendLine("IF OBJECT_ID('" + constraintName + "', 'F') IS NOT NULL");
                sqlBuilder.AppendLine("BEGIN");
                sqlBuilder.AppendLine("  ALTER TABLE [" + tableName + "] DROP CONSTRAINT [" + constraintName + "]");
                sqlBuilder.AppendLine("END");

                conn.Execute(sqlBuilder.ToString());
            }
        }

        public static void ExecuteStatement(string sql)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                conn.Open();

                conn.Execute(sql);
            }
        }

    }
}