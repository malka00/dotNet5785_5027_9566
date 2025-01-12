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
    /// Interaction logic for ChooseCallWindow.xaml
    /// </summary>
    public partial class ChooseCallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public BO.EOpenCallInList OpenCallInList { get; set; } = BO.EOpenCallInList.Id;
        public BO.OpenCallInList? SelectedCall { get; set; }
        public int VolunteerId { get; set; }
        public IEnumerable<BO.OpenCallInList> OpenCallList
        {
            get { return (IEnumerable<BO.OpenCallInList>)GetValue(OpenCallListProperty); }
            set { SetValue(OpenCallListProperty, value); }
        }

        public static readonly DependencyProperty OpenCallListProperty =
            DependencyProperty.Register("OpenCallList", typeof(IEnumerable<BO.OpenCallInList>), typeof(ChooseCallWindow), new PropertyMetadata(null));

        public ChooseCallWindow(int id)
        {
            VolunteerId = id;
            InitializeComponent();
        }

        private void CallSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { QueryCallList(); }

        private void CallFilter(object sender, SelectionChangedEventArgs e)
        {
            OpenCallInList = (BO.EOpenCallInList)(((ComboBox)sender).SelectedItem);
            OpenCallList = s_bl?.Calls.GetOpenCall(VolunteerId, null, OpenCallInList)!;
        }

        private void QueryCallList()
        => OpenCallList = (OpenCallInList == BO.EOpenCallInList.Id) ?
        s_bl?.Calls.GetOpenCall(VolunteerId, null, null)! : s_bl?.Calls.GetOpenCall(VolunteerId, null, OpenCallInList)!;

        private void CallListObserver() => QueryCallList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Calls.AddObserver(CallListObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Calls.RemoveObserver(CallListObserver);

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void BtnChoose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Calls.ChoseForTreat(VolunteerId, SelectedCall.Id);
                MessageBox.Show($"Call {SelectedCall.Id} was successfully Chosen!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                this.Close();
            }

            catch (BO.BlAlreadyExistsException ex)

            {
                MessageBox.Show(ex.Message, "Choose Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Choose Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

        }
    }
}




//   

//    




//    private void dtgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
//    {
//        if (SelectedCall?.Id != null) // בדיקה אם SelectedCall ו-Id אינם null
//        {
//            new CallWindow(SelectedCall.CallId).Show();
//        }
//        else
//        {
//            MessageBox.Show("No call selected for editing.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
//        }
//    }

//    private void ButtonAdd_Click(object sender, RoutedEventArgs e)
//    {
//        new CallWindow().Show();
//    }

//    private void ButtonDelete_Click(object sender, RoutedEventArgs e)
//    {
//        MessageBoxResult mbResult = MessageBox.Show("Are you sure you want to delete this call?", "Reset Confirmation",
//                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
//        if (mbResult == MessageBoxResult.Yes)
//        {
//            try
//            {
//                if (SelectedCall?.Id != null)
//                {
//                    s_bl.Calls.Delete(SelectedCall.Id.Value);
//                }
//                else
//                {
//                    MessageBox.Show("No call selected for deletion.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
//                }
//            }
//            catch (BO.BlDeleteNotPossibleException ex)
//            {
//                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
//                this.Close();
//            }
//            catch (Exception ex)
//            {
//                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
//            }
//        }
//    }
















