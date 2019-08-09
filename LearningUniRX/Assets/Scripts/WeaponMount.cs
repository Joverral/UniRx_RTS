using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using System;

public class WeaponMount : MonoBehaviour {

    // I could probably split this up...but meh
    [SerializeField]
    Transform MuzzleFirePoint;

    [SerializeField]
    GameObject ProjectilePrefab;

    [SerializeField]
    float ReloadTime = 1.0f;

    [SerializeField]
    float FirePauseTime = 0.25f;

    [SerializeField]
    float ShellVelocity = 100;

    [SerializeField]
    float MinAccuracyAngle = 10.0f;

    [SerializeField]
    RecoilBarrel Barrel;

    [SerializeField]
    GameObject FireEffectPrefab;

    BoolReactiveProperty weaponLoaded = new BoolReactiveProperty(false);
    IDisposable fireSubscription = null;

    // Use this for initialization
    void Start () {
        this.GetComponentInParent<Controllable>().LatestTarget().Subscribe(target =>
        {
            // Stop any fire subs 
            if (fireSubscription != null)
            {
                fireSubscription.Dispose();
                fireSubscription = null;
            }

            if (target != null)
            {
                fireSubscription = EveryUpdate().Where(_ => CanFireAtTarget(target)).Subscribe(_ => Fire());
            }
        });

        // Not sure how I could later modify the reload time, save the sub and redo when damaged?
        weaponLoaded.Where(loaded => loaded == false).Delay(TimeSpan.FromSeconds(ReloadTime)).Subscribe(_ => weaponLoaded.Value = true);
    }

    private void Fire()
    {
        // Fire!
        var projectile = GameObjectPooler.Current.GetObject(ProjectilePrefab);
        projectile.transform.position = MuzzleFirePoint.position;
        projectile.transform.rotation = MuzzleFirePoint.rotation;

        var projectileRB = projectile.GetComponent<Rigidbody>();

        projectileRB.AddRelativeForce(new Vector3(0, 0, ShellVelocity), ForceMode.Impulse);

        Debug.Log("Firing ze Cannon!!");

        Barrel.Fire();

        if (FireEffectPrefab)
        {
            var fireObject = GameObjectPooler.Current.GetObject(FireEffectPrefab);
            fireObject.transform.SetPositionAndRotation(MuzzleFirePoint.position, MuzzleFirePoint.rotation);
            fireObject.transform.parent = MuzzleFirePoint;
        } 

        weaponLoaded.Value = false;
    }

    private bool CanFireAtTarget(GameObject target)
    {
        if (weaponLoaded.Value &&
            Vector3.Angle(MuzzleFirePoint.forward, target.transform.position - MuzzleFirePoint.position) < MinAccuracyAngle)
        {
            return true;
        }
       
        return false;
    }

    private IObservable<long> EveryUpdate()
    {
        return Observable.EveryUpdate();
    }
}
