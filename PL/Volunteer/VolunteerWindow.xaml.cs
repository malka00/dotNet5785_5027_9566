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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerWindow.xaml
    /// </summary>
    public partial class VolunteerWindow : Window
    {

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            init => SetValue(ButtonTextProperty, value);
        }
        public static readonly DependencyProperty ButtonTextProperty =
         DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(VolunteerWindow));

        public VolunteerWindow(int id=0)
        {
            ButtonText = id == 0 ? "Add" : "Update";
            InitializeComponent();
            try
            {
                CurrentVolunteer = (id != 0) ? s_bl.Volunteers.Read(id)! : new BO.Volunteer() { Id = 0, FullName = "", PhoneNumber = "", Email="", TypeDistance=BO.Distance.Aerial, Job=BO.Role.Volunteer, Active = false };
            }
            catch (BO.BlDoesNotExistException ex)
            {
                CurrentVolunteer = null;
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }


        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));
    }
}
