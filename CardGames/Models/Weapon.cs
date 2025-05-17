using CardGames.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CardGames.Models
{
	internal class Weapon : IWeapon
	{
		public required string WeaponName { get; init; }
		public int AttackPower { get; init; }
		public IEnemy? LastEnemyDefeated { get; set; } = null;
	}
}
