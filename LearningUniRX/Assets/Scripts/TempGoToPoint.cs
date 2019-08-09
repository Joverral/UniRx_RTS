using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UniRx;

public class TempGoToPoint : MonoBehaviour {

    [SerializeField]
    NavMeshAgent agent;

    //[SerializeField]
    //private UnityEngine.UI.Text SelectedObjectText;
    //UniRx.ReactiveProperty<GameObject> selectedGameObject = new ReactiveProperty<GameObject>();

    // Use this for initialization
    void Start()
    {
        //selectedGameObject.SubscribeToText(SelectedObjectText, go => go == null ? "No Object Selected" : go.name);
        //selectedGameObject.SetValueAndForceNotify(null);


        TapStream().Subscribe(p =>
        {
            RaycastHit hit;
            
            if (Physics.Raycast(Camera.main.ScreenPointToRay(p), out hit))
            {
                //var go = hit.collider.gameObject;

                //var navMeshAgent = go.GetComponent<UnityEngine.AI.NavMeshAgent>();
                //if(navMeshAgent != n)
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
