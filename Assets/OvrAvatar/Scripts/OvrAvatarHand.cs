using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class OvrAvatarHand : MonoBehaviour, IAvatarPart
{

    float alpha = 1.0f;

    public void UpdatePose(OvrAvatarDriver.ControllerPose pose)
    {
    }

    public void SetAlpha(float alpha)
    {
        this.alpha = alpha;
    }

    public void OnAssetsLoaded()
    {
        SetAlpha(this.alpha);
    }
}