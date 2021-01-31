using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkBite : MonoBehaviour
{
    public float Damage = 10.0f;
    private AudioSource damageAudio;
    public GameObject[] DmgPrefabs;
    private int currentClip = 0;

    private void OnTriggerEnter(Collider other) {
        Submarine submarine = other.GetComponent<Submarine>();
        if(submarine != null)
        {
            Transform dmg = Instantiate(DmgPrefabs[currentClip]).transform;
            dmg.position = transform.position;
            currentClip = 1 - currentClip;
            other.GetComponent<Submarine>()?.TakeDamage( Damage );
        }
    }
}
