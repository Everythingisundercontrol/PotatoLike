
namespace GameLogic.Player.Weapons.Gun
{
    public class GunModel
    {
        public float WeaponAtk; //武器附加攻击力

        public float LastAttackTime; //上次攻击时间

        public float TimeLock; //最短攻击时间间隔

        public void Init()
        {
            WeaponAtk = 2f;
            TimeLock = 0.5f;
        }
    }
}