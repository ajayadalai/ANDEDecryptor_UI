using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace NBDecryptor
{
    /// <summary>
    /// Interaction logic for Certificate.xaml
    /// </summary>
    public partial class Certificate : Page
    {
        public Certificate()
        {
            InitializeComponent();
            this.Loaded += Certificate_Loaded;
        }

        private void Certificate_Loaded(object sender, RoutedEventArgs e)
        {
            var mgr = new Data.CertificateManager();
            var cert = mgr.GetCertificate();
            if(!(cert == null || string.IsNullOrEmpty(cert.Path)))
            {
                txtCert.Text = cert.Path;
                txtPassword.Text = cert.Password;
            }
        }

        private void CancelClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Decryptor());
        }

        private void SaveClick(object sender, RoutedEventArgs e)
        {
            errLbl.Content = string.Empty;
            try
            {
                if (string.IsNullOrEmpty(txtCert.Text))
                {
                    errLbl.Content = "Please select the certificate path.";
                    return;
                }
                if (!System.IO.File.Exists(txtCert.Text))
                {
                    errLbl.Content = "Invalid certificate path.";
                    return;
                }
                if (string.IsNullOrEmpty(txtPassword.Text))
                {
                    errLbl.Content = "Please enter the password for the certificate.";
                    return;
                }
                if(!txtCert.Text.Trim().ToLower().EndsWith("pfx"))
                {
                    errLbl.Content = "Please use certificate with .pfx extension.";
                    return;
                }
                var mgr = new Data.CertificateManager();
                mgr.SaveCertificate(txtCert.Text.Trim(), txtPassword.Text.Trim());
                MessageBox.Show("Saved Successfully");
                NavigationService.Navigate(new Decryptor());
            }
            catch(Exception ex)
            {

            }
        }

        private void BtnBrowse_Click(object sender, RoutedEventArgs e)
        {
            var certPath = GetPath();
            txtCert.Text = certPath;
            

        }

        private string GetPath()
        {
            System.Windows.Forms.OpenFileDialog openFileDlg = new System.Windows.Forms.OpenFileDialog();
            openFileDlg.Multiselect = false;
            var result = openFileDlg.ShowDialog();
            if (result.ToString() != string.Empty)
            {
                return openFileDlg.FileName;
            }
            return string.Empty;
        }
    }
}
