using System.Collections;
using UnityEngine;

namespace GameLogic.Player.MVC
{
    public class PlayerView : MonoBehaviour
    {
        public GameObject cameraTarget;
        public Rigidbody2D playerRigidbody2D;
        public Collider2D playerCollider2D;
        public Animator anim;
        public SpriteRenderer spriteRenderer;
        
        public GameObject handLeftObj;
        public GameObject handRightObj;

        private static readonly int IsRun = Animator.StringToHash("isRun");
        private static readonly int IsDead = Animator.StringToHash("isDead");
        
        /// <summary>
        /// 受攻击动画
        /// </summary>
        /// <returns></returns>
        public IEnumerator UnderAttackAnim()
        {
            var oldColor = spriteRenderer.color;
            var attackedColor = new Color(1f, 0, 0);
            spriteRenderer.color = attackedColor;
            yield return new WaitForSeconds(0.2f);
            spriteRenderer.color = oldColor;
        }
        
        /// <summary>
        /// 移动/站立动画切换事件
        /// </summary>
        /// <param name="isRun"></param>
        public void SetIsRunAnim(bool isRun)
        {
            anim.SetBool(IsRun, isRun);
        }

        /// <summary>
        /// 设置是否播放死亡动画
        /// </summary>
        /// <param name="isDead"></param>
        public void SetIsDeadAnim(bool isDead)
        {
            anim.SetBool(IsDead, isDead);
        }
    }
}