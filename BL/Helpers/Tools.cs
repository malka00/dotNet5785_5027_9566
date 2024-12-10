

using System.Reflection;

namespace Helpers;

internal static class Tools
{
    /// <summary>
    /// Converts all the properties of an object into a formatted string 
    /// that displays each property's name and its value.
    /// </summary>
    /// <typeparam name="T">The type of the object whose properties are being processed.</typeparam>
    /// <param name="t">The object whose properties will be converted to a string.</param>
    /// <returns>A string representation of the object's properties and their values.</returns>
    public static string ToStringProperty<T>(this T t)
    {
        string str = "";
        foreach (PropertyInfo item in t.GetType().GetProperties())
        {
            str += "\n" + item.Name + ": " + item.GetValue(t);
        }
        return str;
    }
}




