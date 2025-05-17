namespace CardGames.Interfaces{
    public interface IDeck{
        const int DECKSIZE = 52;
        public List<ICard> Cards{get;init;}
        public void Shuffle();
    }    
}