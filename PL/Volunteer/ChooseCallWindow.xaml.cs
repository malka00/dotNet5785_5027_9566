using BO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
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

        public BO.CallType? TypeCallInList { get; set; }

        public ChooseCallWindow(int id)
        {
            VolunteerId = id;
            this.Closed += Window_Closed;
            this.Loaded += Window_Loaded;
            InitializeComponent();
        }

        private void Call_Filter(object sender, SelectionChangedEventArgs e)
        {
            OpenCallInList = (BO.EOpenCallInList)(((ComboBox)sender).SelectedItem);

            OpenCallList = s_bl?.Calls.GetOpenCall(VolunteerId, TypeCallInList, OpenCallInList)!;
        }

        private void CallSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { QueryCallList(); }

        private void QueryCallList()
        => OpenCallList = (OpenCallInList == BO.EOpenCallInList.Id) ?
        s_bl?.Calls.GetOpenCall(VolunteerId, null, null)! : s_bl?.Calls.GetOpenCall(VolunteerId, TypeCallInList, OpenCallInList)!;

        private void CallListObserver() => QueryCallList();

        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Calls.AddObserver(CallListObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Calls.RemoveObserver(CallListObserver);

    

        private void BtnChoose_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Calls.ChoseForTreat(VolunteerId, SelectedCall.Id);
                MessageBox.Show($"Call was successfully Chosen!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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

        

        private void mouseEnterLeft(object sender, MouseButtonEventArgs e)
        {
            MessageBox.Show(SelectedCall.Description, $"Description {SelectedCall.Id}", MessageBoxButton.OK);
        }
    }

}




























