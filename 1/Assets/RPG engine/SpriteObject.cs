using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// A sprite that represents an object in the world,
/// as opposed to the map or something intangible.
/// </summary>
public abstract class SpriteObject: Sprite
{
    [Bind(BindingScope.Global)]
    public TileMap TileMap;

    [Bind(BindingScope.Global)]
    protected SpriteScene SpriteScene;

    /// <summary>
    /// The docking area of this object relative to its Position
    /// </summary>
    public Rect DockingRect;

    /// <summary>
    /// The docking area of this object in screen coordinates.
    /// </summary>
    public Rect ScreenDockingRect
    {
        get
        {
            return new Rect(
                this.DockingRect.xMin + Position.x,
                Position.y - this.DockingRect.yMin,
                this.DockingRect.width,
                this.DockingRect.height);
        }
    }

    /// <summary>
    /// The tiles corresponding to the docking area.
    /// </summary>
    public TileRect DockingTiles
    {
        get
        {
            return TileMap.TileRect(this.ScreenDockingRect);
        }
    }

    /// <summary>
    /// Screen rectangle, relative to CenterOffset, that the object is considered to be "resting on".
    /// Any map tiles overlapped by this region will be treated as occupied by the object.
    /// </summary>
    public Rect Footprint;

    /// <summary>
    /// If true, this object never moves.
    /// </summary>
    public bool IsStatic;

    /// <summary>
    /// The tile type this object corresponds to.
    /// </summary>
    public TileType TileType = TileType.Freespace;

    /// <summary>
    /// Area of the screen that object is "standing on"
    /// </summary>
    public Rect ScreenFootPrint
    {
        get
        {
            return new Rect(Position.x + Footprint.xMin, Position.y - Footprint.yMin, Footprint.width, Footprint.height);
        }
    }

    public TileRect FootprintTiles
    {
        get
        {
            return TileMap.TileRect(ScreenFootPrint);
        }
    }

    /// <summary>
    /// True of the object is currently colliding with some other object
    /// </summary>
    public bool CheckIsColliding()
    {
            return this.IsCollidingWithDynamicObject || !this.TileMap.IsFreespace(this.ScreenFootPrint);
    }


    /// <summary>
    /// True if this SpriteObject's ScreenFootprint is currently overlapping some other dynamic SpriteObject's ScreenFootprint.
    /// </summary>
    public bool IsCollidingWithDynamicObject
    {
        get
        {
            var rect = this.ScreenFootPrint;
            this.DynamicObstacle = null;
            foreach (var spriteObject in (List<SpriteObject>)Registry(typeof(SpriteObject)))
            {
                if (spriteObject != this && !spriteObject.IsStatic && rect.Overlaps(spriteObject.ScreenFootPrint))
                {
                    //Debug.Log("Collides with " + spriteObject.gameObject.name);
                    this.DynamicObstacle = spriteObject;
                    return true;
                }
            }
            return false;
        }
    }

    /// <summary>
    /// True if there are any dynamic obstacles in Rect, other than this object
    /// </summary>
    public bool ContainsDymanicObstacle(Rect rect)
    {
        foreach (var spriteObject in (List<SpriteObject>)Registry(typeof(SpriteObject)))
        {
            if (spriteObject != this && !spriteObject.IsStatic && rect.Overlaps(spriteObject.ScreenFootPrint))
            {
                return true;
            }
        }
        return false;
    }

    /// <summary>
    /// Finds all objects other than this object in the specified Rect.
    /// It's much better style to use the DynamicObstacles iterator, but that generates garbage,
    /// whereas FindObjects writes to a buffer that you supply
    /// </summary>
    /// <param name="rect">Area to search in screen coordinates</param>
    /// <param name="output">A list that will be overwritten with objects in the area.</param>
    /// <param name="includeStatic">True if objects marked IsStatic should be included.</param>
    public void FindObjects(Rect rect, bool includeStatic, List<SpriteObject> output)
    {
        output.Clear();
        foreach (var spriteObject in (List<SpriteObject>)Registry(typeof(SpriteObject)))
            if (spriteObject != this && (includeStatic || !spriteObject.IsStatic) && rect.Overlaps(spriteObject.ScreenFootPrint))
                output.Add(spriteObject);
    }

    /// <summary>
    /// Returns all the dynamic objects in the specified rectangle, except for this object itself.
    /// </summary>
    /// <param name="rect">The rectangle to search</param>
    /// <returns>An iterator for all the objects in the rectangle</returns>
    public IEnumerator<SpriteObject> DynamicObstacles(Rect rect)
    {
        foreach (var spriteObject in (List<SpriteObject>)Registry(typeof(SpriteObject)))
        {
            if (spriteObject != this && !spriteObject.IsStatic && rect.Overlaps(spriteObject.ScreenFootPrint))
                yield return spriteObject;
        }
    }

    /// <summary>
    /// True if the character was unable to complete its motion on its last update because there was an object blocking its way.
    /// </summary>
    public bool IsBlockedByDynamicObject
    {
        get
        {
            return this.DynamicObstacle != null;
        }
    }

    public bool IsBlocked;

    /// <summary>
    /// The (dynamic) spriteobject currently blocking our path, if any
    /// </summary>
    public SpriteObject DynamicObstacle;

    /// <summary>
    /// Update the position of the sprite
    /// </summary>
    public virtual void Update()
    {
        if (IsStatic)
            return;

        var savedPosition = Position;

        // ReSharper disable CompareOfFloatsByEqualityOperator
        if (Velocity.x != 0 || Velocity.y != 0)   // Fast path for when velocity = 0
        // ReSharper restore CompareOfFloatsByEqualityOperator
        {
            Position = Position + Velocity * Time.deltaTime;
        }

        if (IsBlocked = this.CheckIsColliding())
        {
            Position = savedPosition;
        }
    }
}
