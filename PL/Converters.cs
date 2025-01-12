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

    //public class BoolConvertIsCallInProsses : IValueConverter
    //{
    //    public object Convert(Object  value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        // אם הערך הוא null, מחזיר false
    //        bool result = (value.CallIn != null && value.Active);
            

    //        // אם יש פרמטר, בודקים אם הוא שווה ל-False והופכים את התוצאה
    //        if (parameter is string param && bool.TryParse(param, out bool invert) && invert)
    //        {
    //            result = !result;
    //        }

    //        return result;
    //    }

    //    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}

    public class BoolConvertIsCallInProsses : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // בדיקה אם value הוא null
            if (value == null)
            {
                return false;
            }

            // המרה לסוג המתאים
            if (value is BO.Volunteer myValue) // החלף את MyType בשם הסוג המתאים
            {
                bool result = myValue.CallIn != null && myValue.Active;

                // אם יש פרמטר, בודקים אם הוא שווה ל-False והופכים את התוצאה
                if (parameter is string param && bool.TryParse(param, out bool invert) && invert)
                {
                    result = !result;
                }

                return result;
            }

            // אם value אינו מהסוג המתאים, מחזירים false
            return false;
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
                return Visibility.Collapsed; ; // אם ב-"עדכון", הופך ל-True
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




 

        public class ConvertStatusOrTypeToEnum : IValueConverter
        {
            // המרת הערך שנבחר ב-ComboBox הראשון לערכים המתאימים ב-ComboBox השני
            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value == null)
                    return null;

                // אם הערך שנבחר הוא Status, מחזירים את כל הערכים מתוך ה-StatusTreat Enum
                if (value.ToString() == "Status")
                {
                    return Enum.GetValues(typeof(StatusTreat));
                }
                // אם הערך שנבחר הוא CallType, מחזירים את כל הערכים מתוך ה-CallType Enum
                else if (value.ToString() == "CallType")
                {
                    return Enum.GetValues(typeof(CallType));
                }

                return null; // אם הערך לא תואם לשום דבר, מחזירים null
            }

            // המרת חזרה (לא נדרשת במקרה הזה)
            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return null;
            }
        }
    
}


