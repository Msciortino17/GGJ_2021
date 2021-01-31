using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterColor : MonoBehaviour
{
    [Range(0f, 600f)]
    public float MinHeight = 200f;

    [Range(0f,600f)]
    public float MaxHeight = 400f;

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
        float t = Mathf.InverseLerp(MinHeight, MaxHeight, transform.position.y);

        print(t);

        camera.backgroundColor = Gradient.Evaluate(t);
        RenderSettings.fogColor = Color.Lerp(Color.black, Gradient.Evaluate(t), t);
    }
}
