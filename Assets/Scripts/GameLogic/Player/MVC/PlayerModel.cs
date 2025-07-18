using System;
using System.Collections.Generic;
using GameLogic.Player.WeaponBase;
using UnityEngine;
using Yu;

namespace GameLogic.Player.MVC
{
    public class PlayerModel
    {
        public readonly List<string> WeaponsList = new List<string>()
        {
            "ShovelPrefab", "PitchforkPrefab", "SicklePrefab", "GunPrefab", "MachineGunPrefab", "ShotGunPrefab"
        };

        public List<WeaponCtrlBase> WeaponCons; //武器ctrl

        public bool IsRun //是否奔跑，用于检测奔跑动画
        {
            get => _isRun;
            set
            {
                if (_isRun == value)
                {
                    return;
                }

                _isRun = value;
                //动画切换
                EventManager.Instance.Dispatch(EventName.RunAndStopAnimChange, value); //根据速度是否为0来切换动画
            }
        }

        public bool FaceToLeft; //是否面朝左，用于检测方向
        public bool IsDead; //是否死亡，用于检测是否播放死亡动画
        public bool IsUnderAttackAnim; //是否正在播放受攻击动画

        public float HorizontalMovement; //移动键键入
        public float VerticalMovement; //移动键键入

        public float MovementSpeed = 12f; //移动速度乘率

        public float Acceleration = 0.1f; //加速
        public float Deceleration = 0.3f; //减速

        public int maxHp;
        public int Hp; //生命值
        public Vector2 Speed; //实时速度

        public float ATKSpeed; //攻击速度
        public float AttackMultiplier; //攻击乘数
        public float Attack; //攻击力

        public int Money; //金钱
        public int BoxNum; //箱子数
        public int HealthPackNum; //血包数

        public float CriticalRate; //暴击率
        public float CriticalDamageMultiplier; //暴击伤害乘数

        public float Armor; //护甲值
        public float Evade; //闪避率

        public float LastUnderAttackTime; //上次受攻击时间
        public float UnderAttackCoolDown; //受攻击冷却时间

        public float HorizontalSpeed; //速度
        public float VerticalSpeed; //速度

        //旧值
        private float _horizontalSpeed;
        private float _verticalSpeed;

        private float _horizontalMovement;
        private float _verticalMovement;

        private bool _isRun;

        /// <summary>
        /// 初始化数据
        /// </summary>
        public void Init()
        {
            maxHp = 10;
            Hp = 10;
            Armor = 0;
            Speed = new Vector2(0, 0);
            Attack = 1;
            UnderAttackCoolDown = 1f;
        }

        /// <summary>
        /// 受击时护甲检测
        /// </summary>
        public float AttackedArmorCheck(float atk)
        {
            if (Armor >= atk)
            {
                Armor -= atk;
                return -1;
            }

            if (!(Armor < atk) || !(Armor > 0))
            {
                return atk;
            }

            atk -= Armor;
            Armor = 0;
            return atk;
        }

        /// <summary>
        /// 有键入时速度判断
        /// </summary>
        public void OnInputSpeedCheck()
        {
            //加速
            if (Math.Abs(Speed.x - HorizontalMovement * MovementSpeed) > 0 || Math.Abs(Speed.y - VerticalMovement * MovementSpeed) > 0)
            {
                SpeedCalculate(Acceleration);
            }
        }

        /// <summary>
        /// 没有键入时速度判断
        /// </summary>
        public void NotInputSpeedCheck()
        {
            if (Speed.x == 0 && Speed.y == 0)
            {
                return;
            }

            //减速
            SpeedCalculate(Deceleration);
        }

        /// <summary>
        /// 计算速度
        /// </summary>
        private void SpeedCalculate(float t)
        {
            HorizontalSpeed = Mathf.Lerp(_horizontalSpeed, HorizontalMovement * MovementSpeed, t);
            VerticalSpeed = Mathf.Lerp(_verticalSpeed, VerticalMovement * MovementSpeed, t);
            var v2 = SetMAXAndMINEdge(MovementSpeed - 0.5f, 0.1f, HorizontalSpeed, VerticalSpeed);

            _horizontalSpeed = v2.x;
            _verticalSpeed = v2.y;
            Speed = v2;
        }

        /// <summary>
        /// 设置速度最大最小临界值，越过临界值则直接赋极值
        /// </summary>
        private Vector2 SetMAXAndMINEdge(float max, float min, float horizontalSpeed, float verticalSpeed)
        {
            if (horizontalSpeed < min && horizontalSpeed > -min)
            {
                horizontalSpeed = 0;
            }

            if (verticalSpeed < min && verticalSpeed > -min)
            {
                verticalSpeed = 0;
            }

            if (horizontalSpeed > max || horizontalSpeed < -max)
            {
                horizontalSpeed = HorizontalMovement * MovementSpeed;
            }

            if (verticalSpeed > max || verticalSpeed < -max)
            {
                verticalSpeed = VerticalMovement * MovementSpeed;
            }

            return new Vector2(horizontalSpeed, verticalSpeed);
        }
    }
}