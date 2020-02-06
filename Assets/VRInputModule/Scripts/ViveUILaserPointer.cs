using UnityEngine;
using Valve.VR;

namespace Wacki {

    public class ViveUILaserPointer : IUILaserPointer {
        
        public SteamVR_Action_Boolean ButtonPressState = SteamVR_Input.GetBooleanAction("InteractUI");
        public SteamVR_Action_Vibration HapticAction;
        public SteamVR_Input_Sources Hand = SteamVR_Input_Sources.RightHand;
        private bool _connected = false; 
        public bool Haptics = true;

        protected override void Initialize()
        {
            base.Initialize();


            if(Hand != null) {
                _connected = true;
            }
        }

        public override bool ButtonDown()
        {
            if(!_connected)
                return false;
            
            return ButtonPressState.GetStateDown(Hand);
        }

        public override bool ButtonUp()
        {
            if(!_connected)
                return false;

            return ButtonPressState.GetStateUp(Hand);
        }
        
        public override void OnEnterControl(GameObject control)
        {
            if (!_connected)
                return;
            
            if(Haptics)
	        	TriggerHapticPulse(1000, Hand);
        }

        public override void OnExitControl(GameObject control)
        {
            if (!_connected)
                return;
            
            if(Haptics)
	        	TriggerHapticPulse(600, Hand);
        }

        public void TriggerHapticPulse(ushort microSecondsDuration, SteamVR_Input_Sources hand)
		{
			float seconds = (float)microSecondsDuration / 1000000f;
			HapticAction.Execute(0, seconds, 1f / seconds, 1, hand);
		}
    }

}
