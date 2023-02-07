using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace publishing.Models
{
    public class TypeProduct
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название типа печатной продукции")]
        [DataType(DataType.Text)]
        [StringLength(30, MinimumLength = 5, ErrorMessage = "Длина строки не входит в заданный диапазон:[5,30]")]
        [DisplayName("Название типа печатной продукции")]
        public string Name { get; set; } 
        public double Margin { get; set; }

    }
}
