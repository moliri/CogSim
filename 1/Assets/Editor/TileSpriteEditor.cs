using UnityEditor;

[CustomEditor(typeof(TileSprite))]
// ReSharper disable CheckNamespace
public class TileSpriteEditor : SpriteObjectEditor
{
    private TileSprite tileSprite;

    public void Awake()
    {
        tileSprite = (TileSprite)target;
    }

    public override void OnInspectorGUI()
    {
        if (tileSprite.TileSet == null)
            tileSprite.TileSet = (TileSet)FindObjectOfType(typeof(TileSet));
        this.DrawDefaultInspector();
        this.TileSelectionPopup();
    }

    private string[] tileNames;
    void TileSelectionPopup()
    {
        var tileSet = tileSprite.TileSet;
        if (tileNames == null || tileNames.Length != tileSet.Tiles.Count)
            tileNames = new string[tileSet.Tiles.Count];
        for (int i = 0; i < tileNames.Length; i++)
        {
            if (tileSet.Tiles[i].Width == 0)
            {
                tileSet.Tiles[i].Width = 1;
                tileSet.Tiles[i].Height = 1;
            }
            tileNames[i] = tileSet.Tiles[i].Name;
        }
        var selection = EditorGUILayout.Popup("Select tile: ", -1, tileNames);
        if (selection != -1)
        {
            tileSprite.Tile = tileSet.Tiles[selection];
            tileSprite.TileType = tileSprite.Tile.Type;
            this.Repaint();
        }
    }
}