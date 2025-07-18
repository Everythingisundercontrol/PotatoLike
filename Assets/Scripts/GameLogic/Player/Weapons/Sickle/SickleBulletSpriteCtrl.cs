using UnityEngine;
using UnityEngine.Serialization;
using Yu;

namespace GameLogic.Player.Weapons.Sickle
{
    public class SickleBulletSpriteCtrl : MonoBehaviour
    {
        public SickleBulletCtrl sickleBulletCtrl;
        
        /// <summary>
        /// 出视野后自动回收
        /// </summary>
        private void OnBecameInvisible()
        {
            if (sickleBulletCtrl.Active)
            {
                PoolManager.Instance.ReturnObject(sickleBulletCtrl);
            }
        }
    }
}