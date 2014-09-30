using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;
using System.Data.SqlClient;

namespace nCode.Security
{
    public class SystemRoleProvider : RoleProvider
    {
        public Guid GetRoleID(string roleName)
        {
            object id = null;

            using (SqlConnection conn = new SqlConnection(SqlUtilities.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT ID From System_Roles WHERE Name = @Name", conn);
                cmd.Parameters.AddWithValue("@Name", roleName);

                id = cmd.ExecuteScalar();
            }

            if (id == null)
                throw new ApplicationException("The role '" + roleName + "' could not be found.");

            return (Guid)id;
        }

        public override void AddUsersToRoles(string[] usernames, string[] roleNames)
        {
            using (SqlConnection conn = new SqlConnection(SqlUtilities.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO System_UsersInRoles (ID, Role, [User]) VALUES (@ID, @Role, @User)", conn);
                cmd.Parameters.Add("@ID", System.Data.SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@Role", System.Data.SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@User", System.Data.SqlDbType.UniqueIdentifier);

                foreach (string username in usernames)
                {
                    foreach (string roleName in roleNames)
                    {
                        if (!IsUserInRole(username, roleName))
                        {
                            MembershipUser user = Membership.GetUser(username);
                            Guid roleID = GetRoleID(roleName);

                            cmd.Parameters["@ID"].Value = Guid.NewGuid();
                            cmd.Parameters["@Role"].Value = roleID;
                            cmd.Parameters["@User"].Value = user.ProviderUserKey;

                            cmd.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        public override string ApplicationName
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public override void CreateRole(string roleName)
        {
            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentException("Role Name cannot be null or empty", "roleName");

            if (RoleExists(roleName))
                throw new ApplicationException("Cannot add role '" + roleName + "' because it already exists.");

            using (SqlConnection conn = new SqlConnection(SqlUtilities.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO System_Roles (" +
                    "ID, Created, Modified, Name" +
                    ") VALUES (" +
                    "@ID, @Modified, @Modified, @Name" +
                    ")", conn);

                cmd.Parameters.AddWithValue("@ID", Guid.NewGuid());
                cmd.Parameters.AddWithValue("@Modified", DateTime.Now);
                cmd.Parameters.AddWithValue("@Name", roleName);

                cmd.ExecuteNonQuery();
            }
        }

        public override bool DeleteRole(string roleName, bool throwOnPopulatedRole)
        {
            if (string.IsNullOrEmpty(roleName))
                throw new ArgumentException("Role Name cannot be null or empty", "roleName");
            
            bool success = false;

            using (SqlConnection conn = new SqlConnection(SqlUtilities.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("DELETE FROM System_Roles WHERE Name = @Name", conn);

                cmd.Parameters.AddWithValue("@Name", roleName);

                if (cmd.ExecuteNonQuery() > 0)
                    success = true;
            }

            return success;
        }

        public override string[] FindUsersInRole(string roleName, string usernameToMatch)
        {
            throw new NotImplementedException();
        }

        public override string[] GetAllRoles()
        {
            List<string> roles = new List<string>();

            using (SqlConnection conn = new SqlConnection(SqlUtilities.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT Name FROM System_Roles ORDER BY Name", conn);

                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                    roles.Add((string)rdr["Name"]);
            }

            return roles.ToArray();
        }

        public override string[] GetRolesForUser(string username)
        {
            List<string> roles = new List<string>();

            if (Settings.IsSetupComplete)
            {
                using (SqlConnection conn = new SqlConnection(SqlUtilities.ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmd = new SqlCommand("SELECT r.Name FROM System_Roles r INNER JOIN System_UsersInRoles ur ON r.ID = ur.Role INNER JOIN System_Users u ON ur.[User] = u.ID WHERE u.Username = @Username", conn);
                    cmd.Parameters.AddWithValue("@Username", username);

                    SqlDataReader rdr = cmd.ExecuteReader();

                    while (rdr.Read())
                        roles.Add((string)rdr["Name"]);

                    rdr.Close();
                }
            }

            return roles.ToArray();
        }

        public override string[] GetUsersInRole(string roleName)
        {
            throw new NotImplementedException();
        }

        public override bool IsUserInRole(string username, string roleName)
        {
            bool exists = false;

            using (SqlConnection conn = new SqlConnection(SqlUtilities.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT 1 FROM System_UsersInRoles ur INNER JOIN System_Users u ON ur.[User] = u.id INNER JOIN System_Roles r ON ur.Role = r.ID WHERE r.Name = @RoleName AND u.Username = @Username", conn);
                cmd.Parameters.AddWithValue("@RoleName", roleName);
                cmd.Parameters.AddWithValue("@Username", username);

                if (cmd.ExecuteScalar() != null)
                    exists = true;
            }

            return exists;
        }

        public override void RemoveUsersFromRoles(string[] usernames, string[] roleNames)
        {
            using (SqlConnection conn = new SqlConnection(SqlUtilities.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("DELETE FROM System_UsersInRoles WHERE Role = @Role AND [User] = @User", conn);
                cmd.Parameters.Add("@Role", System.Data.SqlDbType.UniqueIdentifier);
                cmd.Parameters.Add("@User", System.Data.SqlDbType.UniqueIdentifier);

                foreach (string username in usernames)
                {
                    foreach (string roleName in roleNames)
                    {
                        MembershipUser user = Membership.GetUser(username);
                        Guid roleID = GetRoleID(roleName);

                        cmd.Parameters["@Role"].Value = roleID;
                        cmd.Parameters["@User"].Value = user.ProviderUserKey;

                        cmd.ExecuteNonQuery();
                    }
                }
            }
        }

        public override bool RoleExists(string roleName)
        {
            bool exists = false;

            using (SqlConnection conn = new SqlConnection(SqlUtilities.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT 1 FROM System_Roles WHERE Name = @Name", conn);
                cmd.Parameters.AddWithValue("@Name", roleName);

                if (cmd.ExecuteScalar() != null)
                    exists = true;
            }

            return exists;
        }
    }
}
