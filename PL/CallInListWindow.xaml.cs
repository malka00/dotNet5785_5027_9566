﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using BO;
using PL.Volunteer;


namespace PL;

/// <summary>
/// Interaction logic for CallInListWindow.xaml
/// </summary>
public partial class CallInListWindow : Window
{
    static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

    public IEnumerable<BO.CallInList> CallList
    {
        get { return (IEnumerable<BO.CallInList>)GetValue(CallListProperty); }
        set { SetValue(CallListProperty, value); }
    }

    public static readonly DependencyProperty CallListProperty =
        DependencyProperty.Register("CallList", typeof(IEnumerable<BO.CallInList>), typeof(CallInListWindow), new PropertyMetadata(null));

    public BO.CallInList? SelectedCall { get; set; }

    public BO.StatusTreat StatusCallInList { get; set; }
    
    public BO.ECallInList CallInList { get; set; } = BO.ECallInList.Id;

    public int Id { get; set; }
    public CallInListWindow(int id=0)
    {
        Id = id;
        InitializeComponent();
        DataContext = this;
    }

    private void CallSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    { queryCallList(); }

    private void CallFilter(object sender, SelectionChangedEventArgs e)
    {
        CallInList = (BO.ECallInList)(((ComboBox)sender).SelectedItem);
        CallList = s_bl?.Calls.GetCallInLists(BO.ECallInList.Status, StatusCallInList, CallInList)!;
    }


    private void queryCallList()
    => CallList = (CallInList == BO.ECallInList.Id) ?
    s_bl?.Calls.GetCallInLists(null, null, null)! : s_bl?.Calls.GetCallInLists(BO.ECallInList.Status, StatusCallInList, CallInList)!;

    private volatile DispatcherOperation? _observerOperation = null; //stage 7
    private void callListObserver() //stage 7
    {
        if (_observerOperation is null || _observerOperation.Status == DispatcherOperationStatus.Completed)
            _observerOperation = Dispatcher.BeginInvoke(() =>
            {
                queryCallList();
            });
    }

    private void Window_Loaded(object sender, RoutedEventArgs e)
        => s_bl.Calls.AddObserver(callListObserver);

    private void Window_Closed(object sender, EventArgs e)
        => s_bl.Calls.RemoveObserver(callListObserver);


    private void dtgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedCall?.CallId != null) // בדיקה אם SelectedCall ו-Id אינם null
        {
            new CallWindow(SelectedCall.CallId) { Owner = this }.Show();      //    new CallWindow(SelectedCall.CallId).Show();
        }
        else
        {
            MessageBox.Show("No call selected for editing.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void btnAdd_Click(object sender, RoutedEventArgs e)
    {
        new CallWindow() { Owner = this }.Show();
        //   new CallWindow().Show();
    }

    private void btnDelete_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult mbResult = MessageBox.Show($"Are you sure you want to delete {SelectedCall.CallId} call?", "Reset Confirmation",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (mbResult == MessageBoxResult.Yes)
        {
            try
            {
                s_bl.Calls.Delete(SelectedCall.CallId);
            }
            catch (BO.BlDeleteNotPossibleException ex)
            {
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
        }
    }
    private void Call_Filter(object sender, SelectionChangedEventArgs e)
    {
        CallInList = (BO.ECallInList)(((ComboBox)sender).SelectedItem);
        CallList = s_bl?.Calls.GetCallInLists(BO.ECallInList.Status, StatusCallInList, CallInList)!;
    }

    private void btnCancelAssignment_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult mbResult = MessageBox.Show("Are you sure you want to cancel this assignment?", "Reset Confirmation",
                                                   MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (mbResult == MessageBoxResult.Yes)
        {
            try
            {
               s_bl.Calls.CancelTreat(Id, (int)SelectedCall.Id);
            }
            catch (BO.BlDeleteNotPossibleException ex)
            {
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }
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
}







