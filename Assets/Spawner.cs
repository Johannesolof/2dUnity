using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = UnityEngine.Random;

public class Spawner : MonoBehaviour
{
    public GameObject Block;
    public float Interval;
    public float XRange;

    private float _lastTime;
    private Transform _origin;
    private Vector2 _size;
    private Color[] _colors = { Color.red, Color.magenta, Color.yellow, Color.cyan, Color.blue, Color.gray, };

// Use this for initialization
    void Start()
    {
        _origin = transform;
        _lastTime = Time.time;
        _size = Block.GetComponent<BoxCollider2D>().size;
        Spawn((Shape)Random.Range(0, 6));
    }

    // Update is called once per frame
    void Update()
    {
        if (Interval < 0 || _lastTime + Interval > Time.time)
            return;
        _lastTime = Time.time;
        Spawn((Shape) Random.Range(0,6));
    }


    private enum Shape{ I,J,L,O,S,T,Z }

    private readonly Dictionary<Shape, List<Vector2>> _shapes = new Dictionary<Shape, List<Vector2>>
    {
        {Shape.I, new List<Vector2> { new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(3, 1) } },
        {Shape.J, new List<Vector2> { new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(2, 0) } },
        {Shape.L, new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1) } },
        {Shape.O, new List<Vector2> { new Vector2(0, 0), new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0) } },
        {Shape.S, new List<Vector2> { new Vector2(0, 0), new Vector2(1, 0), new Vector2(1, 1), new Vector2(2, 1) } },
        {Shape.T, new List<Vector2> { new Vector2(0, 1), new Vector2(1, 1), new Vector2(2, 1), new Vector2(1, 0) } },
        {Shape.Z, new List<Vector2> { new Vector2(0, 1), new Vector2(1, 1), new Vector2(1, 0), new Vector2(2, 0) } },
    };

    void Spawn(Shape shape)
    {
        var xOffset = new Vector3(Random.Range(-XRange/2, XRange/2), 0);
        var blocks = new List<GameObject>();
        foreach (var point in _shapes[shape])
        {
            var pos = new Vector3(_size.x*point.x, _size.y*point.y) + xOffset + _origin.position;
            blocks.Add((GameObject)Instantiate(Block, pos, Quaternion.identity));
            blocks.Last().GetComponent<SpriteRenderer>().color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));

            if (shape == Shape.T && blocks.Count == 4)
            {
                var fj = blocks.Last().AddComponent<FixedJoint2D>();
                fj.connectedBody = blocks[1].GetComponent<Rigidbody2D>();
                fj.anchor = blocks[1].transform.position - blocks.Last().transform.position;
            }
            else if (blocks.Count > 1 )
            {
                var fj = blocks.Last().AddComponent<FixedJoint2D>();
                fj.connectedBody = blocks[blocks.Count - 2].GetComponent<Rigidbody2D>();
                fj.anchor = blocks[blocks.Count - 2].transform.position - blocks.Last().transform.position;
            }
        }

        if (shape == Shape.O)
        {
            var fj = blocks.First().AddComponent<FixedJoint2D>();
            fj.connectedBody = blocks.Last().GetComponent<Rigidbody2D>();
            fj.anchor = blocks.Last().transform.position - blocks.First().transform.position;
        }

        foreach (var block in blocks)
        {
            block.transform.RotateAround(blocks.First().transform.position ,Vector3.forward, Random.Range(0f, 360f));
        }
    }
}
