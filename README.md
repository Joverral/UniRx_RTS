# UniRx_RTS
Learning UniRx by making a really simple RTS prototype.  The tank controllers are observable streams that take commands, and listen for various commands to respond to.  Though basically only did movement and firing.  There's more in there for armor and facing, simplified compared to the other tanklords game.  I think I disabled the hit point tracking for some testing, and probably should've put it back in before I forgot =)


I came across UniRX as a possible solution to the doldrums of writing event systems and doing SendMessage all over the place, and then decided to learn it by just starting a game from scratch with it.  And using it everywhere, even if it might not be the best fit, just to see what it could do.  What's UniRX?  Well [here's a small presentation](https://www.slideshare.net/neuecc/unirx-reactive-extensions-for-unityen), and here's a [link](https://github.com/neuecc/UniRx) to the github.

So, the game?  A simple RTS, a much simpler version of that turn based tank game that's forever on the back burner.  So the short version Mech Commander, with Tanks instead of Mechs.

Let's start with our PlayerController, it basically does two things, unit selection, and sending commands to the selected unit.

public class PlayerController : MonoBehaviour
{
    UniRx.ReactiveProperty<Controllable> _selectedObject = new ReactiveProperty<Controllable>();

    void Start () {
        LeftClicks().Subscribe(p =>
        {
            RaycastHit hit;
            GameObject go = null;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(p), out hit, unitLayer))
            {
                go = hit.collider.gameObject;
            }
            _selectedObject.Value = go ? go.GetComponent<Controllable>() : null;
        });

        RightClicks().Where(_=> _selectedObject.Value != null).Subscribe(p =>
        {
            RaycastHit hit;
            GameObject go = null;
            if (Physics.Raycast(Camera.main.ScreenPointToRay(p), out hit))
            {
                go = hit.collider.gameObject;
                if(go.GetComponent<Targetable>())
                {
                    if (go != this.gameObject)
                    {
                        _selectedObject.Value.CommandStream.SetValueAndForceNotify(new AttackTargetCommand(go));
                    }
                }
                else
                {
                    _selectedObject.Value.CommandStream.SetValueAndForceNotify(new MoveToCommand(hit.point));
                }
            }
        });
    }

    public IObservable<Vector3> LeftClicks() {
        return Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Select(_ => Input.mousePosition); }

    public IObservable<Vector3> RightClicks() {
        return Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(1))
            .Select(_ => Input.mousePosition); }  
Okay, so on a basic level, each unit has a Controllable component that is fairly simple, it's basically just a ICommand queue

public class Controllable : MonoBehaviour {
  public UniRx.ReactiveProperty<ICommand> CommandStream = new ReactiveProperty<ICommand>(); 
  public IObservable<GameObject> LatestTarget() {
   return CommandStream.Where(cmd => cmd is AttackTargetCommand).Select(cmd => ((AttackTargetCommand)cmd).Target);
  }
}
And then we have optional components that look at that command stream, like Moveable and Turret.  Lets look at Moveable first, it's really simple at the moment.

[RequireComponent(typeof(NavMeshAgent))]
public class Moveable : MonoBehaviour {
  void Start () { MoveCommandStream().Subscribe(p => this.GetComponent<NavMeshAgent>().SetDestination(p));}
  private IObservable<Vector3> MoveCommandStream() {
         return this.GetComponentInParent<Controllable>().CommandStream.Where(cmd => cmd is MoveToCommand).Select(cmd => ((MoveToCommand)cmd).Position); }
}
The turret is a little more complicated

public class Turret : MonoBehaviour {  
    IDisposable rotateTowardsSubscription = null;
    void Start () {
        this.GetComponentInParent<Controllable>().LatestTarget().Subscribe(target => {
            if(rotateTowardsSubscription != null) { // Stop any rotations
                rotateTowardsSubscription.Dispose();
                rotateTowardsSubscription = null;
            }
            if (target != null) {
                rotateTowardsSubscription = EveryUpdate().Subscribe(x => {
                    if (target != null) {
                        RotateTowards(target.transform.position);
                    }
                });
            }
        });
    }
  private RotateTowards() {} // not showing this for brevity's sake, and I'm cheesing it at the moment anyway
}
And then the WeaponMount which does the actual firing, with a bunch of non-uniRX code culled out, I abstracted it out into separate components for multi gun turrets, or fixed guns, etc, as well as code clarity.

public class WeaponMount : MonoBehaviour {

    [SerializeField]
    float ReloadTime = 1.0f;
    BoolReactiveProperty weaponLoaded = new BoolReactiveProperty(false);
    IDisposable fireSubscription = null;

    void Start () {
        this.GetComponentInParent<Controllable>().LatestTarget().Subscribe(target => {
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

        weaponLoaded.Where(loaded => loaded == false).Delay(TimeSpan.FromSeconds(ReloadTime)).Subscribe(_ => weaponLoaded.Value = true);
    }
 

What's interesting is just how different the code looks from standard Unity code, even coroutine code (Though that's perhaps the closest).  I started to really enjoy it once I managed to wrap my mind around it, it's a rather small amount of code to select, move and attack with units.  Once I'd built up my vocabulary of Linq-like syntax, it makes it very easy to see whats going on and do things like the weapon reload, which is just one line.

 

Minor quibble: I do wish Unity had greater C# version support, I want to do foo?.Dispose() and be done with it already.
