using System;
using System.Security.Cryptography;
using System.Windows.Forms;
using Franklin_Templeton_DAL.Constants;
using Franklin_Templeton_DAL.Helpers;

namespace Franklin_Templeton_DAL
{
    public partial class SettingPopup : Form
    {
        #region Initialize Popup

        public SettingPopup()
        {
            InitializeComponent();
            txtUserName.Focus();
            PopulateInputs();
        }

        private void PopulateInputs()
        {
            txtUserName.Text = DalSettings.username ?? string.Empty;
            txtPassword.Text = DalSettings.password ?? string.Empty;

            switch (DalSettings.SelectedApiUrl)
            {
                case ApiUrls.prod:
                    rBtn_api_prod.Checked = true;
                    break;
                case ApiUrls.dev:
                    rBtn_api_dev.Checked = true;
                    break;
                case ApiUrls.uat:
                    rBtn_api_uat.Checked = true;
                    break;
                case ApiUrls.test:
                    rBtn_api_test.Checked = true;
                    break;
                default:
                    rBtn_api_prod.Checked = true;
                    break;
            }
            rBtn_api_prod.ForeColor = Properties.Settings.Default.EnvironLabelColorProduction;
            rBtn_api_dev.ForeColor = Properties.Settings.Default.EnvironLabelColorDevelopment;
            rBtn_api_uat.ForeColor = Properties.Settings.Default.EnvironLabelColorUat;
            rBtn_api_test.ForeColor = Properties.Settings.Default.EnvironLabelColorTest;
        }

        #endregion


        private void SaveSettings(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrEmpty(txtUserName.Text))
                {
                    MessageBox.Show("User name is mandatory");
                    txtUserName.Focus();
                    return;
                }

                if (string.IsNullOrEmpty(txtPassword.Text))
                {
                    MessageBox.Show("Password is mandatory");
                    txtPassword.Focus();
                    return;
                }

                SetApiUrl();

                DalSettings.username = txtUserName.Text;
                DalSettings.password = txtPassword.Text;
                new OktaHelper().UpdateAccessToken();
                var encryptionHelper = new EncryptionHelper();
                Properties.Settings.Default.username = encryptionHelper.Encrypt(DalSettings.username);
                Properties.Settings.Default.password = encryptionHelper.Encrypt(DalSettings.password);
                Properties.Settings.Default.apiUrl = DalSettings.SelectedApiUrl;
                Properties.Settings.Default.Save();
                new MasterDataHelper().UpdateMasterData();
                this.Dispose();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                this.Dispose();
            }
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Dispose();
        }

        private void SetApiUrl()
        {
            if (rBtn_api_dev.Checked)
            {
                DalSettings.SelectedApiUrl = ApiUrls.dev;
            }
            else if (rBtn_api_test.Checked)
            {
                DalSettings.SelectedApiUrl = ApiUrls.test;
            }
            else if (rBtn_api_uat.Checked)
            {
                DalSettings.SelectedApiUrl = ApiUrls.uat;
            }
            else// (rBtn_api_prod.Checked)
            {
                DalSettings.SelectedApiUrl = ApiUrls.prod;
            }
            return;
        }

    }
}
