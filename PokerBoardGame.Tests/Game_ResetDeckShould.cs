using NUnit.Framework;
using PokerBoardGame;
using System.Linq;

[TestFixture]
public class Game_ResetDeckShould
{
    private Game _game;
    [SetUp]
    public void Setup()
    {
        List<IPlayer> players = new List<IPlayer>();
        players.Add(new Player("1", "A", "human"));
        players.Add(new Player("2", "B", "human"));
        players.Add(new Player("3", "C", "human"));
        players.Add(new Player("4", "D", "human"));
        
        List<ICard> board = new List<ICard>();
        // List<ICard> deck = new List<ICard>();
        List<IPot> pots = new List<IPot>();
        List<ICard> deck = new List<ICard>
        {
            {new Card(Suit.Clubs, Rank.Two)},
            {new Card(Suit.Clubs, Rank.Three)},
            {new Card(Suit.Clubs, Rank.Four)},
            {new Card(Suit.Clubs, Rank.Five)},
            {new Card(Suit.Clubs, Rank.Six)},
            {new Card(Suit.Clubs, Rank.Seven)},
            {new Card(Suit.Clubs, Rank.Eight)},
            {new Card(Suit.Clubs, Rank.Nine)},
            {new Card(Suit.Clubs, Rank.Ten)},
            {new Card(Suit.Clubs, Rank.Jack)},
            {new Card(Suit.Clubs, Rank.Queen)},
            {new Card(Suit.Clubs, Rank.King)},
            {new Card(Suit.Clubs, Rank.Ace)},
            {new Card(Suit.Spades, Rank.Two)},
            {new Card(Suit.Spades, Rank.Three)},
            {new Card(Suit.Spades, Rank.Four)},
            {new Card(Suit.Spades, Rank.Five)},
            {new Card(Suit.Spades, Rank.Six)},
            {new Card(Suit.Spades, Rank.Seven)},
            {new Card(Suit.Spades, Rank.Eight)},
            {new Card(Suit.Spades, Rank.Nine)},
            {new Card(Suit.Spades, Rank.Ten)},
            {new Card(Suit.Spades, Rank.Jack)},
            {new Card(Suit.Spades, Rank.Queen)},
            {new Card(Suit.Spades, Rank.King)},
            {new Card(Suit.Spades, Rank.Ace)},
            {new Card(Suit.Diamonds, Rank.Two)},
            {new Card(Suit.Diamonds, Rank.Three)},
            {new Card(Suit.Diamonds, Rank.Four)},
            {new Card(Suit.Diamonds, Rank.Five)},
            {new Card(Suit.Diamonds, Rank.Six)},
            {new Card(Suit.Diamonds, Rank.Seven)},
            {new Card(Suit.Diamonds, Rank.Eight)},
            {new Card(Suit.Diamonds, Rank.Nine)},
            {new Card(Suit.Diamonds, Rank.Ten)},
            {new Card(Suit.Diamonds, Rank.Jack)},
            {new Card(Suit.Diamonds, Rank.Queen)},
            {new Card(Suit.Diamonds, Rank.King)},
            {new Card(Suit.Diamonds, Rank.Ace)},
            {new Card(Suit.Hearts, Rank.Two)},
            {new Card(Suit.Hearts, Rank.Three)},
            {new Card(Suit.Hearts, Rank.Four)},
            {new Card(Suit.Hearts, Rank.Five)},
            {new Card(Suit.Hearts, Rank.Six)},
            {new Card(Suit.Hearts, Rank.Seven)},
            {new Card(Suit.Hearts, Rank.Eight)},
            {new Card(Suit.Hearts, Rank.Nine)},
            {new Card(Suit.Hearts, Rank.Ten)},
            {new Card(Suit.Hearts, Rank.Jack)},
            {new Card(Suit.Hearts, Rank.Queen)},
            {new Card(Suit.Hearts, Rank.King)},
            {new Card(Suit.Hearts, Rank.Ace)}
        };
         
        _game = new Game(players, board, deck, pots, 10, 20);
    }
    [Test]
    public void ResetDeck_DoesDeckAlreadyReset()
    {
        List<Rank> ranks = new List<Rank>();
        ranks.AddRange(Rank.GetValues<Rank>());
        List<Suit> suits = new List<Suit>();
        suits.AddRange(Suit.GetValues<Suit>());
        
        List<ICard> deck = new List<ICard>();
        foreach(Rank rank in ranks)
        {
            foreach(Suit suit in suits)
            {
                deck.Add(new Card(suit, rank));
            }
        }
        //Tukar sedikit urutan kartunya
        (deck[1],deck[4]) = (deck[4],deck[1]);
        (deck[3],deck[23]) = (deck[23],deck[3]);


        List<ICard> deckOne = deck;
        _game.ResetDeck();
        List<ICard> deckTwo = _game.GetDeck();

        Assert.That(deckOne.SequenceEqual(deckTwo), Is.False, "The deck has already reset to a new one!!" );
    }
}

