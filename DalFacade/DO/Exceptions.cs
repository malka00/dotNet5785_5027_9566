namespace DO;

/// <summary>
/// Custom exception for cases where an attempt is made to add an already existing member.
/// </summary>
[Serializable]
public class DalExsitException : Exception
{
    /// <summary>
    /// Constructor for DalExsitException that accepts a custom message.
    /// </summary>
    /// <param name="message">The error message describing the exception.</param>
    public DalExsitException(string? message) : base(message) { }
}

/// <summary>
/// Custom exception for cases where deleting or updating a non-existent member is attempted.
/// </summary>
[Serializable]
public class DalDeletImposible : Exception
{
    /// <summary>
    /// Constructor for DalDeletImposible that accepts a custom message.
    /// </summary>
    /// <param name="message">The error message describing the exception.</param>
    public DalDeletImposible(string? message) : base(message) { }
}

/// <summary>
/// Custom exception for cases where an incorrect input is provided.
/// </summary>
[Serializable]
public class DalWrongInput : Exception
{
    /// <summary>
    /// Constructor for DalWrongInput that accepts a custom message.
    /// </summary>
    /// <param name="message">The error message describing the exception.</param>
    public DalWrongInput(string? message) : base(message) { }
}

/// <summary>
/// Custom exception for issues related to XML file loading or creation.
/// </summary>
[Serializable]
public class DalXMLFileLoadCreateException : Exception
{
    /// <summary>
    /// Constructor for DalXMLFileLoadCreateException that accepts a custom message.
    /// </summary>
    /// <param name="message">The error message describing the exception.</param>
    public DalXMLFileLoadCreateException(string? message) : base(message) { }
}


