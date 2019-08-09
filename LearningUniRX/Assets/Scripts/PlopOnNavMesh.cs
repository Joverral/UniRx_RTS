using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PlopOnNavMesh : MonoBehaviour {

    [SerializeField]
    GameObject navMesh;

	// Use this for initialization
	void Start () {
        NavMeshHit hit;
        if(NavMesh.SamplePosition(this.transform.position, out hit, 500, 1))
        {
            this.transform.position = hit.position;
            this.gameObject.AddComponent<NavMeshAgent>();
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
