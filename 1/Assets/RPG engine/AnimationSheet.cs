/// <summary>
/// A SpriteSheet containing multiple sprites for a single object, corresponding to animation for the object,
/// rather than single sprites for many different objects (a TileSet).
/// </summary>
public abstract class AnimationSheet : SpriteSheet
{
    /// <summary>
    /// Find an animation, given its name.
    /// </summary>
    /// <param name="animationName">Name of the animation.</param>
    /// <returns>The animation object</returns>
    public abstract SpriteAnimation this[string animationName] { get; }
}

