using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
[RequireComponent(typeof(ParabolaRaycaster))]
public class ParabolaDrawer : MonoBehaviour {

    LineRenderer lineRenderer;
    ParabolaRaycaster parabolaRaycaster;

	void Start () {
		lineRenderer = GetComponent<LineRenderer>();
        parabolaRaycaster = GetComponent<ParabolaRaycaster>();
	}
	
	void Update () {

        Vector3[] positions;
        var len = parabolaRaycaster.ParabolaData(out positions);
        lineRenderer.numPositions = len;
		lineRenderer.SetPositions( positions );
	}
}
