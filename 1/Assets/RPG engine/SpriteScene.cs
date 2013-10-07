using System;
using System.Collections.Generic;

using UnityEngine;

/// <summary>
/// Displays all the sprites in the scene.
/// One of these components must exist in any scene that contains sprites.
/// </summary>
[ExecuteInEditMode]
public class SpriteScene : BindingBehaviour
{
    [Bind(BindingScope.Global)]
// ReSharper disable FieldCanBeMadeReadOnly.Local
    private List<Sprite> allSprites=null;
// ReSharper restore FieldCanBeMadeReadOnly.Local
 
    /// <summary>
    /// Shader with which to draw the scene.
    /// The standard unity "GUI/Text Shader" shader works.
    /// </summary>
    public Material Shader;

    /// <summary>
    /// Fills in Shader, if not already set.
    /// </summary>
    public override void Awake()
    {
		base.Awake();
        if (Shader == null)
        {
            Shader = new Material(UnityEngine.Shader.Find("GUI/Text Shader"));
        }
    }

    /// <summary>
    /// Calls thunk with the world matrix set to map directly to screen coordinates, with Z ignored.
    /// </summary>
    /// <param name="thunk">Drawing procedure to call.</param>
    public static void WithPixelMatrix(Action thunk)
    {
        GL.PushMatrix();
        GL.LoadPixelMatrix(0, Screen.width, Screen.height, 0);
        thunk();
        GL.PopMatrix();
    }

    /// <summary>
    /// Draws all the sprites in the scene, as determined by Sprite.AllSprites.
    /// </summary>
    public void OnRenderObject()
    {
        this.SortSprites();
        WithPixelMatrix(
            () =>
                {
                    currentShader = Shader;
                    Shader.SetPass(0);
                    foreach (var sprite in allSprites)
                        sprite.Render();
                    Shader.SetPass(0);
                    foreach (var sprite in allSprites)
                        sprite.DebugDraw();
                });
    }

    /// <summary>
    /// This is needed only so that ResetShader, which is static, knows 
    /// what the Shader of the SpriteScene is while it's OnRenderObject method is running.
    /// </summary>
    private static Material currentShader;

    /// <summary>
    /// Resets the SpriteScene's shader.
    /// 
    /// Seems to be needed when switching from DrawTexture to using the GL calls.
    /// DrawTexture often, but not always, leaves the shader in a state where GL calls to draw
    /// quads will run but not render anything.
    /// </summary>
    public static void ResetShader()
    {
        currentShader.SetPass(0);
    }

    /// <summary>
    /// Sorts the sprites by "depth" for proper back-to-front rendering.
    /// Note that "depth" here really means y coordinate.
    /// </summary>
    private void SortSprites()
    {
#if UNITY_EDITOR
        if (Application.isEditor)
            ForceRebinding();
#endif
		//if (allSprites == null) {
		//	Debug.Log("Rebinding");
		//	ForceRebinding();
		//}
        allSprites.Sort((a, b) => a.Position.y.CompareTo(b.Position.y));
    }
}
