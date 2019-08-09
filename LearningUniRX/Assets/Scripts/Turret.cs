using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

//TODO Cannon or weapon behavior, just looks if we have a target, fires if within target (maybe check cone of accuracy? 

public class Turret : MonoBehaviour {

    [SerializeField]
    GameObject horizontalComponent;
    [SerializeField]
    GameObject verticalComponent;
    
    IDisposable rotateTowardsSubscription = null;

    void Start () {

        this.GetComponentInParent<Controllable>().LatestTarget().Subscribe(target =>
        {
            // Stop any rotations
            if(rotateTowardsSubscription != null)
            {
                rotateTowardsSubscription.Dispose();
                rotateTowardsSubscription = null;
            }
            if (target != null)
            {
                rotateTowardsSubscription = EveryUpdate().Subscribe(x =>
                {
                    if (target != null)
                    {
                        RotateTowardsHorizontal(target.transform.position);
                    }
                });
            }
        });
        // might be better to do it via coroutine here
        
    }

    private IObservable<GameObject> EveryUpdateTarget()
    {
        return Observable.EveryUpdate(). WithLatestFrom(this.GetComponentInParent<Controllable>().LatestTarget(), (_, b) => b);
    }

    private IObservable<long> EveryUpdate()
    {
        return Observable.EveryUpdate();
    }

    void RotateTowardsHorizontal(Vector3 point)
    {
        var horzTrans = horizontalComponent.transform;
        Vector3 horzPoint = new Vector3(point.x, 0, point.z);
        Quaternion desiredRotation = Quaternion.LookRotation(horzPoint - new Vector3(horzTrans.position.x, 0, horzTrans.position.z));
        horzTrans.rotation = Quaternion.Lerp(horzTrans.rotation, desiredRotation, Time.deltaTime * 10);
    }
}
