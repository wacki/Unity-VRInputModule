using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class OvrAvatarBody : MonoBehaviour, IAvatarPart {

    float alpha = 1.0f;

	// Use this for initialization
	void Start () {
	}
	
    public virtual void UpdatePose(float voiceAmplitude)
    {
    }

    public void SetAlpha(float alpha)
    {
        this.alpha = alpha;
        List<Renderer> renderers = new List<Renderer>();
        this.gameObject.GetComponentsInChildren<Renderer>(true, renderers);
        foreach (Renderer renderer in renderers)
        {
            foreach (Material material in renderer.sharedMaterials)
            {
                material.SetFloat("_Alpha", alpha);
            }
        }
    }

    public void OnAssetsLoaded()
    {
        SetAlpha(alpha);
    }
}