using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Shark : MonoBehaviour
{

    public GameObject Bite;

    public float Acceleration;
    public float Speed;
    public float RotationSpeed;
    public float BiteImpulseForce;

    private float StunImmunity = 10.0f;
    private float StunTime = 3.0f;
    private float BiteRate_sec = 5.0f;
    private float BiteTriggerRange = 3.0f;
    private float BiteAccuracyRequired = 0.9f;
    private float AggroRange = 60.0f;
    private float PatrolRange = 30.0f;
    private float PatrolPointReach = 5.0f;

    public enum SharkState
    {
        Aggro,
        Patrol
    }

    public void SetSharkNest(SharkNest _sharkNest)
    {
        sharkNest = _sharkNest;
    }

    public void SetRadius( float radius )
    {
        AggroRange = radius;
        PatrolRange = radius / 2;
    }

    public void SetPlayer(GameObject _player)
    {
        player = _player;
        playerRigidBody = _player.GetComponent<Rigidbody>();
    }

    public void SetState(SharkState _state)
    {
        state = _state;
    }

    private void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
        if (Bite != null)
        {
            Bite.SetActive(false);
        }

        BiteRestTimer.SetTime(BiteRate_sec);
        StunImmunityTimer.SetTime(StunImmunity);
    }

    private void FixedUpdate()
    {

        if (rigidBody == null) return;

        if (!Dead)
        {
            Vector3 toPlayer = targetPoint - transform.position;
            Vector3 toPlayerUnit = toPlayer.normalized;

            // use this to slow down a bit when not looking at the player
            float straightRatio = Mathf.Max(0.2f, Mathf.Abs(Vector3.Dot(toPlayerUnit, transform.forward)));

            // Move To
            if(!Stunned)
            {
                rigidBody.velocity = transform.forward * Speed * straightRatio;
            }

            // Next idea to improve shark behavior would be to rotate the velocity based on a rotation speed, if necesarry



            // Maintain maximum speed
            if (!Biting && rigidBody.velocity.magnitude > Speed)
            {
                rigidBody.velocity = rigidBody.velocity.normalized * Speed;
            }
        }
    }

    void Update()
    {
        if (Dead)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.forward, -Vector3.up), 2.0f * Time.deltaTime);
            return;
        }

        // Check if player out of range of nest
        Vector3 targetVectorFromNest = player.transform.position - sharkNest.transform.position;
        float distanceFromNest = targetVectorFromNest.magnitude;
        if( distanceFromNest > AggroRange || BiteRestTimer.Seconds < BiteRate_sec )
        {
            if( state == SharkState.Aggro )
            {
                SetState( SharkState.Patrol );
                NewPatrolPoint();
            }
        }
        else
        {
            SetState( SharkState.Aggro );
        }

        // Determine current target point based on state
        switch(state)
        {
            case SharkState.Aggro:
                targetPoint = player.transform.position + playerRigidBody.velocity - player.transform.forward * 0.75f;
            break;
            case SharkState.Patrol:
                targetPoint = patrolPoint;
            break;
        }

        if(Stunned)
        {
            StunImmunityTimer.Reset();

            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.forward, Vector3.back), 5.0f * Time.deltaTime);

            StunTimer.Interval();
            if(StunTimer.Seconds > StunTime)
            {
                Stunned = false;
                StunTimer.Reset();
            }
        }
        else
        {
            StunImmunityTimer.Interval();
        }

        // Track target
        Vector3 toTarget = targetPoint - transform.position;
        Vector3 toTargetUnit = toTarget.normalized;
        float distanceToTarget = toTarget.magnitude;

        // Look at target
        if(!Stunned)
        {
            transform.rotation = Quaternion.Slerp( transform.rotation,
                                                   Quaternion.LookRotation(toTargetUnit, Vector3.up),
                                                   RotationSpeed * Time.deltaTime);
        }

        BiteRestTimer.Interval();

        // Patrol logic
        if (state == SharkState.Patrol )
        {
            if(distanceToTarget < PatrolPointReach )
            {
                NewPatrolPoint();
            }
        }
        // Aggro Logic
        else if (state == SharkState.Aggro)
        {
            // Bite
            float biteAccuracy = Vector3.Dot(toTargetUnit, transform.forward);
            if ( BiteRestTimer.Seconds >= BiteRate_sec
                 && toTarget.magnitude < BiteTriggerRange
                 && biteAccuracy > BiteAccuracyRequired)
            {
                rigidBody?.AddForce(transform.forward * BiteImpulseForce, ForceMode.Impulse);
                Biting = true;
                BiteRestTimer.Reset();
            }
        }

        if (Biting)
        {
            if (!Bite.activeSelf) Bite.SetActive(true);

            BiteActiveTimer.Interval();
            if (BiteActiveTimer.Seconds > 0.5)
            {
                Biting = false;
                Bite.SetActive(false);
                BiteActiveTimer.Reset();
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {

        Torpedo torpedo = other.gameObject.GetComponent<Torpedo>();
        if (torpedo != null)
        {
            Destroy(gameObject, 0.5f);

            Dead = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("SonarPing"))
        {
            if(StunImmunityTimer.Seconds >= StunImmunity)
            {
                Stunned = true;
            }
        }
    }

    private void NewPatrolPoint()
    {
        Vector3 point = sharkNest.SpawnPoint.transform.position;

        point.x += (Random.value - 0.5f) * 2 * PatrolRange; // left or right
        point.z += (Random.value - 0.5f) * 2 * PatrolRange; // forward or back
        point.y += Random.value * PatrolRange; // only up

        patrolPoint = point;
    }

    private SharkState state;
    private bool Biting = false;
    private Timer BiteRestTimer = new Timer();
    private Timer BiteActiveTimer = new Timer();
    
    private bool Stunned;
    private Timer StunTimer = new Timer();
    private Timer StunImmunityTimer = new Timer();

    private bool Dead;
    private Rigidbody rigidBody;
    private SharkNest sharkNest;
    private GameObject player;
    private Rigidbody playerRigidBody;
    private Vector3 patrolPoint;
    private Vector3 targetPoint;
}
