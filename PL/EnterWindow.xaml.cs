using PL.Volunteer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;


namespace PL
{
    /// <summary>
    /// Interaction logic for EnterWindow.xaml
    /// </summary>
    public partial class EnterWindow : Window
    {

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public string Password { get; set; }


         public int Id { get; set; }

        public EnterWindow()
        {
            InitializeComponent();

        }
        private void PasswordBox_PasswordChanged(object sender, RoutedEventArgs e)
        {
            if (sender is PasswordBox passwordBox)
            {
                Password = passwordBox.Password;
            }
        }

        private void PasswordBox_LostFocus(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(Password))
            {
                // אפשר להוסיף כאן לוגיקה כדי להחזרת ערך ברירת מחדל או שינוי צבע
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            // אפשר להוסיף כאן לוגיקה כדי להסיר ערך ברירת מחדל אם יש
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            BO.Volunteer currentVolunteer = null;
            try
            {
                currentVolunteer = s_bl.Volunteers.Read(Id);
            }
            catch (BO.BlDoesNotExistException ex)
            {

                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
             
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            if (currentVolunteer != null)
            {
                if (currentVolunteer!.Password != Password)
                    MessageBox.Show("wrong password!", "Error", MessageBoxButton.OK);
                else
                {
                    MessageBox.Show("WELLCOME TO SYSTEM", "WellCome");
                    if (currentVolunteer.Job == BO.Role.Boss)
                    {
                        MessageBoxResult mbResult = MessageBox.Show("Do you want to open the Manage Window?", "Manage or Volunteer",
                                             MessageBoxButton.YesNo, MessageBoxImage.Question);


                        if (mbResult == MessageBoxResult.Yes)
                            new MainWindow(Id).Show();
                        else
                            new VolunteerWindow(Id).Show();
                    }
                    else
                        new VolunteerWindow(Id).Show();

                }
            }
            

        }
    }
}
