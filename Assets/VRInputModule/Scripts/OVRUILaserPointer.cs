using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace Wacki {
	public class OVRUILaserPointer : IUILaserPointer {

	    public OVRInput.Button primaryTrigger;

	    protected override void Initialize()
	    {
	        base.Initialize();
	    }

	    public override bool ButtonDown()
	    {
	        //Debug.Log("ButtonDown");
	        bool down = OVRInput.GetDown(primaryTrigger);
	        if (down) {
	            Debug.LogFormat("{0} Down!",this.name);
	        }

	        return down;
	    }

	    public override bool ButtonUp()
	    {
	        return OVRInput.GetUp(primaryTrigger);
	    }

	}
}