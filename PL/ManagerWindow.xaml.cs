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
using PL.Volunteer;

namespace PL
{
    /// <summary>
    /// Interaction logic for ManagerWindow.xaml
    /// </summary>
    public partial class ManagerWindow : Window
    {
        int Id{  get; set; }
        public ManagerWindow(int id)
        {
            Id= id;
            InitializeComponent();
        }

        private void btnMainWindow_Click(object sender, RoutedEventArgs e)
        {
            new MainWindow(Id).Show();
        }

        private void btnVolunteerWindow_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerWindow(Id).Show();
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            // יצירת מופע חדש של חלון הכניסה
            var loginWindow = new EnterWindow();

            // הצגת חלון הכניסה
            new EnterWindow().Show();

            // סגירת החלון הנוכחי
            this.Close();
        }
    }
}
