namespace publishing.Models
{
    public class Booking
    {
        public int Id { get; set; }

        public DateTime Start { get; set; }
        public DateTime End { get; set; }

        public string Status { get; set; }

        public double Cost { get; set; }

        public int PrintingHouseId { get; set; }

        public PrintingHouse PrintingHouse { get; set; }

        public int EmployeeId { get; set; }

        public Employee Employee { get; set; }
    }
}
