using UnityEngine;
using Valve.VR;

namespace Wacki {

    public class ViveUILaserPointer : IUILaserPointer {

        public EVRButtonId button = EVRButtonId.k_EButton_SteamVR_Trigger;

        private SteamVR_TrackedObject _trackedObject;
        private bool _connected = false;

        protected override void Initialize()
        {
            base.Initialize();
            Debug.Log("Initialize");

            _trackedObject = GetComponent<SteamVR_TrackedObject>();

            if(_trackedObject != null) {
                _connected = true;
            }
        }

        public override bool ButtonDown()
        {
            if(!_connected)
                return false;
            
            var device = SteamVR_Controller.Input(controllerIndex);
            if(device != null) {
                var result = device.GetPressDown(button);
                return result;
            }

            return false;
        }

        public override bool ButtonUp()
        {
            if(!_connected)
                return false;

            var device = SteamVR_Controller.Input(controllerIndex);
            if(device != null)
                return device.GetPressUp(button);

            return false;
        }
        
        public override void OnEnterControl(GameObject control)
        {
            if (!_connected)
                return;
            var device = SteamVR_Controller.Input(controllerIndex);
            device.TriggerHapticPulse(1000);
        }

        public override void OnExitControl(GameObject control)
        {
            if (!_connected)
                return;
            var device = SteamVR_Controller.Input(controllerIndex);
            device.TriggerHapticPulse(600);
        }

        int controllerIndex
        {
            get {
                if (!_connected) return 0;
                return (int)(_trackedObject.index);
            }
        }
    }

}