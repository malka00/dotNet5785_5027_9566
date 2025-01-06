using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace PL
{
    public class ConvertUpdateToTrue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string buttonText)
            {
                return buttonText == "Update"; // אם ב-"עדכון", הופך ל-True
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
        // המרת ערך Boolean ל-Visibility
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // אם הערך הוא true, מחזירים Visibility.Visible
            if (value is bool boolValue && boolValue)
            {
                return Visibility.Visible;
            }
            // אם הערך הוא false, מחזירים Visibility.Collapsed
            return Visibility.Collapsed;
        }

        // המרת Visibility חזרה ל-Boolean (לא חובה, אם לא משתמשים ב-ConvertBack)
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // אם הערך הוא Visibility.Visible מחזירים true, אחרת false
            if (value is Visibility visibility && visibility == Visibility.Visible)
            {
                return true;
            }
            return false;
        }
    }
}