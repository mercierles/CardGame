using CardGames.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGames.Models
{
	internal class Enemy : IEnemy
	{
		public required string EnemyName { get; init; }
		public int Health { get; init; }
		public int AttackStrength { get; init; }
	}
}
