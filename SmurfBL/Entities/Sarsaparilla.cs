namespace SmurfBL.Entities
{
    public class Sarsaparilla : Item
    {
        public bool IsGolden { get; set; } = true;
        public bool IsCollected { get; set; } = false;

        public Sarsaparilla()
        {
            Name = "Salsepareille Dorée";
        }
    }
}