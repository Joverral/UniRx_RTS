using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;

public class TempGoToPoint : MonoBehaviour {

    [SerializeField]
    NavMeshAgent agent;

   
    // Use this for initialization
    void Start()
    {
        TapStream().Subscribe(p =>
        {
            RaycastHit hit;
            
            if (Physics.Raycast(Camera.main.ScreenPointToRay(p), out hit))
            {
                agent.SetDestination(hit.point);
            }

            
        });
    }


    /// <summary>
    /// Input handling
    /// </summary>
    /// <returns></returns>
    public IObservable<Vector3> TapStream()
    {
        return Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Select(_ => Input.mousePosition);
    }
}
