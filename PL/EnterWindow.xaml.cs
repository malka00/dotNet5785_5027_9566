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
        string Id
        {
            get => (string)GetValue(IdProperty);
            init => SetValue(IdProperty, value);
        }
        public static readonly DependencyProperty IdProperty =
         DependencyProperty.Register(nameof(Id), typeof(string), typeof(EnterWindow));

      public  string Password { get; set; }
       

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
            }
        }

        private void PasswordBox_GotFocus(object sender, RoutedEventArgs e)
        {
            
        }

        private void btnEnter_Click(object sender, RoutedEventArgs e)
        {
            BO.Volunteer currentVolunteer = null;
            try
            {
                currentVolunteer = s_bl.Volunteers.ReadString(Id);
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
                int.TryParse(Id, out int numericId);
                if (currentVolunteer!.Password != Password)
                    MessageBox.Show("wrong password!", "Error", MessageBoxButton.OK);
                else
                {
                    MessageBox.Show("WELLCOME TO SYSTEM", "WellCome");
                    if (currentVolunteer.Job == BO.Role.Boss)
                        new ManagerWindow(numericId).Show();
                    else
                        new VolunteerWindow(numericId).Show();
                }
            }
        }

        private void id_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                var textBox = sender as TextBox;
                textBox?.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
            }
        }

        private void enter_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                BO.Volunteer currentVolunteer = null;
                try
                {
                    currentVolunteer = s_bl.Volunteers.ReadString(Id);
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
                        int.TryParse(Id, out int numericId);
                        if (currentVolunteer.Job == BO.Role.Boss) 
                            try
                            {
                                new ManagerWindow(numericId).Show();
                            }
                            catch (Exception ex)
                            {
                                MessageBox.Show(ex.Message, "ERROR", MessageBoxButton.OK);
                            }


                        else
                            new VolunteerWindow(numericId).Show();

                    }
                }
            }
        }

       
    }
}