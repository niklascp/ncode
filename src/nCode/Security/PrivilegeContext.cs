using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Web;

namespace nCode.Security
{
    /// <summary>
    /// Represents a Context of which Privilege are defined.
    /// </summary>
    public class PrivilegeContext
    {
        private Guid objectID = Guid.Empty;
        private Privilege[] privileges;
        private IList<RolePrivileges> entries;

        public PrivilegeContext(Privilege[] privileges)
        {
            // Set refferences.
            this.privileges = privileges;

            // Load Entries
            this.entries = new List<RolePrivileges>();

            // Add the Everyone Role
            entries.Add(new RolePrivileges(privileges, null));

            // The role table does not nessesary exists, only try load roles.
            try
            {
                // Add specific Roles
                foreach (string roleName in Roles.GetAllRoles())
                {
                    entries.Add(new RolePrivileges(privileges, roleName));
                }
            }
            catch (SqlException ex) {
                Log.WriteEntry("System", "Initialize Privilege Context", EntryType.Error, ex);
            }
        }

        public void Load(Guid objectID)
        {
            this.objectID = objectID;

            try
            {
                using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
                {
                    conn.Open();

                    SqlCommand cmdEveryone = new SqlCommand("SELECT 1 FROM System_PrivilegeEntries entries WHERE entries.Object = @Object AND entries.Privilege = @Privilege AND entries.Role IS NULL", conn);
                    cmdEveryone.Parameters.Add("@Object", System.Data.SqlDbType.UniqueIdentifier);
                    cmdEveryone.Parameters.Add("@Privilege", System.Data.SqlDbType.UniqueIdentifier);
                    cmdEveryone.Parameters["@Object"].Value = objectID;

                    SqlCommand cmdByRole = new SqlCommand("SELECT 1 FROM System_PrivilegeEntries entries INNER JOIN System_Roles roles ON entries.Role = roles.ID WHERE entries.Object = @Object AND entries.Privilege = @Privilege AND roles.Name = @RoleName", conn);
                    cmdByRole.Parameters.Add("@Object", System.Data.SqlDbType.UniqueIdentifier);
                    cmdByRole.Parameters.Add("@Privilege", System.Data.SqlDbType.UniqueIdentifier);
                    cmdByRole.Parameters.Add("@RoleName", System.Data.SqlDbType.VarChar);
                    cmdByRole.Parameters["@Object"].Value = objectID;

                    foreach (RolePrivileges rolePrivileges in entries)
                    {
                        // Load The Everyone Entry
                        if (rolePrivileges.RoleName == null)
                        {
                            foreach (Privilege privilege in privileges)
                            {
                                cmdEveryone.Parameters["@Privilege"].Value = privilege.ID;
                                rolePrivileges.SetGranted(privilege, cmdEveryone.ExecuteScalar() != null);
                            }
                        }

                        // Load By Role Entries
                        else
                        {
                            cmdByRole.Parameters["@RoleName"].Value = rolePrivileges.RoleName;

                            foreach (Privilege privilege in privileges)
                            {
                                cmdByRole.Parameters["@Privilege"].Value = privilege.ID;
                                rolePrivileges.SetGranted(privilege, cmdByRole.ExecuteScalar() != null);
                            }
                        }
                    }
                }
            }
            catch (SqlException ex)
            {
                Log.WriteEntry("System", "Load Privilege Context", EntryType.Error, ex);
            }
        }

        /// <summary>
        /// Save this Context to the object that originally was loaded.
        /// </summary>
        public void Save()
        {
            if (objectID == Guid.Empty)
                throw new NotSupportedException("Con only save to original object, if the Load method have been called a leat once.");
            Save(objectID);
        }

        public void Save(Guid objectID)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                conn.Open();

                // Query to check if the Everyone entry exists, and if to get it's ID.
                SqlCommand cmdEveryone = new SqlCommand("SELECT ID FROM System_PrivilegeEntries entries WHERE entries.Object = @Object AND entries.Privilege = @Privilege AND entries.Role IS NULL", conn);
                cmdEveryone.Parameters.Add("@Object", System.Data.SqlDbType.UniqueIdentifier);
                cmdEveryone.Parameters.Add("@Privilege", System.Data.SqlDbType.UniqueIdentifier);
                cmdEveryone.Parameters["@Object"].Value = objectID;

                // Query to check if a Role entry exists, and if to get it's ID.
                SqlCommand cmdByRole = new SqlCommand("SELECT entries.ID FROM System_PrivilegeEntries entries INNER JOIN System_Roles roles ON entries.Role = roles.ID WHERE entries.Object = @Object AND entries.Privilege = @Privilege AND roles.Name = @RoleName", conn);
                cmdByRole.Parameters.Add("@Object", System.Data.SqlDbType.UniqueIdentifier);
                cmdByRole.Parameters.Add("@Privilege", System.Data.SqlDbType.UniqueIdentifier);
                cmdByRole.Parameters.Add("@RoleName", System.Data.SqlDbType.VarChar);
                cmdByRole.Parameters["@Object"].Value = objectID;

                // Query to get Role ID from a Role Name.
                SqlCommand cmdRoleID = new SqlCommand("SELECT ID FROM System_Roles WHERE Name = @RoleName", conn);
                cmdRoleID.Parameters.Add("@RoleName", System.Data.SqlDbType.VarChar);

                // Query to insert a entry.
                SqlCommand cmdInsert = new SqlCommand("INSERT INTO System_PrivilegeEntries (ID, Object, Privilege, Role) VALUES (@ID, @Object, @Privilege, @Role)", conn);
                cmdInsert.Parameters.Add("@ID", System.Data.SqlDbType.UniqueIdentifier);
                cmdInsert.Parameters.Add("@Object", System.Data.SqlDbType.UniqueIdentifier);
                cmdInsert.Parameters.Add("@Privilege", System.Data.SqlDbType.UniqueIdentifier);
                cmdInsert.Parameters.Add("@Role", System.Data.SqlDbType.UniqueIdentifier);
                cmdInsert.Parameters["@Object"].Value = objectID;

                // Query to delete a entry.
                SqlCommand cmdDelete = new SqlCommand("DELETE FROM System_PrivilegeEntries WHERE ID = @ID", conn);
                cmdDelete.Parameters.Add("@ID", System.Data.SqlDbType.UniqueIdentifier);

                foreach (RolePrivileges rolePrivileges in entries)
                {
                    // Save the Entry
                    foreach (Privilege privilege in privileges)
                    {
                        object entryID = null;

                        if (rolePrivileges.RoleName == null) {
                            cmdEveryone.Parameters["@Privilege"].Value = privilege.ID;
                            entryID = cmdEveryone.ExecuteScalar();
                        }
                        else
                        {
                            cmdByRole.Parameters["@Privilege"].Value = privilege.ID;
                            cmdByRole.Parameters["@RoleName"].Value = rolePrivileges.RoleName;
                            entryID = cmdByRole.ExecuteScalar();
                        }
                        
                        // The Privilege is Granted, but doesn't exists in the Database
                        if (entryID == null && rolePrivileges.IsGranted(privilege))
                        {
                            cmdInsert.Parameters["@ID"].Value = Guid.NewGuid();
                            cmdInsert.Parameters["@Privilege"].Value = privilege.ID;

                            object roleID;
                            if (rolePrivileges.RoleName == null) {
                                 roleID = DBNull.Value;
                            }
                            else {
                                cmdRoleID.Parameters["@RoleName"].Value = rolePrivileges.RoleName;
                                roleID = cmdRoleID.ExecuteScalar();
                            }

                            cmdInsert.Parameters["@Role"].Value = roleID;
                            cmdInsert.ExecuteNonQuery();
                        }
                        // The Privilege is no longer Granted, but exists in the Database
                        else if (entryID != null && !rolePrivileges.IsGranted(privilege))
                        {
                            cmdDelete.Parameters["@ID"].Value = (Guid)entryID;
                            cmdDelete.ExecuteNonQuery();
                        }
                    }
                }
            }
        }
        
        public Privilege[] Privileges
        {
            get { return privileges; }
        }

        public IList<RolePrivileges> Entries
        {
            get { return entries; }
        }

        /// <summary>
        /// Gets the Privilege provided by its name, or null if the Privilege doen't exits in this context.
        /// </summary>
        public Privilege GetPrivilege(string privilegeName)
        {
            foreach (Privilege p in Privileges)
                if (p.Name.Equals(privilegeName, StringComparison.OrdinalIgnoreCase))
                    return p;

            return null;
        }

        /// <summary>
        /// Returns true if the current logged on user has the Privilege provided by its name.
        /// </summary>
        public bool HasPrivilege(string privilegeName)
        {
            // If logged in as System user just accept.
            if (HttpContext.Current != null &&
                HttpContext.Current.User.Identity.IsAuthenticated &&
                HttpContext.Current.User.Identity.Name.Equals(((SystemMembershipProvider)Membership.Provider).SystemUsername))
                return true;

            Privilege privilege = GetPrivilege(privilegeName);

            if (privilege == null)
                return false;

            foreach (RolePrivileges rolePrivileges in Entries)
            {
                if (rolePrivileges.RoleName == null || Roles.IsUserInRole(rolePrivileges.RoleName))
                {
                    if (rolePrivileges.IsGranted(privilege))
                        return true;
                }
            }

            return false;
        }
    }
}
