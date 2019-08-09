using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using System;

public class PlayerController : MonoBehaviour
{

    //TODO:  Should I allow multi select?  ....becomes a reactive List<GameObject>
    UniRx.ReactiveProperty<Controllable> _selectedObject = new ReactiveProperty<Controllable>();

    UniRx.ReactiveProperty<GameObject> mouseOverObject = new ReactiveProperty<GameObject>();

    [SerializeField]
    Text selectedText;

    [SerializeField]
    Text selectedUnitHealthText;

    [SerializeField]
    Text highlightedText;

    [SerializeField]
    LayerMask unitLayer;

    CompositeDisposable selectedObjectPropertySubs = new CompositeDisposable();

    // Use this for initialization
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


        // TODO, do i want to split this up into some wheres??
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
                    //what should we do when someone right clicks on themself?  Stop?
                }
                else
                {
                    _selectedObject.Value.CommandStream.SetValueAndForceNotify(new MoveToCommand(hit.point));
                }
            }
        });

        _selectedObject.SubscribeToText(selectedText, c => c ? c.name : "No Unit Selected");

        //selectedObject.Where(c => c != null && c.GetComponent<DamageManager>()).Select(c => c.GetComponent<DamageManager>()).Select(dm => dm.HitPointStream).SubscribeToText(selectedUnitHealthText, hp => "Health: " + hp);

        _selectedObject.Subscribe(c => SubscribeToSelectedObjectComponents(c));

        //{
          
        //    //damManager.HitPointStream.Subscribe(hps => selectedUnitHealthText.text = "Health: " + hps.ToString()).AddTo(selectedObjectPropertySubs);
        //});

        // I think I want a mouseover gameobject? 
    }

    void SubscribeToSelectedObjectComponents(Controllable selectedObject)
    {
        selectedObjectPropertySubs.Clear(); // Clear disposes all the disposables, but le
       
        if (selectedObject == null)
        {
            selectedUnitHealthText.text = string.Empty;
        }
        else
        {
            var damManager = selectedObject.GetComponent<DamageManager>();
            damManager.HitPointStream.SubscribeToText(selectedUnitHealthText, hp => "Health: " + hp.ToString()).AddTo(selectedObjectPropertySubs);
        }
    }

    /// <summary>
    /// Input handling
    /// </summary>
    /// <returns></returns>
    public IObservable<Vector3> LeftClicks()
    {
        return Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Select(_ => Input.mousePosition);
    }

    public IObservable<Vector3> RightClicks()
    {
        return Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(1))
            .Select(_ => Input.mousePosition);
    }

    public IObservable<GameObject> MouseOverObject()
    {
        return Observable.EveryUpdate()
            .Select(_ =>
            {
                RaycastHit hit;
                GameObject go = null;
                if (Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit))
                {
                    go = hit.collider.gameObject;
                }
                return go;
            });
    }
}

