using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Profile;
using System.Configuration;
using System.Collections.Specialized;
using System.Data.SqlClient;
using System.Web.Security;

namespace nCode.Security
{
    public class SystemProfileProvider : ProfileProvider
    {

        public SystemProfileProvider()
        { }

        public override void Initialize(string name, NameValueCollection config)
        {
            // Verify that config isn't null
            if (config == null)
                throw new ArgumentNullException("config");

            // Assign the provider a default name if it doesn't have one
            if (String.IsNullOrEmpty(name))
                name = "SystemProfileProvider";

            // Call the base class's Initialize method
            base.Initialize(name, config);
        }

        public override int DeleteInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            throw new NotImplementedException();
        }

        public override int DeleteProfiles(string[] usernames)
        {
            throw new NotImplementedException();
        }

        public override int DeleteProfiles(ProfileInfoCollection profiles)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection FindInactiveProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection FindProfilesByUserName(ProfileAuthenticationOption authenticationOption, string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection GetAllInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override ProfileInfoCollection GetAllProfiles(ProfileAuthenticationOption authenticationOption, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override int GetNumberOfInactiveProfiles(ProfileAuthenticationOption authenticationOption, DateTime userInactiveSinceDate)
        {
            throw new NotImplementedException();
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

        public override SettingsPropertyValueCollection GetPropertyValues(SettingsContext context, SettingsPropertyCollection properties)
        {
            if ((bool)context["IsAuthenticated"] == false)
                throw new Exception("Anonymous profile data is not supported by this Profile Provider.");

            SettingsPropertyValueCollection settings = new SettingsPropertyValueCollection();

            // Do nothing if there are no properties to retrieve
            if (properties.Count == 0)
                return settings;

            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmdUser = new SqlCommand("SELECT ID FROM System_Users WHERE Username = @Username", conn);
                cmdUser.Parameters.AddWithValue("@Username", context["UserName"]);

                object userID = cmdUser.ExecuteScalar();

                if (userID != null)
                {
                    SqlCommand cmdProfile = new SqlCommand("SELECT * FROM System_Profiles WHERE [User] = @User", conn);
                    cmdProfile.Parameters.AddWithValue("@User", userID);

                    SqlDataReader rdrProfile = cmdProfile.ExecuteReader();

                    bool hasProfile = rdrProfile.Read();

                    foreach (SettingsProperty property in properties)
                    {
                        SettingsPropertyValue value = new SettingsPropertyValue(property);
                        if (hasProfile)
                            value.PropertyValue = rdrProfile[property.Name.Replace(".", "_")];
                        settings.Add(value);
                    }

                    rdrProfile.Close();
                }
            }

            return settings;
        }

        public override void SetPropertyValues(SettingsContext context, SettingsPropertyValueCollection settings)
        {
            if ((bool)context["IsAuthenticated"] == false)
                throw new Exception("Anonymous profile data is not supported by this Profile Provider.");

            // Do nothing if there are no properties to save
            if (settings.Count == 0)
                return;

            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmdUser = new SqlCommand("SELECT ID FROM System_Users WHERE Username = @Username", conn);
                cmdUser.Parameters.AddWithValue("@Username", context["UserName"]);

                object userID = cmdUser.ExecuteScalar();

                // Check that the Profile Record Exists
                if (userID != null)
                {
                    SqlCommand cmdProfile = new SqlCommand("SELECT ID FROM System_Profiles WHERE [User] = @User", conn);
                    cmdProfile.Parameters.AddWithValue("@User", userID);

                    object profileID = cmdProfile.ExecuteScalar();

                    // Create the Profile Record
                    if (profileID == null)
                    {
                        profileID = Guid.NewGuid();
                        SqlCommand cmdCreateProfile = new SqlCommand("INSERT INTO System_Profiles (ID, Created, Modified, [User]) VALUES (@ID, @Modified, @Modified, @User)", conn);
                        cmdCreateProfile.Parameters.AddWithValue("@ID", profileID);
                        cmdCreateProfile.Parameters.AddWithValue("@Modified", DateTime.Now);
                        cmdCreateProfile.Parameters.AddWithValue("@User", userID);
                        cmdCreateProfile.ExecuteNonQuery();
                    }

                    bool update = false;

                    SqlCommand cmdUpdate = new SqlCommand();
                    cmdUpdate.Connection = conn;

                    StringBuilder sbUpdate = new StringBuilder();
                    sbUpdate.AppendLine("UPDATE System_Profiles SET");

                    foreach (SettingsPropertyValue value in settings)
                    {
                        if (value.IsDirty)
                        {
                            update = true;
                            string columnName = value.Name.Replace(".", "_");
                            sbUpdate.AppendLine("\t[" + columnName + "] = @" + columnName);
                            if (value.PropertyValue is string)
                                cmdUpdate.Parameters.AddWithValue("@" + columnName, SqlUtilities.FromString((string)value.PropertyValue));
                            else
                                cmdUpdate.Parameters.AddWithValue("@" + columnName, value.PropertyValue);
                        }
                    }

                    if (update)
                    {
                        sbUpdate.AppendLine("WHERE ID = @ID");
                        cmdUpdate.Parameters.AddWithValue("@ID", profileID);

                        cmdUpdate.CommandText = sbUpdate.ToString();
                        cmdUpdate.ExecuteNonQuery();
                    }
                }

            }
        }
    }
}
