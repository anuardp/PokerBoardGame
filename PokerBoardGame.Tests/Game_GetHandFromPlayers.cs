using NUnit.Framework;
using PokerBoardGame;
using System.Linq;

// namespace PokerBoardGame.UnitTests;

[TestFixture]
public class Game_MovetoNextPhaseHasMoved
{
    private Game _game;
    [SetUp]
    public void Setup()
    {
        List<IPlayer> players = new List<IPlayer>();
        List<ICard> board = new List<ICard>();
        List<ICard> deck = new List<ICard>();
        List<IPot> pots = new List<IPot>();
         
        _game = new Game(players, board, deck, pots, 10, 20);
    }
    [Test]
    public void MoveToNextPhase_IsAlreadyMoveToNextPhase()
    {   
        GamePhase previousPhase = _game.GetPhase();
        _game.MoveToNextPhase();
        GamePhase currentPhase = _game.GetPhase();

        Assert.That(previousPhase != currentPhase, Is.True, "The game has moved to next phase!" );
    }

    [Test]
    public void MoveToNextPhase_IsCurrentGamePhaseShowdown()
    {
        GamePhase previousPhase = _game.GetPhase();
        while(previousPhase != GamePhase.Showdown)
        {
            _game.MoveToNextPhase();
        }
        previousPhase = _game.GetPhase();
        GamePhase currentPhase = _game.GetPhase();
        Assert.That(previousPhase != currentPhase, Is.False, "The game already reached Showdown phase, it's already over..." );
    }
}