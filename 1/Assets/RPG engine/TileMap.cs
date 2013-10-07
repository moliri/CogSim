using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// A map (background) built out of tiles in a TileSet (a kind of sprite sheet).
/// </summary>
public class TileMap : Sprite
{
    #region Editable fields
    /// <summary>
    /// TileSet from which this map is build.
    /// </summary>
    [Bind(BindingScope.Global)]
    public TileSet TileSet;
    /// <summary>
    /// Number of columns in the map (as opposed to in the TileSet).
    /// </summary>
    public int MapColumns=1;
    /// <summary>
    /// Number of rows in the map (as opposed to in the TileSet).
    /// </summary>
    public int MapRows=1;
    #endregion

    #region Derived properties
    /// <summary>
    /// Number of PIXELS wide the map is
    /// </summary>
    public override int Width
    {
        get { return MapColumns * this.TileSet.TileWidth; }
    }

    /// <summary>
    /// Number of PIXELS high the map is.
    /// </summary>
    public override int Height
    {
        get { return MapRows * this.TileSet.TileHeight; }
    }
    #endregion

    #region Initialization
    [Bind(BindingScope.Global)]
// ReSharper disable FieldCanBeMadeReadOnly.Local
    private List<SpriteObject> allSpriteObjects = null;
// ReSharper restore FieldCanBeMadeReadOnly.Local

    /// <summary>   
    /// Initialize object
    /// </summary>
    public void Start()
    {
        this.BuildContents();
        foreach (var s in this.allSpriteObjects)
        {
            if (s.IsStatic)
                foreach (var p in s.FootprintTiles)
                    contents[p.Column, p.Row].TileType = s.TileType;
        }
    }

    /// <summary>
    /// Build the contents[] array from the serialized data.
    /// </summary>
    private void BuildContents()
    {
        this.contents = new MapSector[this.MapColumns,this.MapRows];
        for (int i = 0; i < this.MapColumns; i++)
        {
            for (int j = 0; j < this.MapRows; j++)
            {
                var sector = this.SerializationInfo(i, j);
                this.contents[i, j] = new MapSector(this.TileSet[sector.TilePosition]);
            }
        }
    }

#if UNITY_EDITOR
    /// <summary>
    /// Kluge to deal with the fact that the editor gives you no way of insuring consistent update of
    /// one variable when another is edited by the user.
    /// </summary>
    void FixSectorMap()
    {
        if (MapRows == 0)
            return;   // This almost certainly means they're editing the number and it's only temporarily zero
        if (this.sectorsSerializationInfo.Length != MapRows * MapColumns)
        {
            this.sectorsSerializationInfo = new MapSectorSerializationInfo[MapRows * MapColumns];
            contents = null;
        }
        if (contents == null)
            BuildContents();
    }

    //static void SetArraySize<T>(ref T[] array, int size, Func<int, T> defaultGenerator)
    //{
    //    if (size == array.Length)
    //        return;
    //    var newArray = new T[size];

    //    if (size < array.Length)
    //    {
    //        Array.Copy(array, newArray, size);
    //    }
    //    else
    //    {
    //        Array.Copy(array, newArray, array.Length);
    //        for (int i = array.Length; i < newArray.Length; i++)
    //            newArray[i] = defaultGenerator(i);
    //    }
    //    array = newArray;
    //}
#endif
    #endregion

    #region Contents tracking
    [SerializeField, HideInInspector]
    private MapSectorSerializationInfo[] sectorsSerializationInfo = new MapSectorSerializationInfo[0];
    private MapSector[,] contents;

    internal MapSectorSerializationInfo SerializationInfo(int column, int row)
    {
        return this.sectorsSerializationInfo[row * this.MapColumns + column];
    }

    internal void SetSerializationInfo(TilePosition p, MapSectorSerializationInfo value)
    {
#if UNITY_EDITOR
        this.FixSectorMap();
#endif
        //sectorsSerializationInfo[p.Row][p.Column] = value;
        this.sectorsSerializationInfo[p.Row * this.MapColumns + p.Column] = value;
    }

    internal MapSectorSerializationInfo SerializationInfo(TilePosition p)
    {
#if UNITY_EDITOR
        this.FixSectorMap();
#endif
        //return sectorsSerializationInfo[p.Row][p.Column];
        return this.sectorsSerializationInfo[p.Row * this.MapColumns + p.Column];
    }

#if UNITY_EDITOR
    public void SetTile(TilePosition p, Tile t)
    {
        contents[p.Column, p.Row].Tile = t;
        if (t == null)
        {
            this.SetSerializationInfo(p, null);
        }
        else
        {
            var sector = this.SerializationInfo(p);
            if (sector != null)
                sector.TilePosition = t.Position;
            {
                sector = new MapSectorSerializationInfo(t.Position);
                this.SetSerializationInfo(p, sector);
            }
        }
    }

    public void SetRegionTiles(TileRect r, Tile tile)
    {
        foreach (var p in r)
            this.SetTile(p, tile);
    }

    public void SetRegionTiles(TileRect r, Tile[,] tiles)
    {
        if (r.Width != tiles.GetLength(0) || r.Height != tiles.GetLength(1))
            throw new ArgumentException("Tile array has different size from tile region");
        foreach (var p in r)
            this.SetTile(p, tiles[p.Column - r.CMin, p.Row - r.RMin]);
    }
#endif

    public Tile[,] GetRegionTiles(TileRect r)
    {
        var tiles = new Tile[r.Width, r.Height];
        foreach (var p in r)
            tiles[p.Column - r.CMin, p.Row - r.RMin] = contents[p.Column, p.Row].Tile;
        return tiles;
    }

    public bool IsFreespace(Vector2 v)
    {
        return IsFreespace(this.TilePosition(v));
    }

    public bool IsFreespace(TilePosition p)
    {
        if (p.Column < 0 || p.Row < 0 || p.Column >= MapColumns || p.Row >= MapRows)
            return false;
        return contents[p.Column, p.Row].TileType == TileType.Freespace;
    }

    public bool IsFreespace(TileRect r)
    {
        foreach (var p in r)
            if (!IsFreespace(p))
                return false;
        return true;
    }

    public bool IsFreespace(Rect r)
    {
        return IsFreespace(this.TileRect(r));
    }

    #endregion

    #region Coordinate transforms of various types
    public TilePosition TilePosition(Vector2 v)
    {
        return this.TileSet.TilePosition(v, this.CenterOffset);
    }

    public TileRect TileRect(Rect r)
    {
        var upperLeft = TilePosition(new Vector2(r.xMin, r.yMin));
        var width = (int)Math.Ceiling((r.xMax - upperLeft.Column * TileSet.TileWidth)/TileSet.TileWidth);
        var height = (int)Math.Ceiling((r.yMax - upperLeft.Row * TileSet.TileHeight)/TileSet.TileHeight);
        return new TileRect(upperLeft.Column, upperLeft.Row, width, height);
    }
    #endregion

    /// <summary>
    /// Draw the map
    /// </summary>
    public override void Render()
    {
        if (TileSet == null)
            return;
#if UNITY_EDITOR
        FixSectorMap();
#endif

        for (int j = 0; j < MapRows; j++)
            for (int i = 0; i < MapColumns; i++)
            {
                Tile t = contents[i, j].Tile;
                if (t != null)
                    this.TileSet.Draw(t.Position, this.TileSet.TileScreenRect(new TilePosition(i, j)));
            }
    }

    public override void DebugDraw()
    {
        foreach (var overlay in overlays)
            overlay.Draw(this);
    }

    #region Map overlays (for debugging visualization)
    /// <summary>
    /// Overlays to display on top of the map.
    /// </summary>
    private readonly HashSet<MapOverlay> overlays = new HashSet<MapOverlay>();

    /// <summary>
    /// Adds an overlay to display on top of the map.
    /// Has no effect if the overlay is already being displayed.
    /// </summary>
    /// <param name="overlay">Overlay to display.</param>
    public void AddOverlay(MapOverlay overlay)
    {
        if (!overlays.Contains(overlay))
            overlays.Add(overlay);
    }

    /// <summary>
    /// Disables display of the overlay.
    /// </summary>
    /// <param name="overlay">Overlay to disable.</param>
    public void RemoveOverlay(MapOverlay overlay)
    {
        overlays.Remove(overlay);
    }

    /// <summary>
    /// Cancels display of all overlays
    /// </summary>
    public void RemoveAllOverlays()
    {
        overlays.Clear();
    }
    #endregion

    /// <summary>
    /// The center of the tile at the specified TilePosition
    /// </summary>
    public Vector2 TileCenter(TilePosition tilePosition)
    {
        return new Vector2(
            tilePosition.Column*TileSet.TileWidth + TileSet.TileWidth*0.5f,
            tilePosition.Row*TileSet.TileHeight + TileSet.TileHeight*0.5f
            );
    }
}
