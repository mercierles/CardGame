using CardGames.Interfaces;
using CardGames.Models;
using static CardGames.Interfaces.IGameBoardActions;
using static CardGames.GameType.Scoundral.ScoundralGameBoard;
using System.Text;
using System.Runtime.CompilerServices;

namespace CardGames.GameType.Scoundral
{
	internal class ScoundralGameBoardActions : GameBoardActions
	{
		private ScoundralGameBoard _scoundralGameBoard;
		
		public const int TICKRATE = 800;

		public ScoundralGameBoardActions(ScoundralGameBoard GameBoard) : base(GameBoard) {
			this._scoundralGameBoard = GameBoard;
		}
		public override void StartGame()
		{
			base.StartGame();
			this.InitGame();
			_scoundralGameBoard.Draw();
			//Thread.Sleep(TICKRATE);
		}
		public override void BeginningPhase()
		{
			if (_scoundralGameBoard.dungeon.Count == 1)
			{
				_scoundralGameBoard.WriteToGameBoard(_scoundralGameBoard.dctCoordinates[CoordniateType.gameMessages][0], [$"Dungeon Complete, Press Enter to Draw the new Dungeon"],Placements.center);
				Console.ReadKey();
				this._scoundralGameBoard.ClearMessages();
				GameBoard.Player.UsedHealthPotion = false;
				this.InitDungeon(DUNGEONSIZE - this._scoundralGameBoard.dungeon.Count);
				this._scoundralGameBoard.DrawDungeon();
				this._scoundralGameBoard.DrawPlayerInfo();
				this.GameBoard.Round++;
			}
			if(!this.GameBoard.Player.CanFlee.Value && (this.GameBoard.Round - this.GameBoard.Player.CanFlee.Key >= 2))
			{
				this.GameBoard.Player.CanFlee = new KeyValuePair<int, bool>(this.GameBoard.Round, true);
				this._scoundralGameBoard.DrawPlayerInfo();
			}
			//Thread.Sleep(TICKRATE);
		}

		public override void MainPhase()
		{
			KeyValuePair<List<Actions>,List<string>> actionList = CreateActionList();
			this._scoundralGameBoard.WriteToGameBoard(_scoundralGameBoard.dctCoordinates[CoordniateType.gameMessages][0], actionList.Value, Placements.center,"|");
			if (Enum.TryParse(Console.ReadLine(), out Actions action) && Enum.IsDefined(action) && actionList.Key.Contains(action))
			{
				switch (action)
				{
					case Actions.quit:
						IsGameOver = true;
						break;
					case Actions.save:
						//TODO: Save current state to json/xml file and load in
						_scoundralGameBoard.WriteToGameBoard(_scoundralGameBoard.dctCoordinates[CoordniateType.gameMessages][0], [" Save Game Feature Unavailable"]);
						break;
					case Actions.fight:
						if(PerformAction("Fight an Enemy", Fight, [ICard.Suit.spades, ICard.Suit.clubs]))
						{
							this._scoundralGameBoard.Draw();
						}
						break;
					case Actions.run:
						_scoundralGameBoard.WriteToGameBoard(_scoundralGameBoard.dctCoordinates[CoordniateType.gameMessages][0], [" Trying to escape..."]);
						if (Flee()) { 
							this._scoundralGameBoard.DrawDungeon();
							this._scoundralGameBoard.DrawPlayerInfo();
							this._scoundralGameBoard.DrawGameBoardInfo();
						}
						break;
					case Actions.heal:
						_scoundralGameBoard.WriteToGameBoard(_scoundralGameBoard.dctCoordinates[CoordniateType.gameMessages][0], [" Healing Player"]);
						if(PerformAction("Use a Potion", UseHealthPotion, [ICard.Suit.hearts]))
						{
							this._scoundralGameBoard.DrawPlayerInfo();
							this._scoundralGameBoard.DrawGameBoardInfo();
							this._scoundralGameBoard.DrawDungeon();
						}
						break;
					case Actions.equipWeapon:
						_scoundralGameBoard.WriteToGameBoard(_scoundralGameBoard.dctCoordinates[CoordniateType.gameMessages][0], [" Equipping Weapon"]);
						if(PerformAction("Equip a Weapon", EquipWeapon, [ICard.Suit.diamonds]))
						{
							this._scoundralGameBoard.DrawDungeon();
							this._scoundralGameBoard.DrawWeaponInfo();
							this._scoundralGameBoard.DrawGameBoardInfo();
						}
						break;
					case Actions.restart:
						
						_scoundralGameBoard.WriteToGameBoard(_scoundralGameBoard.dctCoordinates[CoordniateType.gameMessages][0], [" Game Successfully Reset"]);
						this.Reset();
						this._scoundralGameBoard.Draw();
						break;
				}
			}
			else
			{
				MainPhase();
			}
			Thread.Sleep(TICKRATE);
			_scoundralGameBoard.ClearMessages();
		}
		public override void CombatPhase()
		{
			//Skip combat phase (This is integrated with the Main phase)
			//Thread.Sleep(TICKRATE);
		}
		public override void EndPhase()
		{
			base.EndPhase();
			_scoundralGameBoard.ClearMessages();
			//Thread.Sleep(TICKRATE);
		}
		public override void CheckWinCondition()
		{
			if (this.GameBoard.Player.Health <= 0)
			{
				Console.Clear();
				Console.WriteLine($"Player has died - HP:{this.GameBoard.Player.Health}");
				IsGameOver = true;
			}
			else
			{
				if (this.GameBoard.Deck != null && (this.GameBoard.Deck.Cards.Count == 0 || !this.GameBoard.Deck.Cards.Any(card => (card.EnumSuit == ICard.Suit.spades || card.EnumSuit == ICard.Suit.clubs))))
				{
					Console.Clear();
					Console.WriteLine($"Player Won!");
					IsGameOver = true;
				}
			}
		}

		//Private Methods
		private KeyValuePair<List<Actions>,List<string>> CreateActionList()
		{
			List<Actions> actionList = [];
			List<string> strActionList = [];
			if (this._scoundralGameBoard.Player.CanFlee.Value) { actionList.Add(Actions.run); strActionList.Add("1:Run"); }
			if (this._scoundralGameBoard.dungeon.Any(card =>card.EnumSuit == ICard.Suit.spades || card.EnumSuit == ICard.Suit.clubs)){ { actionList.Add(Actions.fight); strActionList.Add("2:Fight");}}
			if (this._scoundralGameBoard.dungeon.Any(card => card.EnumSuit == ICard.Suit.diamonds)) { { actionList.Add(Actions.equipWeapon); strActionList.Add("3:Equip Weapon"); } }
			if (this._scoundralGameBoard.dungeon.Any(card => card.EnumSuit == ICard.Suit.hearts)) { { actionList.Add(Actions.heal); strActionList.Add("4:Heal"); } }
			actionList.AddRange([Actions.restart, Actions.save, Actions.quit]);
			strActionList.AddRange(["7:Reset", "8:Save", "9:Quit"]);
			return new KeyValuePair<List<Actions>, List<string>>(actionList,strActionList);
		}
		private bool PerformAction(string actionType, Action<ICard> callBack, List<ICard.Suit> suits)
		{
			bool functionSuccess = false;
			bool boolPerformingAction = true;
			if (!_scoundralGameBoard.dungeon.Any(card=>suits.Contains(card.EnumSuit))) {
				_scoundralGameBoard.WriteToGameBoard(_scoundralGameBoard.dctCoordinates[CoordniateType.gameMessages][0], [$" {actionType} not Available"]);
				boolPerformingAction = false;
			}
			while (boolPerformingAction && _scoundralGameBoard != null)
			{
				int dungeonCount = _scoundralGameBoard.dungeon.Count;
				_scoundralGameBoard.WriteToGameBoard(_scoundralGameBoard.dctCoordinates[CoordniateType.gameMessages][0], [$" {actionType}! Enter the dungeon board position ({GetBoardPositions(suits)}) or Press 9 to cancel."]);

				if (int.TryParse(Console.ReadLine(), out int position) && (dungeonCount >= position - 1 && position - 1 >= 0) && suits.Contains(_scoundralGameBoard.dungeon[position - 1].EnumSuit))
				{
					ICard selectedCard = _scoundralGameBoard.dungeon[position - 1];
					callBack(selectedCard);
					this.GameBoard.DiscardDeck.Cards.Add(selectedCard);
					_scoundralGameBoard.dungeon.RemoveAt(position - 1);
					boolPerformingAction = false;
					functionSuccess = true;
				}
				else if (position ==9)
				{
					_scoundralGameBoard.WriteToGameBoard(_scoundralGameBoard.dctCoordinates[CoordniateType.gameMessages][0], [$" Canceling {actionType}"]);
					boolPerformingAction = false;
				} else
				{
					_scoundralGameBoard.WriteToGameBoard(_scoundralGameBoard.dctCoordinates[CoordniateType.gameMessages][0], [$" Valid board positions as listed: {GetBoardPositions(suits)}"]);
				}
			}
			return functionSuccess;
		}
		private bool Flee()
		{
			bool functionSuccess = false;
			if (this._scoundralGameBoard.dungeon.Count == 4 && this.GameBoard.Player.CanFlee.Value){
				this.GameBoard.Player.CanFlee = new KeyValuePair<int, bool>(this.GameBoard.Round, false);
				this._scoundralGameBoard.dungeon.ForEach(card => this._scoundralGameBoard.Deck.Cards.Add(card));
				this.InitDungeon(DUNGEONSIZE);
				_scoundralGameBoard.WriteToGameBoard(_scoundralGameBoard.dctCoordinates[CoordniateType.gameMessages][0], [" Trying to escape... Success!"]);
				functionSuccess = true;
			}else{
				_scoundralGameBoard.WriteToGameBoard(_scoundralGameBoard.dctCoordinates[CoordniateType.gameMessages][0], [" Unable to escape!"]);
			}
			return functionSuccess;
		}
		private void Fight(ICard enemyCard)
		{
			IWeapon? playerWeapon = this.GameBoard.Player.Weapons.Find(item => (item.LastEnemyDefeated == null || item.LastEnemyDefeated.Health > enemyCard.CardValue));
			if (playerWeapon != null)
			{
				int fightResults = (enemyCard.CardValue - playerWeapon.AttackPower);
				this.GameBoard.Player.Health = fightResults <= 0 ? this.GameBoard.Player.Health : this.GameBoard.Player.Health - fightResults;
				if (playerWeapon != null)
				{
					playerWeapon.LastEnemyDefeated = new Enemy() { EnemyName = enemyCard.StrCardValue, AttackStrength = enemyCard.CardValue, Health = enemyCard.CardValue };
				}
			}
			else
			{
				this.GameBoard.Player.Health -= enemyCard.CardValue;
			}
		}
		private void EquipWeapon(ICard weaponCard)
		{
			this.GameBoard.Player.Weapons.Clear();
			this.GameBoard.Player.Weapons.Add(new Weapon() { WeaponName = Enum.IsDefined(weaponCard.EnumSuit)?Enum.GetName(weaponCard.EnumSuit)!:"RandoName", AttackPower = weaponCard.CardValue, LastEnemyDefeated =null });
		}
		private void UseHealthPotion(ICard potionCard)
		{
			if (!GameBoard.Player.UsedHealthPotion)
			{
				GameBoard.Player.Health += potionCard.CardValue;
				if(GameBoard.Player.Health > 20) { GameBoard.Player.Health = 20; }
				GameBoard.Player.UsedHealthPotion = true;
			}
			else
			{
				_scoundralGameBoard.WriteToGameBoard(_scoundralGameBoard.dctCoordinates[CoordniateType.gameMessages][0], [" You have already used a poition this round, discarding potion"]);
			}
		}
		private string GetBoardPositions(List<ICard.Suit> suitList)
		{
			string boardPositions = "";
			for (int i = 0; i < _scoundralGameBoard.dungeon.Count; i++)
			{
				if (suitList.Contains(_scoundralGameBoard.dungeon[i].EnumSuit))
				{
					boardPositions += $"{i+1}, ";
				}
			}
			return boardPositions.TrimEnd().Trim(',');
		}

		//Private Init Methods
		private void InitGame()
		{
			this._scoundralGameBoard.DiscardDeck.Cards.Clear();
			this._scoundralGameBoard.Round = 0;
			this.InitPlayer();
			this.InitPrimaryDeck();
			this.InitDungeon(ScoundralGameBoard.DUNGEONSIZE);
		}
		private void InitPlayer()
		{
			this._scoundralGameBoard.Player.Health = 20;
			this._scoundralGameBoard.Player.Weapons.Clear();
			this._scoundralGameBoard.Player.EnemiesDefeated.Clear();
			this._scoundralGameBoard.Player.UsedHealthPotion = false;
			this._scoundralGameBoard.Player.CanFlee = new KeyValuePair<int, bool>(this._scoundralGameBoard.Round, true);
		}
		private void InitPrimaryDeck()
		{
			this._scoundralGameBoard.Deck.Cards.Clear();
			foreach (var tupleCard in this._scoundralGameBoard.scoundralCards)
			{
				foreach (var suit in tupleCard.Item2)
				{
					this._scoundralGameBoard.Deck.Cards.Add(new Card() { CardValue = tupleCard.Item1, EnumSuit = (ICard.Suit)suit, StrCardValue = tupleCard.Item3 });
				}
			}
			this._scoundralGameBoard.Deck.Shuffle();
		}
		private void InitDungeon(int dungeonSize)
		{
			if (dungeonSize == 4) { this._scoundralGameBoard.dungeon.Clear(); }
			this._scoundralGameBoard.Deck.Cards.Slice(0, dungeonSize).ForEach(card => { this._scoundralGameBoard.dungeon.Add(card); this._scoundralGameBoard.Deck.Cards.Remove(card); });
		}
		private void Reset()
		{
			this.InitGame();
		}
	}
}
