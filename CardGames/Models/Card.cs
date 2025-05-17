using CardGames.Interfaces;

namespace CardGames.Models
{
	public class Card : ICard
	{
		public required ICard.Suit EnumSuit { get; init; }
		public required int CardValue { get; init; }

		public required string StrCardValue { get; init; }

		public Card() { }
	}
}
