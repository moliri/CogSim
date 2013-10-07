using UnityEngine;

/// <summary>
/// Steers a CharacterSprite based on keyboard commands (arrow keys)
/// </summary>
public class SpriteKeyboardController : MonoBehaviour
{
    /// <summary>
    /// The CharacterSprite to steer
    /// </summary>
    public CharacterSprite Sprite;

    /// <summary>
    /// The speed at which to move
    /// </summary>
    public float Speed = 50;

    internal void Start()
    {
        if (Sprite == null)
            Sprite = this.GetComponent<CharacterSprite>();
    }

    internal void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
            Sprite.Velocity = -this.Speed * Vector2.up;
        else if (Input.GetKey(KeyCode.DownArrow))
            Sprite.Velocity = this.Speed * Vector2.up;
        else if (Input.GetKey(KeyCode.RightArrow))
            Sprite.Velocity = this.Speed * Vector2.right;
        else if (Input.GetKey(KeyCode.LeftArrow))
            Sprite.Velocity = -this.Speed * Vector2.right;
        else
            Sprite.Velocity = Vector2.zero;
    }
}
