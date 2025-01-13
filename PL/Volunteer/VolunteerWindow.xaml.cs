using PL.Volunteer;
using System;
using System.Windows;

namespace PL.Volunteer
{
    public partial class VolunteerWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerWindow), new PropertyMetadata(null));

        public BO.Call? Call
        {
            get { return (BO.Call?)GetValue(CallProperty); }
            set { SetValue(CallProperty, value); }
        }

        /// <summary>
        /// Dependency property for CurrentVolunteer
        /// </summary>
        public static readonly DependencyProperty CallProperty =
            DependencyProperty.Register("Call", typeof(BO.Call), typeof(VolunteerWindow), new PropertyMetadata(null));


        public int UserId { get; set; }

        public VolunteerWindow(int id = 0)
        {
         

            UserId = id;

            try
            {
                CurrentVolunteer = s_bl.Volunteers.Read(UserId);
                if (CurrentVolunteer.CallIn != null) 
                Call = s_bl.Calls.Read(CurrentVolunteer.CallIn.IdCall); 
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

            this.DataContext = this;
            s_bl.Volunteers.AddObserver(UserId, VolunteerObserver);
            if(Call != null)
                s_bl.Calls.AddObserver(UserId, CallObserver);
            InitializeComponent();
        }


        private void CallObserver()
            => QueryCall();

        private void QueryVolunteer()
        { CurrentVolunteer = s_bl.Volunteers.Read(UserId); }

        private void QueryCall()
        {
            QueryVolunteer();
            if (Call != null)
            {
                if (CurrentVolunteer.CallIn == null || CurrentVolunteer.CallIn.IdCall != Call.Id)
                {
                    s_bl.Calls.RemoveObserver(Call.Id, CallObserver);

                }

            }
            if (CurrentVolunteer.CallIn != null && Call != null && CurrentVolunteer.CallIn.IdCall != Call.Id)
            {
                s_bl.Calls.AddObserver(CurrentVolunteer.CallIn.IdCall, CallObserver);
            }
            else

                Call = null;
            if (CurrentVolunteer.CallIn != null)
            {

                Call = s_bl.Calls.Read(CurrentVolunteer.CallIn.IdCall);
            }
        }

        private void VolunteerObserver()
        {
            //CurrentVolunteer = s_bl.Volunteers.Read(UserId);
            QueryCall();
        }
       
      
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer!.Id != 0)
                s_bl.Volunteers.AddObserver(CurrentVolunteer!.Id, VolunteerObserver);
            if (Call != null)
                s_bl.Calls.AddObserver(Call.Id, CallObserver);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Volunteers.RemoveObserver(CurrentVolunteer!.Id, VolunteerObserver);
            if (Call != null)
                s_bl.Calls.RemoveObserver(Call.Id, CallObserver);
        }

        private void btnCallsHistory_Click(object sender, RoutedEventArgs e)
        {
            new HistoryCalls(UserId).Show();
        }

        private void btnUpdate_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Volunteers.Update(CurrentVolunteer.Id, CurrentVolunteer!);
                MessageBox.Show($" Successfully updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
             
            }
            catch (BO.BlDoesNotExistException ex)
            {
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnChooseCall_Click(object sender, RoutedEventArgs e)
        {
            new ChooseCallWindow(UserId).Show();
           
        }

        private void btnClosed_Call(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Calls.CloseTreat(UserId, CurrentVolunteer.CallIn.Id);
                MessageBox.Show($"Call was successfully Closed!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
               
            }
            catch (BO.BlDeleteNotPossibleException ex)
            {
                MessageBox.Show(ex.Message, "Close Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Close Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnCansel_Call(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Calls.CancelTreat(UserId, CurrentVolunteer.CallIn.Id);
                MessageBox.Show($"Call was successfully Canceld!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            }
            catch (BO.BlDeleteNotPossibleException ex)
            {
                MessageBox.Show(ex.Message, "Cancel Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Cancel Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
       private void btnBack_Click(object sender, RoutedEventArgs e)
        {

            var MainWindow = Application.Current.MainWindow;
            if (MainWindow != null)
            {
                MainWindow.Show();
            }

            // סגור את החלון הנוכחי
            this.Close();
        }
    }
}

