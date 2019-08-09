using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageDealer : MonoBehaviour {

    public int Damage { get { return damage; } set { damage = value; } }

    [SerializeField]
    int damage;
}
