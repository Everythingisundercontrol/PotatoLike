using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI.Windows.Battle
{
    public class BattleView : MonoBehaviour
    {
        //hp
        public Image hpBarImage;
        public Text hpBarText;

        //gold
        // public GameObject gold;
        public Text goldText;

        // public GameObject box;
        public Text boxText;

        // public GameObject healthPack;
        public Text healthPackText;

        public Text timeText;

        public List<Sprite> weaponSpriteList;
        public Image nowWeaponImage;

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
        /// 金币变化
        /// </summary>
        /// <param name="num"></param>
        public void ChangeGold(int num)
        {
            goldText.text = num.ToString();
        }

        /// <summary>
        /// 箱子数目变化
        /// </summary>
        /// <param name="num"></param>
        public void ChangeBox(int num)
        {
            boxText.text = " × " + num;
        }

        /// <summary>
        /// 血包数目变化
        /// </summary>
        /// <param name="num"></param>
        public void ChangeHealthPack(int num)
        {
            healthPackText.text = " × " + num;
        }

        /// <summary>
        /// 改变武器图片
        /// </summary>
        /// <param name="num"></param>
        public void ChangeWeaponImage(int num)
        {
            if (num >= 0 && num < weaponSpriteList.Count)
            {
                nowWeaponImage.sprite = weaponSpriteList[num];
            }
        }

        /// <summary>
        /// 倒计时变化
        /// </summary>
        /// <param name="num"></param>
        public void ChangeTime(int num)
        {
            timeText.text = num.ToString();
        }

        /// <summary>
        /// 打开窗口
        /// </summary>
        public void OpenWindow()
        {
            gameObject.SetActive(true);
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        public void CloseWindow()
        {
            if (!gameObject.activeInHierarchy)
            {
                return;
            }

            gameObject.SetActive(false);
        }
    }
}