// ******************************************************************
//@file         GMCommandTest.cs
//@brief        gm指令示例
//@author       yufulao, yufulao@qq.com
//@createTime   2025.02.11 01:35:44
// ******************************************************************

using GameLogic.BattleManager;
using UnityEngine;
using Yu;

public class GMCommandTest
{
    /// <summary>
    /// 不显示在gm面板
    /// </summary>
    [GMMethod("输出0")]
    public static void Test0()
    {
        Debug.Log("Test0");
    }

    /// <summary>
    /// 显示在gm面板
    /// </summary>
    [GMMethodUI, GMMethod("输出0，显示在面板")]
    public static void TestUI0()
    {
        Debug.Log("TestUI0");
    }

    /// <summary>
    /// 显示在gm面板
    /// </summary>
    [GMMethodUI, GMMethod("角色死亡")]
    public static void TestPlayer0()
    {
        BattleManager.Instance.TestPlayer();
    }

    /// <summary>
    /// 显示在gm面板
    /// </summary>
    [GMMethodUI, GMMethod("生成敌人")]
    public static void TestEnemy0()
    {
        BattleManager.Instance.TestEnemy();
    }

    /// <summary>
    /// 显示在gm面板
    /// </summary>
    [GMMethodUI, GMMethod("切换武器")]
    public static void TestWeaponsChange()
    {
        BattleManager.Instance.TestWeaponsChange();
    }

    /// <summary>
    /// 带参数指令
    /// </summary>
    [GMMethod("带参数指令", "输出str")]
    public static void TestParams(string str)
    {
        Debug.Log("带参数指令" + str);
    }

    /// <summary>
    /// 带参数指令，显示在面板，手动输入
    /// </summary>
    [GMMethodUI, GMMethod("带参数指令，手动输入", "输出str")]
    public static void TestUIParams(string str)
    {
        Debug.Log("TestUIParams" + str);
    }

    /// <summary>
    /// 带参数指令，显示在面板，手动输入
    /// </summary>
    [GMMethodUI, GMMethod("武器放大倍数", "输出str")]
    public static void TestChangeWeaponScale(float str)
    {
        BattleManager.Instance.TestChangeWeaponScale(str);
    }

    /// <summary>
    /// 带参数指令，显示在面板，范围限定（下拉列表）
    /// </summary>
    [GMMethodUI, GMMethod("带参数指令，范围限定", "输出str")]
    public static void TestUIParamsProxy(TestStringProxy str)
    {
        Debug.Log("TestUIParams" + str);
    }
}

/// <summary>
/// 代理类型示例,限定TestUIParams的参数范围
/// </summary>
public class TestStringProxy : GmParameterOptionsBase<string>
{
    protected override object[] GetOptionsSource()
    {
        return new object[] {"可选项0", "可选项1", "可选项2"};
    }
}