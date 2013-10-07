using UnityEngine;

/// <summary>
/// These are just "extension methods", i.e. methods that appear within other classes,
/// that provide functionality that should probably have been within the original classes
/// to begin with.
/// </summary>
public static class GeometryExtensions
{
    /// <summary>
    /// True if this Rect overlaps with the other (including just touching the boundary)
    /// </summary>
    public static bool Overlaps(this Rect r1, Rect r2)
    {
        return (r1.xMin <= r2.xMax) && (r1.yMin <= r2.yMax) && (r1.xMax >= r2.xMin) && (r1.yMax >= r2.yMin);
    }

    /// <summary>
    /// Returns the vector v rotated 90 degrees clockwise
    /// </summary>
    public static Vector2 PerpClockwise(this Vector2 v)
    {
        return new Vector2(v.y, -v.x);
    }

    /// <summary>
    /// Returns the vector v rotated 90 degrees counter-clockwise
    /// </summary>
    public static Vector2 PerpCounterClockwise(this Vector2 v)
    {
        return new Vector2(-v.y, v.x);
    }

    /// <summary>
    /// Returns the Rect shifted by the specified offset vector.
    /// </summary>
    public static Rect Shift(this Rect r, Vector2 offset)
    {
        return new Rect(r.xMin+offset.x, r.yMin+offset.y, r.width, r.height);
    }

    public static Vector3 ToVector3(this Vector2 v)
    {
        return new Vector3(v.x, v.y, 0);
    }
}