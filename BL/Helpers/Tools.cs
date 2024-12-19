using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
namespace Helpers;

internal static class Tools
{
    /// <summary>
    /// The ToStringProperty function is an extension method extended on any object (T),
    /// which converts the object to a string. Its purpose is to convert the values of all the properties of the 
    /// object into a string, so that each property and its corresponding value are included in the final string.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="t"></param>
    /// <returns></returns>
    public static string ToStringProperty<T>(this T t)
    {
        string str = "";
        foreach (PropertyInfo item in typeof(T).GetProperties())
        {
            var value = item.GetValue(t, null);
            str += item.Name + ": ";
            if (value is not string && value is IEnumerable)
            {
                str += "\n";
                foreach (var it in (IEnumerable<object>)value)
                {
                    str += it.ToString() + '\n';
                }
            }
            else
                str += value?.ToString() + '\n';
            //str += "\n" + item.CourseName + ": " + item.GetValue(t, null);
        }
        return str;
    }
}




