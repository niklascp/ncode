using System;
using System.Collections.Generic;
using System.Text;

namespace nCode.Security
{    
    public class RolePrivileges
    {
        private string roleName;
        private IDictionary<Guid, bool> granted;

        public RolePrivileges(Privilege[] privileges, string roleName)
        {
            this.roleName = roleName;
            this.granted = new Dictionary<Guid, bool>();

            foreach (Privilege privilege in privileges)
                granted.Add(privilege.ID, false);
        }

        public string RoleName
        {
            get { return roleName; }
        }

        public bool IsGranted(Privilege privilege)
        {
            if (privilege == null)
                return false;

            return IsGranted(privilege.ID);
        }

        public bool IsGranted(Guid privilegeID)
        {
            if (!granted.ContainsKey(privilegeID))
                return false;

            return granted[privilegeID];
        }

        public void SetGranted(Privilege privilege, bool granted)
        {
            if (privilege == null)
                return;

            SetGranted(privilege.ID, granted);
        }

        public void SetGranted(Guid privilegeID, bool granted)
        {
            if (!this.granted.ContainsKey(privilegeID))
                return;

            this.granted[privilegeID] = granted;
        }
    }
}