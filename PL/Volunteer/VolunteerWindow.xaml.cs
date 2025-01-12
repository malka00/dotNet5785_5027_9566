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

            s_bl.Volunteers.AddObserver(UserId, VolunteerObserver);
            InitializeComponent();
        }

        private void VolunteerObserver()
        {
            CurrentVolunteer = s_bl.Volunteers.Read(UserId);
        }
       
      
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentVolunteer!.Id != 0)
                s_bl.Volunteers.AddObserver(CurrentVolunteer!.Id, VolunteerObserver);
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Volunteers.RemoveObserver(CurrentVolunteer!.Id, VolunteerObserver);
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
                s_bl.Volunteers.AddObserver(UserId, VolunteerObserver);
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
            s_bl.Volunteers.AddObserver(CurrentVolunteer.Id, VolunteerObserver);
        }

        private void btnClosed_Call(object sender, RoutedEventArgs e)
        {
            try
            {
                s_bl.Calls.CloseTreat(UserId, CurrentVolunteer.CallIn.Id);
                MessageBox.Show($"Call was successfully Closed!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                s_bl.Volunteers.AddObserver(CurrentVolunteer.Id, VolunteerObserver);
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
                s_bl.Volunteers.AddObserver(CurrentVolunteer.Id, VolunteerObserver);
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
}
