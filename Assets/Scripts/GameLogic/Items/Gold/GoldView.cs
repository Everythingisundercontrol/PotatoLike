using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Serialization;

namespace GameLogic.Items.Gold
{
    public class GoldView : MonoBehaviour
    {
        public SpriteRenderer spriteRenderer;

        public List<Sprite> spriteList;
        
        public NavMeshAgent agent;

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            agent.updateUpAxis = false;
            agent.updateRotation = false;
            agent.isStopped = true;
        }

        /// <summary>
        /// 设置贴图
        /// </summary>
        public void SetSprite(int num)
        {
            if (spriteList.Count > num)
            {
                spriteRenderer.sprite = spriteList[num];
            }
        }
        
    }
}