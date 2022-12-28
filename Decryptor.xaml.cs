using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using System.Reflection;
using Ionic.Zip;
using System.Runtime.InteropServices;
using System.Windows.Interop;
using System.Threading;
using System.ComponentModel;
using System.Threading.Tasks;

namespace NBDecryptor
{
    /// <summary>
    /// Interaction logic for Decryptor.xaml
    /// </summary>
    public partial class Decryptor : Page
    {
        private bool decryptMode_ = true;
        const string certName = @"PX_Service_20130402_160247.pfx";
        public Decryptor()
        {
            InitializeComponent();

            //Populate();
            //string filename = @"PX_Agent_20130402_160415.pfx";

            var mgr = new Data.CertificateManager();
            var certificate = mgr.GetCertificate();

            if (certificate == null || string.IsNullOrEmpty(certificate.Path))
            {
                string currentDirectory = Directory.GetCurrentDirectory();
                string certFilePath = System.IO.Path.Combine(System.IO.Path.Combine(currentDirectory, "Certificate"), certName);
                mgr.SaveCertificate(certFilePath, "12345678");
                certificate = mgr.GetCertificate();
            }

            string fileNameExtless = System.IO.Path.GetFileNameWithoutExtension(certificate.Path);

            lblcerts_.Text = fileNameExtless;


            AppDomain.CurrentDomain.AssemblyResolve += LoadDLLs;
            try
            {

                if (!IsCertificateInstalled("CN=" + fileNameExtless))
                    CertHelper.ImportCertificateWithPrivateKey(certificate.Path, certificate.Password);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Please check the certificate. It seems to be incorrect certificate path is selected. Please contact adminstrator if the issue persists.");
            }

            //else
            //{
            //    Console.WriteLine("Certificate is already installed");
            //}
            //Console.WriteLine("Decrypting....");

            //if (certs_.Items.Count == 0)
            //    lblCertNote.Content = "Please install certificate(s) for decryption";
        }


        static bool IsCertificateInstalled(string certName)
        {
            var all_certs = CertHelper.GetCertificatesLike("(i[0-9][0-9][0-9][0-9]_[0-9]+_[0-9]+$)|(^PX_)|(^RDNA_)");
            foreach (var cert in all_certs)
            {
                if (cert.Subject == certName)
                    return true;
            }
            return false;
        }

        public static Assembly LoadDLLs(object sender, ResolveEventArgs args)
        {
            AssemblyName aname = new AssemblyName(args.Name);
            string resource_name = "NBDecryptor.EmbeddedDLLs." + aname.Name + ".dll";
            using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resource_name))
            {
                byte[] assemblyData = new byte[stream.Length];
                stream.Read(assemblyData, 0, assemblyData.Length);
                return Assembly.Load(assemblyData);
            }
        }

        private void DecryptMode()
        {
            decrypt_.Content = "Decrypt";
            decryptMode_ = true;
            signCerts_.Visibility = System.Windows.Visibility.Hidden;
            signCertsLabel_.Visibility = System.Windows.Visibility.Hidden;
        }

        private void EncryptMode()
        {
            decrypt_.Content = "Encrypt";
            decryptMode_ = false;
            signCerts_.Visibility = System.Windows.Visibility.Visible;
            signCertsLabel_.Visibility = System.Windows.Visibility.Visible;
        }

        //private void Populate()
        //{
        //    PopulateCerts("(i[0-9][0-9][0-9][0-9]_[0-9]+_[0-9]+$)|(^PX_)|(^RDNA_)",
        //                  certs_,
        //                  Properties.Settings.Default.CurrentCert);

        //    PopulateCerts("I[0-9][0-9][0-9][0-9]$",
        //                  signCerts_,
        //                  Properties.Settings.Default.CurrentSignCert);

        //    SaveSelCerts();
        //}

        //private void PopulateCerts(string pattern, ComboBox combo, string defaultCert)
        //{
        //    var all_certs = CertHelper.GetCertificatesLike(pattern);

        //    int idx = 0;
        //    int sel = 0;
        //    foreach (var cert in all_certs)
        //    {
        //        if (!cert.HasPrivateKey)
        //        {
        //            continue;
        //        }

        //        string name = cert.BaseName();
        //        combo.Items.Add(name);
        //        if (name == defaultCert)
        //        {
        //            sel = idx;
        //        }
        //        ++idx;
        //    }
        //    combo.SelectedIndex = sel;
        //}

        //private void SaveSelCerts()
        //{
        //    bool modified = false;

        //    if (certs_.Items.Count > 0)
        //    {
        //        string cert = certs_.SelectedItem as string;
        //        Properties.Settings.Default.CurrentCert = cert;
        //        modified = true;
        //    }

        //    if (signCerts_.Items.Count > 0)
        //    {
        //        string sign_cert = signCerts_.SelectedItem as string;
        //        Properties.Settings.Default.CurrentSignCert = sign_cert;
        //        modified = true;
        //    }

        //    if (modified)
        //    {
        //        Properties.Settings.Default.Save();
        //    }
        //}

        private void inputPath_PreviewDragEnter(object sender, DragEventArgs e)
        {
            e.Handled = true;

        }

        private void inputPath__Drop(object sender, DragEventArgs e)
        {
            ClearStatus();

            if (e.Data.GetDataPresent(DataFormats.FileDrop, true))
            {
                string[] paths = e.Data.GetData(DataFormats.FileDrop, true) as string[];
                if (paths.Length > 0)
                {
                    string path = paths[0];

                    // SelectRecoveryCert(path);

                    inputPath_.Text = path;
                    inputPath_.CaretIndex = inputPath_.Text.Length;
                    var rect = inputPath_.GetRectFromCharacterIndex(inputPath_.CaretIndex);
                    inputPath_.ScrollToHorizontalOffset(rect.Right);

                    if (path.ToLower().EndsWith("_decrypted.zip"))
                    {
                        // EncryptMode();
                        //SelectSignCert(path);
                    }
                    else
                    {
                        DecryptMode();
                    }
                }
            }
        }

        private static bool IsRecoveryFile(string path)
        {
            return System.IO.Path.GetFileName(path).ToLower().EndsWith("recovery.txt");
        }

        //private void SelectRecoveryCert(string path)
        //{
        //    if (IsRecoveryFile(path))
        //    {
        //        string[] lines = File.ReadAllLines(path);
        //        if (lines.Length > 1)
        //        {
        //            string serial_no = lines[0].Trim();

        //            var rec_certs = CertHelper.GetCertificatesLike("^RDNA_");
        //            foreach (var cert in rec_certs)
        //            {
        //                if (cert.SerialNumber == serial_no)
        //                {
        //                    SelectCert(cert.BaseName(), certs_);
        //                }
        //            }
        //        }
        //    }
        //}

        //private void SelectSignCert(string path)
        //{
        //    var mc = Regex.Matches(System.IO.Path.GetFileName(path),
        //                           "i[0-9][0-9][0-9][0-9]",
        //                           RegexOptions.IgnoreCase);
        //    if (mc.Count > 0)
        //    {
        //        string cand_sign_cert = mc[0].Value.ToUpper();
        //        var cand_sign_certs = CertHelper.GetCertificatesLike(cand_sign_cert);
        //        if (cand_sign_certs.Count > 0)
        //        {
        //            SelectCert(cand_sign_certs[0].BaseName(), signCerts_);
        //        }
        //    }
        //}

        //private void SelectCert(string certName, ComboBox combo)
        //{
        //    foreach (object item in combo.Items)
        //    {
        //        string other_cert_name = item as string;
        //        if (certName == other_cert_name)
        //        {
        //            combo.SelectedItem = item;
        //            break;
        //        }
        //    }
        //}



        private void inputPath__PreviewDragOver(object sender, DragEventArgs e)
        {
            e.Handled = true;
        }



        private void Status(string msg)
        {
            status_.Text = msg;
            status_.Refresh();
        }

        private void ClearStatus()
        {
            status_.Text = "";
            tempPw_.Text = "";
        }

        private void decrypt__Click(object sender, RoutedEventArgs e)
        {
           
            string path = inputPath_.Text.Trim();
            string output = string.Empty;
            if (string.IsNullOrEmpty(path))
            {
                Status("Enter Zip File Path or Drag/Drop zip file.");
                return;
            }

            try
            {
                busyIndicator.IsBusy = true;
                LayoutRoot.IsEnabled = false;
                ClearStatus();
                Task.Run(() =>
                {

                    //string currentDirectory = Directory.GetCurrentDirectory();
                    //string certFilePath = System.IO.Path.Combine(System.IO.Path.Combine(currentDirectory, "Certificate"), certName);
                    //string fileNameExtless = System.IO.Path.GetFileNameWithoutExtension(certFilePath);

                    var mgr = new Data.CertificateManager();
                    var certificate = mgr.GetCertificate();
                    string fileNameExtless = System.IO.Path.GetFileNameWithoutExtension(certificate.Path);

                    //Status(FileDecrypter.DecryptFile(path, fileNameExtless));
                    output = FileDecrypter.DecryptFile(path, fileNameExtless);
                }).ContinueWith((t) =>
                {
                    if (t.Status == TaskStatus.RanToCompletion)
                    {
                        Dispatcher.Invoke(() =>
                        {
                            busyIndicator.IsBusy = false;
                            LayoutRoot.IsEnabled = true;
                            if (output.StartsWith("Success:"))
                            {
                                //mainFrame_.Navigate(new Decryptor());
                                NavigationService navService = NavigationService.GetNavigationService(this);
                                Summary summaryPage = new Summary(output.Replace("Success:", ""));
                                navService.Navigate(summaryPage);
                            }
                            else
                            {
                                Status(output);
                            }
                        });
                    }
                   
                });
                //busyIndicator.IsBusy = false;

            }
            catch (Exception ex)
            {
                Status(string.Format("Problem decrypting: {0}", ex.Message));
            }
        }


        private void DecryptExportedData()
        {
            string cert = certName;//certs_.SelectedItem as string;

            string from_path = inputPath_.Text.Trim();
            if (string.IsNullOrEmpty(from_path))
            {
                Status("Enter Zip File Path or Drag/Drop zip file");
                return;
            }
            if (!File.Exists(from_path))
            {
                Status("Specified path does not exist.");
                return;
            }

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                Status("Decrypting...");

                string to_path = System.IO.Path.Combine(
                                        System.IO.Path.GetDirectoryName(from_path)
                                      , System.IO.Path.GetFileNameWithoutExtension(from_path) + "_decrypted.zip");

                using (var crypto = new Crypto(cert))
                {
                    if (!crypto.Valid)
                    {
                        Status(string.Format("No key for {0}.cer", cert));
                        return;
                    }

                    using (ZipFile dec = new ZipFile())
                    {
                        using (ZipFile enc = ZipFile.Read(from_path))
                        {
                            foreach (ZipEntry enc_entry in enc)
                            {
                                try
                                {
                                    MemoryStream buffer = new MemoryStream();
                                    enc_entry.Extract(buffer);
                                    buffer.Seek(0, 0);

                                    buffer = crypto.CheckSignatureAndExtract(buffer);
                                    buffer = crypto.Decrypt(buffer);
                                    buffer = crypto.CheckSignatureAndExtract(buffer);

                                    // Avoid .Replace() - be sure that we only remove from the end
                                    string dec_filename = enc_entry.FileName;
                                    if (dec_filename.ToLower().EndsWith(".enc"))
                                    {
                                        dec_filename = dec_filename.Substring(0, dec_filename.Length - 4);
                                    }

                                    dec.AddEntry(dec_filename, buffer.ToArray());

                                }
                                catch (Exception)
                                {
                                    continue;
                                }
                            }
                        }

                        dec.Save(to_path);

                        //Status("File decrypted successfully.");

                    }
                }

                // ClearStatus();
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void DecryptRecovery()
        {
            string cert = certName;//certs_.SelectedItem as string;
            string rec_file = inputPath_.Text.Trim();

            string[] lines = File.ReadAllLines(rec_file);
            if (lines.Length < 2)
            {
                Status("Recovery.txt file not in expected format");
                return;
            }

            try
            {
                string tmp_pw = "";

                Mouse.OverrideCursor = Cursors.Wait;
                Status("Decrypting...");

                string enc = lines[1].Trim();
                byte[] enc_bytes = Convert.FromBase64String(enc);
                using (var crypto = new Crypto(cert))
                {
                    byte[] clear = crypto.RSADecrypt(enc_bytes);
                    tmp_pw = new string(Encoding.ASCII.GetChars(clear));
                }

                ClearStatus();

                tempPw_.Text = tmp_pw;
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void EncryptExportedData()
        {
            //if (certs_.Items.Count == 0 || signCerts_.Items.Count == 0)
            //{
            //    return;
            //}

            string cert = certName; //certs_.SelectedItem as string;

            string from_path = inputPath_.Text.Trim();
            if (string.IsNullOrEmpty(from_path))
            {
                Status("Enter Zip File Path or Drag/Drop zip file");
                return;
            }
            if (!File.Exists(from_path))
            {
                Status("Specified path does not exist.");
                return;
            }

            try
            {
                Mouse.OverrideCursor = Cursors.Wait;
                Status("Encrypting...");

                string to_path = from_path.Replace("_decrypted.zip", ".zip");
                if (File.Exists(to_path))
                {
                    throw new ApplicationException(string.Format("{0} already exists.", to_path));
                }

                string sign_cert = signCerts_.SelectedItem as string;

                using (var crypto = new Crypto(cert, sign_cert))
                {
                    if (!crypto.Valid)
                    {
                        Status(string.Format("No key for {0}.cer", cert));
                        return;
                    }

                    using (ZipFile enc = new ZipFile())
                    {
                        using (ZipFile dec = ZipFile.Read(from_path))
                        {
                            foreach (ZipEntry dec_entry in dec)
                            {
                                MemoryStream buffer = new MemoryStream();
                                dec_entry.Extract(buffer);
                                buffer.Seek(0, 0);

                                buffer = crypto.Sign(buffer);
                                buffer = crypto.Encrypt(buffer);
                                buffer = crypto.Sign(buffer);

                                // Avoid .Replace() - be sure that we only remove from the end
                                string enc_filename = dec_entry.FileName + ".enc";

                                enc.AddEntry(enc_filename, buffer.ToArray());
                            }
                        }

                        enc.Save(to_path);
                    }
                }

                ClearStatus();
            }
            finally
            {
                Mouse.OverrideCursor = null;
            }
        }

        private void certs__SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ClearStatus();
        }

        private void clear__Click(object sender, RoutedEventArgs e)
        {
            inputPath_.Text = string.Empty;
            ClearStatus();
        }

        private void ChangeCertClick(object sender, RoutedEventArgs e)
        {
            NavigationService.Navigate(new Certificate());
        }
    }
}
