namespace CardGames.Interfaces
{
    public interface IEnemy{
        public string EnemyName{get;init;}
        public int Health{get;init;}
        public int AttackStrength{get;init;}
    }
}