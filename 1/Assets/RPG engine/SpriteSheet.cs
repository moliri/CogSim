using UnityEngine;

/// <summary>
/// A tiled collection of sprites in a single texture.
/// </summary>
public class SpriteSheet : MonoBehaviour
{
    #region Fields and properties
    /// <summary>
    /// The underlying texture containing the tiles.
    /// </summary>
    public Texture2D Texture;

    /// <summary>
    /// The width of a single tile, in pixels
    /// </summary>
    public int TileWidth
    {
        get
        {
            if (Texture == null || Columns == 0)
                return 0;
            return this.Texture.width / Columns;
        }
    }

    /// <summary>
    /// The height of a single tile, in pixels
    /// </summary>
    public int TileHeight
    {
        get
        {
            if (Texture == null || Rows == 0)
                return 0;
            return this.Texture.height / Rows;
        }
    }

    /// <summary>
    /// Total number of rows of tiles.  This should be the height of the texture divided by TileHeight.
    /// </summary>
    public int Rows = 1;

    /// <summary>
    /// Total number of columns of tiles.  This should be the width of the texture divided by TileWidth.
    /// </summary>
    public int Columns = 1;
    #endregion

    #region Position and rectangle conversions
    /// <summary>
    /// The screen position of a tile at this position, assuming tile position 0,0 is at screen position 0,0.
    /// </summary>
    /// <param name="p">Tile position</param>
    /// <returns>The screen rect in which to draw this tile.</returns>
    public Rect TileScreenRect(TilePosition p)
    {
        return new Rect(p.Column * TileWidth, p.Row * TileHeight, TileWidth, TileHeight);
    }

    /// <summary>
    /// The rect in UV coordinates of this tile within the texture.
    /// </summary>
    /// <param name="p">Tile position (row and column) within the tileset</param>
    /// <returns>A rect in UV coordinates within the texture.</returns>
    public Rect TileTextureRect(TilePosition p)
    {
        var uvRow = Rows - (1 + p.Row);
        return new Rect(
            (p.Column * TileWidth)/(float)Texture.width, 
            (uvRow * TileHeight)/(float)Texture.height,
            TileWidth/(float)Texture.width,
            TileHeight/(float)Texture.height);
    }

    /// <summary>
    /// The rect in UV coordinates of within the texture of a multi-tile group.
    /// </summary>
    /// <param name="p">Tile position (row and column) within the tileset of the upper-left corner of the group.</param>
    /// <param name="width">Number of tiles wide the group is.</param>
    /// <param name="height">Number of rows tall the group is.</param>
    /// <returns>A rect in UV coordinates within the texture.</returns>
    public Rect MultiTileTextureRect(TilePosition p, int width, int height)
    {
        var uvRow = Rows - (height + p.Row);
        return new Rect(
            (p.Column * TileWidth) / (float)Texture.width,
            (uvRow * TileHeight) / (float)Texture.height,
            width*TileWidth / (float)Texture.width,
            height*TileHeight / (float)Texture.height);
    }

    /// <summary>
    /// TilePosition on the map (row, column) of a given screen location, in pixels.
    /// </summary>
    /// <param name="v">Screen position to get the tile for.</param>
    /// <param name="upperLeft">Screen coordinates of the upper-left corner of tile 0,0.</param>
    /// <returns>tile position (row and column)</returns>
    public TilePosition TilePosition(Vector2 v, Vector2 upperLeft)
    {
        return new TilePosition(
            ((int)(v.x-upperLeft.x)) / TileWidth,
            ((int)(v.y-upperLeft.y)) / TileHeight);
    }
    #endregion

    #region Drawing
    /// <summary>
    /// Draws a tile on the screen.  Don't call this.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="destination"></param>
    public void Draw(TilePosition tile, Rect destination)
    {
        Graphics.DrawTexture(destination, Texture, this.TileTextureRect(tile), 0, 0, 0, 0); //TileWidth, 0, TileHeight, 0);
        //Graphics.DrawTexture(destination, Texture, new Rect(32, 64, 32, 64), 0, 0, 0, 0);
    }

    /// <summary>
    /// Draws a tile on the screen.  Don't call this.
    /// </summary>
    /// <param name="tile"></param>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <param name="destination"></param>
    public void DrawMultiTile(TilePosition tile, int width, int height, Rect destination)
    {
        Graphics.DrawTexture(destination, Texture, this.MultiTileTextureRect(tile, width, height), 0, 0, 0, 0); //TileWidth, 0, TileHeight, 0);
        //Graphics.DrawTexture(destination, Texture, new Rect(32, 64, 32, 64), 0, 0, 0, 0);
    }
    #endregion
}