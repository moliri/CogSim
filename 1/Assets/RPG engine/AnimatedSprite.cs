using System;
using UnityEngine;

/// <summary>
/// A sprite that can be animated from a spritesheet (specifically, an AnimationSheet).
/// </summary>
public class AnimatedSprite : SpriteObject
{
    #region Editable fields
    /// <summary>
    /// Spritesheet used by this sprite.
    /// </summary>
    public AnimationSheet AnimationSheet;
    #endregion

    #region Derived properties
    public override int Width
    {
        get { return AnimationSheet == null ? 0 : this.AnimationSheet.TileWidth; }
    }

    public override int Height
    {
        get { return AnimationSheet == null ? 0 : this.AnimationSheet.TileHeight; }
    }
    #endregion

    /// <summary>
    /// Controls how an animation updates
    /// </summary>
    enum AnimationMode
    {
        /// <summary>
        /// Animation stays at its current frame until further notice.
        /// </summary>
        Stopped,
        /// <summary>
        /// Animation changes frames based on distance moved by the sprite, rather than time
        /// The animation's Stride determines the distance the sprite has to move to cycle through the whole animation.
        /// </summary>
        Distance,
        /// <summary>
        /// The animation advances based on elapsed time.
        /// </summary>
        Time
    }

    #region State variables for animation
    /// <summary>
    /// Currently playing animation
    /// </summary>
    protected SpriteAnimation CurrentAnimation;

    /// <summary>
    /// Current state of the current animation.
    /// </summary>
    private AnimationMode mode = AnimationMode.Stopped;

    /// <summary>
    /// Time the current animation was started, if it is in Time mode.
    /// </summary>
    private float startTime;

    /// <summary>
    /// Position the current animation started, if it's in Distance mode.
    /// </summary>
    private Vector2 startPosition;

    /// <summary>
    /// Unit vector in the direction of current motion.
    /// Used to measure distance traveled when playing an animation in Distance mode.
    /// </summary>
    protected Vector2 MovementDirection;

    /// <summary>
    /// Current frame of the animation as determined by mode, time, and/or distance.
    /// </summary>
    public TilePosition CurrentFrame
    {
        get
        {
            if (this.CurrentAnimation == null)
                return new TilePosition(0, 0);

            switch (mode)
            {
                case AnimationMode.Stopped:
                    return this.CurrentAnimation.FrameAtPhase(0, 1);

                case AnimationMode.Time:
                    return this.CurrentAnimation.FrameAtPhase(Time.fixedTime - startTime, this.CurrentAnimation.Seconds);

                case AnimationMode.Distance:
                    float phase = Vector2.Dot(Position - startPosition, this.MovementDirection);
                    return this.CurrentAnimation.FrameAtPhase(
                        phase, this.CurrentAnimation.Stride);

                default:
                    throw new InvalidOperationException("Bad spriteAnimation mode");
            }
        }
    }
    #endregion

    #region User-callable procedures for controling animation.
    /// <summary>
    /// Halt the current animation, whatever it may be, on the current frame, whatever it may be.
    /// </summary>
    public void StopAnimation()
    {
        mode = AnimationMode.Stopped;
    }

    /// <summary>
    /// Switch to the first frame of the specified animation and stay there.
    /// </summary>
    /// <param name="animationName">Name of the animation</param>
    public void StartIdleAnimation(string animationName)
    {
        this.StartIdleAnimation(AnimationSheet[animationName]);
    }

    /// <summary>
    /// Switch to the first frame of the specified animation and stay there.
    /// </summary>
    /// <param name="spriteAnimation">The animation to switch to</param>
    public void StartIdleAnimation(SpriteAnimation spriteAnimation)
    {
        this.CurrentAnimation = spriteAnimation;
        mode = AnimationMode.Stopped;
    }

    /// <summary>
    /// Switch to the first frame of this animation and start it playing in timed mode.
    /// </summary>
    /// <param name="animationName">The animation to switch to.</param>
    public void StartTimedAnimation(string animationName)
    {
        StartTimedAnimation(AnimationSheet[animationName]);
    }

    /// <summary>
    /// Switch to the first frame of this animation and start it playing in timed mode.
    /// </summary>
    /// <param name="spriteAnimation">The animation to switch to.</param>
    public void StartTimedAnimation(SpriteAnimation spriteAnimation)
    {
        this.CurrentAnimation = spriteAnimation;
        mode = AnimationMode.Time;
        startTime = Time.fixedTime;
    }

    /// <summary>
    /// Switch to the first frame of this animation and start it playing in distance mode.
    /// </summary>
    /// <param name="animationName">The animation to switch to.</param>
    /// <param name="motionDirection">Direction in which to measure distance.</param>
    public void StartPositionalAnimation(string animationName, Vector2 motionDirection)
    {
        StartPositionalAnimation(AnimationSheet[animationName], motionDirection);
    }

    /// <summary>
    /// Switch to the first frame of this animation and start it playing in distance mode.
    /// </summary>
    /// <param name="spriteAnimation">The animation to switch to.</param>
    /// <param name="motionDirection">Direction in which to measure distance.</param>
    public void StartPositionalAnimation(SpriteAnimation spriteAnimation, Vector2 motionDirection)
    {
        this.CurrentAnimation = spriteAnimation;
        startPosition = Position;
        this.MovementDirection = motionDirection;
        mode = AnimationMode.Distance;
    }
    #endregion

    /// <summary>
    /// Draw the sprite.
    /// </summary>
    public override void Render()
    {
        if (AnimationSheet != null)
        {
            if (this.CurrentAnimation == null)
                this.StartIdleAnimation((SpriteAnimation)null);  // Choose default spriteAnimation

            AnimationSheet.Draw(CurrentFrame, BoundingBox);
        }
    }
}
