using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;
using DG.Tweening;
using Unity.Profiling;
using UnityEngine.Experimental.TerrainAPI;

public class Tentacle : MonoBehaviour
{
    public Spline TentacleSpline;

    private float RotationSpeed = 5.0f;
    private float AttackRate = 5.0f;
    private float AttackTime = 1.0f;
    private float AttackRange = 20.0f;

    private float WaveScale = 3.0f;
    private float WaveFreq = 2.0f;
    
    private int BaseNodes = 2;
    public float DamageCooldown = 2.0f;
    public float Damage = 20.0f;

    public LayerMask HitMask;
    
    private void Awake()
    {
        originalNodePositions = new Vector3[TentacleSpline.nodes.Count];
        for( int i = 0; i < TentacleSpline.nodes.Count; ++i )
        {
            originalNodePositions[i] = TentacleSpline.nodes[i].Position;
        }

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

        CooldownTimer.Interval();
        WaveTimer.Interval();
        IncrementTentacle();
    }

    private void IncrementTentacle()
    {
        Vector3 playerPos = submarine?.transform.position ?? Vector3.zero;
        Vector3 toPlayer = playerPos - transform.position;
        toPlayer.y = 0;
        float distance = toPlayer.magnitude;

        transform.rotation = Quaternion.Slerp( transform.rotation,
                                               Quaternion.LookRotation( toPlayer, Vector3.up ),
                                               RotationSpeed * Time.deltaTime );

        AttackTimer.Interval();
        if( distance < AttackRange )
        {
            if( AttackTimer.Seconds > AttackRate && !Attacking )
            {
                Attacking = true;
            }
        }
        else
        {
            Attacking = false;
            AttackingTimer.Reset();
        }

        if(Attacking)
        {
            if( AttackingTimer.Seconds > AttackTime / 2.0 )
            {
                if( CooldownTimer.Seconds >= DamageCooldown )
                {
                    // Play sound?
                    submarine.GetComponent<Submarine>()?.TakeDamage( Damage );
                    CooldownTimer.Reset();
                }
            }

            AttackingTimer.Interval();
            if(AttackingTimer.Seconds > AttackTime)
            {
                Attacking = false;
                AttackingTimer.Reset();
                AttackTimer.Reset();
            }
        }

        var nodes = TentacleSpline.nodes;
        for( int i = BaseNodes; i < nodes.Count; ++i )
        {
            SplineNode node = TentacleSpline.nodes[i];

            float nodeRatio = (float)(i-BaseNodes) / (nodes.Count - BaseNodes - 1);

            float attackTimeRatio = AttackingTimer.Seconds / (AttackTime/2);
            if( attackTimeRatio > 1 )
            {
                attackTimeRatio = 2 - attackTimeRatio;
            }

            float xResult = Mathf.Sin( ( nodeRatio * (2*Mathf.PI) * WaveFreq ) + WaveTimer.Seconds ) * WaveScale;
            
            float zResult = attackTimeRatio * nodeRatio * (distance*1.1f);

            float nodeYPos = originalNodePositions[i].y;
            float yDistance = playerPos.y - (nodeYPos + transform.position.y);
            float yResult = nodeYPos + ( attackTimeRatio * nodeRatio * (yDistance*1.1f) );

            Vector3 pos = node.Position;
            pos.x = xResult;
            pos.z = zResult;
            pos.y = yResult;

            Vector3 dir = pos;
            dir.y += 1;

            node.Position = pos;
            node.Direction = dir;
        }
    }

    private void OnCollisionEnter( Collision other )
    {
        Torpedo torpedo = other.gameObject.GetComponent<Torpedo>();
        if( torpedo != null )
        {
            Destroy( gameObject, 0.5f );
            Dead = true;
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

    private bool Dead;
    private Vector3[] originalNodePositions;
    private bool Attacking = false;
    private Timer CooldownTimer = new Timer();
    private Timer WaveTimer = new Timer();
    private Timer AttackTimer = new Timer();
    private Timer AttackingTimer = new Timer();
    private Vector3[] bones;
    private bool isActive;
    private GameObject submarine;
}
