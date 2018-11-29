using System;
using System.Windows.Forms;

using Microsoft.ManagementConsole;
using Microsoft.ManagementConsole.Advanced;

namespace GT.BizTalk.SSO.AdminMMC.UI
{
    /// <summary>
    /// MessageBox helpers.
    /// </summary>
    public static class MsgBoxUtil
    {
        #region Fields
        private static volatile object sync = new object();
        #endregion

        #region MessageBox without a window owner
        public static bool Confirm(string message)
        {
            DialogResult ret = MessageBox.Show(message, Properties.Resources.EnterpriseSSO, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            return (ret == DialogResult.Yes);
        }

        public static void Show(string message)
        {
            MessageBox.Show(message, Properties.Resources.EnterpriseSSO, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        public static void Show(Exception ex)
        {
            string text = string.Format(Properties.Resources.ErrorMessage, ex.Message);
            MessageBox.Show(text, Properties.Resources.EnterpriseSSO, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }
        #endregion

        #region MessageBox with IWin32Window owner
        public static bool Confirm(IWin32Window owner, string message)
        {
            DialogResult ret = MessageBox.Show(owner, message, Properties.Resources.EnterpriseSSO, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            return (ret == DialogResult.Yes);
        }

        public static void Show(IWin32Window owner, string message)
        {
            MessageBox.Show(owner, message, Properties.Resources.EnterpriseSSO, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        public static void Show(IWin32Window owner, Exception ex)
        {
            string text = string.Format(Properties.Resources.ErrorMessage, ex.Message);
            MessageBox.Show(owner, text, Properties.Resources.EnterpriseSSO, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }
        #endregion

        #region MessageBox with PropertySheet owner
        public static bool Confirm(PropertySheet owner, string message)
        {
            DialogResult ret = MsgBoxUtil.Show(owner, message, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            return (ret == DialogResult.Yes);
        }

        public static void Show(PropertySheet owner, string message)
        {
            MsgBoxUtil.Show(owner, message, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        public static void Show(PropertySheet owner, Exception ex)
        {
            string text = string.Format(Properties.Resources.ErrorMessage, ex.Message);
            MsgBoxUtil.Show(owner, text, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }
        #endregion

        #region MessageBox with NamespaceSnapInBase owner
        public static bool Confirm(NamespaceSnapInBase owner, string message)
        {
            DialogResult ret = MsgBoxUtil.Show(owner, message, MessageBoxButtons.YesNo, MessageBoxIcon.Question, MessageBoxDefaultButton.Button1);
            return (ret == DialogResult.Yes);
        }

        public static void Show(NamespaceSnapInBase owner, string message)
        {
            MsgBoxUtil.Show(owner, message, MessageBoxButtons.OK, MessageBoxIcon.Information, MessageBoxDefaultButton.Button1);
        }

        public static void Show(NamespaceSnapInBase owner, Exception ex)
        {
            string text = string.Format(Properties.Resources.ErrorMessage, ex.Message);
            MsgBoxUtil.Show(owner, text, MessageBoxButtons.OK, MessageBoxIcon.Error, MessageBoxDefaultButton.Button1);
        }
        #endregion

        #region Helpers
        private static DialogResult Show(object owner, string message, MessageBoxButtons buttons, MessageBoxIcon icon, MessageBoxDefaultButton defaultButton)
        {
            DialogResult ret = DialogResult.None;
            lock (MsgBoxUtil.sync)
            {
                MessageBoxParameters parameters = new MessageBoxParameters();
                parameters.Caption = Properties.Resources.EnterpriseSSO;
                parameters.Text = message;
                parameters.Buttons = buttons;
                parameters.Icon = icon;
                parameters.DefaultButton = defaultButton;
                if (owner is PropertySheet)
                    ret = (owner as PropertySheet).ShowDialog(parameters);
                else if (owner is NamespaceSnapInBase)
                    ret = (owner as NamespaceSnapInBase).Console.ShowDialog(parameters);
            }
            return ret;
        }
        #endregion
    }
}
