using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Resources;

namespace nCode.Security
{
    public static class SystemRoles
    {
        private static List<string> systemRoles;

        private static ResourceManager resourceManager;

        static SystemRoles()
        {
            systemRoles = new List<string>();
            systemRoles.Add("SuperAdministrator");
            systemRoles.Add("Administrator");
        }

        public static ResourceManager ResourceManager
        {
            get
            {
                if (resourceManager == null)
                {
                    resourceManager = new ResourceManager("nCode.Security.Resources.SystemRoles", typeof(SystemRoles).Assembly);
                }
                return resourceManager;
            }
        }

        public static IList<string> Roles {
            get { return systemRoles.AsReadOnly(); }
        }

        public static string GetLocalizedRoleName(string roleName)
        {
            if (!systemRoles.Contains(roleName))
                return roleName;

            string displayName = ResourceManager.GetString(roleName);

            if (displayName == null)
                return roleName;

            return displayName;
        }

        public static string SuperAdministrator
        {
            get { return systemRoles[0]; }
        }

        public static string Administrator 
        {
            get { return systemRoles[1]; }
        }
    }
}
