﻿using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Specifies a rectangular region of tiles.
/// </summary>
public struct TileRect : IEnumerable<TilePosition>
{
    static TileRect()
    {
        Empty = new TileRect(0, 0, -1, -1);
    }

    /// <summary>
    /// The minimum/maximum row/column of the rectangle
    /// </summary>
    public readonly int CMin, CMax, RMin, RMax;
    /// <summary>
    /// A TileRect containing no tiles.
    /// </summary>
    public static TileRect Empty;

    /// <summary>
    /// Makes a new TileRect given its upper left hand corner and its width and height.
    /// A width or height of zero means the rect is empty.
    /// A width and height of 1 mean the rect is a single tile.
    /// </summary>
    /// <param name="cMin">Column of left edge of the rect.</param>
    /// <param name="rMin">Row of the upper edge of the rect</param>
    /// <param name="width">Width of the rect (1=single tile wide, 0=empty rect)</param>
    /// <param name="height">Height of the rect (1=single tile wide, 0=empty rect)</param>
    public TileRect(int cMin, int rMin, int width, int height)
    {
        this.CMin = cMin;
        this.RMin = rMin;
        this.CMax = cMin + width - 1;
        this.RMax = rMin + height - 1;
    }

    /// <summary>
    /// Returns the smallest TileRect that includes both tile positions.
    /// </summary>
    /// <param name="p1">A tile position to include.</param>
    /// <param name="p2">Another tile position to include.</param>
    public TileRect(TilePosition p1, TilePosition p2)
    {
        CMin = Math.Min(p1.Column, p2.Column);
        CMax = Math.Max(p1.Column, p2.Column);
        RMin = Math.Min(p1.Row, p2.Row);
        RMax = Math.Max(p1.Row, p2.Row);
    }

    /// <summary>
    /// Returns a TileRect consisting of only the specified tile
    /// </summary>
    /// <param name="p">Tile to include</param>
    public TileRect(TilePosition p)
        : this(p, p)
    { }

    /// <summary>
    /// Number of tiles wide the rect is
    /// </summary>
    public int Width
    {
        get
        {
            return 1 + this.CMax - this.CMin;
        }
    }

    /// <summary>
    /// Number of tiles high the rect is.
    /// </summary>
    public int Height
    {
        get
        {
            return 1 + this.RMax - this.RMin;
        }
    }

    /// <summary>
    /// True if this is only a single tile.
    /// </summary>
    public bool IsSingleton
    {
        get
        {
            return Width == 1 && Height == 1;
        }
    }

    /// <summary>
    /// True if this rect contains no tiles at all.
    /// </summary>
    public bool IsEmpty
    {
        get
        {
            return CMin > CMax || RMin > RMax;
        }
    }

    /// <summary>
    /// All the tile positions contained in the rect
    /// </summary>
    /// <returns>Tile positions</returns>
    IEnumerator<TilePosition> IEnumerable<TilePosition>.GetEnumerator()
    {
        for (int y = this.RMin; y <= this.RMax; y++)
            for (int x = this.CMin; x <= this.CMax; x++)
                yield return new TilePosition(x, y);
    }

    /// <summary>
    /// All the tile positions contained in the rect
    /// </summary>
    /// <returns>Tile positions</returns>
    IEnumerator IEnumerable.GetEnumerator()
    {
        for (int y = this.RMin; y <= this.RMax; y++)
            for (int x = this.CMin; x <= this.CMax; x++)
                yield return new TilePosition(x, y);
    }

    internal bool Contains(TilePosition tilePosition)
    {
        return CMin <= tilePosition.Column && tilePosition.Column <= CMax
            && RMin <= tilePosition.Row && tilePosition.Row <= RMax;
    }

    /// <summary>
    /// Returns the cardinal direction from tilePosition that faces this Rect, if there is one, else zero.
    /// </summary>
    public Vector2 FacingDirection(TilePosition tilePosition)
    {
        if (this.Contains(tilePosition))
            return Vector2.zero;

        if (RMin <= tilePosition.Row && tilePosition.Row <= RMax)
            return new Vector2(Math.Sign(CMin - tilePosition.Column), 0);
            
        if (CMin <= tilePosition.Column && tilePosition.Column <= CMax)
            return new Vector2(0, Math.Sign(RMin - tilePosition.Row));

        return Vector2.zero;
    }
}