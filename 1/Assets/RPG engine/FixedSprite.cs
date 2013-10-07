using UnityEngine;

/// <summary>
/// A sprite drawn based on a dedicated texture.
/// </summary>
public class FixedSprite : Sprite
{
    #region Editable fields
    /// <summary>
    /// The texture to draw
    /// </summary>
    public Texture2D Image;
    #endregion

    #region Derived properties
    public override int Width
    {
        get { return Image.width; }
    }

    public override int Height
    {
        get { return Image.height; }
    }
    #endregion

    /// <summary>
    /// Draw the sprite
    /// </summary>
    public override void Render()
    {
        Graphics.DrawTexture(BoundingBox, Image);
    }
}
