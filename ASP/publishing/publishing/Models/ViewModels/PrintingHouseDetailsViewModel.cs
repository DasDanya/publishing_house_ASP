namespace publishing.Models.ViewModels
{
    public class PrintingHouseDetailsViewModel
    {
        public PrintingHouse PrintingHouse { get; set; }

        public IEnumerable<Booking> Bookings { get; set; }
    }
}
