using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

/// <summary>
/// An AnimationSheet in which each row is its own animation.
/// </summary>
[DebuggerDisplay("{Name}")]
public class RowAnimationSheet : AnimationSheet
{
    /// <summary>
    /// The animations contained in this sheet.
    /// </summary>
    public List<RowSpriteAnimation> Animations = new List<RowSpriteAnimation>();

    /// <summary>
    /// Returns an animation given its name
    /// </summary>
    /// <param name="animationName">The name of the animation</param>
    /// <returns>The animation object</returns>
    public override SpriteAnimation this[string animationName]
    {
        get { return this.Animations.Find(a => a.Name == animationName); }
    }

    [Serializable]
    public class RowSpriteAnimation : SpriteAnimation
    {
// ReSharper disable InconsistentNaming
        /// <summary>
        /// The row of the SpriteSheet in which the animation appears
        /// </summary>
        [HideInInspector]
        public int row;

        /// <summary>
        /// The distance this animation covers, if it's a distance-based animation
        /// </summary>
        public int stride;
        /// <summary>
        /// The time this animation covers, if it's time-based.
        /// </summary>
        public int seconds;
// ReSharper restore InconsistentNaming

        /// <summary>
        /// Number of frames in the animation (fixed at the number of columns for a RowAnimationSheet).
        /// </summary>
        public override int Frames
        {
            get { return AnimationSheet.Columns; }
        }

        /// <summary>
        /// The i'th frame of this animation
        /// </summary>
        /// <param name="frameNumber">Frame number of the frame (0=first frame)</param>
        /// <returns>The frame, specified as a TilePosition in the underlying SpriteSheet.</returns>
        public override TilePosition Frame(int frameNumber)
        {
            return new TilePosition(frameNumber, this.row);
        }

        /// <summary>
        /// Distance the character has to move to cycle through the complete animation.
        /// </summary>
        public override float Stride
        {
            get { return stride; }
        }

        /// <summary>
        /// Number of seconds for the animation
        /// </summary>
        public override float Seconds
        {
            get { return seconds; }
        }

        /// <summary>
        /// Just returns the name of the animationsheet.
        /// </summary>
        /// <returns>the name</returns>
        public override string ToString()
        {
            return Name;
        }
    }
}