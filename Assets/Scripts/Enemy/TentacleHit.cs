using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class TentacleHit : MonoBehaviour
{
    public float Damage = 20.0f;

    public float Cooldown = 2.0f;

    private void Update()
    {
        CooldownTimer.Interval();
    }

    private void OnTriggerEnter( Collider other )
    {
        Submarine submarine = other.GetComponent<Submarine>();
        if( submarine != null )
        {
            if(CooldownTimer.Seconds >= Cooldown)
            {
                other.GetComponent<Submarine>()?.TakeDamage( Damage );
                CooldownTimer.Reset();
            }
        }
    }

    private Timer CooldownTimer = new Timer();
}
