using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// A SpriteSheet used to hold images of different objects or building blocks, rather than animations.
/// </summary>
public class TileSet : SpriteSheet
{
    [HideInInspector]
    public List<Tile> Tiles = new List<Tile>();

    /// <summary>
    /// Gets the information about the tile at the specified position.
    /// </summary>
    /// <param name="position">Position of the tile to get data on</param>
    /// <returns>Data about the specified tile.</returns>
    public Tile this[TilePosition position]
    {
        get
        {
            return this.Tiles.Find(t => t.Position == position);
        }
    }

#if UNITY_EDITOR
    public Tile CreateTile(TilePosition position)
    {
        var tile = this[position];
        if (tile == null)
        {
            tile = new Tile("New Tile", position);
            Tiles.Add(tile);
        }
        return tile;
    }

    [HideInInspector]
    public Tile SelectedTile;
#endif
}


