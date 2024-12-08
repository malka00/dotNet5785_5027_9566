namespace DO;

[Serializable]
public class DalExsitException : Exception    //Attempt to add an already existing member
{
    public DalExsitException(string? message) : base(message) { }
}

[Serializable]
public class DalDeletImposible : Exception    //deleting/updating a member that does not exist
{
    public DalDeletImposible(string? message) : base(message) { }
}

[Serializable]
public class DalWrongInput : Exception     //Incorrect input
{
    public DalWrongInput(string? message) : base(message) { }
}

[Serializable]
public class DalXMLFileLoadCreateException : Exception     //Help page exception
{
    public DalXMLFileLoadCreateException(string? message) : base(message) { }
}

