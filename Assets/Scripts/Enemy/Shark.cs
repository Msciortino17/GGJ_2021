using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Shark : MonoBehaviour, IHeatSeekable
{
    public GameObject Target;

    public GameObject Bite;

    public float Acceleration = 5.0f;
    public float Speed = 15.0f;
    public float RotationSpeed = 13.0f;

    public float BiteRate_sec = 5.0f;
    public float BiteRange = 3.0f;

    public float BiteAccuracyRequired = 0.8f;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        if(Bite != null)
        {
            Bite.SetActive(false);
        }

        BiteRestTimer.SetTime(BiteRate_sec);
    }

    private void FixedUpdate() {

        if( rigidBody == null ) return;

        if(!Dead)
        {
            // Move To
            rigidBody.AddForce(transform.forward * Acceleration, ForceMode.Force);

            if(rigidBody.velocity.magnitude > Speed)
            {
                rigidBody.velocity = rigidBody.velocity.normalized * Speed;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if( Target != null)
        {
            // Track
            if(!Dead)
            {
                Vector3 toPlayer = Target.transform.position - transform.position;
                Vector3 toPlayerUnit = toPlayer.normalized;

                // Look at target
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(toPlayerUnit, Vector3.up), RotationSpeed * Time.deltaTime);

                // Bite
                float biteAccuracy = Vector3.Dot(toPlayerUnit, transform.forward);
                BiteRestTimer.Interval();
                if( BiteRestTimer.Seconds >= BiteRate_sec
                    && toPlayer.magnitude < BiteRange
                    && biteAccuracy > BiteAccuracyRequired )
                {
                    Biting = true;
                    BiteRestTimer.Reset();
                }
            }
            else
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.forward, -Vector3.up), RotationSpeed * Time.deltaTime);
            }
        }

        if( Biting )
        {
            if(!Bite.activeSelf) Bite.SetActive(true);

            BiteActiveTimer.Interval();
            if(BiteActiveTimer.Seconds > 0.5)
            {
                Biting = false;
                Bite.SetActive(false);
                BiteActiveTimer.Reset();
            }
        }
    }

    private void OnCollisionEnter(Collision other) {
        
        Torpedo torpedo = other.gameObject.GetComponent<Torpedo>();
        if(torpedo != null)
        {
            Destroy(gameObject, 0.5f);
            
            Dead = true;
        }
    }

    private bool Biting = false;
    private Timer BiteRestTimer = new Timer();
    private Timer BiteActiveTimer = new Timer();

    private bool Dead;
    private Rigidbody rigidBody;
}
