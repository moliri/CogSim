using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Random = System.Random;

[AddComponentMenu("Sims/Sim Controller")]
public class SimController : BindingBehaviour
{
    #region Bindings to other components
    [Bind]
#pragma warning disable 649
    private CharacterSprite sprite;

    [Bind]
    private CharacterSteeringController steering;

    [Bind(BindingScope.Global, BindingDefault.Create)]
    private PathPlanner planner;

    [Bind(BindingScope.Global)]
    private TileMap map;

    [Bind(BindingScope.Global)]
    private List<Advertisement> allAdvertisements;
#pragma warning restore 649
    #endregion

    #region Private fields
    //readonly Random random = new Random();
    string speechBubble;
    #endregion

    #region Need states
    public NeedState Hunger = new NeedState();
    public NeedState Thirst = new NeedState();
    public NeedState Energy = new NeedState();
    public NeedState Bladder = new NeedState();
    public NeedState Hygiene = new NeedState();
    public NeedState Fun = new NeedState();
    public NeedState Social = new NeedState();

    /// <summary>
    /// Returns the NeedState associated with the specified Need.
    /// This is gross.  We should just use an array indexed by Need.
    /// But doing it this way makes it easy to tweak the NeedStates
    /// in the editor, so it's worth it.
    /// </summary>
    /// <param name="need">The need to get the NeedState for</param>
    /// <returns>The NeedState object for that Need.</returns>
    public NeedState NeedState(Need need)
    {
        switch (need)
        {
            case Need.Hunger:
                return Hunger;

            case Need.Thirst:
                return Thirst;

            case Need.Energy:
                return Energy;

            case Need.Bladder:
                return Bladder;

            case Need.Hygiene:
                return Hygiene;

            case Need.Fun:
                return Fun;

            case Need.Social:
                return Social;

            default:
                throw new ArgumentException("Bad need value: "+need.ToString());
        }
    }
    #endregion

    // These are here as fields, only so they'll be displayed in the inspector
    public Advertisement CurrentAdvertisement;
    public GameObject Target;

    /// <summary>
    /// Basic loop for the agent: pick an advertisement and run its Action() coroutine.
    /// </summary>
    IEnumerator SatisfyNeeds()
    {
        yield return null;  // This is needed to let the other objects finish initializing
        while (true)
        {
            //var newAd = CurrentAdvertisement;
            //while (newAd == CurrentAdvertisement) newAd = allAdvertisements[random.Next(allAdvertisements.Count)];
            //CurrentAdvertisement = newAd;

            CurrentAdvertisement = allAdvertisements.ArgMax(this.ScoreAdvertisement);

            Target = CurrentAdvertisement.gameObject;
            yield return this.StartCoroutine(CurrentAdvertisement.Action(this));
        }
        // ReSharper disable FunctionNeverReturns
    }
    // ReSharper restore FunctionNeverReturns

    private float ScoreAdvertisement(Advertisement ad)
    {
        return ad.Satisfactions.Sum(sat => this.SatisfactionScore(sat));
    }

    float SatisfactionScore(NeedSatisfaction s)
    {
        var needState = this.NeedState(s.Need);
        var current = needState.SatisfactionLevel;
        var future = Math.Min(0, current + s.SatisfactionDelta);
        return needState.Attenuation(future) - needState.Attenuation(current);
    }

    /// <summary>
    /// This exists only to start the SatisfyNeeds routine
    /// </summary>
    public void Start()
    {
        StartCoroutine(this.SatisfyNeeds());
    }

    /// <summary>
    /// An array of all the Needs values, just so it's easy to iterate over all the Needs
    /// This really out to just be built into the language so you could say foreach (var n in Needs).
    /// The usual thing is to call GetValues inside of foreach, but then you're making a new array
    /// each time (ick).
    /// </summary>
    private static readonly Need[] AllNeeds = (Need[])Enum.GetValues(typeof(Need));

    /// <summary>
    /// Update all the NeedStates.
    /// IMPORTANT: this does not choose any actions.  That's done by the SatisfyNeeds coroutine.
    /// </summary>
    internal void Update()
    {
        foreach (var n in AllNeeds)
            this.NeedState(n).Update();
    }

    #region Primitive actions the charcter can perform
    /// <summary>
    /// Makes the character walk to the specified object
    /// </summary>
    /// <param name="target">GameObject to walk to</param>
    /// <returns>A coroutine to run (using StartCoroutine) that will walk there.</returns>
    public IEnumerator Goto(GameObject target)
    {
        var targetSprite = target.GetComponent<SpriteObject>();
        var path = planner.Plan(map.TilePosition(sprite.Position), map.TileRect(targetSprite.ScreenDockingRect));
        if (path==null)
        {
            Debug.LogError("Couldn't plan path to "+target.name);
            return null;
        }
        return path.FollowPath(steering);
    }

    /// <summary>
    /// Turns character to face the specified GameObject
    /// </summary>
    /// <param name="target">Object to face</param>
    public void Face(GameObject target)
    {
        var targetSprite = target.GetComponent<SpriteObject>();
        sprite.Face(targetSprite.FootprintTiles.FacingDirection(map.TilePosition(sprite.Position)));
    }

    public void Satisfy(Need need, float increment)
    {
        this.NeedState(need).Satisfy(increment);
    }

    /// <summary>
    /// Displays the specified string.
    /// </summary>
    /// <param name="speech">String to display</param>
    public void Say(string speech)
    {
        this.speechBubble = speech;
    }
    #endregion

    #region Speech bubbles
    public GUIStyle SpeechBubbleStyle;

    internal void OnGUI()
    {
        if (this.speechBubble != null)
        {
            var bubblelocation = sprite.Position + new Vector2(-150, -130);
            GUI.Label(new Rect(bubblelocation.x, bubblelocation.y, 300, 60), this.speechBubble, SpeechBubbleStyle);
        }
    }
    #endregion
}

[Serializable]
public class NeedState
{
    /// <summary>
    /// Abstract level of satisfaction of this need on a 0-100 scale.
    /// </summary>
    [Range(0,100)]
    public float SatisfactionLevel = 50;

    /// <summary>
    /// Time in minutes it takes for the need to go from 100% to 0%
    /// </summary>
    //[Range(1,60)]
    public float DepletionTime = 5;

    /// <summary>
    /// Specifies the hedonic value of a given satisfaction state.
    /// </summary>
    public AnimationCurve AttentuationCurve = AnimationCurve.Linear(0, 0, 100, 100);

    /// <summary>
    /// Returns the hedonic value associated with a given satisfaction level for this need.
    /// </summary>
    public float Attenuation(float satisfactionLevel)
    {
        return AttentuationCurve.Evaluate(satisfactionLevel);
    }

    public void Update()
    {
        var depletionSeconds = DepletionTime * 60;
        var depletionUnitsPerSecond = 100 / depletionSeconds;
        var depletion = Time.deltaTime * depletionUnitsPerSecond;
        SatisfactionLevel = Mathf.Max(0, SatisfactionLevel - depletion);
    }

    internal void Satisfy(float increment)
    {
        SatisfactionLevel = Math.Min(100, Math.Max(0, SatisfactionLevel + increment));
    }
}
