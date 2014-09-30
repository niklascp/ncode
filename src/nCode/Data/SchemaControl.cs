using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using System.Xml;
using System.Web;

namespace nCode.Data
{
    public sealed class SchemaControl
    {
        public static IList<Schema> Schemas
        {
            get
            {
                IList<Schema> schemas = new List<Schema>();

                using (SqlConnection conn = new SqlConnection(SqlUtilities.ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(
                        "SELECT [Name], [DefinitionFile], [CurrentVersion] " +
                        "FROM [System_Schemas] " +
                        "ORDER BY [Name]", conn);

                    SqlDataReader rdr;

                    try
                    {
                        rdr = cmd.ExecuteReader();
                    }
                    catch (SqlException)
                    {
                        /* If we get a exception just return the empty list. */
                        return schemas;
                    }

                    /* Load installed schemas. */
                    while (rdr.Read())
                    {
                        Schema schema = new Schema();
                        schema.Name = (string)rdr["Name"];
                        schema.DefinitionFile = (string)rdr["DefinitionFile"];
                        schema.CurrentVersion = new Version((string)rdr["CurrentVersion"]);
                        schemas.Add(schema);
                    }

                    rdr.Close();

                    return schemas;
                }
            }
        }

        public static void Install(Schema schema)
        {
            schema.Definition.UpdateDatabase();
            schema.CurrentVersion = schema.Definition.Version;

            using (SqlConnection conn = new SqlConnection(SqlUtilities.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    "INSERT INTO [System_Schemas] (" +
                    "[ID], [Created], [Modified], [Name], [DefinitionFile], [CurrentVersion]" +
                    ") VALUES (" +
                    "@ID, @Created, @Modified, @Name, @DefinitionFile, @CurrentVersion" +
                    ")", conn);

                cmd.Parameters.AddWithValue("@ID", Guid.NewGuid());
                cmd.Parameters.AddWithValue("@Created", DateTime.Now);
                cmd.Parameters.AddWithValue("@Modified", DateTime.Now);
                cmd.Parameters.AddWithValue("@Name", schema.Name);
                cmd.Parameters.AddWithValue("@DefinitionFile", schema.DefinitionFile);
                cmd.Parameters.AddWithValue("@CurrentVersion", schema.CurrentVersion.ToString());
                cmd.ExecuteNonQuery();
            }
        }

        public static void Uninstall(string schemaName)
        {
            using (SqlConnection conn = new SqlConnection(SqlUtilities.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand(
                    "DELETE FROM [System_Schemas] WHERE [Name] = @Name", conn);

                cmd.Parameters.AddWithValue("@Name", schemaName);
                cmd.ExecuteNonQuery();
            }
        }


        public static void Update(Schema schema) {

            if (schema.Definition.Version > schema.CurrentVersion)
            {
                schema.Definition.UpdateDatabase();
                schema.CurrentVersion = schema.Definition.Version;

                using (SqlConnection conn = new SqlConnection(SqlUtilities.ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand(
                        "UPDATE [System_Schemas] SET [Modified] =  @Modified, [CurrentVersion] = @CurrentVersion " +
                        "WHERE [Name] = @Name", conn);

                    cmd.Parameters.AddWithValue("@Modified", DateTime.Now);
                    cmd.Parameters.AddWithValue("@CurrentVersion", schema.CurrentVersion.ToString());

                    cmd.Parameters.AddWithValue("@Name", schema.Name);

                    cmd.ExecuteNonQuery();
                }
            }

        }

        /// <summary>
        /// Ensures that the database schema matches the corresponding schema files. 
        /// </summary>
        public static void Update()
        {
            foreach (Schema schema in Schemas)
            {
                Update(schema);
            }
        }
    }
}