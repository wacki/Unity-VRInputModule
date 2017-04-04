using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ParabolaRaycaster : PhysicsRaycaster {

    [SerializeField]
    float initialSpeed;

    Vector3[] positions = new Vector3[256];        
    int len = 0;

    public int ParabolaData(out Vector3[] positions){
        positions = this.positions;
        return len;
    }

    public override void Raycast(PointerEventData eventData, List<RaycastResult> resultAppendList)
    {
        if (eventCamera == null)
            return;

        var ray = eventCamera.ScreenPointToRay(eventData.position);

        float dist = eventCamera.farClipPlane - eventCamera.nearClipPlane;

        //var hits = Physics.RaycastAll(ray, dist, finalEventMask);

        len = 0;
        float t = 0;
        float d = 0;
        float dt = 0.1f;
        var a = Physics.gravity;
        var u = ray.direction.normalized * initialSpeed;
        var v = u;
        var p0 = ray.origin;
        var p1 = p0;
        float dd = 0;
        RaycastHit hit;
        while(d<dist && len<positions.Length){
            t += dt;
            v = u + a*dt;
            p1 = p0 + u*dt+0.5f*a*dt*dt;
            dd = Vector3.Distance(p0,p1);
            d += dd;
            positions[len++] = p0;
            bool hitSomething = Physics.Raycast( p0, u.normalized, out hit, dd, finalEventMask);
            if( hitSomething ){
                var result = new RaycastResult
                {
                    gameObject = hit.collider.gameObject,
                    module = this,
                    distance = hit.distance,
                    index = resultAppendList.Count
                };
                resultAppendList.Add(result);

                positions[len++] = hit.point;
                break;

            }
            u = v;
            p0 = p1;

        }

        /*
        if (hits.Length > 1)
            System.Array.Sort(hits, (r1, r2) => r1.distance.CompareTo(r2.distance));

        if (hits.Length != 0)
        {
            eventData.worldPosition = hits[0].point;
            eventData.worldNormal = hits[0].normal;
            for (int b = 0, bmax = hits.Length; b < bmax; ++b)
            {
                var result = new RaycastResult
                {
                    gameObject = hits[b].collider.gameObject,
                    module = this,
                    distance = hits[b].distance,
                    index = resultAppendList.Count
                };
                resultAppendList.Add(result);
            }
        }
        */
    }
}
