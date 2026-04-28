public class Chip : IChip
{
    public int Value{get;}
    public string Color{get;}

    public Chip(int value, string color)
    {
        Value = value;
        Color = color;
    }
}