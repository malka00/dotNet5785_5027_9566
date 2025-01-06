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

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
    public partial class VolunteerListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();



        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }

        public static readonly DependencyProperty VolunteerListProperty =
            DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

        public BO.VolunteerInList? SelectedVolunteer { get; set; }

        public BO.EVolunteerInList VolunteerInList { get; set; } = BO.EVolunteerInList.Id;

        public VolunteerListWindow()
        {
            InitializeComponent();
         
        }


        private void cbVolunteerSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

            QueryVolunteerList();
        }

        private void VolunteerFilter(object sender, SelectionChangedEventArgs e)
        {
            VolunteerInList = (BO.EVolunteerInList)(((ComboBox)sender).SelectedItem);
            VolunteerList = s_bl?.Volunteers.GetVolunteerList(null, VolunteerInList)!;
        }


        private void QueryVolunteerList()
=> VolunteerList = (VolunteerInList == BO.EVolunteerInList.Id) ?
   s_bl?.Volunteers.GetVolunteerList(null, null)! : s_bl?.Volunteers.GetVolunteerList(null, VolunteerInList)!;

        private void VolunteerListObserver()
            => QueryVolunteerList();
 
        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Volunteers.AddObserver(VolunteerListObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteers.RemoveObserver(VolunteerListObserver);

        private void dtgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (SelectedVolunteer!= null)
                new VolunteerDetailsWindow(SelectedVolunteer.Id).Show();
        }

      
        private void btnCAdd_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerDetailsWindow().Show();
        }

       
        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {

            MessageBoxResult mbResult = MessageBox.Show("Are you sure you want to delete this volunteer?", "Reset Confirmation",
                                                        MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbResult == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Volunteers.Delete(SelectedVolunteer.Id);
                }
                catch (BO.BlDeleteNotPossibleException ex)
                {
                   
                    MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            } 
            
        }

    }

}


