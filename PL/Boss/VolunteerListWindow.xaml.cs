﻿using System;
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
using System.Windows.Threading;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerListWindow.xaml
    /// </summary>
    public partial class VolunteerListWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        public IEnumerable<BO.VolunteerInList> VolunteerList
        {
            get { return (IEnumerable<BO.VolunteerInList>)GetValue(VolunteerListProperty); }
            set { SetValue(VolunteerListProperty, value); }
        }

        public static readonly DependencyProperty VolunteerListProperty =
        DependencyProperty.Register("VolunteerList", typeof(IEnumerable<BO.VolunteerInList>), typeof(VolunteerListWindow), new PropertyMetadata(null));

        public BO.VolunteerInList? SelectedVolunteer { get; set; }

        public BO.EVolunteerInList VolunteerInList { get; set; } = BO.EVolunteerInList.Id;
        public int Id { get; set; }
        public VolunteerListWindow(int bossdId)
        {
            Id = bossdId;
            InitializeComponent();
        }

        private void VolunteerSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
        { queryVolunteerList();}

        private void VolunteerFilter(object sender, SelectionChangedEventArgs e)
        {
            VolunteerInList = (BO.EVolunteerInList)(((ComboBox)sender).SelectedItem);
            VolunteerList = s_bl?.Volunteers.GetVolunteerList(null, VolunteerInList)!;
        }

        private void queryVolunteerList()
        => VolunteerList = (VolunteerInList == BO.EVolunteerInList.Id) ?
        s_bl?.Volunteers.GetVolunteerList(null, null)! : s_bl?.Volunteers.GetVolunteerList(null, VolunteerInList)!;

        
        private volatile DispatcherOperation? _observerOperation = null; //stage 7
        private void volunteerListObserver() //stage 7
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    queryVolunteerList();
                });
        }



        private void Window_Loaded(object sender, RoutedEventArgs e)
            => s_bl.Volunteers.AddObserver(volunteerListObserver);

        private void Window_Closed(object sender, EventArgs e)
            => s_bl.Volunteers.RemoveObserver(volunteerListObserver);

        private void dtgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is DataGrid dataGrid && dataGrid.SelectedItem is BO.VolunteerInList selectedVolunteer)
            {

                new VolunteerDetailsWindow(SelectedVolunteer.Id, Id) { Owner = this }.Show();

            }
            else
            {
                MessageBox.Show("No volunteer selected. Please select a volunteer.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            }
        }
  
        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            new VolunteerDetailsWindow() { Owner = this }.Show();
           // new VolunteerDetailsWindow().Show();
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e)
        {
            MessageBoxResult mbResult = MessageBox.Show("Are you sure you want to delete this volunteer?", "Reset Confirmation",
                                                        MessageBoxButton.YesNo, MessageBoxImage.Question);
            if (mbResult == MessageBoxResult.Yes)
            {
                try
                {
                    s_bl.Volunteers.Delete(SelectedVolunteer.Id);
                }
                catch (BO.BlDeleteNotPossibleException ex)
                {
                    MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            } 
        }
        private bool isFiltered = false;

        private void btnFilter_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (isFiltered)
                {
                    VolunteerList = s_bl?.Volunteers.GetVolunteerList(null, VolunteerInList)!;
                    isFiltered = false;
                    ((Button)sender).Content = "Filter active volunteer";
                }
                else
                {
                    
                    VolunteerList = s_bl?.Volunteers.GetVolunteerList(true, VolunteerInList)!;
                    isFiltered = true;
                    ((Button)sender).Content = "Show all volunteers";
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow;
            if (mainWindow != null)
            {
                mainWindow.Show();
            }
            this.Close();
        }

        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}


