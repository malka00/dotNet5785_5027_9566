using PL.Volunteer;
using PL;
using System.ComponentModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

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
            s_bl.Admin.Definition(MaxRange);
        }
        private void clockObserver()
        {
            CurrentTime = s_bl.Admin.GetClock();
        }
        private void configObserver()
        {
            MaxRange = s_bl.Admin.GetMaxRange();
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
        { new VolunteerListWindow(Id).Show(); }


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
        { new CallInListWindow(Id).Show(); }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {

            var enterWindow = Application.Current.Windows.OfType<EnterWindow>().FirstOrDefault();

            // אם החלון כבר קיים, מציג אותו
            if (enterWindow != null)
            {
                enterWindow.Show();
            }
            else
            {
                // אם לא נמצא, יוצר את החלון ומציג אותו
                EnterWindow newEnterWindow = new EnterWindow();
                newEnterWindow.Show();
            }

            // סגור את החלון הנוכחי
            this.Close();
        }

    }
}