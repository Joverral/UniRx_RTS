using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
public class TankShellScript : MonoBehaviour {

    [SerializeField]
    float LifeSpan = 10.0f; // todo move to max range?

    //[SerializeField]
    //float Penetration = 50.0f; // in mm

    //[SerializeField]
    //AnimationCurve PenetrationDropoff;

    [SerializeField]
    float MaxRange = 2000.0f; // in meters

    
    public GameObject Owner;
    public GameObject IntendedTarget;
    public LayerMask collisionMask; // Don't remember what I used this for =)

    float distanceTraveled = 0.0f;
    Vector3 lastPosition;
    
    CompositeDisposable rePoolingSubs = new CompositeDisposable();

    DamageDealer damageDealer;

    // Use this for initialization
    void Start () {
        lastPosition = this.transform.position;

        Observable.EveryFixedUpdate().Subscribe(_=>
        {
            distanceTraveled += Vector3.Distance(lastPosition, this.transform.position);
            if(distanceTraveled > MaxRange)
            {
                Debug.Log("Shell Exceeded Max Range!");
                TerminateShell();
            }
            lastPosition = this.transform.position;
        }).AddTo(rePoolingSubs);

        Observable.Timer(TimeSpan.FromSeconds(LifeSpan)).Subscribe(_ =>
        {
            Debug.Log("Shell Timed Out!");
            TerminateShell();
        }).AddTo(rePoolingSubs);

        damageDealer = this.GetComponent<DamageDealer>();

        Debug.Assert(damageDealer);
    }
	
    void TerminateShell()
    {
        rePoolingSubs.Clear();
        GameObjectPooler.Current.PoolObject(this.gameObject);
    }

    void OnCollisionEnter(Collision other)
    {
       

        //if (other.collider.gameObject.GetComponent<DamageableComponent>() != null)
        //{
        //    //hasCollided = true;
        //    //lastContactNormal = other.contacts[0].normal;
        //    //lastContactPoint = other.contacts[0].point;

        //    //      Time.timeScale = 0.005f;
        //}
        Debug.Log("Shell hit!" + other.collider.gameObject.name);

        var damageableHit = other.gameObject.GetComponent<Damageable>();
        if (damageDealer && damageableHit && this.damageDealer.Damage > damageableHit.Armor)
        {
            // We've dealt damage to the tank
            TerminateShell();
        }
        else
        {
            if(damageDealer.Damage == 0)
            {
                TerminateShell(); // terminate a shell that's already bounced once
            }
            // we've hit something else, an undamageable object, or too strong of armor, whatever
            damageDealer.Damage = 0; // TODO -1? instead, that would put 
        }
        

        // Check penetration:  
        // randomize damage based on whta was hit.  The component should know what that is.  
        // FrontHull, SideHull, RearHull  FrontTurret, SideTurret,RearTurret

        // Engine (Mobility -- three states, Normal, Reduced, Immobile)
        // Turret (Mobility -- three states, Normal, Reduced, Immobile)
        // Gun (three states, Normal, Reduced, Inoperable) (Accuracy?)
        // Crew??
        //    Commander  (View range?  Order responsiveness??  )
        //    Gunner  (Accuracy?
        //    Loader  (Reload Speed)
        //    Driver  (Turn speed)
        //    Radio Operator (??  View Range?   Cull?)

        // Tanks should be fairly distance based on where things are placed, Front or Rear mounted engine?

        // FrontHull:  Driver,  Transmission, Machine gunner  (So Mobility)
        // SideHull:  Ammo*  Commander, Loader
        // RearHull:  Engine

        // Front Turret:  Gunner, Commander, Turret Mobility
        // SideTurret:  Same
        // Rear Turret:  Ammo

        // Generic Internal:  Can hit anything, avoid hitting the same thing thta was hit on the first roll
    }
}
