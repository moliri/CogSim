using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(TileMap))]
public class TileMapEditor : SpriteEditor
{
    private TileMap tileMap; 

    private readonly TileSetOverlay selectionOverlay = new TileSetOverlay(new Color(0, 0.7f, 0, 0.3f));

    protected override void OnEnable()
    {
        this.AllowDrag = false;
        tileMap = (TileMap)target;
        base.OnEnable();
    }

    protected override void AddOverlays()
    {
        tileMap.AddOverlay(selectionOverlay);
    }

    protected override void RemoveOverlays()
    {
        tileMap.RemoveOverlay(selectionOverlay);
    }

    public override void OnSceneGUI()
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        switch (Event.current.type)
        {
            case EventType.MouseDown:
            case EventType.MouseDrag:
                this.HandleMouse(Event.current.type);
                break;

            case EventType.ValidateCommand:
            case EventType.ExecuteCommand:
                this.HandleCommand(Event.current.commandName, Event.current.type == EventType.ExecuteCommand);
                break;

            case EventType.KeyDown:
                this.HandleKeyDown();
                break;

            default:
                base.OnSceneGUI();
                break;
        }
    }

    private void HandleKeyDown()
    {
        switch (Event.current.keyCode)
        {
            case KeyCode.Return:
                this.PaintSelection();
                Event.current.Use();
                break;

            case KeyCode.Escape:
                Selection = TileRect.Empty;
                break;

            default:
                base.OnSceneGUI();
                break;
        }
    }

    private void PaintSelection()
    {
        if (tileMap.TileSet.SelectedTile != null)
            tileMap.SetRegionTiles(Selection, tileMap.TileSet.SelectedTile);
    }

    private static Tile[,] clipboard;
    private void HandleCommand(string commandName, bool execute)
    {
        switch (commandName)
        {
            case "Delete":
                if (execute)
                    tileMap.SetRegionTiles(Selection, (Tile)null);
                Event.current.Use();
                break;

            case "Copy":
                if (execute)
                    clipboard = tileMap.GetRegionTiles(Selection);
                Event.current.Use();
                break;

            case "Paste":
                if (execute && Selection.IsSingleton)
                    tileMap.SetRegionTiles(new TileRect(Selection.CMin, Selection.RMin, clipboard.GetLength(0), clipboard.GetLength(1)), clipboard);
                Event.current.Use();
                break;

            case "Cut":
                if (execute)
                {
                    clipboard = tileMap.GetRegionTiles(Selection);
                    tileMap.SetRegionTiles(Selection, (Tile)null);
                }
                Event.current.Use();
                break;

            case "SelectAll":
                if (execute)
                    Selection = new TileRect(0, 0, tileMap.MapColumns, tileMap.MapRows);
                Event.current.Use();
                break;

            default:
                base.OnSceneGUI();
                break;
        }
    }

    private TilePosition startDrag;
    private TilePosition lastPosition = new TilePosition(-1, -1);
    private bool mouseOverMap;
    private TileRect mSelection;

    TileRect Selection
    {
        get
        {
            return this.mSelection;
        }

        set
        {
            this.mSelection = value;
            this.selectionOverlay.SetRect(value);
            this.Repaint();
        }
    }

    private void HandleMouse(EventType type)
    {
        TilePosition tilePosition = this.tileMap.TilePosition(Event.current.mousePosition);
        this.mouseOverMap = tilePosition.Column >= 0 && tilePosition.Column < this.tileMap.MapColumns && tilePosition.Row >= 0
               && tilePosition.Row < this.tileMap.MapRows;

        if (!this.mouseOverMap || UnityEditor.Selection.activeObject != tileMap.gameObject)
            base.OnSceneGUI();

        switch (type)
        {
            case EventType.MouseDown:
                var selection = Sprite.Pick(Event.current.mousePosition);
                if (selection != null && selection != tileMap)
                    UnityEditor.Selection.activeGameObject = selection.gameObject;
                else
                {
                    startDrag = tilePosition;
                    this.Selection = new TileRect(startDrag, startDrag);
                }
                Event.current.Use();
                break;


            case EventType.MouseDrag:
                if (tilePosition != lastPosition)
                    this.Selection = new TileRect(startDrag, tilePosition);
                Event.current.Use();
                break;

            default:
                base.OnSceneGUI();
                break;
        }

        lastPosition = tilePosition;
    }

}
