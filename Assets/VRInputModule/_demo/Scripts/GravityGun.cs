using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Wacki;

public class GravityGun : MonoBehaviour, IPointerDownHandler, IPointerUpHandler {

    [SerializeField]
    Vector3 offset;
    [SerializeField]
    float attraction;
    [SerializeField]
    float dampening;

    Rigidbody current;

    public void OnPointerDown(PointerEventData eventData)
    {
        Debug.Log("OnPointerDown");
            
        if (eventData is LaserPointerEventData) {
            var e = eventData as LaserPointerEventData;
            current = e.current.GetComponent<Rigidbody>();
            if (current) {
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        Debug.Log("OnPointerUp");
        current = null;
    }


    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void FixedUpdate () {
        if (current) {
            var dest = transform.TransformPoint(offset);
            var force = attraction * (dest - current.transform.position);
            current.AddForce( -current.velocity * dampening );
            current.AddForce(force, ForceMode.Acceleration);
        }
	}
}
