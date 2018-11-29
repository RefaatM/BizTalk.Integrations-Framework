using System;
using System.Windows.Forms;

using Microsoft.ManagementConsole;

using GT.BizTalk.SSO.AdminMMC.Management;

namespace GT.BizTalk.SSO.AdminMMC.UI.MMC
{
    public class SSOListView : MmcListView
    {
        #region Properties
        public new SSOScopeNode ScopeNode
        {
            get { return base.ScopeNode as SSOScopeNode; }
        }
        #endregion

        #region Events
        public static event EventHandler<EventArgs<string>> ListViewChanged;

        protected void OnListViewChanged()
        {
            if (ListViewChanged != null)
                ListViewChanged(this, new EventArgs<string>(this.ScopeNode.DisplayName));
        }
        #endregion

        #region Helpers
        protected void Import()
        {
            using (OpenFileDialog openFileDialog = new OpenFileDialog())
            {
                openFileDialog.CheckFileExists = true;
                openFileDialog.CheckPathExists = true;
                openFileDialog.Filter = "SSO Application Config files (*.xml)|*.xml|All files (*.*)|*.*";
                openFileDialog.DefaultExt = "*.xml";
                openFileDialog.Title = "Import SSO Application Configuration";
                if (this.SnapIn.Console.ShowDialog(openFileDialog) == DialogResult.OK)
                {
                    // import the app configuration from the file
                    SSOManager.ImportApplication(openFileDialog.FileName, true);
                }
            }
        }

        protected void Export(string appName)
        {
            using (SaveFileDialog saveFileDialog = new SaveFileDialog())
            {
                saveFileDialog.CheckPathExists = true;
                saveFileDialog.Filter = "SSO Application Config files (*.xml)|*.xml|All files (*.*)|*.*";
                saveFileDialog.DefaultExt = "*.xml";
                saveFileDialog.FileName = appName + ".xml";
                saveFileDialog.Title = "Export SSO Application Configuration";
                if (this.SnapIn.Console.ShowDialog(saveFileDialog) == DialogResult.OK)
                {
                    SSOManager.ExportApplication(appName, saveFileDialog.FileName);
                }
            }
        }
        #endregion
    }
}
