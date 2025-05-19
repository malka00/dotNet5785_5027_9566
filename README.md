
# Volunteer Management System

As part of the course project, we developed a comprehensive system for managing a volunteer-based organization. The system is designed to streamline the handling of service calls and the administration of volunteers. This solution is suitable for various types of volunteer organizations, such as **YEDIDIM - Roadside Assistance**.

##  User Roles
The system supports two primary user roles:
- ** Manager** – Responsible for overseeing service calls and managing the volunteer database.
- ** Volunteer** – Handles service calls and reports updates regarding their status.

Managers also have the option to operate as regular volunteers, based on their preferences.

##  CManagement
- Managers can create new service calls of different types.
- Each call includes a defined maximum resolution time.
- Managers can specify a risk time window, beyond which a call is considered at risk of not being resolved on time.
- Calls that exceed their resolution time without being handled are marked as expired.
- The system maintains a complete assignment history for each call, including all volunteers who were assigned or released the call.

##  Volunteer 
- Once a volunteer selects a call, it is marked as assigned to them.
- Volunteers can report a successful resolution or choose to cancel the assignment.
- If a volunteer cancels an assignment, the call becomes available for others to handle.
- Volunteers can update their address and define their maximum preferred distance for receiving calls.

##  Technologies
- **Programming Language:** C#
- **Development Framework:** WPF (.NET)
- **Data Management:** XML files
- **Algorithms:** Queue management, date/time calculations, and range evaluations
- **Concurrency:** Asynchronous task and process handling

##  System Clock
To simulate real-time progression, the system uses an internal logical clock that operates independently from the actual system time.

##  System Architecture
The project is implemented based on the Layered Architecture Model, including the following layers:
- **Presentation Layer** – User interface developed using WPF.
- **Business Logic Layer (BL)** – Contains the logic for managing calls, scheduling, and volunteer data.
- **Data Access Layer (DAL)** – Handles reading and writing of data to XML files, including system clock updates.

## Written By:
- [Efrat Sharabi](https://github.com/efratsharabi1)
- [Malka Haupt](https://github.com/malka00)

## Imagaes
![Enter Screenshot](./images/Enter.png)
![Management Screenshot](./images/ManagerWindow.png)
![Volunteer Screenshot](./images/VolunteerWindow.png)



- 

# README - úåñôåú åùéôåøéí áôøåé÷è

## úåñôåú åùéôåøéí á÷åã

| **úåñôú** | **ôéøåè (äéëï ä÷åã ùì äúåñôú îåôéò áôøåé÷è)**  | **ðé÷åã ùäåöò áîñîê äøùîé** |
|------------|-------------------------------------------------|-----------------------------|
| **ëôúåø Enter** | îñê ëðéñä øàùé |  1 |
| **TryParse** | áùëáú ä-DAL |  1 |
| **äåñôú úëåðä - ñéñîà** | áéùåú `volunteer`, ëðéñä ìîòøëú  | 2 |
| **äöôðú äñéñîà** | `VolunteerManager` |  2 |
| **úöåâä âøôéú àéðèøà÷èéáéú** (ùéðåé öáòéí áî÷øä ùì ÷ìè ìà ú÷éï) | áäëðñú ëúåáú ìà ðëåðä - öáò àãåí. îñê îúðãá, îñê ÷øéàä  | 1 |
| **àéé÷åï (icon)** | áëì äîñëéí  | 1 |
| **èøéâø îàôééðéí** | îñê îðäì - áçéøä ìàéæä îñê ììëú; îñê øàùé - ëôúåøéí îùðéí öáò ëùäí ìà îàåôùøéí | 1 |
| **ùéîåù á- `IMultiValueConverter`** | áäëðñú ëúåáú ìà ðëåðä - öáò àãåí. îñê îúðãá, îñê ÷øéàä  | 1 |
| **äæðú ñéñîà áîñê äëðéñä** | áäæðú äñéñîà ãøê äîñê - äñéñîà ìà îåöâú àìà îåôéòåú ëð÷åãåú | 1 |
| **äëôúåø ìîçé÷ú ÷øéàä éåôéò ø÷ àí àôùø ìîçå÷ àú ä÷øéàä** | îñê ðéäåì ÷øéàåú |  2 |
| **äëôúåø ìîçé÷ú îúðãá éåôéò ø÷ àí àôùø ìîçå÷ àú äîúðãá** | îñê ðéäåì îúðãáéí |  2 |
| **áãé÷ä ùäñéñîà çæ÷ä** | îñê äåñôú îúðãá | 1 |
| **äåñôú ëôúåø àçåøä** | áëì äîñëéí | 1 |
| **èøéâø úëåðä** | îñê îúðãá - ëôúåø áçéøú ÷øéàä | 1 |
| **âøôé÷ä (drawing, shapes)** | îñê áçéøä ùì îðäì | 1 |

## äòøåú
- ëì äúåñôåú ðáã÷å åðåñôå áäúàîä ìîòøëú.


