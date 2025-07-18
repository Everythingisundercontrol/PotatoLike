using UnityEngine;
using Yu;

namespace GameLogic.Bullet
{
    public class BulletSpriteCtrl : MonoBehaviour
    {
        public BulletController bulletController;

        /// <summary>
        /// 出视野后自动回收
        /// </summary>
        private void OnBecameInvisible()
        {
            if (bulletController.Active)
            {
                PoolManager.Instance.ReturnObject(bulletController);
            }
        }
    }
}