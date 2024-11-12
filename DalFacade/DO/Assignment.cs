namespace DO;
public record Assignment
{
     int Id { get; set; }             // מזהה ייחודי להקצאה
     int CallId { get; set; }         // מזהה הקריאה שהמתנדב בחר לטפל בה
   int VolunteerId { get; set; }    // ת.ז של המתנדב שבחר לטפל בקריאה
    DateTime TimeStart { get; set; } // זמן כניסה לטיפול (תאריך ושעה)
     DateTime? TimeEnd = null;  // זמן סיום הטיפול בפועל (תאריך ושעה)
     TypeEnd? TypeEndTreat = null; // סוג סיום הטיפול (טופלה, ביטול עצמי, ביטול מנהל, ביטול פג תוקף)

   public Assignment() : this(default(DateTime)) { }

}
