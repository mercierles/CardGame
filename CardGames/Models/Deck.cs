using CardGames.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace CardGames.Models
{
	public class Deck : IDeck
	{
		public string DeckName { get; init; }
		public List<ICard> Cards { get; init; }

		public Deck(string deckName){
			this.DeckName = deckName;
			this.Cards = [];
		}
		private static Random s_random = new Random();
		public void Shuffle()
		{
			int deckSize = this.Cards.Count - 1;
			int x = deckSize;
			while (x >= 0)
			{
				int indexToSwap = s_random.Next(1, deckSize);
				ICard tmpCard = this.Cards[x];
				this.Cards.RemoveAt(x);
				this.Cards.Insert(indexToSwap, tmpCard);
				x--;
			}
		}
	}
}
