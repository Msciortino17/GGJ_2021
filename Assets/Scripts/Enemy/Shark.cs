using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public class Shark : MonoBehaviour
{

    public GameObject Bite;

    public float Acceleration;
    public float Speed;
    public float RotationSpeed;

    private float BiteImpulseForce = 5.0f;
    private float BiteRate_sec = 5.0f;
    private float BiteTriggerRange = 4.0f;
    private float BiteAccuracyRequired = 0.8f;
    private float AggroRange = 60.0f;
    private float PatrolRange = 30.0f;
    private float PatrolPointRange = 4.0f;

    public enum SharkState
    {
        Aggro,
        Patrol
    }

    public void SetSharkNest(SharkNest sharkNest)
    {
        this.sharkNest = sharkNest;
    }

    public void SetPlayer(GameObject _player)
    {
        player = _player;
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
        Vector3 targetVectorFromNest = player.transform.position - sharkNest.transform.position;
        float distanceFromNest = targetVectorFromNest.magnitude;
        if (distanceFromNest > AggroRange)
        {
            SetState(SharkState.Patrol);
            NewPatrolPoint();
        }
        else
        {
            SetState(SharkState.Aggro);
        }

        // Determine current target point based on state
        switch(state)
        {
            case SharkState.Aggro:
                targetPoint = player.transform.position;
            break;
            case SharkState.Patrol:
                targetPoint = patrolPoint;
            break;
        }

        // Track target
        Vector3 toTarget = targetPoint - transform.position;
        Vector3 toTargetUnit = toTarget.normalized;
        float distanceToTarget = toTarget.magnitude;

        // Look at target
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(toTargetUnit, Vector3.up), RotationSpeed * Time.deltaTime);

        // Patrol logic
        if (state == SharkState.Patrol )
        {
            if(distanceToTarget < PatrolPointRange )
            {
                NewPatrolPoint();
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

    private bool Dead;
    private Rigidbody rigidBody;
    private SharkNest sharkNest;
    private GameObject player;
    private Vector3 patrolPoint;
    private Vector3 targetPoint;
}
