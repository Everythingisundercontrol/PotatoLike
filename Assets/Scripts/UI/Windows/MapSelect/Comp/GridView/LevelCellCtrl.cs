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
    public ScrollRect scrollRect;

    public float maxScale = 2f;
    public float minScale = 1f;
    public float maxDistance = 500f;

    private LevelCellModel _model;
    private LevelCellView _view;

    public void OnActivate() // 激活时
    {
        Active = true;
        LastUsedTime = Time.time;
        gameObject.SetActive(true);
    }

    public void OnDeactivate() // 主动归还时
    {
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

    public void Start()
    {
        var parent = gameObject.transform.parent.parent.parent;
        windowTrans = parent.transform;
        scrollRect = parent.GetComponent<ScrollRect>();
    }

    public void Update()
    {
        if (scrollRect.velocity.magnitude > 0.1f)
        {
            ChangeScale();
        }
    }

    /// <summary>
    /// 初始化
    /// </summary>
    public void OnInit()
    {
        _model = new LevelCellModel();//todo:不应该是初始化，而应该是从MapSelectModel里面获取
        _view = gameObject.GetComponent<LevelCellView>();
    }

    /// <summary>
    /// 根据与windowTrans的距离来动态改变缩放比例
    /// </summary>
    private void ChangeScale()
    {
        var viewportCenterX = windowTrans.position.x;
        var cellCenterX = transform.position.x;
        var distance = Mathf.Abs(viewportCenterX - cellCenterX);

        // 计算缩放比例（0~1）
        var scaleRatio = Mathf.Clamp01(1 - distance / maxDistance);
        var scale = Mathf.Lerp(minScale, maxScale, scaleRatio);

        // 应用缩放
        trans.localScale = new Vector3(scale, scale, 1f);
        Debug.Log(trans.position.x);
    }
}