using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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
    /// Interaction logic for Summary.xaml
    /// </summary>
    public partial class Summary : Page
    {
        public Summary()
        {
            InitializeComponent();
        }

        public Summary(string status)
        {
            InitializeComponent();

            List<SuccessSummary> summary = new List<SuccessSummary>();
            var summaryStatus = status.Split('/');

            foreach (var item in summaryStatus)
            {
                if (!string.IsNullOrEmpty(item))
                {
                    var sum = item.Split('|');
                    summary.Add(new SuccessSummary()
                    {
                        FileName = string.Format("Filename: {0}", sum[0]),
                        SuccessCount = sum.Length > 1 ? string.Format("Success Count: {0}", sum[1]) : "",
                        FailedFiles = sum.Length > 2 ? sum[2].Split(',').ToList() : null,
                    });
                }
            }

            this.DataContext = this;
            this.liste.ItemsSource = summary;
        }

        private void back__Click(object sender, RoutedEventArgs e)
        {
            NavigationService navService = NavigationService.GetNavigationService(this);
            Decryptor decryptorPage = new Decryptor();
            navService.Navigate(decryptorPage);
        }
    }


    public class SuccessSummary
    {
        public string FileName { get; set; }
        public string SuccessCount { get; set; }
        public List<string> FailedFiles { get; set; }
    }


}
