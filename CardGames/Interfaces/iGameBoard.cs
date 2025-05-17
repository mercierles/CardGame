using CardGames.Models;
namespace CardGames.Interfaces{
    public interface IGameBoard{
        public IGameBoardActions GameBoardActions { get; init; }
        public ICoordinate HealthLocation{get;set;}
        public ICoordinate PrimaryDeckLocation{get;set;}
        public IDeck Deck {get;set;}
        public ICoordinate DiscardDeckLocation{get;set;}
        public IDeck DiscardDeck { get;set;}
        public string[,] GameBoardGrid {get;set;}
        public IPlayer Player {get;set;}
        public int Round { get; set;}
        public void Reset();
        public void Draw();
        public void GameOver();
        public void DrawGameRules();
    }
}