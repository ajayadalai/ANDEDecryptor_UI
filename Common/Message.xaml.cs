// -----------------------------------------------------------------------
// © 2017 ANDE Corporation.All rights reserved
// -----------------------------------------------------------------------

namespace NBDecryptor.Common
{
    using System.Windows;

    /// <summary>
    /// Interaction logic for Message.xaml
    /// </summary>
    public partial class Message : Window
    {
        static bool BoolisMainWindow = false;
        public Message(string purpose, string message)
        {
            try
            {
                InitializeComponent();

                // Needed so that WindowStartupLocation = CenterOwner takes effect
                if (!(Application.Current.MainWindow is Message))
                {
                    Owner = Application.Current.MainWindow;
                }

                purpose_.Text = purpose;

                if (!string.IsNullOrEmpty(purpose) && purpose != "Message")
                {
                    message_.Text = string.Format("{0}: {1}", purpose, message);
                }
                else
                {
                    message_.Text = message;
                }
            }
            catch
            {

            }
        }

        public Message(string purpose, string message,bool flag)
        {
            try
            {
                InitializeComponent();

                // Needed so that WindowStartupLocation = CenterOwner takes effect
                //if (!(Application.Current.MainWindow is Message))
                //{
                //    Owner = Application.Current.MainWindow;
                //}

                purpose_.Text = purpose;

                if (!string.IsNullOrEmpty(purpose) && purpose != "Message")
                {
                    message_.Text = string.Format("{0}: {1}", purpose, message);
                }
                else
                {
                    message_.Text = message;
                }
            }
            catch
            {

            }
        }

        private void OKClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public static void Display(string purpose, string message)
        {
            BoolisMainWindow = false;
            Display(purpose, message, false);
        }

        public static void Display(string purpose, string message,string isMainWindow)
        {
            BoolisMainWindow = true;
            Display(purpose, message, false);
        }

        public static void Display(string purpose, string message, bool widen)
        {
            Message msg = null;
            if (BoolisMainWindow)
            {
                 msg = new Message(purpose, message,true);
            }
            else
            {
                 msg = new Message(purpose, message);
            }
            

            if (widen)
            {
                // Widen window and subordinate elements
                int inc = 200;
                msg.purpose_.Width += inc;
                msg.message_.Width += inc;
                msg.messageWindow_.Width += inc;
                msg.grid_.Width += inc;

                // Move button over
                Thickness new_margin = msg.ok_.Margin;
                new_margin.Left += inc / 2;
                //msg.ok_.Margin = new_margin;
            }

            msg.ShowDialog();
        }

        public static void Err_CouldNotFindCert(string serialNum)
        {
            Message.Display("Error", string.Format("Could not find decryption key for instrument with serial number ending with {0}", serialNum));
        }

        public static void Err_CouldNotDecrypt_CryptoEx(string exMessage)
        {
            //Message.Display("Error", AppendSupportNote("Could not decrypt.  Please ensure that the correct certificate is installed on this instrument.", exMessage));
            var errorMessage = string.Format("ANDE FAIRS cannot decrypt the data for import using the currently installed certificate. Please verify that the correct certificate installed in ANDE FAIRS is consistent with the certificate installed in ANDE instrument {0}.");
            Message.Display("Error", AppendSupportNote("Could not decrypt.  Please ensure that the correct certificate is installed on this instrument.", exMessage));
        }

        public static void Err_CouldNotDecrypt_CryptoEx(string exMessage, string instrumentName)
        {
            var errorMessage = string.Format("ANDE FAIRS cannot decrypt the data for import using the currently installed certificate. Please verify that the correct certificate installed in ANDE FAIRS is consistent with the certificate installed in ANDE instrument {0}.", instrumentName);
            Message.Display("Error", AppendSupportNote(errorMessage, exMessage));
        }

        public static void Err_CouldNotDecrypt_Generic(string exMessage)
        {
            Message.Display("Error", AppendSupportNote("Could not decrypt.", exMessage));
        }

        public static void Err_NoUSBDriveInsertedTryAgain()
        {
            Message.Display("Message", "There is no USB stick in the USB port, please insert USB stick and try again.");
        }

        public static string AppendSupportNote(string message, string exMessage)
        {
            return string.Format("{0}\n\nTroubleshooting Information: report error message below to technical support:\n{1}", message, exMessage);
        }
    }
}

// -----------------------------------------------------------------------
// © 2017 ANDE Corporation.All rights reserved
// -----------------------------------------------------------------------
