namespace publishing.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }

        public string Middlename { get; set; }

        public string Type { get; set; }

        public string Phone { get; set; }
        public string Email { get; set; }

        public DateTime Birthday { get; set; }

        public byte[] Visual { get; set;}

    }
}
