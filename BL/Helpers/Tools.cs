

using System.Reflection;

namespace Helpers;

internal static class Tools
{
    public static string ToStringProperty<T>(this T t)
    {
        string str = "";
        foreach (PropertyInfo item in t.GetType().GetProperties())
        {
            str += "\n" + item.Name + ": " + item.GetValue(t);
        }
        return str;
    }

    /// <summary>
    /// מחשבת את המרחק בין שתי נקודות (קו אורך ורוחב) במטרים
    /// </summary>
    /// <param name="lat1">קו רוחב של הנקודה הראשונה</param>
    /// <param name="lon1">קו אורך של הנקודה הראשונה</param>
    /// <param name="lat2">קו רוחב של הנקודה השנייה</param>
    /// <param name="lon2">קו אורך של הנקודה השנייה</param>
    /// <returns>מרחק במטרים בין שתי הנקודות</returns>
    public static double CalculateDistance(double lat1, double lon1, double lat2, double lon2)
    {
        const double EarthRadius = 6371000; // רדיוס כדור הארץ במטרים

        // המרת קווי רוחב ואורך מרדיאנים למעלות
        double lat1Rad = lat1 * Math.PI / 180;
        double lon1Rad = lon1 * Math.PI / 180;
        double lat2Rad = lat2 * Math.PI / 180;
        double lon2Rad = lon2 * Math.PI / 180;

        // הפרשי קווי הרוחב והאורך
        double deltaLat = lat2Rad - lat1Rad;
        double deltaLon = lon2Rad - lon1Rad;

        // נוסחת ההאברסין
        double a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                   Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                   Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

        double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

        // המרחק הסופי במטרים
        return EarthRadius * c;
    }

}

