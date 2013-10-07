﻿using System;
using System.Diagnostics;

/// <summary>
/// Specifies the position of a tile on a map or in a TileSet.
/// Positions are row and column with the top row being row 0.
/// </summary>
[Serializable, DebuggerDisplay("({Column}, {Row})")]
public struct TilePosition
{
    /// <summary>
    /// The column of the map at which this tile appears.  Leftmost column=0;
    /// </summary>
    public readonly int Column;
    /// <summary>
    /// The row of the map at which this tile appears.  Top row = 0;
    /// </summary>
    public readonly int Row;

    /// <summary>
    /// Specifies the position of a tile in a map in terms of its row and column.
    /// </summary>
    /// <param name="column">Column of the tile (0=left)</param>
    /// <param name="row">Row of the tile (0=top)</param>
    public TilePosition(int column, int row)
    {
        this.Column = column;
        this.Row = row;
    }

    /// <summary>
    /// Returns the distance (in tiles, not pixels) between two tiles.
    /// This is Euclidean distance, so the distance may not be an integer.
    /// </summary>
    /// <param name="a">Tile to find the distance from</param>
    /// <param name="b">tile to find the distance to</param>
    /// <returns></returns>
    public static float EuclideanDistance(TilePosition a, TilePosition b)
    {
        int dc = a.Column - b.Column;
        int dr = a.Row - b.Row;
        return (float)Math.Sqrt(dc * dc + dr * dr);
    }

    /// <summary>
    /// Returns the distance from the TilePosition to the closest tile in the TileRect
    /// </summary>
    /// <param name="a">Tile to find the distance from</param>
    /// <param name="r">TileRect to find the distance to</param>
    /// <returns></returns>
    public static float EuclideanDistance(TilePosition a, TileRect r)
    {
        if (r.Contains(a))
            // Inside rect
            return 0;
        if (r.CMin <= a.Column && a.Column <= r.CMax)
            // Closest point is on one of the horiztonal edges
            return Math.Min(Math.Abs(a.Row - r.RMin), Math.Abs(a.Row - r.RMax));
        if (r.RMin <= a.Row && a.Row <= r.RMax)
            // Closest point is on one of the vertical edges
            return Math.Min(Math.Abs(a.Column - r.CMin), Math.Abs(a.Column - r.CMax));
        // Closest point is one of the corners.
        return
            Math.Min(
                Math.Min(
                    EuclideanDistance(a, new TilePosition(r.CMin, r.RMin)),
                    EuclideanDistance(a, new TilePosition(r.CMin, r.RMax))),
                Math.Min(
                    EuclideanDistance(a, new TilePosition(r.CMax, r.RMin)),
                    EuclideanDistance(a, new TilePosition(r.CMax, r.RMax))));
    }

    /// <summary>
    /// The tile above this one
    /// </summary>
    public TilePosition Up
    {
        get
        {
            return new TilePosition(Column, Row-1);
        }
    }

    /// <summary>
    /// The tile below this one
    /// </summary>
    public TilePosition Down
    {
        get
        {
            return new TilePosition(Column, Row + 1);
        }
    }

    /// <summary>
    /// The tile to the left of this one
    /// </summary>
    public TilePosition Left
    {
        get
        {
            return new TilePosition(Column - 1, Row);
        }
    }

    /// <summary>
    /// The tile to the right of this one
    /// </summary>
    public TilePosition Right
    {
        get
        {
            return new TilePosition(Column + 1, Row);
        }
    }

    #region Standard comparision operators
    public override bool Equals(object obj)
    {
        if (obj is TilePosition)
        {
            var p = (TilePosition)obj;
            return p.Column == this.Column && p.Row == this.Row;
        }
        return false;
    }
    public override int GetHashCode()
    {
        return this.Column.GetHashCode() ^ this.Row.GetHashCode();
    }
    public static bool operator ==(TilePosition a, TilePosition b)
    {
        return a.Column == b.Column && a.Row == b.Row;
    }
    public static bool operator !=(TilePosition a, TilePosition b)
    {
        return a.Column != b.Column || a.Row != b.Row;
    }
    #endregion

    public override string ToString()
    {
        return string.Format("({0}, {1})", Column, Row);
    }
}