using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TileSet))]
// ReSharper disable CheckNamespace
public class TileSetEditor : Editor
// ReSharper restore CheckNamespace
{
    int row;
    int column;
    private TileSet tileSet;

    internal void OnEnable()
    {
        tileSet = (TileSet)target;
    }

    private const EventModifiers ControlAlt = EventModifiers.Control | EventModifiers.Alt;
    public override void OnInspectorGUI()
    {
        switch (Event.current.type)
        {
            case EventType.keyDown:
                this.HandleKeyDown();
                break;
        }

        this.DrawDefaultInspector();
        this.DrawTileInspector();
    }

    private void DrawTileInspector()
    {
        GUILayout.Label("Edit tile:");

        var tilePosition = new TilePosition(this.column, this.row);

        this.row = this.row % this.tileSet.Rows;
        this.column = this.column % this.tileSet.Columns;

        var tile = this.tileSet[tilePosition];
        this.tileSet.SelectedTile = tile;

        int width = tile==null?1:tile.Width;
        int height = tile==null?1:tile.Height;
        this.tileSet.DrawMultiTile(
            tilePosition,
            width,
            height,
            GUILayoutUtility.GetRect(3 * width * this.tileSet.TileWidth, 3 * height * this.tileSet.TileHeight, GUILayout.ExpandWidth(false)));

        this.TileSelectionPopup();

        this.row = EditorGUILayout.IntField("Row", this.row);
        this.column = EditorGUILayout.IntField("Column", this.column);

        if (tile == null)
        {
            if (GUILayout.Button("Define attributes"))
            {
                this.tileSet.CreateTile(tilePosition);
            }
        }
        else
        {
            var tname = EditorGUILayout.TextField("Name", tile.Name);
            if (tname != tile.Name)
            {
                tile.Name = tname;
                this.tileSet.Tiles.Sort((a, b) => System.String.CompareOrdinal(a.Name, b.Name));
            }
            tile.Type = (TileType)EditorGUILayout.EnumPopup("Type", tile.Type);
            tile.Width = EditorGUILayout.IntField("Width", width);
            tile.Height = EditorGUILayout.IntField("Height", tile.Height);
        }
    }

    private void HandleKeyDown()
    {
        var changedRowColumn = false;
        if ((Event.current.modifiers & ControlAlt) == ControlAlt)
        {
            switch (Event.current.keyCode)
            {
                case KeyCode.UpArrow:
                    if (this.row > 0)
                    {
                        this.row -= 1;
                    }
                    changedRowColumn = true;
                    break;

                case KeyCode.DownArrow:
                    if (this.row < this.tileSet.Rows - 1)
                    {
                        this.row += 1;
                    }
                    changedRowColumn = true;
                    break;

                case KeyCode.LeftArrow:
                    if (this.column > 0)
                    {
                        this.column -= 1;
                    }
                    changedRowColumn = true;
                    break;

                case KeyCode.RightArrow:
                    if (this.column < this.tileSet.Columns - 1)
                    {
                        this.column += 1;
                    }
                    changedRowColumn = true;
                    break;
            }

            this.row = this.row % this.tileSet.Rows;
            this.column = this.column % this.tileSet.Columns;

            if (changedRowColumn)
            {
                this.Repaint();
                Event.current.Use();
            }
        }
    }

    private string[] tileNames;

    void TileSelectionPopup()
    {
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
        var selection = EditorGUILayout.Popup("Go to tile", -1, tileNames);
        if (selection != -1)
        {
            var tile = tileSet.Tiles[selection];
            row = tile.Position.Row;
            column = tile.Position.Column;
            this.Repaint();
        }
    }
}
