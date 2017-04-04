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
        if (eventData is LaserPointerEventData) {
            var e = eventData as LaserPointerEventData;
            current = e.current.GetComponent<Rigidbody>();
            if (current) {
            }
        }
    }

    public void OnPointerUp(PointerEventData eventData)
    {
       current = null;
    }
	
	void FixedUpdate () {
        if (current) {
            var dest = transform.TransformPoint(offset);
            var force = attraction * (dest - current.transform.position);
            current.AddForce( -current.velocity * dampening );
            current.AddForce(force, ForceMode.Acceleration);
        }
	}
}
