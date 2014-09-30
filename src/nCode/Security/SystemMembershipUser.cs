using System;
using System.Collections.Generic;
using System.Text;
using System.Web.Security;

namespace nCode.Security
{
    public class SystemMembershipUser : MembershipUser
    {
        /// <summary>
        /// Gets the current loged in user.
        /// </summary>
        public static SystemMembershipUser Current
        {
            get
            {
                return Membership.GetUser() as SystemMembershipUser;
            }
        }

        public Guid ID
        {
            get { return (Guid)ProviderUserKey; }
        }

        /// <summary>
        /// Gets or sets the full name of the User.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets whether the user must change password upun login.
        /// </summary>
        public bool MustChangePassword { get; set; }

        /// <summary>
        /// Initialixes a new System User
        /// </summary>
        public SystemMembershipUser(string providerName, string username, object providerUserKey, string email, string passwordQuestion, string comment, bool isApproved, bool isLockedOut, DateTime creationDate, DateTime lastLoginDate, DateTime lastActivityDate, DateTime lastPasswordChangedDate, DateTime lastLockoutDate, string name)
            : base(providerName, username, providerUserKey, email, passwordQuestion, comment, isApproved, isLockedOut, creationDate, lastLoginDate, lastActivityDate, lastPasswordChangedDate, lastLockoutDate)
        {
            Name = name;
        }
    }
}
