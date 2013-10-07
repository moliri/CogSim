using System;

using UnityEditor;
using UnityEngine;

// ReSharper disable CheckNamespace
[CustomEditor(typeof(Sprite))]
public class SpriteEditor : Editor
{
    private Sprite sprite;

    protected virtual void OnEnable()
    {
        sprite = (Sprite)target;
        this.AddOverlays();
    }

    protected virtual void OnDisable()
    {
        this.RemoveOverlays();
    }

    protected virtual void AddOverlays()
    {
    }

    protected virtual void RemoveOverlays()
    {
    }

    protected bool AllowDrag = true;
    public virtual void OnSceneGUI()
    {
        HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

        switch (Event.current.type)
        {
            case EventType.MouseDown:
                if (sprite.BoundingBox.Contains(Event.current.mousePosition))
                    Event.current.Use();
                else
                {
                    var selection = Sprite.Pick(Event.current.mousePosition);
                    //Debug.Log("Got "+selection==null?"null":selection.ToString());
                    if (selection != null)
                       Selection.activeGameObject = selection.gameObject;
                }
                break;

            case EventType.MouseUp:
                var pos = sprite.transform.position;
                pos.x = Snap(pos.x, sprite.SnapToGridX);
                pos.y = Snap(pos.y, sprite.SnapToGridY);
                sprite.transform.position = pos;
                Event.current.Use();
                break;

            case EventType.MouseDrag:
                if (this.AllowDrag)
                {
                    sprite.transform.position = sprite.transform.position
                                                + new Vector3(Event.current.delta.x, Event.current.delta.y, 0);
                    Event.current.Use();
                }
                break;

            case EventType.Repaint:
                SpriteScene.WithPixelMatrix(
                    () =>
                        {
                            SpriteScene.ResetShader();
                            this.DrawEditorDecorations();
                        });
                break;
        }
    }

    protected virtual void DrawEditorDecorations()
    {
        DrawRect(sprite.BoundingBox, Color.blue);
    }

    protected static void DrawRect(Rect bbox, Color color)
    {
        GL.Begin(GL.LINES);
        GL.Color(color);
        GL.Vertex3(bbox.xMin, bbox.yMin, 0); GL.Vertex3(bbox.xMax, bbox.yMin, 0);
        GL.Vertex3(bbox.xMin, bbox.yMax, 0); GL.Vertex3(bbox.xMax, bbox.yMax, 0);
        GL.Vertex3(bbox.xMin, bbox.yMax, 0); GL.Vertex3(bbox.xMin, bbox.yMin, 0);
        GL.Vertex3(bbox.xMax, bbox.yMax, 0); GL.Vertex3(bbox.xMax, bbox.yMin, 0);
        GL.End();
    }

    private float Snap(float p1, int p2)
    {
        return p2 * (float)Math.Round(p1 / p2);
    }
}
// ReSharper restore CheckNamespace