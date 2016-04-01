using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    public GameObject Block;
    public float Interval;

    private float _lastTime;
    private Transform _origin;
    
    // Use this for initialization
    void Start()
    {
        _origin = transform;
        _lastTime = Time.time;
    }

    // Update is called once per frame
    void Update()
    {
        if(_lastTime + Interval > Time.time)
            return;
        _lastTime = Time.time;
        Instantiate(Block, _origin.position, Quaternion.Euler(0.0f, 0.0f, Random.Range(0.0f, 360.0f)));
    }
}
