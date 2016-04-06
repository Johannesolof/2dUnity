using UnityEngine;
using System.Collections.Generic;
using System;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using Object = UnityEngine.Object;


public class Checker : MonoBehaviour
{


    public int RowLength;
    public float Tolerance;
    public float Delay;
    public GameObject Particles;

    private List<GameObject> _objects;
    private SortY _sorter;
    private Dictionary<float, List<GameObject>> _toRemove;

    private class SortY : IComparer<GameObject>
    {
        public int Compare(GameObject t1, GameObject t2)
        {
            if (t1.transform.position.y > t2.transform.position.y)
                return 1;
            if (t1.transform.position.y < t2.transform.position.y)
                return -1;
            return 0;
        }
    }


    // Use this for initialization
    void Start()
    {
        _objects = new List<GameObject>();
        _sorter = new SortY();
        _toRemove = new Dictionary<float, List<GameObject>>();
    }

    // Update is called once per frame
    void Update()
    {
        _objects.Sort(_sorter);
        //Debug.Log(_objects.Aggregate("", (current, o) => current + String.Format("{0}, ", o.transform.position.y)));
        LineComplete();
        RemoveLines();
    }

    private void RemoveLines()
    {
        if (_toRemove.Count == 0)
            return;
        if (!(Mathf.Abs(_toRemove.First().Value.First().transform.position.y -
                        _toRemove.First().Value.Last().transform.position.y) < Tolerance))
        {
            _objects.AddRange(_toRemove.First().Value);
            _toRemove.Remove(_toRemove.First().Key);
            return;
        }
        if (_toRemove.First().Key + Delay < Time.time)
        {
            var objs = _toRemove.First().Value;
            _toRemove.Remove(_toRemove.First().Key);
            objs.ForEach(DestroyObject);
        }
    }

    private void DestroyObject(GameObject o)
    {
        GameObject p = (GameObject) Instantiate(Particles, o.transform.position, Quaternion.identity);
        Destroy(o);
        Destroy(p, p.GetComponent<ParticleSystem>().duration);
    }

    private void LineComplete()
    {
        if (_objects.Count < RowLength)
            return;
        for (int i = RowLength - 1; i < _objects.Count; i++)
        {
            if (!(Mathf.Abs(_objects[i].transform.position.y - _objects[i - RowLength + 1].transform.position.y) < Tolerance))
                continue;
            _toRemove.Add(Time.time, _objects.GetRange(i - RowLength + 1, RowLength));
            _objects.RemoveRange(i - RowLength + 1, RowLength);
            return;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        
        if (other.tag.Equals("Block") && !_objects.Contains(other.gameObject))
            _objects.Add(other.gameObject);
    }

    void OnTriggerExit2D(Collider2D other)
    {
        _objects.Remove(gameObject);
    }
}