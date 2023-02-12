﻿namespace publishing.Models
{
    public class ProductMaterial
    {
        public int Id { get; set; }
        public int CountMaterials { get; set; }

        public int MaterialId { get; set; }

        public Material? Material { get; set; }

        public int ProductId { get; set; }
        public Product? Product { get; set; }
    }
}
