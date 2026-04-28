public class Deck : IDeck
{
    public List<ICard> Cards{get;}

    public Deck(List<ICard> cards)
    {
        Cards = cards;
    }    
}