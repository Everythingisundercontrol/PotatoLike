namespace GameLogic.Enemy.EnemyBase
{
    public class EnemyBaseModel
    {
        public float MaxHp;   //最大血量
        public float Hp;    //血量
        public float Speed; //速度
        public float Acceleration;  //加速度
        public float Attack;    //攻击力

        public float DemiseAwaitRecycleDuration;   //死亡回收等待时间
        public float AttackedWaitTime;  //受击僵直时间
        
        protected internal bool FaceToLeft; //是否面朝左
        
        public virtual void Init(float speed,float acceleration)
        {
            Hp = 6;
            Speed = speed;
            Acceleration = acceleration;
            Attack = 1;
            DemiseAwaitRecycleDuration = 3;
            AttackedWaitTime = 0.1f;

            MaxHp = Hp;
        }

        /// <summary>
        /// 重置状态
        /// </summary>
        public void RecoverState()
        {
            Hp = MaxHp;
        }
    }
}