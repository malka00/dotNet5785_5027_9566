

namespace DO;
[Serializable]
public class DalExsitException : Exception    //Attempt to add an already existing member
{
    public DalExsitException(string? mass) :base(mass) { }
}
public class DalDeletImposible : Exception    //deleting/updating a member that does not exist
{
    public DalDeletImposible(string? mass) : base(mass) { }
}
public class DalWrongInput : Exception     //Incorrect input
{
    public DalWrongInput(string? mass) : base(mass) { }
}
public class DalXMLFileLoadCreateException : Exception     //Help page exception
{
    public DalXMLFileLoadCreateException(string? mass) : base(mass) { }
}

