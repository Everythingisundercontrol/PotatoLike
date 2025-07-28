using System;
using UI.Windows.MapSelect.Comp.GridView;
using UnityEngine;
using UnityEngine.UI;
using Yu;

public class LevelCellCtrl : MonoBehaviour, IPoolableObject
{
    public float LastUsedTime { get; private set; }
    public bool Active { get; private set; }

    public Transform trans;

    public Transform windowTrans;
    // public ScrollRect scrollRect;

    public float maxScale = 2f;
    public float minScale = 1f;
    public float maxDistance = 500f;

    private LevelCellModel _model;

    public void OnActivate() // 激活时
    {
        Active = true;
        LastUsedTime = Time.time;
        gameObject.SetActive(true);
    }

    public void OnDeactivate() // 主动归还时
    {
        _model = null;
        Active = false;
        LastUsedTime = Time.time;
        gameObject.SetActive(false);
    }

    public void OnIdleDestroy() // PoolManager自动销毁对象时
    {
        if (Active)
        {
            PoolManager.Instance.ReturnObject(this);
        }

        Destroy(gameObject);
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void OnInit(Transform scrollWindowTransform)
    {
        _model = new LevelCellModel(); //todo:不应该是初始化，而应该是从MapSelectModel里面获取
        // _view = gameObject.GetComponent<LevelCellView>();
        windowTrans = scrollWindowTransform;

        //初始化时默认关闭
        Active = false;
        gameObject.SetActive(false);
    }

    /// <summary>
    /// 根据与windowTrans的距离来动态改变缩放比例
    /// </summary>
    public void ChangeScale()
    {
        var viewportCenterX = windowTrans.position.x;
        var cellCenterX = transform.position.x;
        var distance = Mathf.Abs(viewportCenterX - cellCenterX);

        // 计算缩放比例（0~1）
        var scaleRatio = Mathf.Clamp01(1 - distance / maxDistance);
        var scale = Mathf.Lerp(minScale, maxScale, scaleRatio);

        // 应用缩放
        trans.localScale = new Vector3(scale, scale, 1f);

        if (distance > 200)
        {
            return;
        }

        EventManager.Instance.Dispatch(EventName.MapSelect_ChangeFocusCell, this);
    }

    /// <summary>
    /// 回收
    /// </summary>
    public void ReturnToPool()
    {
        PoolManager.Instance.ReturnObject(this);
    }

    /// <summary>
    /// 设置关卡信息
    /// </summary>
    public void SetLevelInfo(RowCfgScene rowCfgScene)
    {
        _model.LevelName = rowCfgScene.LevelName;
        _model.SceneID = rowCfgScene.Id;
        _model.LevelContent = rowCfgScene.LevelContent;
    }

    /// <summary>
    /// 获取内容
    /// </summary>
    public string GetContent()
    {
        return _model.LevelContent;
    }

    /// <summary>
    /// 获取ID
    /// </summary>
    public string GetLevelID()
    {
        return _model.SceneID;
    }

    /// <summary>
    /// 获取名字
    /// </summary>
    public string GetLevelName()
    {
        return _model.LevelName;
    }
}