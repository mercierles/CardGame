using CardGames.BaseClass;
using CardGames.Interfaces;
using CardGames.Models;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.InteropServices;

namespace CardGames.GameType.Scoundral
{
	public class ScoundralGameBoard : GameBoard
	{
		public enum Placements
		{
			none = 0,
			left = 1,
			right = 2,
			center = 3,
			evenSpacing = 4
		}
		public enum CoordniateType
		{
			playerInfo=0,
			weaponInfo=1,
			dungeonInfo=2,
			gameInfo=3,
			gameMessages=4,
			gameActionList=5,
			gameUserInput=6,
			gameDivider=7,
		}

		private const int MAXBOARDWIDTH = 80;
		private const int MAXBOARDHEIGHT = 20;
		private const int RULESCONSOLEWIDTH = MAXBOARDWIDTH + 20;
		private const int RULESCONSOLEHEIGHT = MAXBOARDHEIGHT+5;

		public const int DUNGEONSIZE = 4;
		public readonly (int, int[], string)[] scoundralCards = [
				(14,[2,3],"A"),
				(2,[0,1,2,3],"2"),
				(3,[0,1,2,3],"3"),
				(4,[0,1,2,3],"4"),
				(5,[0,1,2,3],"5"),
				(6,[0,1,2,3],"6"),
				(7,[0,1,2,3],"7"),
				(8,[0,1,2,3],"8"),
				(9,[0,1,2,3],"9"),
				(10,[0,1,2,3],"10"),
				(11,[2,3],"J"),
				(12,[2,3],"Q"),
				(13,[2,3],"K"),
			];
		public List<ICard> dungeon = [];
		public Dictionary<CoordniateType, Coordinate[]> dctCoordinates = new Dictionary<CoordniateType, Coordinate[]>()
			{
				{ CoordniateType.playerInfo,[new Coordinate("PlayerInfo", 1, 1)]},
				{ CoordniateType.weaponInfo, [new Coordinate("Weapon", 1, 2)]},
				{ CoordniateType.dungeonInfo, [new Coordinate("Dungeon", 1, 5)]},
				{ CoordniateType.gameInfo,[new Coordinate("GameInfo", 1, 8)]},
				{ CoordniateType.gameMessages,[new Coordinate("Messages", 1, 11)]},
				{ CoordniateType.gameDivider,[new Coordinate("Divider", 1, 12),new Coordinate("Divider2", 1, 10)]},
				{ CoordniateType.gameUserInput, [new Coordinate("UserInput", 4, 13)]},
			};

		[SetsRequiredMembers]
		public ScoundralGameBoard() : base()  {
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				Console.SetWindowSize(RULESCONSOLEWIDTH, RULESCONSOLEHEIGHT);
				Console.SetBufferSize(RULESCONSOLEWIDTH, RULESCONSOLEHEIGHT+100);
			}
			base.GameBoardActions = new ScoundralGameBoardActions(this);
			base.Player = new Player() { Health = 20 };
		}
		public override void Draw()
		{
			this.DrawPlayerInfo();
			this.DrawGameBoardInfo();
			this.DrawWeaponInfo();
			this.DrawDungeon();
			dctCoordinates[CoordniateType.gameDivider].ToList().ForEach(x => { this.WriteToGameBoard(x, [new string('-', MAXBOARDWIDTH-2)], Placements.none); });
		}
		public override void Reset()
		{
			Console.Clear();
			this.Draw();
		}
		public override void GameOver()
		{
			this.WriteToGameBoard(dctCoordinates[CoordniateType.gameMessages][0], ["GAME OVER"], Placements.center);
			Console.ReadKey();
		}
		public override void DrawGameRules()
		{
			//AI Generated
			string gameText = @"~~~~~~~~Intro:~~~~~~~~
The	  'Scoundrel' card game is a single-player, rogue-like game played using a standard 52-card deck with the red face cards (Jacks, Queens, Kings) and red Aces removed, leaving 36 cards.

~~~~~~~~Objective:~~~~~~~~
The  goal of Scoundrel is to survive the entire 'dungeon' (the deck of cards) without your health reaching zero. The game ends if you exhaust the entire deck or if your health reaches zero.

~~~~~~~~Setup:~~~~~~~~
1.  Remove all Jokers, red Jacks, red Queens, red Kings, and red Aces from the deck.

2.  Shuffle the remaining 36 cards (all Clubs, all Spades, all Diamonds, and all Hearts). This forms the 'Dungeon' deck and is placed face down.

3.  Keep track of your starting health, which is 20. You can use paper and pen or your memory.

~~~~~~~~Card Meanings:~~~~~~~~
*  Clubs & Spades: These 26 black cards are 'Monsters'. Their damage value is equal to their rank (2-10, Jack=11, Queen=12, King=13, Ace=14).

*  Diamonds: These 9 red cards are 'Weapons'. A weapon's damage value is equal to their rank (2-10, Jack=11, Queen=12, King=13, Ace=14). When you acquire a weapon, you must equip it immediately, discarding any previously held weapon. Weapons have a binding effect: after using a weapon to defeat a monster, it can only be used on monsters with a rank equal to or lower than the last defeated monster.

*  Hearts: These 9 red cards are 'Health Potions'. The value of a Health Potion is equal to its rank (2-10, Jack=11, Queen=12, King=13, Ace=14), and this amount is added to your current health. You can only use one Health Potion per turn; any additional potions drawn in the same turn are discarded without effect.

~~~~~~~~Gameplay:~~~~~~~~
1.  Forming a Room: On your turn, flip over the top four cards from the Dungeon deck and place them face up in front of you. This is the 'Room.'

2.  Choosing to Face or Avoid the Room: You have the option to either 'face' the Room or 'avoid' it.
    *  Avoid: If you choose to avoid the Room, collect all four face-up cards and place them at the bottom of the Dungeon deck in one motion. However, you cannot avoid two Rooms in a row.
    *  Face: If you choose to face the Room, you must interact with three of the four cards in the Room, one at a time. The fourth card remains face up and will be part of the next Room.

3.  Interacting with Cards: When facing a Room, you will encounter Monsters, Weapons, or Health Potions.
    *  Monster (Club or Spade): You can choose to fight the monster 'barehanded' or with your equipped weapon.
        *  Barehanded: Subtract the monster's full damage value from your health, and then discard the monster card.
        *  With Weapon: Place the monster card face up on top of your equipped weapon (stacking it on any previously defeated monsters, ensuring the weapon's value is still visible). Calculate the damage you take: (Monster's value) - (Weapon's value). Subtract this remaining damage from your health. Remember the weapon's binding rule for subsequent attacks.
    *  Weapon (Diamond): You must equip the weapon by placing it face up between you and the remaining cards in the Room. If you had a weapon equipped previously, discard it along with any monsters defeated by that weapon.
    *  Health Potion (Heart): Add the potion's value to your current health (up to a maximum of 20), and then discard the potion. You can only use one Health Potion per turn; any additional potions drawn in the same turn are discarded without effect.

4.  End of Turn: After you have interacted with three of the four cards in the Room, your turn ends. The remaining face-up card becomes the first card of the next Room.

5.  Game End: The game ends in one of two ways:
    *  You survive the Dungeon: If you draw and interact with all the cards in the Dungeon deck without your health reaching zero.
    *  Your health reaches zero: If at any point your health is reduced to 0 or less.

'Scoundrel' is a game of strategic choices and risk management, where you must decide when to fight, when to use healing, and when it's best to avoid a dangerous room to survive the perils of the dungeon.";
			Console.WriteLine(WordWrap(gameText, RULESCONSOLEWIDTH));
			Console.WriteLine("\n\n\n>>>	Scroll up to read rules, or Press Enter to Start!");
			Console.ReadKey(true);
			this.AdjustConsoleSize(MAXBOARDWIDTH, MAXBOARDHEIGHT);
		}
		public void DrawWeaponInfo()
		{
			if (this.Player.Weapons.Count > 0)
			{
				IWeapon tmpWeapon = this.Player.Weapons.FirstOrDefault()!;
				this.WriteToGameBoard(dctCoordinates[CoordniateType.weaponInfo][0], [$"Weapon:{tmpWeapon.AttackPower}", $"Last Enemy Slain:{((tmpWeapon.LastEnemyDefeated != null) ? tmpWeapon.LastEnemyDefeated.Health : "None")}"], Placements.evenSpacing);
			}
			else
			{
				this.WriteToGameBoard(dctCoordinates[CoordniateType.weaponInfo][0], [""]);
			}
		}
		public void DrawGameBoardInfo()
		{
			this.WriteToGameBoard(dctCoordinates[CoordniateType.gameInfo][0], [$"Cards Left:{this.Deck.Cards.Count}", $"Cards Discarded:{this.DiscardDeck.Cards.Count}"], Placements.evenSpacing);
		}
		public void DrawPlayerInfo()
		{
			this.WriteToGameBoard(dctCoordinates[CoordniateType.playerInfo][0], [$"Health:{this.Player.Health}", $"Potion Available:{(this.Player.UsedHealthPotion ? 'N' : 'Y')}", $"Can Flee:{(this.Player.CanFlee.Value ? "Y" : "N")}"], Placements.evenSpacing);
		}
		public void DrawDungeon()
		{
			List<string> listDungeonCards = [];
			this.dungeon.ForEach(card => listDungeonCards.Add($"{card.CardValue}-{Enum.GetName(card.EnumSuit)}"));
			this.WriteToGameBoard(dctCoordinates[CoordniateType.dungeonInfo][0], [TextCenter(listDungeonCards, "|")], Placements.center);
		}
		public void WriteToGameBoard(ICoordinate coordinate, List<string> lstString, Placements placement = Placements.none, string divider="")
		{
			string tmpStr = "";
			switch (placement)
			{
				case Placements.none:
				case Placements.left:
				default: 
					lstString.ForEach(str => tmpStr += str + divider);
					if (divider != String.Empty) { tmpStr = tmpStr.TrimEnd([Char.Parse(divider), ' ']); }
					break;
				case Placements.right:
					tmpStr = TextRight(lstString,divider);
					break;
				case Placements.center:
					tmpStr = TextCenter(lstString,divider);
					break;
				case Placements.evenSpacing:
					tmpStr = SpaceTextEvenly(lstString, divider);
					break;
			}
			Console.SetCursorPosition(0, coordinate.Y);
			Console.Write(new string(' ', Console.BufferWidth));
			Console.SetCursorPosition(coordinate.X, coordinate.Y);
			Console.Write(tmpStr);
			Console.SetCursorPosition(dctCoordinates[CoordniateType.gameUserInput][0].X, dctCoordinates[CoordniateType.gameUserInput][0].Y);
			this.ClearPlayerInput();
		}
		public void ClearMessages()
		{
			Console.SetCursorPosition(0, dctCoordinates[CoordniateType.gameMessages][0].Y);
			Console.Write(new string(' ', Console.BufferWidth));
			Console.SetCursorPosition(dctCoordinates[CoordniateType.gameUserInput][0].X, dctCoordinates[CoordniateType.gameUserInput][0].Y);
		}
		public void ClearPlayerInput()
		{
			Console.SetCursorPosition(0, dctCoordinates[CoordniateType.gameUserInput][0].Y);
			Console.Write(new string(' ', 1));
			Console.Write(new string('>', 2));
			Console.Write(new string(' ', Console.BufferWidth-2));
			Console.SetCursorPosition(Console.BufferWidth-3, dctCoordinates[CoordniateType.gameUserInput][0].Y);
			Console.Write("<<");
			Console.SetCursorPosition(dctCoordinates[CoordniateType.gameUserInput][0].X, dctCoordinates[CoordniateType.gameUserInput][0].Y);
		}
		
		//Private Drawing Helpers
		private string SpaceTextEvenly(List<string> strs, string divider = "")
		{
			if(strs.Count > 1)
			{
				string tmpStr ="";
				int spacing = (MAXBOARDWIDTH-2 - strs.Sum(str => str.Length))/(strs.Count-1);
				int spacingRemainder = (MAXBOARDWIDTH-2 - strs.Sum(str => str.Length)) % (strs.Count - 1);
				if (spacing < 2) { spacing = 2; }
				for (int i = 0; i < strs.Count; i++)
				{
					if (i == strs.Count-1)
					{
						tmpStr = tmpStr.PadRight(tmpStr.Length + spacingRemainder, ' ');
					}
					tmpStr += (strs[i].PadRight(strs[i].Length + (spacing), ' ') + divider);
				}
				if(divider != String.Empty) { tmpStr = tmpStr.TrimEnd([Char.Parse(divider), ' ']); }
				return tmpStr.TrimEnd();
			}
			return strs[0];
		}
		private string TextCenter(List<string> lstStr, string strDivider = "")
		{
			string str = "";
			foreach (var item in lstStr)
			{
				str += item+strDivider;
			}
			if (strDivider != String.Empty) { str = str.TrimEnd([Char.Parse(strDivider), ' ']); }
			int spaceAvailable = (MAXBOARDWIDTH - str.Length - 2);
			if (spaceAvailable < 2) { spaceAvailable = 2; }
			return (str.PadLeft(str.Length + (spaceAvailable / 2), ' ')).PadRight(str.Length + (spaceAvailable), ' ');
		}
		private string TextRight(List<string> lstStr, string strDivider = "")
		{
			string str = "";
			foreach (var item in lstStr)
			{
				str += item + strDivider;
			}
			if (strDivider != String.Empty) { str = str.TrimEnd([Char.Parse(strDivider), ' ']); }
			int spaceAvailable = (MAXBOARDWIDTH - str.Length - 2);
			if (spaceAvailable < 2) { spaceAvailable = 2; }
			return (str.PadLeft(str.Length + (spaceAvailable / 2), ' ')).PadLeft(str.Length + (spaceAvailable), ' ').TrimEnd();
		}
		private string WordWrap(string text, int boardLength = MAXBOARDWIDTH)
		{
			//AI Generated
			int maxCharsPerLine = boardLength;
			if (text.Length <= maxCharsPerLine)
				return text;

			string[] words = text.Split(' ');
			string result = "";
			string line = "";

			foreach (string word in words)
			{
				if (word.Contains("\n"))
				{
					string[] newWords = word.Split("\n");
					for (int i = 0; i < newWords.Length; i++)
					{
						if (i == 0) {
							if ((line + newWords[i]).Length <= maxCharsPerLine)
							{
								line += newWords[i] + "\n ";
								result += line.TrimEnd() + "\n";
							}
							else
							{
								result += line.TrimEnd() + "\n";
								result += newWords[i] + "\n";
							}
							line = " ";

						}
						else if (newWords.Length - 1 == i)
						{
							line += newWords[i].Trim();
						}
						else
						{
							result += newWords[i].Trim() + "\n";
							line = " ";

						}
					}
				}
				else if ((line + word).Length <= maxCharsPerLine)
				{
					line += word + " ";
				}
				else
				{
					result += line.TrimEnd() + "\n";
					line = word + " ";
				}
			}
			result += line.TrimEnd();
			return result;
		}
		private void AdjustConsoleSize(int width, int height)
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				Console.Clear();
				Console.SetWindowSize(MAXBOARDWIDTH, MAXBOARDHEIGHT);
				Console.SetBufferSize(MAXBOARDWIDTH, MAXBOARDHEIGHT);
			}
		}

	}

}