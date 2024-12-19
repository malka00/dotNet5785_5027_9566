namespace BlApi;


/// <summary>
/// Definition of the volunteer methods
/// </summary>
public interface IVolunteer
{
    /// <summary>
    /// Logs in a volunteer using their username and password.
    /// </summary>
    /// <param name="usingName">The volunteer's username.</param>
    /// <param name="password">The volunteer's password.</param>
    /// <returns>
    /// The volunteer's role as a <see cref="BO.Role"/>.
    /// </returns>
    /// <exception cref="BO.BlDoesNotExistException">
    /// Thrown if the username doesn't exist or the password is incorrect.
    /// </exception>
    /// <exception cref="DO.DalDeletImposible">
    /// Thrown if there is a data access error in the DAL.
    /// </exception>
    BO.Role EnterSystem(int usingName, String password);

    /// <summary>
    /// Retrieves a list of volunteers with optional filtering by activity status and sorting.
    /// </summary>
    /// <param name="active">Optional filter for active/inactive volunteers.</param>
    /// <param name="sortBy">Optional sorting criteria as <see cref="BO.EVolunteerInList"/>.</param>
    /// <returns>
    /// A sorted and optionally filtered list of volunteers as <see cref="IEnumerable{BO.VolunteerInList}"/>.
    /// </returns>
    IEnumerable<BO.VolunteerInList> GetVolunteerList(bool? active, BO.EVolunteerInList? sortBy);

    /// <summary>
    /// Retrieves a volunteer's details using their ID (a unique identification number).
    /// </summary>
    /// <param name="id">The ID of the volunteer.</param>
    /// <returns>
    /// A <see cref="BO.Volunteer"/> object representing the volunteer, including a nested 
    /// <see cref="BO.CallInProgress"/> object if the volunteer has an active call.
    /// </returns>
    /// <exception cref="BO.BlDoesNotExistException">
    /// Thrown if no volunteer with the provided ID exists in the data layer.
    /// </exception>
    /// <remarks>
    /// 1. Sends a request to the data layer (using the <see cref="Read"/> method) to fetch details 
    ///    about the volunteer and their current call, if any.
    /// 2. Constructs a logical entity object of type <see cref="BO.Volunteer"/> from the retrieved data, 
    ///    including an object of type <see cref="BO.CallInProgress"/> for any active call.
    /// 3. Returns the constructed <see cref="BO.Volunteer"/> object.
    /// 4. If no volunteer with the given ID exists in the data layer:
    ///    - The data layer throws an exception that is caught here.
    ///    - A new exception of type <see cref="BO.BlDoesNotExistException"/> is thrown to the presentation layer.
    /// </remarks>
    BO.Volunteer Read(int id);

    /// <summary>
    /// Updates the details of an existing volunteer.
    /// </summary>
    /// <param name="id">The ID of the manager performing the update.</param>
    /// <param name="boVolunteer">The updated <see cref="BO.Volunteer"/> details.</param>
    /// <exception cref="BO.DeleteNotPossibleException">
    /// Thrown if the volunteer with the given ID does not exist.
    /// </exception>
    /// <exception cref="BO.BlWrongItemtException">
    /// Thrown if the manager lacks permissions or the update violates rules.
    /// </exception>
    /// <exception cref="BO.BlAlreadyExistsException">
    /// Thrown if the update fails due to a conflict in the data layer.
    /// </exception>
    /// <remarks>
    /// Verifies manager permissions, validates updated data, recalculates coordinates if the address changes, 
    /// and updates the volunteer in the data layer.
    /// </remarks>
    void Update(int id, BO.Volunteer boVolunteer);

    /// <summary>
    /// Deletes a volunteer from the system.
    /// </summary>
    /// <param name="id">The ID of the volunteer to delete.</param>
    /// <exception cref="BO.DeleteNotPossibleException">
    /// Thrown if the volunteer ID is invalid or deletion fails in the data layer.
    /// </exception>
    /// <exception cref="Exception">
    /// Thrown if the volunteer has ongoing assignments (uncompleted tasks).
    /// </exception>
    /// <remarks>
    /// 1. Verifies that the volunteer has no ongoing assignments by checking their related tasks.
    /// 2. Deletes the volunteer from the data layer if validation passes.
    /// 3. Handles exceptions from the data layer and converts them to meaningful business exceptions.
    /// </remarks>
    void Delete(int id);

    /// <summary>
    /// Creates a new volunteer in the system.
    /// </summary>
    /// <param name="boVolunteer">The <see cref="BO.Volunteer"/> object containing volunteer details.</param>
    /// <exception cref="BO.BlAlreadyExistsException">
    /// Thrown if a volunteer with the same ID already exists in the system.
    /// </exception>
    /// <remarks>
    /// 1. Validates the business logic and formatting of the volunteer's details.
    /// 2. Converts the <see cref="BO.Volunteer"/> object to a <see cref="DO.Volunteer"/> object.
    /// 3. Adds the new volunteer to the data layer.
    /// 4. Handles exceptions from the data layer and converts them to meaningful business exceptions.
    /// </remarks>
    void Create(BO.Volunteer boVolunteer);
}
