using GameLogic.Enemy.EnemyBase;
using UnityEngine;

namespace GameLogic.Enemy.Enemys.DashEnemy
{
    public class DashEnemyModel
    {
        public float DashDistance; //触发冲刺的距离
        public Vector3 DashDirection; //冲刺方向
        public float DashSpeed; //冲刺速度
        public Vector3 DashGoal; //冲刺目标点
        
        public float DashTimeConsuming; //冲刺时长
        public float DashCooldown; //冲刺冷却时间
        public float LastDashTime; //最近冲刺时间
        public bool IsDash; //正在冲刺
        public float PREDashAnimation; //冲刺前摇

        public void Init()
        {
            DashDistance = 10;
            DashSpeed = 20f;
            DashTimeConsuming = 0.5f;
            PREDashAnimation = 0.2f;
            DashCooldown = 1;
            IsDash = false;
        }
    }
}