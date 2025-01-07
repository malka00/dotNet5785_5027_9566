using BO;
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
    //    /// <summary>
    //    /// Interaction logic for History_Calls.xaml
    //    /// </summary>
    public partial class History_Calls : Window
    {

        //        public IEnumerable<BO.EClosedCallInList> VolunteerList
        //        {
        //            get { return (IEnumerable<BO.ClosedCallInList>)GetValue(ClosedCallInListProperty); }
        //            set { SetValue(ClosedCallInListProperty, value); }
        //        }

        //        public static readonly DependencyProperty ClosedCallInListProperty =
        //            DependencyProperty.Register("ClosedCallInListProperty", typeof(IEnumerable<BO.VolunteerInList>), typeof(History_Calls), new PropertyMetadata(null));

        //        // public BO.VolunteerInList? SelectedVolunteer { get; set; }

        //        public BO.EVolunteerInList VolunteerInList { get; set; } = BO.EVolunteerInList.Id;



        //        public History_Calls()
        //        {
        //            InitializeComponent();
        //        }




        //        private void cbVolunteerSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //        {

        //            queryClosedCallList();
        //        }

        //        private void VolunteerFilter(object sender, SelectionChangedEventArgs e)
        //        {
        //            VolunteerInList = (BO.EVolunteerInList)(((ComboBox)sender).SelectedItem);
        //            VolunteerList = s_bl?.Volunteers.GetVolunteerList(null, VolunteerInList)!;
        //        }


        //        private void queryClosedCallList()
        //    => ClosedCallList = (ClosedCallList == BO.EClosedCallInList.Id) ?
        //    s_bl?.Volunteers.GetVolunteerList(null, null)! : s_bl?.Volunteers.GetVolunteerList(null, VolunteerInList)!;


    }
}