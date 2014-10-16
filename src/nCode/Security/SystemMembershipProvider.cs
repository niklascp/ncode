using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.Security;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;
using System.Collections.Specialized;
using System.Configuration;
using System.Configuration.Provider;

namespace nCode.Security
{
    public class SystemMembershipProvider : System.Web.Security.MembershipProvider
    {
        private const int SALT_SIZE_IN_BYTES = 16;
        private static string[] strCharacters = { "A", "B", "C", "D", "E", "F", "G", "H", "I", "J", "K", "L", "M", "N", "O", "P", "Q", "R", "S", "T", "U", "V", "W", "X", "Y", "Z", "1", "2", "3", "4", "5", "6", "7", "8", "9", "0", "a", "b", "c", "d", "e", "f", "g", "h", "i", "j", "k", "l", "m", "n", "o", "p", "q", "r", "s", "t", "u", "v", "w", "x", "y", "z" };

        #region Static Methods

        /// <summary>
        /// Generates a salt using a random crypto algorithm.
        /// </summary>
        protected static string GenerateSalt()
        {
            byte[] buf = new byte[SALT_SIZE_IN_BYTES];
            (new RNGCryptoServiceProvider()).GetBytes(buf);
            return Convert.ToBase64String(buf);
        }

        /// <summary>
        /// Encodes a password with a given salt using the SHA512 algorithm.
        /// </summary>
        protected static string EncodePassword(string pass, string salt)
        {
            // Copy the password and data into byte arrays.
            byte[] bIn = Encoding.Unicode.GetBytes(pass);
            byte[] bSalt = Convert.FromBase64String(salt);
            byte[] bAll = new byte[bSalt.Length + bIn.Length];
            byte[] bRet = null;

            Buffer.BlockCopy(bSalt, 0, bAll, 0, bSalt.Length);
            Buffer.BlockCopy(bIn, 0, bAll, bSalt.Length, bIn.Length);

            HashAlgorithm hashAgorithm = new SHA512Managed();

            bRet = hashAgorithm.ComputeHash(bAll);

            return Convert.ToBase64String(bRet);
        }

        protected static void UpdateLastLogin(Guid id)
        {
            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                conn.Open();

                // Set LastLogin
                SqlCommand cmdUpdateLastLogin = new SqlCommand("UPDATE System_Users SET LastLogin = @LastLogin WHERE ID = @ID", conn);
                cmdUpdateLastLogin.Parameters.AddWithValue("@ID", id);
                cmdUpdateLastLogin.Parameters.AddWithValue("@LastLogin", DateTime.Now);
                cmdUpdateLastLogin.ExecuteNonQuery();
            }
        }

        #endregion

        #region Fields

        // Default Settings
        private int maxInvalidPasswordAttempts = 3;
        private bool enablePasswordReset = true;
        private bool enablePasswordRetrieval = false;
        private int minRequiredNonAlphanumericCharacters = 0;
        private int minRequiredPasswordLength = 8;
        private int passwordAttemptWindow = 20;
        private MembershipPasswordFormat passwordFormat = MembershipPasswordFormat.Hashed;
        private string passwordStrengthRegularExpression = "";
        private bool requiresQuestionAndAnswer = false;
        private bool requiresUniqueEmail = true;

        private string systemUsername = "System";
        private string systemPassword;

        #endregion

        #region Properties

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
        
        public override int MaxInvalidPasswordAttempts
        {
            get { return maxInvalidPasswordAttempts; }
        }        

        public override bool EnablePasswordReset
        {
            get { return enablePasswordReset; }
        }

        public override bool EnablePasswordRetrieval
        {
            get { return enablePasswordRetrieval; }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get { return minRequiredNonAlphanumericCharacters; }
        }

        public override int MinRequiredPasswordLength
        {
            get { return minRequiredPasswordLength; }
        }

        public override int PasswordAttemptWindow
        {
            get { return passwordAttemptWindow; }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get { return passwordFormat; }
        }

        public override string PasswordStrengthRegularExpression
        {
            get { return passwordStrengthRegularExpression; }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get { return requiresQuestionAndAnswer; }
        }

        public override bool RequiresUniqueEmail
        {
            get { return requiresUniqueEmail; }
        }

        public string SystemUsername 
        {
            get { return systemUsername; }
        }

        public string SystemPassword
        {
            get { return systemPassword; }
        }

        #endregion

        #region Methods

        public override void Initialize(string name, NameValueCollection config)
        {

            base.Initialize(name, config);

            if (config["systemUsername"] != null)
                systemUsername = config["systemUsername"];

            if (config["systemPassword"] != null)
                systemPassword = config["systemPassword"];
        }

        protected virtual SystemMembershipUser CreateUserFromDataRecord(IDataRecord rec)
        {
            Guid id = (Guid)rec["ID"];
            string username = (string)rec["Username"];
            string name = SqlUtilities.ToString(rec["Name"]);
            string email = (string)rec["Email"];
            DateTime created = (DateTime)rec["Created"];
            DateTime lastLogin = SqlUtilities.ToDateTime(rec["LastLogin"]) ?? DateTime.MinValue;
            SystemMembershipUser user = new  SystemMembershipUser(Name, username, id, email, null, null, true, false, created, lastLogin, DateTime.MinValue, DateTime.MinValue, DateTime.MinValue, name);
            
            if (Settings.GetProperty<decimal>("nCode.System.Version", 0.0m) >= 1.1m)
                user.MustChangePassword = (bool)rec["MustChangePassword"];

            return user;
        }

        protected virtual string GeneratePassword()
        {
            Random rGen = new Random();

            int p = 0;
            string strPass = "";
            for (int x = 0; x < 8; x++)
            {
                p = rGen.Next(0, 60);
                strPass += strCharacters[p];
            }
            return strPass;
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status)
        {            
            if (GetUser(username, false) != null) {                
                throw new MembershipCreateUserException(MembershipCreateStatus.DuplicateUserName);
            }

            MembershipUser user = null;

            Guid id = Guid.NewGuid();
            string passwordSalt = GenerateSalt();
            string passwordHash = EncodePassword(password, passwordSalt);

            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("INSERT INTO System_Users (ID, Created, Modified, Username, Email, PasswordHash, PasswordSalt, MustChangePassword) VALUES (@ID, @Modified, @Modified, @Username, @Email, @PasswordHash, @PasswordSalt, @MustChangePassword)", conn);

                cmd.Parameters.AddWithValue("@ID", id);
                cmd.Parameters.AddWithValue("@Modified", DateTime.Now);
                cmd.Parameters.AddWithValue("@Username", username);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                cmd.Parameters.AddWithValue("@PasswordSalt", passwordSalt);
                cmd.Parameters.AddWithValue("@MustChangePassword", false);

                if (cmd.ExecuteNonQuery() > 0)
                    user = GetUser(id, false);
            }

            status = MembershipCreateStatus.Success;

            return user;
        }

        public override void UpdateUser(MembershipUser user)
        {
            SystemMembershipUser u = (SystemMembershipUser)user;

            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("UPDATE System_Users SET " + 
                    "Modified = @Modified, " + 
                    "Email = @Email, " +
                    "Name = @Name, " +
                    "MustChangePassword = @MustChangePassword " +
                    "WHERE ID = @ID", conn);

                cmd.Parameters.AddWithValue("@ID", user.ProviderUserKey);
                cmd.Parameters.AddWithValue("@Modified", DateTime.Now);
                cmd.Parameters.AddWithValue("@Email", user.Email);
                cmd.Parameters.AddWithValue("@Name", u.Name);
                cmd.Parameters.AddWithValue("@MustChangePassword", u.MustChangePassword);

                cmd.ExecuteNonQuery();
            }
        }

        /// <summary>
        /// Deletes a user.
        /// </summary>
        /// <param name="username">The username of the user to delete.</param>
        /// <param name="deleteAllRelatedData">True to delete any related data.</param>
        /// <returns>True if user was deleted, otherwise false.</returns>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            if (username == null)
                throw new ArgumentNullException(username);

            bool success = false;

            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("DELETE FROM System_Users WHERE Username = @Username", conn);
                cmd.Parameters.AddWithValue("@Username", username);
                success = cmd.ExecuteNonQuery() != 0;
            }

            return success;
        }

        public override string ResetPassword(string username, string answer)
        {
            bool success;

            string password = GeneratePassword();
            string passwordSalt = GenerateSalt();
            string passwordHash = EncodePassword(password, passwordSalt);

            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("UPDATE System_Users SET PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt WHERE Username = @Username", conn);

                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                cmd.Parameters.AddWithValue("@PasswordSalt", passwordSalt);
                cmd.Parameters.AddWithValue("@Username", username);

                success = cmd.ExecuteNonQuery() > 0;
            }

            return password;
        }

        /// <summary>
        /// Changes the password of a user.
        /// </summary>
        /// <param name="username">The username of the user to update.</param>
        /// <param name="oldPassword">The old password, or null to override validation.</param>
        /// <param name="newPassword">The new password.</param>
        /// <returns>True if password was successfull updated, otherwise false.</returns>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            bool success;

            // Only validate user, if the old password is provided. (Allows administrators to change password without knowing the old password).
            if (oldPassword != null && !ValidateUser(username, oldPassword))
                return false;

            string passwordSalt = GenerateSalt();
            string passwordHash = EncodePassword(newPassword, passwordSalt);

            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("UPDATE System_Users SET PasswordHash = @PasswordHash, PasswordSalt = @PasswordSalt WHERE Username = @Username", conn);

                cmd.Parameters.AddWithValue("@PasswordHash", passwordHash);
                cmd.Parameters.AddWithValue("@PasswordSalt", passwordSalt);
                cmd.Parameters.AddWithValue("@Username", username);

                success = cmd.ExecuteNonQuery() > 0;
            }

            return success;
        }

        /// <summary>
        /// Changes a User's username.
        /// </summary>
        /// <param name="oldUsername">The old username.</param>
        /// <param name="newUsername">The new username.</param>
        /// <returns>True if success, otherwise false.</returns>
        public bool ChangeUsername(string oldUsername, string newUsername)
        {
            bool success = false;

            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("UPDATE System_Users SET Modified = @Modified, Username = @NewUsername WHERE Username = @OldUsername", conn);
                cmd.Parameters.AddWithValue("@Modified", DateTime.Now);
                cmd.Parameters.AddWithValue("@NewUsername", newUsername);
                cmd.Parameters.AddWithValue("@OldUsername", oldUsername);

                success = cmd.ExecuteNonQuery() > 0;
            }

            return success;
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            totalRecords = 0;
            MembershipUserCollection users = new MembershipUserCollection();

            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString)) 
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM System_Users ORDER BY Email", conn);
                SqlDataReader rdr = cmd.ExecuteReader();

                while (rdr.Read())
                {
                    users.Add(CreateUserFromDataRecord(rdr));
                    totalRecords++;
                }

                rdr.Close();
            }

            return users;
        }

        public override int GetNumberOfUsersOnline()
        {
            throw new NotImplementedException();
        }

        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            SystemMembershipUser user = null;

            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM System_Users WHERE Username = @Username", conn);
                cmd.Parameters.AddWithValue("@Username", username);

                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                    user = CreateUserFromDataRecord(rdr);

                rdr.Close();

                if (userIsOnline && user != null)
                    UpdateLastLogin((Guid)user.ProviderUserKey);
            }

            return user;
        }

        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            SystemMembershipUser user = null;

            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT * FROM System_Users WHERE ID = @ID", conn);
                cmd.Parameters.AddWithValue("@ID", providerUserKey);

                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                    user = CreateUserFromDataRecord(rdr);

                rdr.Close();

                if (userIsOnline && user != null)
                    UpdateLastLogin((Guid)user.ProviderUserKey);
            }

            return user;

        }

        public override string GetUserNameByEmail(string email)
        {
            string userName = null;

            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT UserName FROM System_Users WHERE Email = @Email", conn);
                cmd.Parameters.AddWithValue("@Email", email);

                userName = (string)cmd.ExecuteScalar();
            }

            return userName;
        }

        public override bool ValidateUser(string username, string password)
        {            
            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
                return false;

            Log.Info(string.Format("SystemMembershipProvider: Validating user '{0}'.", username));

            // Validate agianst SQL Server
            bool validated = false;

            using (SqlConnection conn = new SqlConnection(Settings.ConnectionString))
            {
                conn.Open();

                SqlCommand cmd = new SqlCommand("SELECT ID, PasswordHash, PasswordSalt FROM System_Users WHERE Username = @Username", conn);
                cmd.Parameters.AddWithValue("@Username", username);

                SqlDataReader rdr = cmd.ExecuteReader();

                if (rdr.Read())
                {

                    string passwordHash = (string)rdr["PasswordHash"];
                    string passwordSalt = (string)rdr["PasswordSalt"];

                    if (passwordHash == EncodePassword(password, passwordSalt))
                    {
                        validated = true;
                        UpdateLastLogin((Guid)rdr["ID"]);
                    }
                }

                rdr.Close();
            }

            if (validated)
            {
                Log.Info(string.Format("SystemMembershipProvider: Successfully validated user '{0}'.", username));
            }
            else
            {
                Log.Info(string.Format("SystemMembershipProvider: Validated of user '{0}' failed.", username));
            }

            return validated;
        }

        #endregion

        #region Not Implemented

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            throw new NotImplementedException();
        }

        public override string GetPassword(string username, string answer)
        {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName)
        {
            throw new NotImplementedException();
        }

        #endregion
    }


}
