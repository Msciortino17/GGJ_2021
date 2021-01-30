using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shark : MonoBehaviour
{
    public GameObject Target;

    public float Acceleration = 5.0f;
    public float Speed = 15.0f;
    public float RotationSpeed = 13.0f;

    // Start is called before the first frame update
    void Start()
    {
        rigidBody = GetComponent<Rigidbody>();
    }

    private void FixedUpdate() {

        if( rigidBody == null ) return;

        // Move To
        rigidBody.AddForce(transform.forward * Acceleration, ForceMode.Force);

        if(rigidBody.velocity.magnitude > Speed)
        {
            rigidBody.velocity = rigidBody.velocity.normalized * Speed;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if( Target != null)
        {
            Vector3 toPlayer = Target.transform.position - transform.position;
            Vector3 toPlayerUnit = toPlayer.normalized;

            // Look at target
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(toPlayerUnit, Vector3.up), RotationSpeed * Time.deltaTime);
        }
    }

    private Rigidbody rigidBody;
}
