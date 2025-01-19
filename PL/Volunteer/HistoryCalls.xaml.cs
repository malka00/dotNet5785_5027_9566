using BO;
using DO;
using System;
using System.Collections.Generic;
using System.Globalization;
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
    //    /// <summary>
    //    /// Interaction logic for History_Calls.xaml
    //    /// </summary>
    public partial class HistoryCalls : Window
    {

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public IEnumerable<BO.ClosedCallInList> ClosedCallList
        {
            get { return (IEnumerable<BO.ClosedCallInList>)GetValue(ClosedCallListProperty); }
            set { SetValue(ClosedCallListProperty, value); }
        }

        public static readonly DependencyProperty ClosedCallListProperty =
            DependencyProperty.Register("ClosedCallList", typeof(IEnumerable<BO.ClosedCallInList>), typeof(HistoryCalls), new PropertyMetadata(null));


        public BO.ClosedCallInList? SelectedColsedCall { get; set; }

        public BO.EClosedCallInList ClosedCallInList { get; set; }
       
        public int IdVolunteer  { get; set; }

        public BO.CallType? TypeCallInList { get; set; }

        public HistoryCalls(int id)
        {
            IdVolunteer = id;
            InitializeComponent();
            DataContext = this;

        }

        private void cbVSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            queryClosedCallList();
        }

        private void Call_Filter(object sender, SelectionChangedEventArgs e)
        {
            ClosedCallInList = (BO.EClosedCallInList)(((ComboBox)sender).SelectedItem);
            ClosedCallList = s_bl?.Calls.GetClosedCall(IdVolunteer, TypeCallInList, ClosedCallInList)!;
        }
       

        private void queryClosedCallList()
    => ClosedCallList = s_bl?.Calls.GetClosedCall(IdVolunteer, TypeCallInList, ClosedCallInList)!;

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var VolunteerWindow = System.Windows.Application.Current.MainWindow;
            if (VolunteerWindow != null)
            {
                VolunteerWindow.Show();
            }

            // סגור את החלון הנוכחי
            this.Close();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}