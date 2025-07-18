using UnityEngine;
using UnityEngine.UI;
using Yu;

namespace UI.HUD.EnemyHp
{
    public class EnemyHpCtrl : MonoBehaviour, IPoolableObject
    {
        public Image hpBarImage;
        public Text hpBarText;
        
        public float LastUsedTime { get; private set; }
        public bool Active { get; private set; }

        /// <summary>
        /// 初始化
        /// </summary>
        public void OnInit()
        {
            gameObject.SetActive(false);
        }
        
        /// <summary>
        /// 血量变化
        /// </summary>
        /// <param name="hp"></param>
        /// <param name="maxHp"></param>
        public void ChangeHp(int hp, float maxHp)
        {
            hpBarImage.fillAmount = hp / maxHp;
            hpBarText.text = hp.ToString();
        }

        /// <summary>
        /// 改变位置
        /// </summary>
        public void ChangePosition(Vector3 pos)
        {
            var screenPos = CameraManager.Instance.GetObjCamera().WorldToScreenPoint(pos);
            gameObject.transform.position = screenPos;
        }

        public void OnActivate()
        {
            Active = true;
            LastUsedTime = Time.time;
            gameObject.SetActive(true);
        }

        public void OnDeactivate()
        {
            Active = false;
            LastUsedTime = Time.time;
            gameObject.SetActive(false);
        }

        public void OnIdleDestroy()
        {
            if (Active)
            {
                PoolManager.Instance.ReturnObject(this);
            }

            Destroy(gameObject);
        }
    }
}