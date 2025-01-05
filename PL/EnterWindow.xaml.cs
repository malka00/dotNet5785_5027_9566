using PL.Volunteer;
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
using System.Windows.Shapes;


namespace PL
{
    /// <summary>
    /// Interaction logic for EnterWindow.xaml
    /// </summary>
    public partial class EnterWindow : Window
    {

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
       


        public EnterWindow()
        {
           
            InitializeComponent();


        }
        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
           
        
            

            try
            {
              BO.Volunteer  currentVolunteer =  s_bl.Volunteers.Read(id);
            }
            catch (BO.BlDoesNotExistException ex)
            {
               
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            if(currentVolunteer.Password!=password)
                MessageBox.Show("wrong password!","Error", MessageBoxButton.OK);

            else
            {
                if(currentVolunteer.Job==BO.Role.Volunteer)
               
            }

        }
    }
}
