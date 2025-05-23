﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Net.Mime.MediaTypeNames;

namespace PL.Volunteer
{
    /// <summary>
    /// Interaction logic for VolunteerDetailsWindow.xaml
    /// </summary>
    public partial class VolunteerDetailsWindow : Window
    {

        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
      
        string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            init => SetValue(ButtonTextProperty, value);
        }
        public static readonly DependencyProperty ButtonTextProperty =
         DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(VolunteerDetailsWindow));
       
        public int ManagerId { get; set; }
        

        public VolunteerDetailsWindow(int id=0, int bossId=0)
        {
            ButtonText = id == 0 ? "Add" : "Update";
            InitializeComponent();
            
            try
            {
                CurrentVolunteer = (id != 0) ? s_bl.Volunteers.Read(id)! : new BO.Volunteer() { Id = 0, FullName = "", PhoneNumber = "", Email="", TypeDistance=BO.Distance.Aerial, Job=BO.Role.Volunteer, Active = false };
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
            ManagerId = bossId;
            s_bl.Volunteers.AddObserver(CurrentVolunteer!.Id, volunteerObserver);
        }

       

        private volatile DispatcherOperation? _observerOperation = null; //stage 7

        private void volunteerObserver() //stage 7
        {
            if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
                _observerOperation = Dispatcher.BeginInvoke(() =>
                {
                    int id = CurrentVolunteer!.Id;
                    CurrentVolunteer = null;
                    CurrentVolunteer = s_bl.Volunteers.Read(id);
                });
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) 
        {
            if (CurrentVolunteer!.Id != 0)
                s_bl.Volunteers.AddObserver(CurrentVolunteer!.Id, volunteerObserver);
        }
        private void Window_Closed(object sender, EventArgs e) 
        {
            s_bl.Volunteers.RemoveObserver(CurrentVolunteer!.Id, volunteerObserver);
        }

        public BO.Volunteer? CurrentVolunteer
        {
            get { return (BO.Volunteer?)GetValue(CurrentVolunteerProperty); }
            set { SetValue(CurrentVolunteerProperty, value); }
        }

        public static readonly DependencyProperty CurrentVolunteerProperty =
            DependencyProperty.Register("CurrentVolunteer", typeof(BO.Volunteer), typeof(VolunteerDetailsWindow), new PropertyMetadata(null));


        private void btnAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonText == "Add")
                try {
                    s_bl.Volunteers.Create(CurrentVolunteer!);
                    MessageBox.Show($"Volunteer {CurrentVolunteer?.Id} was successfully added!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                   this.Close();
                }
                catch (BO.BlAlreadyExistsException ex)
                {
                    MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                }
            else
                try 
                { 
                    s_bl.Volunteers.Update(ManagerId, CurrentVolunteer!);
                    MessageBox.Show($"Volunteer {CurrentVolunteer?.Id} was successfully updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (BO.BlWrongInputException ex)
                {
                    MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
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

        private void TextBox_TextChanged(object sender, TextChangedEventArgs e)
        {

        }

        private void ShowDescription(object sender, MouseEventArgs e)
        {

        }

        private void btnBack_Click(object sender, RoutedEventArgs e)
        {
            var VolunteerListWindow = System.Windows.Application.Current.MainWindow;
            if (VolunteerListWindow != null)
            {
                VolunteerListWindow.Show();
            }
            this.Close();
        }
    }
}
