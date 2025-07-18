namespace GameLogic.Items.Gold
{
    public class GoldModel
    {
        public int GoldValue; //金钱数

        public bool IfMag; //是否磁吸

        /// <summary>
        /// 初始化
        /// </summary>
        public void Init()
        {
            IfMag = false;
        }
    }
}