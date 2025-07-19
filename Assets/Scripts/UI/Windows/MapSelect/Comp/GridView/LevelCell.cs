using System;
using UnityEngine;
using UnityEngine.UI;

public class LevelCell: MonoBehaviour
{
    public Transform trans;
    public Transform windowTrans;
    public ScrollRect scrollRect; 

    public float maxScale = 2f;
    public float minScale = 1f;
    public float maxDistance = 500f;

    public void Start()
    {
        var parent = gameObject.transform.parent.parent.parent;
        windowTrans = parent.transform;
        scrollRect = parent.GetComponent<ScrollRect>();
        ChangeScale();
    }

    private void Update()
    {
        if (scrollRect.velocity.magnitude > 0.1f)
        {
            ChangeScale();
        }
    }

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
