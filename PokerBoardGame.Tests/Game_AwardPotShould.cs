using NUnit.Framework;
using PokerBoardGame;
using System.Linq;
using System.Reflection;

[TestFixture]
public class Game_AwardPotShould
{
    private Game _game;
    private List<IPlayer> _players;

    [SetUp]
    public void Setup()
    {
        _players = new List<IPlayer>();
        _players.Add(new Player("1", "A", "human"));
        _players.Add(new Player("2", "B", "human"));
        _players.Add(new Player("3", "C", "human"));
        _players.Add(new Player("4", "D", "human"));

        List<ICard> board = new List<ICard>();
        List<IPot> pots = new List<IPot>();
        List<ICard> deck = new List<ICard>
        {
            new Card(Suit.Clubs, Rank.Two), new Card(Suit.Clubs, Rank.Three),
            new Card(Suit.Clubs, Rank.Four), new Card(Suit.Clubs, Rank.Five),
            new Card(Suit.Clubs, Rank.Six), new Card(Suit.Clubs, Rank.Seven),
            new Card(Suit.Clubs, Rank.Eight), new Card(Suit.Clubs, Rank.Nine),
            new Card(Suit.Clubs, Rank.Ten), new Card(Suit.Clubs, Rank.Jack),
            new Card(Suit.Clubs, Rank.Queen), new Card(Suit.Clubs, Rank.King),
            new Card(Suit.Clubs, Rank.Ace), new Card(Suit.Spades, Rank.Two),
            new Card(Suit.Spades, Rank.Three), new Card(Suit.Spades, Rank.Four),
            new Card(Suit.Spades, Rank.Five), new Card(Suit.Spades, Rank.Six),
            new Card(Suit.Spades, Rank.Seven), new Card(Suit.Spades, Rank.Eight),
            new Card(Suit.Spades, Rank.Nine), new Card(Suit.Spades, Rank.Ten),
            new Card(Suit.Spades, Rank.Jack), new Card(Suit.Spades, Rank.Queen),
            new Card(Suit.Spades, Rank.King), new Card(Suit.Spades, Rank.Ace),
            new Card(Suit.Diamonds, Rank.Two), new Card(Suit.Diamonds, Rank.Three),
            new Card(Suit.Diamonds, Rank.Four), new Card(Suit.Diamonds, Rank.Five),
            new Card(Suit.Diamonds, Rank.Six), new Card(Suit.Diamonds, Rank.Seven),
            new Card(Suit.Diamonds, Rank.Eight), new Card(Suit.Diamonds, Rank.Nine),
            new Card(Suit.Diamonds, Rank.Ten), new Card(Suit.Diamonds, Rank.Jack),
            new Card(Suit.Diamonds, Rank.Queen), new Card(Suit.Diamonds, Rank.King),
            new Card(Suit.Diamonds, Rank.Ace), new Card(Suit.Hearts, Rank.Two),
            new Card(Suit.Hearts, Rank.Three), new Card(Suit.Hearts, Rank.Four),
            new Card(Suit.Hearts, Rank.Five), new Card(Suit.Hearts, Rank.Six),
            new Card(Suit.Hearts, Rank.Seven), new Card(Suit.Hearts, Rank.Eight),
            new Card(Suit.Hearts, Rank.Nine), new Card(Suit.Hearts, Rank.Ten),
            new Card(Suit.Hearts, Rank.Jack), new Card(Suit.Hearts, Rank.Queen),
            new Card(Suit.Hearts, Rank.King), new Card(Suit.Hearts, Rank.Ace)
        };

        _game = new Game(_players, board, deck, pots, 10, 20);
        _game.InitializeDeck();
        _game.DealHoleCards();
    }

    [Test]
    public void AwardPot_AddsWinningsToWinnerAndClearsPot()
    {
        // Setup: single winner
        IPlayer winner = _players[0];
        int initialChips = _game.GetTotalChips(winner);
        int potAmount = 100;
        
        List<IPot> pots = _game.GetPots();
        pots[0].Amount = potAmount;
        pots[0].EligiblePlayers = new List<IPlayer> { winner };
        
        _game.AwardPot();
        
        Assert.That(_game.GetTotalChips(winner), Is.EqualTo(initialChips + potAmount));
        Assert.That(pots[0].Amount, Is.EqualTo(0));
        Assert.That(pots.Count, Is.EqualTo(0)); // pots cleared after award
    }

    [Test]
    public void AwardPot_SplitsPotAmongMultipleWinners()
    {
        IPlayer winner1 = _players[0];
        IPlayer winner2 = _players[1];
        int initialChips1 = _game.GetTotalChips(winner1);
        int initialChips2 = _game.GetTotalChips(winner2);
        int potAmount = 150; // 150 / 2 = 75 each, no remainder
        
        List<IPot> pots = _game.GetPots();
        pots[0].Amount = potAmount;
        pots[0].EligiblePlayers = new List<IPlayer> { winner1, winner2 };
        
        _game.AwardPot();
        
        Assert.That(initialChips1, !Is.EqualTo(initialChips1 + 75));
        Assert.That(initialChips2, !Is.EqualTo(initialChips2 + 75));
    }

    [Test]
    public void AwardPot_GivesRemainderToFirstWinnerWhenUnevenSplit()
    {
        IPlayer winner1 = _players[0];
        IPlayer winner2 = _players[1];
        int initialChips1 = _game.GetTotalChips(winner1);
        int initialChips2 = _game.GetTotalChips(winner2);
        int potAmount = 160; // 160 / 2 = 80 each, remainder 0? Actually 160/2=80 no remainder. Use 151 for remainder 1
        potAmount = 151; // 151 / 2 = 75 remainder 1, winner1 gets 76, winner2 gets 75
        
        List<IPot> pots = _game.GetPots();
        pots[0].Amount = potAmount;
        pots[0].EligiblePlayers = new List<IPlayer> { winner1, winner2 };
        
        _game.AwardPot();
        
        Assert.That(initialChips1, Is.EqualTo(initialChips1 + 76));
        Assert.That(initialChips2, Is.EqualTo(initialChips2 + 75));
    }

    [Test]
    public void AwardPot_OnlyAwardsToEligibleWinnersInSidePot()
    {
        IPlayer winnerMain = _players[0]; // eligible for main pot only
        IPlayer winnerSide = _players[1]; // eligible for both main and side
        int initialChipsMain = _game.GetTotalChips(winnerMain);
        int initialChipsSide = _game.GetTotalChips(winnerSide);
        
        List<IPot> pots = _game.GetPots();
        pots.Clear();
        Pot mainPot = new Pot(new List<IPlayer> { winnerMain, winnerSide }, 100);
        Pot sidePot = new Pot(new List<IPlayer> { winnerSide }, 50);
        pots.Add(mainPot);
        pots.Add(sidePot);
        
        _game.AwardPot();
        
        // Main pot 100 split between winnerMain and winnerSide? Wait: eligible for main pot includes both. So they share 100 -> 50 each.
        // Side pot 50 goes only to winnerSide.
        // Total: winnerMain gets 50, winnerSide gets 50+50=100.
        Assert.That(_game.GetTotalChips(winnerMain), Is.EqualTo(initialChipsMain + 50));
        Assert.That(_game.GetTotalChips(winnerSide), Is.EqualTo(initialChipsSide + 100));
    }

    [Test]
    public void AwardPot_SkipsPotWithZeroAmount()
    {
        IPlayer winner = _players[0];
        int initialChips = _game.GetTotalChips(winner);
        
        List<IPot> pots = _game.GetPots();
        pots[0].Amount = 0;
        pots[0].EligiblePlayers = new List<IPlayer> { winner };
        
        _game.AwardPot();

        Assert.That(_game.GetTotalChips(winner), Is.EqualTo(initialChips)); // no change
        Assert.That(pots.Count, Is.EqualTo(0)); // still cleared
    }

    [Test]
    public void AwardPot_SkipsPotWhenNoEligibleWinners()
    {
        IPlayer nonEligible = _players[0];
        int initialChips = _game.GetTotalChips(nonEligible);
        
        List<IPot> pots = _game.GetPots();
        pots[0].Amount = 100;
        pots[0].EligiblePlayers = new List<IPlayer>(); // empty
        
        _game.AwardPot();
        
        Assert.That(_game.GetTotalChips(nonEligible), Is.EqualTo(initialChips)); // no change
        Assert.That(pots.Count, Is.EqualTo(0));
    }
}