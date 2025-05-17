using CardGames.Interfaces;

namespace CardGames.Models
{
	public abstract class GameBoardActions : IGameBoardActions
	{

		public bool IsGameOver { get; set; }
		public IGameBoard GameBoard { get; init; }

		public GameBoardActions(IGameBoard gb)
		{
			this.GameBoard = gb;
		}

		public void SaveGame()
		{
			//throw new NotImplementedException();
		}

		public virtual void StartGame()
		{
			Console.WriteLine("Initializing the Game and Shuffling the Deck");
			this.GameBoard.Deck.Shuffle();
			Thread.Sleep(500);
			Console.Clear();
		}
		public abstract void BeginningPhase();
		public abstract void MainPhase();
		public abstract void CombatPhase();
		public abstract void CheckWinCondition();
		public virtual void EndPhase()
		{
			this.CheckWinCondition();
			this.SaveGame();
		}
		public void QuitGame(bool saveGame)
		{
			this.IsGameOver = true;
			if (saveGame) this.SaveGame();
		}
	}
}
