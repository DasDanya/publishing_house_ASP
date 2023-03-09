using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace publishing.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Введите название продукции")]
        [DataType(DataType.Text)]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Длина строки не входит в заданный диапазон:[1,50]")]
        [DisplayName("Название продукции")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Выберите фото продукции")]
        [DataType(DataType.ImageUrl)]
        [DisplayName("Фото")]
        public string Visual { get; set; }

        [Required(ErrorMessage = "Введите тираж")]
        [DisplayName("Тираж")]
        [DataType(DataType.Text)]
        [Range(typeof(int), "1", "1000000", ErrorMessage = "Тираж должен входить в диапазон: [1,1000000]")]
        [RegularExpression(@"\d+", ErrorMessage = "Введите целое число")]
        public int Edition { get; set; }

        [Required(ErrorMessage = "Введите стоимость продукции")]
        [DisplayName("Стоимость")]
        [DataType(DataType.Currency)]
        [Range(typeof(double), "1", "10000", ErrorMessage = "Стоимость должна входить в диапазон: [1,10000]")]
        public double Cost { get; set; }

        public int TypeProductId { get; set; }
        public TypeProduct? TypeProduct { get; set; }
 
        public virtual ICollection<ProductMaterial> ProductMaterials { get; set; }

        public Product()
        {
            ProductMaterials = new List<ProductMaterial>(); 
        }

    }
}
