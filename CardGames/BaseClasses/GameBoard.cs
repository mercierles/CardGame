using CardGames.Interfaces;
using CardGames.Models;
namespace CardGames.BaseClass
{
	public abstract class GameBoard : IGameBoard
	{
		public const int TICKRATE = 500;
		public required IGameBoardActions GameBoardActions { get; init; }
		public required IPlayer Player { get; set; }
		public bool IsGameOver { get; set; }
		public ICoordinate HealthLocation { get; set; }
		public ICoordinate PrimaryDeckLocation { get; set; }
		public IDeck Deck { get; set; }
		public ICoordinate DiscardDeckLocation { get; set; }
		public IDeck DiscardDeck { get; set; }
		public string[,] GameBoardGrid { get; set; }
		public int Round { get; set; } = 0;

		public GameBoard()
		{
			//Init
			this.IsGameOver = false;
			this.HealthLocation = new Coordinate("health", 1, 0);
			this.PrimaryDeckLocation = new Coordinate("drawDeck", 5, 0);
			this.DiscardDeckLocation = new Coordinate("discardDeck", 9, 0);
			this.Deck = new Deck("Primary");
			this.DiscardDeck = new Deck("Discard");

			//Setup Generic Gameboard Grid
			this.GameBoardGrid = new string[10, 10];
		}
		public abstract void Reset();

		public abstract void Draw();

		public virtual void GameOver()
		{
			Console.WriteLine("Press ANY key to close");
			Console.ReadKey();
		}

		public abstract void DrawGameRules();
	}
}
