
# Volunteer Management System

As part of the course project, we developed a comprehensive system for managing a volunteer-based organization. The system is designed to streamline the handling of service calls and the administration of volunteers. This solution is suitable for various types of volunteer organizations, such as **YEDIDIM - Roadside Assistance**.

## ?? User Roles
The system supports two primary user roles:
- **????? Manager** – Responsible for overseeing service calls and managing the volunteer database.
- **????? Volunteer** – Handles service calls and reports updates regarding their status.

Managers also have the option to operate as regular volunteers, based on their preferences.

## ?? CManagement
- Managers can create new service calls of different types.
- Each call includes a defined maximum resolution time.
- Managers can specify a risk time window, beyond which a call is considered at risk of not being resolved on time.
- Calls that exceed their resolution time without being handled are marked as expired.
- The system maintains a complete assignment history for each call, including all volunteers who were assigned or released the call.

## ????? Volunteer 
- Once a volunteer selects a call, it is marked as assigned to them.
- Volunteers can report a successful resolution or choose to cancel the assignment.
- If a volunteer cancels an assignment, the call becomes available for others to handle.
- Volunteers can update their address and define their maximum preferred distance for receiving calls.

## ?? Technologies
- **Programming Language:** C#
- **Development Framework:** WPF (.NET)
- **Data Management:** XML files
- **Algorithms:** Queue management, date/time calculations, and range evaluations
- **Concurrency:** Asynchronous task and process handling

## ?? System Clock
To simulate real-time progression, the system uses an internal logical clock that operates independently from the actual system time.

## ?? System Architecture
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

# README - תוספות ושיפורים בפרויקט

## תוספות ושיפורים בקוד

| **תוספת** | **פירוט (היכן הקוד של התוספת מופיע בפרויקט)**  | **ניקוד שהוצע במסמך הרשמי** |
|------------|-------------------------------------------------|-----------------------------|
| **כפתור Enter** | מסך כניסה ראשי |  1 |
| **TryParse** | בשכבת ה-DAL |  1 |
| **הוספת תכונה - סיסמא** | בישות `volunteer`, כניסה למערכת  | 2 |
| **הצפנת הסיסמא** | `VolunteerManager` |  2 |
| **תצוגה גרפית אינטראקטיבית** (שינוי צבעים במקרה של קלט לא תקין) | בהכנסת כתובת לא נכונה - צבע אדום. מסך מתנדב, מסך קריאה  | 1 |
| **אייקון (icon)** | בכל המסכים  | 1 |
| **טריגר מאפיינים** | מסך מנהל - בחירה לאיזה מסך ללכת; מסך ראשי - כפתורים משנים צבע כשהם לא מאופשרים | 1 |
| **שימוש ב- `IMultiValueConverter`** | בהכנסת כתובת לא נכונה - צבע אדום. מסך מתנדב, מסך קריאה  | 1 |
| **הזנת סיסמא במסך הכניסה** | בהזנת הסיסמא דרך המסך - הסיסמא לא מוצגת אלא מופיעות כנקודות | 1 |
| **הכפתור למחיקת קריאה יופיע רק אם אפשר למחוק את הקריאה** | מסך ניהול קריאות |  2 |
| **הכפתור למחיקת מתנדב יופיע רק אם אפשר למחוק את המתנדב** | מסך ניהול מתנדבים |  2 |
| **בדיקה שהסיסמא חזקה** | מסך הוספת מתנדב | 1 |
| **הוספת כפתור אחורה** | בכל המסכים | 1 |
| **טריגר תכונה** | מסך מתנדב - כפתור בחירת קריאה | 1 |
| **גרפיקה (drawing, shapes)** | מסך בחירה של מנהל | 1 |

## הערות
- כל התוספות נבדקו ונוספו בהתאמה למערכת.


