using System;

/// <summary>
/// Stores data about a single tile in a TileSet.
/// </summary>
[Serializable]
public class Tile
{
    public Tile(string name, TilePosition position)
    {
        Name = name;
        Row = position.Row;
        Column = position.Column;
    }

    public string Name;

    /// <summary>
    /// The type of tile this is (freespace, wall, etc.)
    /// </summary>
    public TileType Type = TileType.Freespace;

    /// <summary>
    /// For "tiles" that are really rectangular groups of tiles: the number of tiles wide this group is.
    /// </summary>
    public int Width = 1;

    /// <summary>
    /// For "tiles" that are really rectangular groups of tiles: the number of tiles wide this group is.
    /// </summary>
    public int Height = 1;

    // NOTE: Row and Column have to be stored as separate ints because Unity can't serialize a TilePosition.

    /// <summary>
    /// Row of the TileSet containing this tile.
    /// </summary>
    public int Row;

    /// <summary>
    /// Column of the TileSet containing this tile.
    /// </summary>
    public int Column;

    /// <summary>
    /// The location of this tile in its TileSheet.
    /// </summary>
    public TilePosition Position
    {
        get
        {
            return new TilePosition(Column, Row);
        }
    }
}