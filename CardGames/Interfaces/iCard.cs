namespace CardGames.Interfaces
{
    public interface ICard
    {
        [Flags]
        enum Suit{
            hearts=0,
            diamonds=1,
            spades=2,
            clubs=3
        }
		public Suit EnumSuit{get;init;}
        public string StrCardValue { get; init; }
        public int CardValue{get;init;}
    }
}