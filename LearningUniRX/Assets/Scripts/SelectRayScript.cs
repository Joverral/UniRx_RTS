using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UniRx;

public class SelectRayScript : MonoBehaviour
{
    [SerializeField]
    private UnityEngine.UI.Text SelectedObjectText;
    UniRx.ReactiveProperty<GameObject> selectedGameObject = new ReactiveProperty<GameObject>();

	// Use this for initialization
	void Start () {
        selectedGameObject.SubscribeToText(SelectedObjectText, go => go == null ? "No Object Selected" : go.name);
        selectedGameObject.SetValueAndForceNotify(null);


        TapStream().Subscribe(p => 
        {
            RaycastHit hit;
            GameObject go = null;
            if(Physics.Raycast(Camera.main.ScreenPointToRay(p), out hit))
            {
                go = hit.collider.gameObject;
            }

            selectedGameObject.Value = go;
        });
	}


    /// <summary>
    /// Input handling
    /// </summary>
    /// <returns></returns>
    public IObservable<Vector3> TapStream()
    {
        return Observable.EveryUpdate()
            .Where(_ => Input.GetMouseButtonDown(0))
            .Select(_ => Input.mousePosition);
    }
}

// Simple Armor: Heavy Medium Light
// Turret is always one less than armor (min, Light)
// Guns are heavy medium light, not depedent on anything
// Turret speed is based on turret armor + gun size
//  (Gun vs armor:  Equal vs Equal:  No effect, maybe small chance?  Or go Rabids route:  Equal: 50/50%, Beats: 100%

//Try real time?