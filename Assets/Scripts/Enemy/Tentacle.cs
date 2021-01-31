using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;
using DG.Tweening;
using Unity.Profiling;

public class Tentacle : MonoBehaviour
{
    public Spline TentacleSpline;

    private float RotationSpeed = 5.0f;
    private float AttackRate = 3.0f;
    private float AttackTime = 0.5f;
    private float AttackRange = 5.0f;

    private float WaveScale = 3.0f;
    private float WaveFreq = 2.0f;

    private int BaseNodes = 2;

    private void Start() {
        WaveTimer.SetTime(Random.value * 5);
        IncrementTentacle();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateTentacle();
    }

    private void UpdateTentacle()
    {
        if( !isActive ) return;

        WaveTimer.Interval();
        IncrementTentacle();
    }

    private void IncrementTentacle()
    {
        Vector3 toPlayer = submarine.transform.position - transform.position;
        toPlayer.y = 0;
        Vector3 toPlayerUnit = toPlayer.normalized;
        float distance = toPlayer.magnitude;

        transform.rotation = Quaternion.Slerp( transform.rotation,
                                               Quaternion.LookRotation( toPlayer, Vector3.up ),
                                               RotationSpeed * Time.deltaTime );

        if( distance < AttackRange )
        {
            AttackTimer.Interval();
            if( AttackTimer.Seconds > AttackRate && !Attacking )
            {
                Attacking = true;
                // Play sound?
            }
        }

        if(Attacking)
        {

            if(AttackingTimer.Seconds > AttackTime)
            {
                AttackingTimer.Reset();
                AttackTimer.Reset();
            }
        }

        var nodes = TentacleSpline.nodes;
        for( int i = BaseNodes; i < nodes.Count; ++i )
        {
            SplineNode node = TentacleSpline.nodes[i];

            float ratio = (float)(i-BaseNodes) / (nodes.Count - BaseNodes - 1) * (2*Mathf.PI) * WaveFreq;

            float result = Mathf.Sin( ratio + WaveTimer.Seconds ) * WaveScale;

            Vector3 pos = node.Position;
            pos.x = result;

            Vector3 dir = pos;
            dir.y += 1;

            node.Position = pos;
            node.Direction = dir;
        }
    }

    private void OnTriggerEnter(Collider other) {
        
        if(other.gameObject.CompareTag("Submarine"))
        {
            submarine = other.gameObject;
            isActive = true;
        }
    }

    private void OnTriggerExit(Collider other) {
        
        if(other.gameObject.CompareTag("Submarine"))
        {
            isActive = false;
        }
    }


    private bool Attacking = false;
    private Timer WaveTimer = new Timer();
    private Timer AttackTimer = new Timer();
    private Timer AttackingTimer = new Timer();
    private Vector3[] bones;
    private bool isActive;
    private GameObject submarine;
}
