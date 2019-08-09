using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;
using UnityEngine.AI;

public class Controllable : MonoBehaviour {
    public UniRx.ReactiveProperty<ICommand> CommandStream = new ReactiveProperty<ICommand>();
    
    public IObservable<GameObject> LatestTarget()
    {
        return CommandStream.Where(cmd => cmd is AttackTargetCommand).Select(cmd => ((AttackTargetCommand)cmd).Target);
    }
}

public interface ICommand {}

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
