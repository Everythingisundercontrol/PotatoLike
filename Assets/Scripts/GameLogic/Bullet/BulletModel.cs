namespace GameLogic.Bullet
{
    public class BulletModel
    {
        public float RotationZ; //子弹旋转角度

        public float Damage; //伤害

        public float BulletSpeed = 24f; //子弹速度

        public float BulletPenetrationCount; //子弹穿透次数

        public float BulletExitTime; //子弹存续时间

        public BulletType BulletType; //子弹类型

        public int EnemyBulletValue; //敌人子弹价值

        public void SetValue(float rotationZ, float damage, float bulletSpeed, float bulletPenetrationCount, float bulletExitTime, BulletType bulletType)
        {
            RotationZ = rotationZ;
            Damage = damage;
            BulletSpeed = bulletSpeed;
            BulletPenetrationCount = bulletPenetrationCount;
            BulletExitTime = bulletExitTime;
            BulletType = bulletType;

            EnemyBulletValue = (int) (Damage - 1);
        }
    }
}