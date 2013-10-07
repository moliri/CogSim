using System;

using UnityEngine;

public class CharacterSteeringController : BindingBehaviour
{
    [Bind]
#pragma warning disable 649
    private CharacterSprite mySprite;
#pragma warning restore 649

    /// <summary>
    /// The current location we're steering to.  If null, then character is stopped.
    /// </summary>
    private Vector2? targetLocation;

    private float maxSpeed;

    public float MaxForce = 1000;

    /// <summary>
    /// The current position of the character.
    /// </summary>
    public Vector2 Position
    {
        get
        {
            return this.mySprite.Position;
        }
    }

    /// <summary>
    /// Stops the character.
    /// </summary>
    public void Stop()
    {
        targetLocation = null;
        this.mySprite.Velocity = Vector2.zero;
    }

    /// <summary>
    /// Switches the target of the seek behavior to be the specified location.
    /// </summary>
    /// <param name="target">New target location for Seek behavior</param>
    /// <param name="speed">Speed at which to drive in this direction.</param>
    public void Seek(Vector2 target, float speed)
    {
        targetLocation = target;
        maxSpeed = speed;
    }

    Vector2 SeekSteering()
    {
        if (targetLocation == null)
            return Vector2.zero;
        var offset = targetLocation.Value - this.mySprite.Position;
        return offset * MaxForce / offset.magnitude;
    }

    public bool collisionDetected;

    public float collisionTime;

    Vector2? CollisionAvoidanceSteering()
    {
        // Replace this with the real algorithm
        return null;
    }

    private Vector2 PredictedPosition(CharacterSprite sprite, float time)
    {
        return sprite.Position + time * sprite.Velocity;
    }

    /// <summary>
    /// Computes the time of closest approach for the two characters, given their velocities
    /// Note: time could be negative (i.e. in the past)
    /// </summary>
    private float TimeOfClosestApproach(CharacterSprite c1, CharacterSprite c2)
    {
        var i = c1.Position - c2.Position;
        var deltaV = c1.Velocity - c2.Velocity;
        return -Vector2.Dot(i, deltaV) / deltaV.sqrMagnitude;
    }

    public void Update()
    {
        if (targetLocation == null)
        {
            this.mySprite.GreenVector = this.mySprite.RedVector = this.mySprite.BlueVector = Vector2.zero;
        }
        else
        {
            var seekSteering = this.SeekSteering();
            var collisionAvoidanceSteering = this.CollisionAvoidanceSteering();

            var force = this.MaybeAdd(seekSteering, collisionAvoidanceSteering);
            var fMag = force.magnitude;
            if (fMag > MaxForce)
                force *= MaxForce / fMag;

            var newVel = this.mySprite.Velocity + Time.deltaTime * force;
            var speed = newVel.magnitude;
            if (speed > maxSpeed)
                newVel *= (maxSpeed / speed);
            this.mySprite.Velocity = newVel;

            this.mySprite.GreenVector = 0.1f*this.SeekSteering();
            this.mySprite.RedVector = 0.1f*collisionAvoidanceSteering ?? Vector2.zero;
            this.mySprite.BlueVector = this.mySprite.Velocity;
        }
    }

    Vector2 MaybeAdd(Vector2 v1, Vector2? v2)
    {
        if (v2 == null)
            return v1;
        return v1 + v2.Value;
    }
}
