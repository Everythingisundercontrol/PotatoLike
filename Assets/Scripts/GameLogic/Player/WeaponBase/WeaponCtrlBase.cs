using UnityEngine;
using Yu;

namespace GameLogic.Player.WeaponBase
{
    public class WeaponCtrlBase : MonoBehaviour
    {
        protected SpriteRenderer SpriteRenderer;
        protected Animator Animator;
        
        protected WeaponModelBase Model;
        
        private GameObject _parentTransformObj;

        public virtual void Init()
        {
            Model = new WeaponModelBase();
            Model.Init();
            _parentTransformObj = gameObject.transform.parent.parent.gameObject;
            SpriteRenderer = _parentTransformObj.GetComponentInChildren<SpriteRenderer>();
            Animator = _parentTransformObj.GetComponentInChildren<Animator>();
            
            BindEvent();
        }

        public virtual void WeaponFixedUpdate()
        {
            Aim();
        }

        public virtual void Quit()
        {
            EventManager.Instance.RemoveListener(EventName.AttackBegin, ChangeIfHoldMouseToTrue);
            EventManager.Instance.RemoveListener(EventName.AttackEnd, ChangeIfHoldMouseToFalse);
        }

        /// <summary>
        /// 改变大小，只有近战重载了这个函数。
        /// </summary>
        /// <param name="scaleNum"></param>
        public virtual void SetScaleNum(float scaleNum)
        {
        }

        /// <summary>
        /// 事件绑定
        /// </summary>
        private void BindEvent()
        {
            EventManager.Instance.AddListener(EventName.AttackBegin, ChangeIfHoldMouseToTrue);
            EventManager.Instance.AddListener(EventName.AttackEnd, ChangeIfHoldMouseToFalse);
        }

        /// <summary>
        /// 武器瞄准鼠标位置
        /// </summary>
        private void Aim()
        {
            if (!Model.Main || !Model.ObjCamera)
            {
                return;
            }

            var mouseScreenPos = InputManager.Instance.GetMousePosition();
            var mouseWorldPos = Model.Main.ScreenToWorldPoint(mouseScreenPos);

            var playerWorldPos = _parentTransformObj.transform.position - Model.ObjCamera.transform.position;

            var direction = mouseWorldPos - playerWorldPos;
            Model.Angle = Vector2.Angle(direction, Vector2.up);
            if (mouseWorldPos.x > playerWorldPos.x)
            {
                Model.Angle = -Model.Angle;
            }

            switch (direction.x)
            {
                case < 0:
                    _parentTransformObj.transform.localScale = new Vector3(-1, 1, 1);
                    _parentTransformObj.transform.eulerAngles = new Vector3(0f, 0f, Model.Angle - 90);
                    break;
                case >= 0:
                    _parentTransformObj.transform.localScale = Vector3.one;
                    _parentTransformObj.transform.eulerAngles = new Vector3(0f, 0f, Model.Angle + 90);
                    break;
            }
        }

        /// <summary>
        /// 设置为true
        /// </summary>
        private void ChangeIfHoldMouseToTrue()
        {
            Model.IfHoldMouse = true;
        }

        /// <summary>
        /// 设置为false
        /// </summary>
        private void ChangeIfHoldMouseToFalse()
        {
            Model.IfHoldMouse = false;
        }
    }
}