﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SharkNest : MonoBehaviour
{
    public GameObject SharkPrefab;

    public Transform SpawnPoint;

    public float Radius = 50.0f;

    public int SharksInNest = 3;

    public float SpawnRate_sec = 5;

    private void Update()
    {
        if(SubmarineDetected)
        {
            if(SharksSpawned < SharksInNest)
            {
                Timer.Interval();
                if( Timer.Seconds > SpawnRate_sec || SharksSpawned == 0 )
                {
                    SpawnShark();
                    Timer.Reset();
                }
            }
        }
        else
        {
            Timer.Reset();
        }
    }

    /// <summary>
    /// Spawns a shark at the spawn point
    /// </summary>
    private void SpawnShark()
    {
        Vector3 toPlayer = SpawnPoint.forward;
        if(Submarine != null) toPlayer = Submarine.transform.position - transform.position; 

        Shark shark = Instantiate(SharkPrefab, SpawnPoint.position, Quaternion.LookRotation(toPlayer, Vector3.up)).GetComponent<Shark>();

        shark.SetSharkNest(this);
        shark.SetPlayer(Submarine);
        shark.SetState(Shark.SharkState.Aggro);
        shark.SetRadius( Radius );

        SharksSpawned++;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.CompareTag("Submarine"))
        {
            SubmarineDetected = true;
            Submarine = other.gameObject;
        }
    }
    
    private void OnTriggerExit(Collider other)
    {
        if(other.gameObject.CompareTag("Submarine"))
        {
            SubmarineDetected = false;
        }
    }

    private bool SubmarineDetected;
    private Timer Timer = new Timer();
    private GameObject Submarine;
    private int SharksSpawned;
}
