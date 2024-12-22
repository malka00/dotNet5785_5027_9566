using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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
        
       
        public TimeSpan MaxRange
        {
            get { return (TimeSpan)GetValue(MyPropertyProperty); }
            set { SetValue(MyPropertyProperty, value); }
        }

        // Using a DependencyProperty as the backing store for MyProperty.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty MyPropertyProperty =
            DependencyProperty.Register("MaxRange", typeof(TimeSpan), typeof(MainWindow), new PropertyMetadata(TimeSpan.Zero));
        
        private void txtBMaxRange(object sender, RoutedEventArgs e)
        {
            s_bl.Admin.Definition(MaxRange);
        }

        





        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }
    }
}