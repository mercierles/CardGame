using CardGames.Interfaces;
public class Player : IPlayer
{
    public int Health { get; set; }
    public List<IWeapon> Weapons { get; } = [];
    public List<IEnemy> EnemiesDefeated { get; } = [];
    public bool UsedHealthPotion { get; set; }
    public KeyValuePair<int,bool> CanFlee { get; set; }

    public Player(){
        this.Health = 100;
        this.Weapons = [];
        this.EnemiesDefeated = [];
        this.UsedHealthPotion = false;
        this.CanFlee =  new KeyValuePair<int, bool>(0,true);
        ;
    }

    public void AttackEnemy(IEnemy enemy){

    }
}