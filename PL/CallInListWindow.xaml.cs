using System;
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
using BO;


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

    public BO.ECallInList CallInList { get; set; } = BO.ECallInList.Id;

    public CallInListWindow()
    {
        InitializeComponent();
        DataContext = this;
    }

    private void CallSelector_SelectionChanged(object sender, SelectionChangedEventArgs e)
    { QueryCallList(); }

    private void CallFilter(object sender, SelectionChangedEventArgs e)
    {
        CallInList = (BO.ECallInList)(((ComboBox)sender).SelectedItem);
        CallList = s_bl?.Calls.GetCallInLists(null, null, CallInList)!;
    }

    private void QueryCallList()
    => CallList = (CallInList == BO.ECallInList.Id) ?
    s_bl?.Calls.GetCallInLists(null, null, null)! : s_bl?.Calls.GetCallInLists(null, null, CallInList)!;

    private void CallListObserver() => QueryCallList();

    private void Window_Loaded(object sender, RoutedEventArgs e)
        => s_bl.Calls.AddObserver(CallListObserver);

    private void Window_Closed(object sender, EventArgs e)
        => s_bl.Calls.RemoveObserver(CallListObserver);

    private void dtgList_MouseDoubleClick(object sender, MouseButtonEventArgs e)
    {
        if (SelectedCall?.CallId != null) // בדיקה אם SelectedCall ו-Id אינם null
        {
            new CallWindow(SelectedCall.CallId).Show();
        }
        else
        {
            MessageBox.Show("No call selected for editing.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    private void ButtonAdd_Click(object sender, RoutedEventArgs e)
    {
        new CallWindow().Show();
    }

    private void ButtonDelete_Click(object sender, RoutedEventArgs e)
    {
        MessageBoxResult mbResult = MessageBox.Show("Are you sure you want to delete this call?", "Reset Confirmation",
                                                    MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (mbResult == MessageBoxResult.Yes)
        {
            try
            {
                if (SelectedCall?.Id != null)
                {
                    s_bl.Calls.Delete(SelectedCall.Id.Value);
                }
                else
                {
                    MessageBox.Show("No call selected for deletion.", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
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




    //public class MainViewModel : INotifyPropertyChanged
    //{
    //    public ObservableCollection<string> StatusOrTypeOptions { get; set; }
    //    public string SelectedStatusOrType { get; set; }
    //    public object SelectedSecondOption { get; set; }

    //    public MainViewModel()
    //    {
    //        StatusOrTypeOptions = new ObservableCollection<string> { "Status", "CallType" };
    //    }

    //    public event PropertyChangedEventHandler? PropertyChanged;
    //}


}






