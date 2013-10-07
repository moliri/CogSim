using UnityEngine;

/// <summary>
/// A sprite made up of one or more tiles in a TileSet.
/// </summary>
public class TileSprite : SpriteObject
{
    #region Editable fields
    /// <summary>
    /// TileSet to take tile from.
    /// </summary>
    [Bind(BindingScope.Global)]
    public TileSet TileSet;
    #endregion

    #region Serialized data
    [HideInInspector]
    public int Row;
    [HideInInspector]
    public int Column;
    #endregion

    #region Derived properties
    public TilePosition TilePosition
    {
        get
        {
            return new TilePosition(this.Column, this.Row);
        }
    }

    /// <summary>
    /// Tile for the Sprite
    /// If this Tile has a width or height of greater than 1, the TileSprite will draw the other tiles inside of this "tile".
    /// </summary>
    public Tile Tile
    {
        get
        {
            if (mTile == null)
            {
                mTile = TileSet[TilePosition];
            }
            return mTile;
        }

        set
        {
            mTile = value;
            this.Row = mTile.Row;
            this.Column = mTile.Column;
        }
    }
    private Tile mTile;



    public override int Width
    {
        get { return Tile.Width * TileSet.TileWidth; }
    }

    public override int Height
    {
        get { return Tile.Height * TileSet.TileHeight; }
    }

    public override int SnapToGridX
    {
        get
        {
            return TileSet.TileWidth;
        }
    }

    public override int SnapToGridY
    {
        get
        {
            return TileSet.TileHeight;
        }
    }
    #endregion

    #region Initialization
    /// <summary>
    /// Assume TileSprites are static unless explicitly set otherwise.
    /// You can't call this yourself; unity calls it.
    /// </summary>
    public TileSprite()
    {
        IsStatic = true;
    }
    #endregion

    /// <summary>
    /// Draw the sprite.
    /// </summary>
    public override void Render()
    {
        this.TileSet.DrawMultiTile(
            Tile.Position,
            Tile.Width,
            Tile.Height,
            BoundingBox);
    }
}
