using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Wacki {
	public class OVRUILaserPointer : IUILaserPointer {

	    public OVRInput.Button primaryTrigger;
        public OVRInput.Controller controller;

        OVRHapticsClip enterHapticClip;
        OVRHapticsClip exitHapticClip;

        protected override void Initialize()
	    {
	        base.Initialize();
            InitHaptics();
        }

	    public override bool ButtonDown()
	    {
	        //Debug.Log("ButtonDown");
	        bool down = OVRInput.GetDown(primaryTrigger, controller);
	        if (down) {
	            Debug.LogFormat("{0} Down!",this.name);
	        }

	        return down;
	    }

	    public override bool ButtonUp()
	    {
	        return OVRInput.GetUp(primaryTrigger, controller);
	    }

        public override void OnEnterControl(GameObject control)
        {
            base.OnEnterControl(control);
            if (controller == OVRInput.Controller.LTouch) {
                OVRHaptics.LeftChannel.Mix(enterHapticClip);
            }

            if (controller == OVRInput.Controller.RTouch)
            {
                OVRHaptics.RightChannel.Mix(exitHapticClip);
            }
        }

        public override void OnExitControl(GameObject control)
        {
            base.OnExitControl(control);
            if (controller == OVRInput.Controller.LTouch)
            {
                OVRHaptics.LeftChannel.Mix(exitHapticClip);
            }

            if (controller == OVRInput.Controller.RTouch)
            {
                OVRHaptics.RightChannel.Mix(exitHapticClip);
            }
        }

        void InitHaptics() {
            int duration = 10;
            int exitAmplitude = 40;
            int enterAmplitude = 80;
            float freq = 50f / OVRHaptics.Config.SampleRateHz;
            enterHapticClip = new OVRHapticsClip(duration);
            exitHapticClip = new OVRHapticsClip(duration);
            WriteHapticSamples(enterHapticClip, freq, enterAmplitude, duration);
            WriteHapticSamples(exitHapticClip, freq, exitAmplitude, duration);
        }

        void WriteHapticSamples(OVRHapticsClip clip, float freq, float amplitude, int duration) {
            for (int i = 0; i < duration; i++)
            {
                clip.WriteSample((byte)(Mathf.RoundToInt(amplitude * 0.5f * (Mathf.Sin(i * freq * 2 * Mathf.PI) + 1))));
            }

        }
    }
}