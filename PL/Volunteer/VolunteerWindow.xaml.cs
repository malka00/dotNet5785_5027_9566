using PL.Volunteer;
using System;
using System.Windows;
using System.Windows.Threading;

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


        public int UserId { get; set; }

        public VolunteerWindow(int id = 0)
        {
         

            UserId = id;
            
            try
            {
                CurrentVolunteer = s_bl.Volunteers.Read(UserId);
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
            s_bl.Volunteers.AddObserver(UserId, volunteerObserver);
            //if(Call != null)
            if (CurrentVolunteer.CallIn != null)
                s_bl.Calls.AddObserver(UserId, callObserver);
            InitializeComponent();
        }


        private void queryVolunteer()
        { CurrentVolunteer = s_bl.Volunteers.Read(UserId); }
        private void queryCall()
        {
            queryVolunteer();
            if (CurrentVolunteer.CallIn != null)
            {
                if (CurrentVolunteer.CallIn == null)
                {
                    s_bl.Calls.RemoveObserver(CurrentVolunteer.CallIn.IdCall, callObserver);

                }

            }
            if (CurrentVolunteer.CallIn != null)
            {
                s_bl.Calls.AddObserver(CurrentVolunteer.CallIn.IdCall, callObserver);
            }
        }

        private volatile DispatcherOperation? _observerCallOperation = null; //stage 7
        private void callObserver() //stage 7
        {
            if (_observerCallOperation is null || _observerCallOperation.Status == DispatcherOperationStatus.Completed)
                _observerCallOperation = Dispatcher.BeginInvoke(() =>
                { 
                    queryCall();
                });
        }

        private volatile DispatcherOperation? _observerVolunteerOperation = null; //stage 7
        private void volunteerObserver() //stage 7
        {
            if (_observerVolunteerOperation is null || _observerVolunteerOperation.Status == DispatcherOperationStatus.Completed)
                _observerVolunteerOperation = Dispatcher.BeginInvoke(() =>
                {
                    queryCall();
                });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer!.Id != 0)
                s_bl.Volunteers.AddObserver(CurrentVolunteer!.Id, volunteerObserver);
            if (CurrentVolunteer.CallIn != null)
                s_bl.Calls.AddObserver(CurrentVolunteer.CallIn.IdCall, callObserver);

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Volunteers.RemoveObserver(CurrentVolunteer!.Id, volunteerObserver);
            if (CurrentVolunteer.CallIn != null)
                s_bl.Calls.RemoveObserver(CurrentVolunteer.CallIn.IdCall, callObserver);

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
            MessageBoxResult mbResult = MessageBox.Show("Are you sure you want to close this call?", "Reset Confirmation",
                                                   MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbResult == MessageBoxResult.Yes)
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
        }

        private void btnCansel_Call(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbResult = MessageBox.Show("Are you sure you want to cancel this call?", "Reset Confirmation",
                                                   MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbResult == MessageBoxResult.Yes)
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
        }
       private void btnBack_Click(object sender, RoutedEventArgs e)
        {

            var ManagerWindow = Application.Current.MainWindow;
            if (ManagerWindow != null)
            {
                ManagerWindow.Show();
            }

   
            this.Close();
        }
    }
}

