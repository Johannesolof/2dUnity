using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Heroine : MonoBehaviour
{
    public LayerMask Masks;
    public float JumpForce;
    public float WalkForce;
    public float AirForce;

    private IHeroineState _state;
    [HideInInspector]
    public Rigidbody2D Rigidbody;

    private BoxCollider2D _feetCollider;
    private Collider2D _groundCollider;

    [HideInInspector]
    public Queue<Action<Rigidbody2D>> PhysicsQueue;
    

    // Use this for initialization
    void Start()
    {
        Rigidbody = GetComponent<Rigidbody2D>();
        PhysicsQueue = new Queue<Action<Rigidbody2D>>(4);
        _state = new JumpingState();
        _feetCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {
        _state = _state.HandelInput(this);
        Debug.Log(_state.ToString());
    }


    void FixedUpdate()
    {
        while (PhysicsQueue.Any())
        {
            PhysicsQueue.Dequeue()(Rigidbody);
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        _groundCollider = other;
    }


    public bool Grounded()
    {
        if (_feetCollider.IsTouchingLayers(Masks))
            if (_feetCollider.IsTouching(_groundCollider) && _groundCollider.attachedRigidbody != null &&
                Mathf.Abs(Rigidbody.velocity.y - _groundCollider.attachedRigidbody.velocity.y) < 0.1f)
                return true;
            else if (Mathf.Abs(Rigidbody.velocity.y) < 0.1f)
                return true;
        return false;
    }
}

internal interface IHeroineState
{
    IHeroineState HandelInput(Heroine heroine);
}

internal class OnGroundState : IHeroineState
{
    public IHeroineState HandelInput(Heroine heroine)
    {
        if (Input.GetButtonDown("Jump"))
        {
            heroine.PhysicsQueue.Enqueue(rb => rb.AddForce(heroine.transform.up * heroine.JumpForce, ForceMode2D.Impulse));
            return new JumpingState();
        }
        heroine.PhysicsQueue.Enqueue(rb => rb.AddForce(new Vector2(Input.GetAxis("Horizontal") * heroine.WalkForce, 0)));
        return this;
    }
}

internal class DuckingState : OnGroundState
{
    public new IHeroineState HandelInput(Heroine heroine)
    {
        return base.HandelInput(heroine);
    }
}

internal class InAirState : IHeroineState
{
    public IHeroineState HandelInput(Heroine heroine)
    {
        heroine.PhysicsQueue.Enqueue(rb => rb.AddForce(new Vector2(Input.GetAxis("Horizontal") * heroine.AirForce, 0)));
        if (heroine.Grounded())
            return new OnGroundState();
        return this;
    }
}

internal class JumpingState : InAirState
{
    public new IHeroineState HandelInput(Heroine heroine)
    {
        return base.HandelInput(heroine);
    }
}