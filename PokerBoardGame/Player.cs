public class Player : IPlayer
{
    public string Id{get; set;}
    public string Name{get; set;}
    public string Type {get; set;}

    public Player(string id, string name, string type)
    {
        Id = id;
        Name = name;
        Type = type;
    }
}