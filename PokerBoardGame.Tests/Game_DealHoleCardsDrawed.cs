using NUnit.Framework;
using PokerBoardGame;
using System.Linq;
using System.Security.Cryptography.X509Certificates;

[TestFixture]
public class Game_DealHoleCardsDrawed
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
    public void DealHoleCards_DoesEachPlayersGotTheirCards()
    {
        List<IPlayer> players = _game.GetPlayers();
        Dictionary<IPlayer, List<ICard>> playerHands = _game.GetPlayerHands();
        
        List<ICard>? deck = _game.GetDeck();
        _game.InitializeDeck();
        deck = _game.GetDeck();
        _game.ShuffleDeck(deck);
        deck = _game.GetDeck();
        
        List<ICard> allCardsForPlayers = deck[0..(players.Count*2)];
        _game.DealHoleCards();
        playerHands = _game.GetPlayerHands();
        bool cekBagiKartuSesuai = true;
        int counter = 0;
        foreach(IPlayer p in playerHands.Keys)
        {
            if(playerHands[p][0]!=allCardsForPlayers[counter] || playerHands[p][1] != allCardsForPlayers[counter + players.Count])
            {
                cekBagiKartuSesuai = false;
            }
            counter++;
        }
        Assert.That(cekBagiKartuSesuai, Is.True, "Each players already receive 2 cards as the rule stated." );
    }
}

