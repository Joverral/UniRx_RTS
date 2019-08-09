using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class TankDeathEffect : MonoBehaviour {

    [SerializeField]
    GameObject ShrapnelPrefab;

	// Use this for initialization
	void Start () {

        // Skip the first stream event, it's the starting hp
        // GetComponent<DamageManager>().HitPointStream.Skip(1).Subscribe(hp => SpawnShrapnel(hp));
        GetComponent<DamageManager>().DamageStream.Skip(1).Subscribe(de => ResolveDamageEvent(de));
    }
	
    void ResolveDamageEvent(DamageEvent de)
    {
        Debug.Log(this.gameObject.name + ": Took Damage!");
        // if (currentHp == 0)
        {
            var shrapnel = GameObjectPooler.Current.GetObject(ShrapnelPrefab);
            shrapnel.transform.SetPositionAndRotation(de.Position, this.transform.rotation);
            shrapnel.GetComponent<Rigidbody>().AddExplosionForce(500, this.transform.position, 10.0f);
        }
    }
    void SpawnShrapnel(int currentHp)
    {
        Debug.Log(this.gameObject.name + ": New HP for me: " + currentHp);
       // if (currentHp == 0)
        {
            var shrapnel = GameObjectPooler.Current.GetObject(ShrapnelPrefab);
            shrapnel.transform.SetPositionAndRotation(this.transform.position, this.transform.rotation);
            shrapnel.GetComponent<Rigidbody>().AddExplosionForce(500, this.transform.position, 10.0f);
        }
    }
}
