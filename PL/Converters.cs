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
                return myValue.CallIn == null&& myValue.Active;

             
            }
            return true;
         
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

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


    public class ConvertUpdateToVisible : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string buttonText)
            {
                return buttonText == "Update" ? Visibility.Visible : Visibility.Collapsed;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }



    public class BooleanToVisibility : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool boolValue && boolValue)
            {
                return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

    
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
          
            if (value is Visibility visibility && visibility == Visibility.Visible)
            {
                return true;
            }
            return false;
        }

    }




 

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


    
}


