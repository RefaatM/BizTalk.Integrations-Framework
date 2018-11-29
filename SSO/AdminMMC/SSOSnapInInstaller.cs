using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;

using Microsoft.ManagementConsole;

namespace GT.BizTalk.SSO.AdminMMC
{
    [RunInstaller(true)]
    public partial class SSOSnapInInstaller : SnapInInstaller
    {
        public SSOSnapInInstaller()
        {
            InitializeComponent();
        }

        public override void Install(IDictionary stateSaver)
        {
            base.Install(stateSaver);
            string assemblyPath = this.Context.Parameters["assemblypath"];
            string companyName = this.Context.Parameters["companyname"];
            if (string.IsNullOrWhiteSpace(companyName) == true)
                throw new InvalidOperationException("Company Name is a required field.");

            Configuration configuration = ConfigurationManager.OpenExeConfiguration(assemblyPath);
            configuration.AppSettings.Settings["CompanyName"].Value = companyName;
            configuration.Save();
        }
    }
}
