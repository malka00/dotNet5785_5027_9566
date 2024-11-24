

namespace DO;
[Serializable]
public class DalExsitException : Exception    //ניסיון להוסיף איבר שכבר קיים
{
    public DalExsitException(string? mass) :base(mass) { }
}
public class DalDeletImposible : Exception     //מחיקת/עדכון איבר שלא קיים
{
    public DalDeletImposible(string? mass) : base(mass) { }
}
public class DalWrongInput : Exception     //קלט לא נכון
{
    public DalWrongInput(string? mass) : base(mass) { }
}
public class DalXMLFileLoadCreateException : Exception     //חריגה של דף עזר
{
    public DalXMLFileLoadCreateException(string? mass) : base(mass) { }
}

