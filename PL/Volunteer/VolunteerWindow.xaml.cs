using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using static System.Net.Mime.MediaTypeNames;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerWindow.xaml
    /// </summary>
    public partial class VolunteerWindow : Window
    {

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();


        //string ButtonText
        //{
        //    get => (string)GetValue(ButtonTextProperty);
        //    init => SetValue(ButtonTextProperty, value);
        //}
        //public static readonly DependencyProperty ButtonTextProperty =
        // DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(VolunteerWindow));

        public int userId {  get; set; }
        public VolunteerWindow(int id = 0)
        {
            userId=id;
         InitializeComponent();

        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

        }
        private void Window_Closed(object sender, EventArgs e)
        {

        }

        private void btnPersonalDetails_Click(object sender, RoutedEventArgs e)
        {
        }
        private void btnCallsHistory_Click(object sender, RoutedEventArgs e)
        {
            new HistoryCalls(userId).Show();
        }



    }

}
