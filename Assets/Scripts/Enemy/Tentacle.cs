using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using SplineMesh;

public class Tentacle : MonoBehaviour
{
    public Spline TentacleSpline;
    public Transform Target;

    public GameObject Player;

    // Update is called once per frame
    void Update()
    {
        var nodes = TentacleSpline.nodes;
        var node = nodes[nodes.Count - 1];

        node.Position = Target.localPosition;
        //node.Direction = Target.forward;
    }

}
