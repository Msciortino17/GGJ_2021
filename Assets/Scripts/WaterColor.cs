using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterColor : MonoBehaviour
{
    [Range(1,200)]
    public float MaxHeight = 100f;

    public Gradient Gradient;

    Terrain terrain;
    Camera camera;

    float initialTerrainHeight;

    // Start is called before the first frame update
    void Start()
    {
        terrain = GameObject.Find("Sea Floor").GetComponent<Terrain>();
        camera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        float t = Mathf.InverseLerp(0, MaxHeight, transform.position.y - terrain.SampleHeight(transform.position));

        camera.backgroundColor = Gradient.Evaluate(1 - t);
        RenderSettings.fogColor = Color.Lerp(Gradient.Evaluate(t), Color.black, t);
    }
}
