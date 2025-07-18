using UnityEngine;
using Yu;

namespace GameLogic.Player.WeaponBase
{
    public class WeaponModelBase
    {
        public Camera Main;
        public Camera ObjCamera;

        public float Angle;
        
        public bool IfHoldMouse; //是否按下鼠标攻击键

        public void Init()
        {
            Main = CameraManager.Instance.GetUICamera();
            ObjCamera = CameraManager.Instance.GetObjCamera();
        }
    }
}