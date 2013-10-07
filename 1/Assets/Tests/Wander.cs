using System.Collections;
using System.Collections.Generic;

public class Wander : BindingBehaviour
{
    public string Destination;
#pragma warning disable 649
    [Bind]
    private CharacterSprite sprite;

    [Bind]
    private CharacterSteeringController steering;

    [Bind(BindingScope.Global, BindingDefault.Create)]
    private PathPlanner planner;

    [Bind(BindingScope.Global)]
    private TileMap map;

    [Bind(BindingScope.Global)]
    private List<SpriteObject> allObjects;
#pragma warning restore 649

    readonly System.Random random = new System.Random();

    public void Start()
    {
        StartCoroutine(this.WanderBetweenObjectsLoop());
    }

    private IEnumerator WanderBetweenObjectsLoop()
    {
        yield return null;
        while (true)
        {
            var destIndex = this.random.Next(this.allObjects.Count);
            var destObject = this.allObjects[destIndex];
            this.Destination = destObject.gameObject.name;
            yield return
                this.StartCoroutine(
                    this.planner.Plan(
                        this.map.TilePosition(this.sprite.Position),
                        this.map.TileRect(destObject.ScreenDockingRect))
                           .FollowPath(this.steering));
        }
// ReSharper disable FunctionNeverReturns
    }
// ReSharper restore FunctionNeverReturns
}
