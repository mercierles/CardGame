namespace CardGames.Interfaces{
    public interface IWeapon{
        public string WeaponName{get;init;}
        public int AttackPower{get;init;}
        public IEnemy? LastEnemyDefeated{get;set;}

    }
}