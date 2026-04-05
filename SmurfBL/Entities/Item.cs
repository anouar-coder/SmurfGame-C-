using System.ComponentModel.DataAnnotations;

namespace SmurfBL.Entities
{
    public abstract class Item
    {
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        public string Name { get; set; } = string.Empty;

        public int X { get; set; }
        public int Y { get; set; }

        public int ForestId { get; set; }
        public virtual Forest? Forest { get; set; }
    }
}