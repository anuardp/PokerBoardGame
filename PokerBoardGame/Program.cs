using System;
using System.Collections.Generic;
using System.Linq;
using Serilog;
using Serilog.Context;
using PokerBoardGame;

class Program
{
    static void Main()
    {
        // Setup Serilog
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .Enrich.WithProperty("Application", "PokerBoardGame")
            .Enrich.WithProperty("Environment", "Production")
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}] {Message:lj}{NewLine}{Properties}")
            .WriteTo.File(
                new Serilog.Formatting.Compact.CompactJsonFormatter(),
                "logs/pokerlog.json",
                rollingInterval: RollingInterval.Day,
                retainedFileCountLimit: 7)
            .CreateLogger();

        try
        {
            Log.Information("Aplikasi PokerBoardGame dimulai");
            GameHomeUIHeader();
            Console.Write("\nSelect Menu: ");
            string? option = Console.ReadLine();

            while (option != "1" && option != "2")
            {
                Console.Clear();
                GameHomeUIHeader();
                Console.Write("\nThere's only 2 options!!\n\nSelect Menu: ");
                option = Console.ReadLine();
            }
            if (option == "1")
            {
                List<IPlayer> players = SelectPlayer();
                List<ICard> deck = new List<ICard>();
                List<ICard> board = new List<ICard>();
                List<IPot> pots = new List<IPot>();
                Game game = new Game(players, board, deck, pots, 10, 20);

                game.OnPlayerActed += (player, action, amount) =>
                {
                    Log.Information("Player {PlayerName} {Action} {Amount}", player.Name, action, amount);
                    Console.WriteLine($"{player.Name} {action}" + (amount > 0 ? $" {amount}" : ""));
                };
                game.OnPhaseChanged += phase =>
                {
                    Log.Debug("Phase changed to {Phase}", phase);
                    Console.WriteLine($"\n*** {phase} ***");
                };
                game.OnHandEnded += winner =>
                {
                    Log.Information("Hand ended. Winner: {WinnerName}", winner.Name);
                    Console.WriteLine($"\n🏆 {winner.Name} wins the hand!\n");
                };

                bool quit = false;
                while (!quit && game.GetPlayers().Count > 1)
                {
                    MainGameUI(game);
                    Console.Write("\nA round has finished. Press Enter to continue, or type 'quit' to exit: ");
                    string? gameFinished = Console.ReadLine();
                    if (gameFinished?.ToLower() == "quit") quit = true;
                }
                if (game.GetPlayers().Count <= 1)
                {
                    Log.Warning("Game over - only one player remains");
                    Console.WriteLine("\nGame over! Only one player remains.");
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("\nThanks for playing!");
                    Main();
                }
            }
            else
            {
                Log.Information("User exited game");
                Console.WriteLine("\nExit the game. Thank you for playing.");
            }
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Unhandled exception in Main");
            Console.WriteLine($"\nError: {ex.Message}");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }

    static void GameHomeUIHeader()
    {
        Console.WriteLine("--------------------Poker--------------------");
        Console.WriteLine("\nMenu:\n[1] Play\n[2] Exit");
    }

    static List<IPlayer> SelectPlayer()
    {
        Console.Clear();
        Console.WriteLine("--------------------Poker--------------------");
        Console.Write("\nNumber of human players: ");
        int totalHumanPlayer = Convert.ToInt32(Console.ReadLine());
        List<IPlayer> players = new List<IPlayer>();
        for (int i = 0; i < totalHumanPlayer; i++)
        {
            Console.Write($"Enter Player {i + 1} name: ");
            string? playerName = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(playerName)) playerName = $"Human{i + 1}";
            players.Add(new Player((i + 1).ToString(), playerName, "human"));
        }

        Console.Write("Number of bot players: ");
        int totalBotPlayer = int.Parse(Console.ReadLine() ?? "0");
        for (int i = 0; i < totalBotPlayer; i++)
        {
            players.Add(new Player((totalHumanPlayer + i + 1).ToString(), $"Bot{i + 1}", "bot"));
        }
        Log.Information("Game started with {HumanCount} human(s) and {BotCount} bot(s)", totalHumanPlayer, totalBotPlayer);
        return players;
    }

    static void MainGameUI(Game game)
    {
        Console.Clear();
        Console.WriteLine("--------------------Poker--------------------");
        game.StartNewRound();

        while (!game.IsGameEndedEarly() && game.Phase != GamePhase.Showdown)
        {
            IPlayer currentPlayer = game.GetCurrentPlayer();
            RenderGameState(game, currentPlayer);

            if (currentPlayer is Player p && p.Type == "human")
            {
                HumanTurn(game, currentPlayer);
            }
            else
            {
                (PlayerAction action, int amount) = game.DecideBotAction(currentPlayer);
                Log.Debug("Bot {BotName} decides: {Action} {Amount}", currentPlayer.Name, action, amount);
                Console.WriteLine($"{currentPlayer.Name} decides: {action}" + (amount > 0 ? $" {amount}" : ""));
                game.HandleAction(currentPlayer, action, amount);
            }

            RenderGameState(game, game.GetCurrentPlayer());
            if (game.IsBettingRoundOver())
            {
                if (game.Phase == GamePhase.PreFlop)
                    game.DealFlop();
                else if (game.Phase == GamePhase.Flop)
                    game.DealTurn();
                else if (game.Phase == GamePhase.Turn)
                    game.DealRiver();
                game.MoveToNextPhase();

                if (game.Phase != GamePhase.Showdown)
                {
                    Console.WriteLine($"\n*** {game.Phase} ***");
                    RenderGameState(game, null);
                }
            }
        }
        if (game.Phase == GamePhase.Showdown)
        {
            RenderShowdown(game);
        }
        else if (game.IsGameEndedEarly())
        {
            IPlayer? winner = game.GetActivePlayers().FirstOrDefault();
            if (winner != null)
            {
                Log.Information("Early end: {WinnerName} wins the pot", winner.Name);
                Console.WriteLine($"\nOnly {winner.Name} remains. They win the pot!");
            }
        }

        game.AwardPot();
        List<IPlayer> dead = game.GetPlayers().Where(p => game.GetTotalChips(p) == 0).ToList();
        foreach (IPlayer d in dead)
        {
            Log.Information("Player {PlayerName} is eliminated", d.Name);
            game.RemovePlayer(d);
        }
    }

    static void HumanTurn(Game game, IPlayer human)
    {
        int toCall = game.CurrentBetAmount - game.GetPlayerBet(human);
        int chips = game.GetTotalChips(human);
        Console.WriteLine($"\nYour turn: {human.Name} (chips: {chips})");
        Console.WriteLine($"Current bet to call: {toCall}");
        Console.WriteLine("Actions: (f)old, (c)call, (r)aise, (a)ll-in");
        Console.Write("Choose: ");
        string input = Console.ReadLine()?.ToLower();

        PlayerAction action;
        int amount = 0;
        switch (input)
        {
            case "f":
                action = PlayerAction.Fold;
                break;
            case "c":
                if (toCall == 0)
                    action = PlayerAction.Check;
                else
                {
                    action = PlayerAction.Call;
                    amount = toCall;
                }
                break;
            case "r":
                action = PlayerAction.Raise;
                while (true)
                {
                    Console.Write("Raise to total amount (must be > current bet): ");
                    if (!int.TryParse(Console.ReadLine(), out amount))
                    {
                        Console.WriteLine("Invalid number. Please enter a valid amount.");
                        continue;
                    }
                    if (amount <= game.CurrentBetAmount)
                    {
                        Console.WriteLine($"Raise must be higher than current bet ({game.CurrentBetAmount}). Try again.");
                        continue;
                    }
                    break;
                }
                break;
            case "a":
                action = PlayerAction.AllIn;
                amount = chips;
                break;
            default:
                Console.WriteLine("Invalid input. Folding.");
                action = PlayerAction.Fold;
                break;
        }

        game.HandleAction(human, action, amount);
    }

    static void RenderGameState(Game game, IPlayer? currentPlayer)
    {
        Console.WriteLine("\n--- Game State ---");
        Console.Write("Cards on Board: \n");
        List<ICard> board = game.GetBoard();
        if (board.Count == 0)
            Console.Write("[Xx] [Xx] [Xx] [Xx] [Xx]");
        else if (board.Count == 3)
            Console.Write($"{CardTranslate(board[0])} {CardTranslate(board[1])} {CardTranslate(board[2])} [Xx] [Xx]");
        else if (board.Count == 4)
            Console.Write($"{CardTranslate(board[0])} {CardTranslate(board[1])} {CardTranslate(board[2])} {CardTranslate(board[3])} [Xx]");
        else if (board.Count == 5)
            Console.Write($"{CardTranslate(board[0])} {CardTranslate(board[1])} {CardTranslate(board[2])} {CardTranslate(board[3])} {CardTranslate(board[4])}");
        Console.WriteLine("\n");

        if (currentPlayer != null && currentPlayer is Player p && p.Type == "human")
        {
            List<ICard> hand = game.GetHand(currentPlayer);
            if (hand.Count == 2)
                Console.WriteLine($"\nYour hand:\n{CardTranslate(hand[0])} {CardTranslate(hand[1])})\n");
        }

        foreach (IPlayer player in game.GetPlayers())
        {
            string status = "";
            if (game.IsFolded(player)) status = "[FOLD]";
            else if (game.IsAllIn(player)) status = "[ALL-IN]";
            Console.WriteLine($"{player.Name,-10} Chips: {game.GetTotalChips(player),-5} Bet: {game.GetPlayerBet(player),-5} {status}");
        }
        Console.WriteLine($"Pot: {game.GetTotalPot()}");
        Console.WriteLine("------------------");
    }

    static void RenderShowdown(Game game)
    {
        Console.WriteLine("\n--- SHOWDOWN ---");
        List<IPlayer> activePlayers = game.GetActivePlayers();
        foreach (IPlayer player in activePlayers)
        {
            List<ICard> hand = game.GetHand(player);
            List<ICard> bestFive = game.GetBestFiveCards(player);
            HandStrength strength = game.EvaluateHand(player);
            Console.Write($"{player.Name}'s hand: {strength.Rank}");
            if (bestFive != null && bestFive.Count == 5)
            {
                Console.Write($" (Best 5: {string.Join(" ", bestFive.Select(c => $"{CardTranslate(c)}"))})");
            }
            Console.WriteLine();
            Log.Debug("{PlayerName} hand rank: {HandRank}", player.Name, strength.Rank);
        }
        List<IPlayer> winners = game.GetWinnersOnRound();
        int totalPot = game.GetTotalPot();
        int numWinners = winners.Count;
        int share = totalPot / numWinners;
        int remainder = totalPot % numWinners;
        Console.Write("\nWinner(s): ");
        for (int i = 0; i < winners.Count; i++)
        {
            int winnings = share + (i == 0 ? remainder : 0);
            Console.Write($"{winners[i].Name} (won {winnings} chips) ");
            Log.Information("Winner: {WinnerName} wins {Winnings} chips", winners[i].Name, winnings);
        }
        Console.WriteLine();
    }

    static string CardTranslate(ICard card)
    {
        string suit = card.Suit switch
        {
            Suit.Hearts   => "♥",
            Suit.Diamonds => "♦",
            Suit.Clubs    => "♣",
            Suit.Spades   => "♠",
            _             => "?"
        };
        string rank = card.Rank switch
        {
            Rank.Ace   => "A",
            Rank.King  => "K",
            Rank.Queen => "Q",
            Rank.Jack  => "J",
            Rank.Ten   => "10",
            _          => ((int)card.Rank).ToString()
        };
        return $"[{rank}{suit}]";
    }
}