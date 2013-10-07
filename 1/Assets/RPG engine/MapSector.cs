using System;

/// <summary>
/// Represents information about what is stored at a particular location in the map.
/// </summary>
public class MapSector
{
    public MapSector(Tile t)
    {
        Tile = t;
        TileType = (t == null) ? TileType.Freespace : t.Type;
    }

    /// <summary>
    /// The tile to be displayed at this location in the map
    /// </summary>
    public Tile Tile;

    /// <summary>
    /// The type (freespace, wall, etc) that appears here.
    /// This may be different than the type of the tile stored here because there may be a
    /// SpriteObject sitting on top of the tile.  If that SpriteObject is static
    /// (IsStatic==true), then TileType here will get set to the type of that object.
    /// </summary>
    public TileType TileType;
}

/// <summary>
/// Represents all the information about a tile of a tilemap that can be changed in the editor.
/// Currently, this is just the choice of tile.
/// 
/// Not exposed as part of the API.  Don't mess with this stuff.
/// 
/// Editorial note: this shouldn't exist.  It exists only because Unity's serialization system
/// contains completely pointless limitations.
/// </summary>
[Serializable]
internal class MapSectorSerializationInfo
{
    public int Row;

    public int Column;

    public TilePosition TilePosition
    {
        get
        {
            return new TilePosition(Column, Row);
        }

        set
        {
            Row = value.Row;
            Column = value.Column;
        }
    }

    public MapSectorSerializationInfo(TilePosition position)
    {
        TilePosition = position;
    }
}