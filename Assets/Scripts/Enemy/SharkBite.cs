using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkBite : MonoBehaviour
{
    public float Damage = 30.0f;

    private void OnTriggerEnter(Collider other) {
        Submarine submarine = other.GetComponent<Submarine>();
        if(submarine != null)
        {
            other.GetComponent<Submarine>()?.TakeDamage( Damage );
        }
    }
}
