public class HandStrength
{
    public HandRank Rank { get; }
    public List<Rank> TieBreakers { get; }

    public HandStrength(HandRank rank, List<Rank> tieBreakers)
    {
        Rank = rank;
        TieBreakers = tieBreakers ?? new List<Rank>();
    }

}