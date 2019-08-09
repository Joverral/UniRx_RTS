using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class DamageManager : MonoBehaviour {

    public UniRx.IntReactiveProperty HitPointStream = new IntReactiveProperty(3);
    public UniRx.ReactiveProperty<DamageEvent> DamageStream = new ReactiveProperty<DamageEvent>();

    public UniRx.IntReactiveProperty MobilityHealthStream = new IntReactiveProperty(3);
    public UniRx.IntReactiveProperty TurretHealthStream = new IntReactiveProperty(3);
    public UniRx.IntReactiveProperty GunStream = new IntReactiveProperty(3);


    public void DoDamage(int damage)
    {
        HitPointStream.Value -= damage;
    }

    public void RaiseDamageEvent(DamageEvent damageEvent)
    {
        DamageStream.SetValueAndForceNotify(damageEvent);
    }

    // Use this for initialization
    void Start () {

        // HitPointStream.Subscribe(hps => Debug.Log("Hitpoints for me: " + hps));
        //DamageStream.Subscribe(damage =>
        //{
        //    // reduce heath
        //    // Determine systems/internals/crew damage here
        //    // Set that particular stream -- I should probably make an array of intreactiveProps eventually
        //});
	}
	
	//// Update is called once per frame
	//void Update () {
		
	//}
}

public enum DamageLocation
{
    Front_Hull,
    Rear_Hull,
    Side_Hull,
    Front_Turret,
    Side_Turret,
    Rear_Turret
}

public enum DamageResultType
{
    Front_Hull,
    Rear_Hull,
    Side_Hull,
    Front_Turret,
    Side_Turret,
    Rear_Turret
}

//public interface IDamageEvent { }


// Uhh, I am tempted by IDamageEvent and structs
// just be careful!

public struct DamageEvent
{
    public DamageEvent(int amount, DamageLocation location, Vector3 position) { Amount = amount; Location = location; Position = position; }
    public readonly int Amount;
    public readonly DamageLocation Location;
    public Vector3 Position;
}

//public struct DamageResult
//{
//    public DamageResult(int amount, DamageLocation location) { Amount = amount; Location = location; }
//    public readonly int Amount;
//    public readonly DamageLocation Location;
//}

//public struct AttackTargetCommand : IDamageEvent
//{
//    public AttackTargetCommand(GameObject target) { Target = target; }
//    public readonly GameObject Target;
//}
