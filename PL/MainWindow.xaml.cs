using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;
using PL.Volunteer;

namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public DateTime CurrentTime
        {
            get { return (DateTime)GetValue(CurrentTimeProperty); }
            set { SetValue(CurrentTimeProperty, value); }
        }

        public static readonly DependencyProperty CurrentTimeProperty =
            DependencyProperty.Register("CurrentTime", typeof(DateTime), typeof(MainWindow));

        public TimeSpan MaxRange
        {
            get { return (TimeSpan)GetValue(MaxRangeProperty); }
            set { SetValue(MaxRangeProperty, value); }
        }
        public static readonly DependencyProperty MaxRangeProperty =
        DependencyProperty.Register("MaxRange", typeof(TimeSpan), typeof(MainWindow));

        public int Interval               //simulator
        {
            get { return (int)GetValue(IntervalProperty); }
            set { SetValue(IntervalProperty, value); }
        }
        public static readonly DependencyProperty IntervalProperty =
        DependencyProperty.Register("Interval", typeof(int), typeof(MainWindow));

        public bool IsSimulatorRunning     //state of simulator
        {
            get { return (bool)GetValue(IsSimulatorRunningProperty); }
            set { SetValue(IsSimulatorRunningProperty, value); }
        }
        public static readonly DependencyProperty IsSimulatorRunningProperty =
        DependencyProperty.Register("IsSimulatorRunning", typeof(bool), typeof(MainWindow));


        public int[] CountCall
        {
            get { return (int[])GetValue(CountCallProperty); }
            set { SetValue(CountCallProperty, value); }
        }
        public static readonly DependencyProperty CountCallProperty =
        DependencyProperty.Register("CountCall", typeof(int[]), typeof(MainWindow));

        public int Id { get; set; }

        public static bool IsOpen { get; set; } = false;
      
        public MainWindow(int bossId)
        {
            if (IsOpen)
                throw  new Exception("There already is one manager in the system");
            else IsOpen=true;
            Id = bossId;
            InitializeComponent();
        }


        private void btnAddOneMinute_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.MINUTE);
        }
        private void btnAddOneYear_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.YEAR);
        }
        private void btnAddOneMonth_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.MONTH);
        }
        private void btnAddOneHour_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.HOUR);
        }
        private void btnAddOneDay_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.ForwardClock(BO.TimeUnit.DAY);
        }
        private void btnUpdateMaxRange_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.setMaxRange(MaxRange);
        }
       
        private volatile DispatcherOperation? _observerClockOperation = null; //stage 7

        private void clockObserver() //stage 7
        {
            if (_observerClockOperation is null || _observerClockOperation.Status == DispatcherOperationStatus.Completed)
                _observerClockOperation = Dispatcher.BeginInvoke(() =>
                {
                    CurrentTime = s_bl.Admin.GetClock();
                });
        }

    
        private volatile DispatcherOperation? _observerConfigOperation = null; //stage 7

        private void configObserver() //stage 7
        {
            if (_observerConfigOperation is null || _observerConfigOperation.Status == DispatcherOperationStatus.Completed)
                _observerConfigOperation = Dispatcher.BeginInvoke(() =>
                {
                    MaxRange = s_bl.Admin.GetMaxRange();
                });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            MaxRange = s_bl.Admin.GetMaxRange();
            CurrentTime = s_bl.Admin.GetClock();
            MaxRange = s_bl.Admin.GetMaxRange();
            CountCall = s_bl.Calls.CountCall();

            s_bl.Admin.AddClockObserver(clockObserver);
            s_bl.Admin.AddConfigObserver(configObserver);

        }
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Admin.RemoveClockObserver(clockObserver);
            s_bl.Admin.RemoveConfigObserver(configObserver);
            IsOpen = false;
        }


        private void ButtonVolunteer_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerListWindow(Id) { Owner = this }.Show();
          //  new VolunteerListWindow(Id).Show();
          }


        private void btnInitDB_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbResult = MessageBox.Show("Are you sure you want to initialize the DB?", "Init Confirmation",
                                                        MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (mbResult == MessageBoxResult.Yes)
            {

                foreach (Window window in Application.Current.Windows)
                {
                    if (window != this)
                    {
                        window.Close();
                    }
                }
                // start the Icon
                Mouse.OverrideCursor = Cursors.Wait;

                s_bl.Admin.initialization();
                MaxRange = s_bl.Admin.GetMaxRange();
                // stop the Icon
                Mouse.OverrideCursor = null;
            }
        }
        private void btnResetDB_Click(object sender, RoutedEventArgs e) //stage 5
        {
            MessageBoxResult mbResult = MessageBox.Show("Are you sure you want to reset the DB?", "Reset Confirmation",
                                                        MessageBoxButton.YesNo, MessageBoxImage.Question);

            if (mbResult == MessageBoxResult.Yes)
            {
                // Close all open windows except the main one
                foreach (Window window in Application.Current.Windows)
                {
                    if (window != this) // Check to ensure it’s not the main window
                    {
                        window.Close();
                    }
                }
                //start the Icon
                Mouse.OverrideCursor = Cursors.Wait;
                s_bl.Admin.Reset();
                MaxRange = s_bl.Admin.GetMaxRange();

                // stop the Icon
                Mouse.OverrideCursor = null;
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        { }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        { }

        private void ButtonCall_Click(object sender, RoutedEventArgs e)
        {
            new CallInListWindow(Id) { Owner = this }.Show();
          //  new CallInListWindow(Id).Show();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {

            var managerWindow = Application.Current.Windows.OfType<ManagerWindow>().FirstOrDefault();

         
            if (managerWindow != null)
            {
                managerWindow.Show();
            }
            else
            
               new ManagerWindow(Id).Show();
                
            this.Close();
        }

      

       
        private void btnSimulator_Click(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.StartSimulator(Interval); //stage 7
          
        }
    }
}