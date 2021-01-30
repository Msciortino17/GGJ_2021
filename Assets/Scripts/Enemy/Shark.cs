using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Shark : MonoBehaviour
{

    public GameObject Bite;
    public GameObject PatrolPoint;

    public float Acceleration;
    public float Speed;
    public float RotationSpeed;

    public float BiteImpulseForce = 5.0f;
    public float BiteRate_sec = 5.0f;
    public float BiteTriggerRange = 4.0f;
    public float BiteAccuracyRequired = 0.8f;
    public float AggroRange = 30.0f;

    public enum SharkState
    {
        Aggro,
        Patrol
    }

    public void SetSharkNest(SharkNest sharkNest)
    {
        this.sharkNest = sharkNest;
    }

    public void SetTargetAndState(GameObject _target, SharkState _state)
    {
        target = _target;
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
    }

    private void FixedUpdate()
    {

        if (rigidBody == null) return;

        if (!Dead)
        {
            Vector3 toPlayer = target.transform.position - transform.position;
            Vector3 toPlayerUnit = toPlayer.normalized;

            // use this to slow down a bit when not looking at the player
            float straightRatio = Mathf.Max(0.2f, Mathf.Abs(Vector3.Dot(toPlayerUnit, transform.forward)));

            // Move To
            rigidBody.velocity = transform.forward * Speed * straightRatio;

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
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(transform.forward, -Vector3.up), 10.0f * Time.deltaTime);
            return;
        }

        // Check if player out of range of nest
        if (target != null && state == SharkState.Aggro)
        {
            Vector3 targetVectorFromNest = target.transform.position - sharkNest.transform.position;
            float distanceFromNest = targetVectorFromNest.magnitude;
            if (distanceFromNest < AggroRange)
            {
                PatrolPoint.gameObject.transform.position = GetPatrolPoint();
                SetTargetAndState(PatrolPoint, SharkState.Patrol);
            }
        }

        // Track target
        Vector3 toTarget = target.transform.position - transform.position;
        Vector3 toTargetUnit = toTarget.normalized;
        float distanceToTarget = toTarget.magnitude;

        // Look at target
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(toTargetUnit, Vector3.up), RotationSpeed * Time.deltaTime);

        // Patrol logic
        if (state == SharkState.Patrol )
        {
            if(distanceToTarget < 0.1 )
            {
                PatrolPoint.gameObject.transform.position = GetPatrolPoint();
                SetTargetAndState(PatrolPoint, SharkState.Patrol);
            }
        }
        // Aggro Logic
        else if (state == SharkState.Aggro)
        {
            // Bite
            float biteAccuracy = Vector3.Dot(toTargetUnit, transform.forward);
            BiteRestTimer.Interval();
            if (BiteRestTimer.Seconds >= BiteRate_sec
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

    private Vector3 GetPatrolPoint()
    {
        return new Vector3();
    }

    private SharkState state;
    private bool Biting = false;
    private Timer BiteRestTimer = new Timer();
    private Timer BiteActiveTimer = new Timer();

    private bool Dead;
    private Rigidbody rigidBody;
    private SharkNest sharkNest;
    private GameObject target;
}
