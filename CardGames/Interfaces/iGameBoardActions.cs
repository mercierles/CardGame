using CardGames.Models;
namespace CardGames.Interfaces{
    public interface IGameBoardActions{
		enum Actions
		{
			none = 0,
			run = 1,
			fight = 2,
			equipWeapon = 3,
			heal = 4,
			restart = 7,
			save = 8,
			quit = 9,
		}
		public IGameBoard GameBoard { get; init; }
		public bool IsGameOver {get;}        
        public void StartGame();
        public void QuitGame(bool saveGame);
        public void SaveGame();
        public void BeginningPhase();
        public void MainPhase();
		public void CombatPhase();
		public void EndPhase();
		public void CheckWinCondition();

	}
}