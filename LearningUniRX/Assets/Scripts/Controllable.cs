using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.AI;

public class Controllable : MonoBehaviour {
    public UniRx.ReactiveProperty<ICommand> CommandStream = new ReactiveProperty<ICommand>();
    // I think I want this to be basically the 'brain' or AI.

    // Controllabe 
    // Basically a command sink.  Commands get sent here from a AI or Player controller.  Various subcomponents like turrets or moveable then subscribe

    // But then what about mouseover?  make that command?  (HighlightTarget


    //[SerializeField]
    //public Turret turret;

    // Use this for initialization
    void Start () {
        //Commands.Subscribe(command =>
        //{
        //    Targetable targetable = null;
        //    if (command.Target)
        //    {
        //        targetable = command.Target.GetComponent<Targetable>();
        //    }

        //    if(targetable == null)
        //    {
        //        this.GetComponent<NavMeshAgent>().SetDestination(command.Point);

                
        //    }
        //    else // rotate turret to face target? attack!?
        //    {
        //        turret.TargetObject.SetValueAndForceNotify(targetable.gameObject);
        //    }
        //}
        //);

        //Commands.SetValueAndForceNotify(new PositionalCommand() { Point = this.transform.position, Target = null });
    }


    // TODO:  HoldFire 


    public IObservable<GameObject> LatestTarget()
    {
        return CommandStream.Where(cmd => cmd is AttackTargetCommand).Select(cmd => ((AttackTargetCommand)cmd).Target);
    }
}


public interface ICommand {}


// Uhh, I am tempted by ICommands and structs
// just be careful!

public struct MoveToCommand : ICommand
{
    public MoveToCommand(Vector3 pos) { Position = pos; }
    public readonly Vector3 Position;
}

public struct AttackTargetCommand : ICommand
{
    public AttackTargetCommand(GameObject target) { Target = target; }
    public readonly GameObject Target;
}
