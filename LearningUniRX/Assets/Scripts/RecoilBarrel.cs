using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RecoilBarrel : MonoBehaviour {

    [Header("Recoil Brake settings")]
    [Tooltip("Time it takes to push back the barrel. (Sec)")]
    public float recoilTime = 0.2f;
    [Tooltip("Time it takes to to return the barrel. (Sec)")] public float returnTime = 1.0f;
    [Tooltip("Movable length for the recoil brake. (Meter)")] public float Length = 0.3f;

    public void Fire()
    {
        var initialZ = gameObject.transform.localPosition.z;
        var finalZ = initialZ - Length;
        LeanTween.moveLocalZ(gameObject, finalZ, recoilTime).setOnComplete( () => LeanTween.moveLocalZ(gameObject, initialZ, returnTime));
    }

}
