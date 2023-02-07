namespace publishing.Models
{
    public class Product
    {
        public int Id { get; set; }

        public int NumEdition { get; set; }

        public string Name { get; set; }

        public byte[] Visual { get; set; }

        public int Edition { get; set; }

        public int Cost { get; set; }

        public int TypeProductId { get; set; }
        public TypeProduct TypeProduct { get; set; }

        public int BookingId { get; set; }
        public Booking? Booking { get; set; }

    }
}
