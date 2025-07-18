// ******************************************************************
//@file         EventName.cs
//@brief        事件名定义类
//@author       yufulao, yufulao@qq.com
//@createTime   2024.05.18 01:25:30
// ******************************************************************
namespace Yu
{
    public enum EventName
    {
        OnMouseLeftClick,
        OnMouseRightClick,
        OnHoldBegin,
        OnHoldEnd,
        ChangeScene,//场景切换时，1.无参。2.(string)角色出生点id
        OnLanguageChanged,
        
        OnExitGame,//退出游戏时
        OnPauseGame,//暂停游戏时
        OnUnpauseGame,//继续游戏时
        OnPauseViewClose,//暂停界面关闭时

        OnLoadGame,//加载存档时
        OnSaveGame,//保存当前存档时
        //OnLastSaveGameChange,//最新存档变化时
        
        InventoryContentChange,//背包物品变化
        OnGameTimeRunUpdate,//(float->本帧流逝的second)，Update调用
        OnGameTimeMinuteChange,//(无参数)
        OnGameTimeHourChange,//(无参数)
        OnGameTimeUnitChange,//(无参数)
        OnGameTimePeriodChange,//(无参数)
        OnGameTimeDayChange,//(无参数)
        OnGameTimeYearChange,//(无参数)
        OnCharacterEnergyChange,//(int,float)角色体力值比率变化时，(当前value，当前ratio)
        OnCharacterHungryStateChange,//(bool)角色饥饿状态变化时，true：转为饥饿状态，false：转为饱腹状态
        OnCharacterHungerChange,//(float,float)角色饥饿值比率变化时，(当前value，当前ratio)
        OnCharacterMoveSpeedChange,//(float)角色移动速度变化时
        OnCharacterCurrentExpInspirationChange,//(int)角色灵感经验变化时
        OnCharacterCurrentLevelInspirationChange,//(int)角色等级变化时
        
        OnCharacterDie,//角色死亡时
        OnCharacterSleepStart,//角色睡觉时
        OnCharacterSleepEnd,//(CharacterSleepType)角色睡觉时
        OnSkipTimeForCharacterSleep,//(GameTime->跳过的时间)角色睡觉跳过时间时
        
        RunAndStopAnimChange,//奔跑/站立动画切换
        
        AttackBegin,
        AttackEnd,
        
        RemoveSickleBulletCtrl,

        OnGmOpen, //打开GM界面时
        
        Pause,//暂停
        CancelPause,//取消暂停
        
        Heal,//使用治疗包
        MagGetGold,//使用磁铁吸金币
    }
}
