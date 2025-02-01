using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Navigation;
using BO;

namespace PL
{
    /// <summary>
    /// convert for null courdinats
    /// </summary>
    public class ConverterThreeValueNullToColor : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
           // if (values.Length < 3) return Brushes.Black;

            object firstValue = values[0];
            object secondValue = values[1];
            object thirdValue = values[2];

            if (firstValue != null && (secondValue == null || thirdValue == null))
            {
                return Brushes.Red;
            }

            return Brushes.White;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }

    /// <summary>
    /// The function checks whether the volunteer has no active call and is active.If so - returns true, otherwise false.
    /// Linked to the call selection button
    /// </summary>
    public class BoolConvertIsCallInProsses : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return false;
            }

            if (value is BO.Volunteer myValue)
            {
                return myValue.CallIn == null && myValue.Active;


            }
            return true;

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Checking whether it is possible to delete a call (CanDelete)
    /// If so → returns Visibility.Visible(the element will be visible).
    /// If not → returns Visibility.Collapsed(the element is hidden).
    /// </summary>
    public class ConvertIsCanDeletCall : IValueConverter
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (s_bl.Calls.CanDelete((int)value))
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Checking whether it is possible to delete a volunteer (CanDelete)
    /// If so → returns Visibility.Visible(the element will be visible).
    /// If not → returns Visibility.Collapsed(the element is hidden).
    /// </summary>
    public class ConvertIsCanDeletVolunteer : IValueConverter
    {
        static readonly BlApi.IBl s_bl = BlApi.Factory.Get();
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (s_bl.Volunteers.CanDelete((int)value))
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Checks whether the volunteer currently has a call in his care and if so returns Visibility.Visible (the element is displayed)
    /// </summary>
    public class ConvertIsCallInProsses : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
            {
                return Visibility.Collapsed; ;
            }
            return Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// Checks whether the user is in voluntary update mode, and if so, returns true - that is, the button for changing the id will be read-only
    /// </summary>
    public class ConvertUpdateToTrue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string buttonText)
            {
                return buttonText == "Update";
            }
            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

  
    //public class ConvertUpdateToVisible : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value is string buttonText)
    //        {
    //            return buttonText == "Update" ? Visibility.Visible : Visibility.Collapsed;
    //        }
    //        return Visibility.Collapsed;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

   

    //public class BooleanToVisibility : IValueConverter
    //{
    //    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        if (value is bool boolValue && boolValue)
    //        {
    //            return Visibility.Visible;
    //        }
    //        return Visibility.Collapsed;
    //    }


    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {

    //        if (value is Visibility visibility && visibility == Visibility.Visible)
    //        {
    //            return true;
    //        }
    //        return false;
    //    }

    //}

    /// <summary>
    /// A function that is used to return a collection of values ​​from an Enum (counter class) based on a value selected for sorting ("Status" or "CallType").
    /// </summary>
    public class ConvertStatusOrTypeToEnum : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null)
                return null;
            if (value.ToString() == "Status")
            {
                return Enum.GetValues(typeof(StatusTreat));
            }
            else if (value.ToString() == "CallType")
            {
                return Enum.GetValues(typeof(CallType));
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }

    /// <summary>
    /// A function that converts a boolean value to a string that will be displayed on the button. 
    /// Linked to the IsSimulatorRunning attribute
    /// </summary>
    public class ConverterSimulatorState : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (bool)value ? "Stop Simulator" : "Start Simulator";
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    /// <summary>
    /// A function that operates on a boolean value and returns its opposite value.
    /// If the value is true, the function returns false.
    /// If the value is false, the function returns true
    /// Linked to the IsSimulatorRunning attribute to know whether to allow clicking on the other buttons
    /// </summary>
    public class InverseBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool booleanValue)
            {
                return !booleanValue;
            }
            return false; 
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException("ConvertBack לא נתמך.");
        }
    }
}


