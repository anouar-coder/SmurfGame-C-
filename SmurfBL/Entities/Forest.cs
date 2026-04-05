using System.ComponentModel.DataAnnotations;

namespace SmurfBL.Entities
{
    public class Forest
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        public virtual ICollection<Creature> Creatures { get; set; } = new List<Creature>();
        public virtual ICollection<Item> Items { get; set; } = new List<Item>();
    }
}