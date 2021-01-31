using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;
using DG.Tweening;
using Unity.Profiling;

public class Tentacle : MonoBehaviour
{
    public Spline TentacleSpline;

    private float WaveScale = 3.0f;
    private float WaveFreq = 2.0f;

    private int BaseNodes = 2;

    private void Start() {
        WaveTimer += Random.value * 5;
        IncrementTentacle();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateTentacle();
    }

    private float WaveTimer = 0;

    private void UpdateTentacle()
    {
        if( !isActive ) return;

        WaveTimer += Time.deltaTime;
        IncrementTentacle();
    }

    private void IncrementTentacle()
    {
        var nodes = TentacleSpline.nodes;
        for( int i = BaseNodes; i < nodes.Count; ++i )
        {
            SplineNode node = TentacleSpline.nodes[i];

            float ratio = (float)(i-BaseNodes) / (nodes.Count - BaseNodes - 1) * (2*Mathf.PI) * WaveFreq;

            float result = Mathf.Sin( ratio + WaveTimer ) * WaveScale;

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

    private Vector3[] bones;
    private bool isActive;
    private GameObject submarine;
}
