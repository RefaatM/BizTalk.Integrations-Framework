using System.Diagnostics;
using System.Security.Principal;
using System.Windows.Forms;

using Microsoft.ManagementConsole;
using Microsoft.ManagementConsole.Advanced;

using GT.BizTalk.SSO.AdminMMC.Management;

namespace GT.BizTalk.SSO.AdminMMC.UI.MMC
{
    [SnapInSettings("{AFEF9BA1-689C-4774-A503-F021D7C0E850}", 
        ConfigurationFile = "GT.BizTalk.SSO.AdminMMC.dll.config", 
        Description = "Allows a BizTalk Administrator to Configure the SSO Configuration Store", 
        DisplayName = "HR - SSO Application Configuration", 
        Vendor = "Holt Renfrew")]
    public class SSOAppSnapIn : SnapIn
    {
        public static bool HasSecurityRights
        {
            get;
            set;
        }

        public SSOAppSnapIn()
        {
            WindowsPrincipal windowsPrincipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            SSOAppSnapIn.HasSecurityRights = windowsPrincipal.IsInRole(SSOManager.SSOAdminAccounts) || windowsPrincipal.IsInRole(SSOManager.SSOAffiliateAdminAccounts);

            if (SSOAppSnapIn.HasSecurityRights)
            {
                AppRootScopeNode rootScopeNode = new AppRootScopeNode();
                this.RootNode = rootScopeNode;
            }
            this.SmallImages.Add(Properties.Resources.authority_16);
            this.SmallImages.Add(Properties.Resources.add_scope);
        }

        protected override bool OnShowInitializationWizard()
        {
            return SSOAppSnapIn.HasSecurityRights;
        }

        protected override void OnInitialize()
        {
            if (!SSOAppSnapIn.HasSecurityRights)
            {
                EventLog.WriteEntry("SSO MMC Snap-In", "User does not have proper rights.  SSO Administrators required.", EventLogEntryType.Error);
                this.Console.ShowDialog(new MessageBoxParameters()
                {
                    Buttons = MessageBoxButtons.OK,
                    Caption = "Access Denied",
                    Text = "User is not an SSO Administrator",
                    Icon = MessageBoxIcon.Hand
                });
            }
            base.OnInitialize();
        }
    }
}
