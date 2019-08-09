using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Moveable : MonoBehaviour {

    [SerializeField]
    NavMeshAgent ReverseAgent;

	// Use this for initialization
	void Start () {
        MoveCommandStream().Subscribe(p => this.GetComponent<NavMeshAgent>().SetDestination(p));
	}

    private IObservable<Vector3> MoveCommandStream()
    {
        return this.GetComponentInParent<Controllable>().CommandStream.Where(cmd => cmd is MoveToCommand).Select(cmd => ((MoveToCommand)cmd).Position);
    }

}
