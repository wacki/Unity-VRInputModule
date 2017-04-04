using UnityEngine;
using UnityEngine.EventSystems;

namespace Wacki {
    public class LaserPointerEventData : PointerEventData
    {
        public GameObject current;
        public IUILaserPointer controller;
        public LaserPointerEventData(EventSystem e) : base(e) { }

        public override void Reset()
        {
            current = null;
            controller = null;
            base.Reset();
        }
    }
}

