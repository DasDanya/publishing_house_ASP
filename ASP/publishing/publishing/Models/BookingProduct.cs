namespace publishing.Models
{
    public class BookingProduct
    {
        public int Id { get; set; }

        public int BookingId { get; set; }
        public Booking? Booking { get; set; }

        public int ProductId { get; set; }

        public Product? Product { get; set; }
    }
}
