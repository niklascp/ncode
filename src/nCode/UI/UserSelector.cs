using System;
using System.Text;
using System.Web.Security;
using System.Web.UI;

namespace nCode.UI
{
    [ToolboxData("<{0}:UserSelector runat=server></{0}:UserSelector>")]
    public class UserSelector : SelectorControl
    {
        private MembershipUser user;

        public UserSelector()
        {

        }

        /// <summary>
        /// Gets or sets the currently selected user.
        /// </summary>
        public MembershipUser User
        {
            get
            {
                if (user != null) {
                    return user;
                }
                else if (!string.IsNullOrEmpty(Value))
                {
                    user = Membership.GetUser(new Guid(Value));
                    return user;
                }
                return null;
            }
            set
            {
                user = value;
                if (user == null)
                    Value = string.Empty;
                else
                    Value = user.ProviderUserKey.ToString();
            }
        }

        protected override string BrowserUrl
        {
            get { return "~/admin/system/dialogs/userselector/userselector.aspx"; }
        }

        protected override void OnInit(EventArgs e)
        {
            StringBuilder callback = new StringBuilder();
            callback.AppendLine("function userSelector(id, userId, username) {");
            callback.AppendLine("  var valueField = document.getElementById(id);");
            callback.AppendLine("  var displayField = document.getElementById(id + '_DisplayField');");
            callback.AppendLine("  valueField.value = userId;");
            callback.AppendLine("  displayField.innerHTML = username;");
            callback.AppendLine("}");

            Page.ClientScript.RegisterClientScriptBlock(typeof(UserSelector), "Callback", callback.ToString(), true);

            base.OnInit(e);
        }

        protected override string GetDisplayText()
        {
            if (User != null)
                return User.UserName;

            return string.Empty;
        }
    }
}
