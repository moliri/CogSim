using UnityEngine;

public class SpriteObjectEditor : SpriteEditor
{
    private SpriteObject spriteObject;
    private TileMap map;

    private readonly TileSetOverlay tileFootprintOverlay = new TileSetOverlay(new Color(1, 1, 0, 0.3f));

    protected override void OnEnable()
    {
        spriteObject = (SpriteObject)target;
        map = (TileMap)FindObjectOfType(typeof(TileMap));
        base.OnEnable();
        this.AddOverlays();
    }

    protected override void AddOverlays()
    {
        map.AddOverlay(tileFootprintOverlay);
    }

    protected override void RemoveOverlays()
    {
        map.RemoveOverlay(tileFootprintOverlay);
    }

    protected override void DrawEditorDecorations()
    {
        base.DrawEditorDecorations();
        // ReSharper disable CompareOfFloatsByEqualityOperator
        if (spriteObject.ScreenFootPrint.width != 0)
            DrawRect(spriteObject.ScreenFootPrint, Color.red);
        if (spriteObject.DockingRect.width != 0)
            DrawRect(spriteObject.ScreenDockingRect, Color.green);
        // ReSharper restore CompareOfFloatsByEqualityOperator
    }

    public override void OnSceneGUI()
    {
        if (spriteObject.TileMap != null)  // When transitioning from Playing to not playing, this can get called before the spriteObject has been awakened.
            tileFootprintOverlay.Set(spriteObject.FootprintTiles);
        base.OnSceneGUI();
    }
}

