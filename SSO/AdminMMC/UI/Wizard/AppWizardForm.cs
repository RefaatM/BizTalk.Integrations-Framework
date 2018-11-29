using System;
using System.Linq;
using System.Windows.Forms;

using Microsoft.EnterpriseSingleSignOn.Interop;

using GT.BizTalk.SSO.AdminMMC.Management;

namespace GT.BizTalk.SSO.AdminMMC.UI.Wizard
{
    public partial class AppWizardForm : Form
    {
        #region Fields
        private AppWizardStep[] steps;
        private AppWizardStepWelcome welcomeStep;
        private AppWizardStepGeneral generalStep;
        private AppWizardStepAccounts accountsStep;
        private int stepNumber = 0;
        #endregion

        #region Constructor
        public AppWizardForm()
        {
            InitializeComponent();

            this.welcomeStep = new AppWizardStepWelcome();
            this.generalStep = new AppWizardStepGeneral();
            this.accountsStep = new AppWizardStepAccounts();
            this.steps = new AppWizardStep[] 
            {
                this.welcomeStep,
                this.generalStep,
                this.accountsStep
            };

            this.generalStep.DataChanged += step_DataChanged;
            this.accountsStep.DataChanged += step_DataChanged;

            this.ShowStep(0);
        }
        #endregion

        #region Event Handlers
        private void step_DataChanged(object sender, System.EventArgs e)
        {
            try
            {
                // update state of the navigation buttons
                this.UpdateButtonsState();
            }
            catch (Exception ex)
            {
                MsgBoxUtil.Show(this, ex);
            }
        }

        private void btnBack_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (this.stepNumber > 0)
                {
                    this.stepNumber--;
                    this.ShowStep(this.stepNumber);
                }
            }
            catch (Exception ex)
            {
                MsgBoxUtil.Show(this, ex);
            }
        }

        private void btnNext_Click(object sender, System.EventArgs e)
        {
            try
            {
                if (this.stepNumber < (this.steps.Length - 1)
                    && this.steps[this.stepNumber].IsValid() == true)
                {
                    this.stepNumber++;
                    this.ShowStep(this.stepNumber, this.stepNumber - 1);
                }
            }
            catch (Exception ex)
            {
                MsgBoxUtil.Show(this, ex);
            }
        }

        private void btnCreate_Click(object sender, System.EventArgs e)
        {
            try
            {
                // create a new instance of an application configuration
                SSOAppConfig appConfig = new SSOAppConfig();
                appConfig.AppInfo.Enabled = true;

                // update the application metadata information with the data entered by the user
                foreach (AppWizardStep step in this.steps)
                {
                    step.Update(appConfig.AppInfo);
                }

                // create the application
                SSOManager.CreateApplication(appConfig);
            }
            catch (Exception ex)
            {
                MsgBoxUtil.Show(this, ex);
            }
        }

        private void btnCancel_Click(object sender, System.EventArgs e)
        {
            this.Close();
        }
        #endregion

        #region Helpers
        private void ShowStep(int stepNumber, int prevStepNumber = -1)
        {
            // get the current step
            AppWizardStep step = this.steps[stepNumber];

            // if there was a previous step; get the data entered in that step
            // and refresh the current step
            if (prevStepNumber != -1)
            {
                SSOAppInfo appInfo = new SSOAppInfo();
                this.steps[prevStepNumber].Update(appInfo);
                step.Refresh(appInfo);
            }

            this.pnlContainer.Controls.Clear();
            this.pnlContainer.Controls.Add(step);

            // update state of navigation buttons
            this.UpdateButtonsState();
        }

        private void UpdateButtonsState()
        {
            AppWizardStep step = this.steps[this.stepNumber];

            this.btnBack.Enabled = (this.stepNumber > 0);
            this.btnNext.Enabled = (this.stepNumber < this.steps.Length - 1 && step.IsValid() == true);
            this.btnCreate.Enabled = (this.IsValid() && this.stepNumber == this.steps.Length - 1);
        }

        private bool IsValid()
        {
            return this.steps.All(s => s.IsValid() == true);
        }
        #endregion
    }
}
