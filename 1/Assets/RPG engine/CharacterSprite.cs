using System;

using UnityEngine;

/// <summary>
/// An animated sprite representing a walking character.
/// </summary>
public class CharacterSprite : AnimatedSprite
{
    /// <summary>
    /// The animations for walking in each direction.
    /// </summary>
    private SpriteAnimation north, south, east, west;

    /// <summary>
    /// An animated sprite representing a walking character.
    /// </summary>
    public CharacterSprite()
    {
        TileType = TileType.Character;
    }

    /// <summary>
    /// Called on object startup.
    /// Find the animations we'll be using and the map.
    /// </summary>
    public void Start()
    {
        north = AnimationSheet["north"];
        if (north == null)
            Debug.LogError("north spriteAnimation not found for "+gameObject.name);
        south = AnimationSheet["south"];
        if (south == null)
            Debug.LogError("south spriteAnimation not found for " + gameObject.name);
        east = AnimationSheet["east"];
        if (east == null)
            Debug.LogError("east spriteAnimation not found for " + gameObject.name);
        west = AnimationSheet["west"];
        if (west == null)
            Debug.LogError("west spriteAnimation not found for " + gameObject.name);
    }

    /// <summary>
    /// Update the position and animation of the character.
    /// </summary>
    public override void Update()
    {
        this.UpdateMovementDirection();
        this.UpdateDirectionAnimation();
        base.Update();
    }

    /// <summary>
    /// Stops character and turns them to face in the specified direction.
    /// If direction vector is zero, the character stops and idles its current animation.
    /// </summary>
    /// <param name="direction">Vector in direction to face.</param>
    public void Face(Vector2 direction)
    {
        MovementDirection = Vector2.zero;
        if (direction == Vector2.zero)
            StartIdleAnimation(this.CurrentAnimation);
        else
        {
            var aX = Math.Abs(direction.x);
            var aY = Math.Abs(direction.y);
            if (aX > aY)
            {
                this.StartIdleAnimation(direction.x > 0 ? this.east : this.west);
            }
            else
            {
                this.StartIdleAnimation(direction.y > 0 ? this.south : this.north);
            }
        }
    }

    /// <summary>
    /// Recompute MovementDirection based on Velocity
    /// </summary>
    private void UpdateMovementDirection()
    {
        this.MovementDirection = Vector2.zero;
        var absVx = Math.Abs(this.Velocity.x);
        var absVy = Math.Abs(this.Velocity.y);
        if (absVx > absVy)
        {
            this.MovementDirection.x = Math.Sign(this.Velocity.x);
        }
        else
        {
            this.MovementDirection.y = Math.Sign(this.Velocity.y);
        }
    }

    /// <summary>
    /// Update current animation based on MovementDirection.
    /// If movementDirection is zero (we're stopped) then idles
    /// whatever the current animation is.
    /// </summary>
    private void UpdateDirectionAnimation()
    {
        if (this.MovementDirection == Vector2.zero)
            StartIdleAnimation(this.CurrentAnimation);
        else
        {
            SpriteAnimation newAnimation = null;
            if (this.MovementDirection.x > 0)
                newAnimation = east;
            else if (this.MovementDirection.x < 0)
                newAnimation = west;
            else if (this.MovementDirection.y < 0)
                newAnimation = north;
            else if (this.MovementDirection.y > 0)
                newAnimation = south;

            if (newAnimation != this.CurrentAnimation)
            {
                StartPositionalAnimation(newAnimation, this.MovementDirection);
            }
        }
    }

    public Vector2 RedVector;

    public Vector2 GreenVector;

    public Vector2 BlueVector;

    public bool DisplayDebugVectors;

    public override void DebugDraw()
    {
        if (DisplayDebugVectors)
        {
            GL.Begin(GL.TRIANGLES);
            DrawLine(RedVector, Color.red);
            DrawLine(GreenVector, Color.green);
            DrawLine(BlueVector, Color.blue);
            GL.End();
        }
    }

    private void DrawLine(Vector2 vector, Color color)
    {
        if (vector != Vector2.zero)
        {
            var start = Position;
            var end = Position + vector;
            var perp = 4*vector.PerpClockwise().normalized;
            GL.Color(color);
            GL.Vertex((start - perp).ToVector3());
            GL.Vertex((end).ToVector3());
            GL.Vertex((start + perp).ToVector3());
        }
    }
}
