using BO;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net.NetworkInformation;
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
using static System.Net.Mime.MediaTypeNames;


namespace PL
{
    /// <summary>
    /// Interaction logic for CallWindow.xaml
    /// </summary>
    public partial class CallWindow : Window
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();

        string ButtonText
        {
            get => (string)GetValue(ButtonTextProperty);
            init => SetValue(ButtonTextProperty, value);
        }
        public static readonly DependencyProperty ButtonTextProperty =
         DependencyProperty.Register(nameof(ButtonText), typeof(string), typeof(CallWindow));

        BO.StatusTreat Status
        {
            get => (BO.StatusTreat)GetValue(StatusProperty);
            init => SetValue(StatusProperty, value);
        }
        public static readonly DependencyProperty StatusProperty =
         DependencyProperty.Register(nameof(Status), typeof(BO.StatusTreat), typeof(CallWindow));

        public BO.Call? CurrentCall
        {
            get { return (BO.Call?)GetValue(CurrentCallProperty); }
            set { SetValue(CurrentCallProperty, value); }
        }

        public static readonly DependencyProperty CurrentCallProperty =
            DependencyProperty.Register("CurrentCall", typeof(BO.Call), typeof(CallWindow), new PropertyMetadata(null));

        public bool IsTextBoxVisible
        {
            get { return (bool)GetValue(IsTextBoxVisibleProperty); }
            set { SetValue(IsTextBoxVisibleProperty, value); }
        }
        public static readonly DependencyProperty IsTextBoxVisibleProperty =
            DependencyProperty.Register("IsTextBoxVisible", typeof(bool), typeof(CallWindow), new PropertyMetadata(false));



        public CallWindow(int id = 0)
        {
            ButtonText = id == 0 ? "Add" : "Update";
            try
            {
                CurrentCall = (id != 0) ? s_bl.Calls.Read(id)! : new BO.Call() { Id = 0, Type = BO.CallType.None, Description = "", FullAddress = "", Latitude =0, Longitude = 0, TimeOpened = DateTime.Now };
                Status = id == 0 ?BO.StatusTreat.Open: CurrentCall.Status;
            }
            catch (BO.BlDoesNotExistException ex)
            {
                CurrentCall = null;
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Operation Fail", MessageBoxButton.OK, MessageBoxImage.Exclamation);
            }

            s_bl.Calls.AddObserver(CurrentCall!.Id, CallObserver);
            InitializeComponent();
        }

        private void CallObserver()
        {
            int id = CurrentCall!.Id;
            CurrentCall = null;
            CurrentCall = s_bl.Calls.Read(id);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (CurrentCall!.Id != 0)
                s_bl.Calls.AddObserver(CurrentCall!.Id, CallObserver);
        }
        private void Window_Closed(object sender, EventArgs e)
        {
            s_bl.Calls.RemoveObserver(CurrentCall!.Id, CallObserver);
        }

    
        private void buttonAddUpdate_Click(object sender, RoutedEventArgs e)
        {
            if (ButtonText == "Add")
                try
                {
                    s_bl.Calls.Create(CurrentCall!);
                    MessageBox.Show($"Call {CurrentCall?.Id} was successfully added!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
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
                    s_bl.Calls.Update(CurrentCall!);
                    MessageBox.Show($"Call {CurrentCall?.Id} was successfully updated!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                    this.Close();
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
    }
}

