using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A 2D image drawn in screen coordinates.
/// </summary>
public abstract class Sprite : BindingBehaviour
{
    #region Fields and properties
    /// <summary>
    /// 2D location of sprite in pixel coordinates
    /// </summary>
    public Vector2 Position
    {
        get
        {
            Vector3 position = transform.position;
            return new Vector2(position.x, position.y);
        }

        set
        {
            transform.position = new Vector3(value.x, value.y, 0);
        }
    }

    /// <summary>
    /// Which pixel of the sprite corresponds to the Position variable
    /// </summary>
    public Vector2 CenterOffset;

    /// <summary>
    /// The width of the sprite on screen (in pixels)
    /// </summary>
    public abstract int Width { get; }

    /// <summary>
    /// The height of the sprite on screen (in pixels)
    /// </summary>
    public abstract int Height { get; }

    /// <summary>
    /// Pixels resolution to which this sprite should snap when moved in the editor.
    /// </summary>
    public virtual int SnapToGridX
    {
        get
        {
            return 1;
        }
    }

    /// <summary>
    /// Pixels resolution to which this sprite should snap when moved in the editor.
    /// </summary>
    public virtual int SnapToGridY
    {
        get
        {
            return 1;
        }
    }

    /// <summary>
    /// AABB for sprite on screen, in pixel coordinates.
    /// </summary>
    public virtual Rect BoundingBox
    {
        get
        {
            var p = transform.position;
            return new Rect(p.x - CenterOffset.x, p.y - CenterOffset.y, Width, Height);
        }
    }

    /// <summary>
    /// Rate of translation of sprite
    /// </summary>
    public Vector2 Velocity;
    #endregion

    #region Picking and hit testing for the mouse
    /// <summary>
    /// Find the sprite that corresponds to a given screen location.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public static Sprite Pick(Vector2 v)
    {
        var allSprites = (List<Sprite>)Registry(typeof(Sprite));
        Sprite best = null;
        float bestHeight = -1;
        for (int i = allSprites.Count - 1; i >= 0; i--)
        {
            var s = allSprites[i];
            if (s.Position.y > bestHeight && s.HitTest(v))
            {
                bestHeight = s.Position.y;
                best = s;
            }
        }
        return best;
    }

    /// <summary>
    /// Test if the sprite overlaps this screen location.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public virtual bool HitTest(Vector2 v)
    {
        return BoundingBox.Contains(v);
    }
    #endregion

    /// <summary>
    /// Draws the sprite
    /// </summary>
    public abstract void Render();

    /// <summary>
    /// Draw debug annotations, if any.
    /// </summary>
    public virtual void DebugDraw() { }
}
