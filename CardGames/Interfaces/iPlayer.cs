namespace CardGames.Interfaces{
    public interface IPlayer{
        public int Health {get;set;}
        public List<IWeapon> Weapons {get;}
        public List<IEnemy> EnemiesDefeated{get;} //Check last Item, clear when new weapon equiped
        public void AttackEnemy(IEnemy enemy);
        public bool UsedHealthPotion { get; set; }
        public KeyValuePair<int, bool> CanFlee { get; set; }
    }
}