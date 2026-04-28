public class Board : IBoard
{
    public List<ICard> CommunityCards{get;}
    
    public Board(List<ICard> communityCards)
    {
        CommunityCards = communityCards;
    }
}