﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damageable : MonoBehaviour
{
    [SerializeField]
    int armor;

    public int Armor { get { return armor; } }

    void OnCollisionEnter(Collision colEvent)
    {
        this.GetComponentInParent<DamageManager>().RaiseDamageEvent(
            new DamageEvent(1, DamageLocation.Front_Hull, colEvent.contacts[0].point)
            );
    }
}