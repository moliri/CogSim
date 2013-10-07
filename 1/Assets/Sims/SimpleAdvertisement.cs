using System.Collections;
using UnityEngine;

[AddComponentMenu("Sims/Simple Advertisement")]
public class SimpleAdvertisement : Advertisement
{
    /// <summary>
    /// How many seconds this action takes.
    /// </summary>
    public float Duration;
    /// <summary>
    /// What to say when talking to the target to perform this action.
    /// </summary>
    public string StartSpeech;
    /// <summary>
    /// What to while while performing the action
    /// </summary>
    public string DuringSpeech;
    /// <summary>
    /// What to say when done with the action
    /// </summary>
    public string EndSpeech;
    public override IEnumerator Action(SimController controller)
    {
        if (StartSpeech != null)
            controller.Say(StartSpeech);
        yield return StartCoroutine(controller.Goto(gameObject));
        controller.Face(gameObject);
        if (DuringSpeech != null)
            controller.Say(DuringSpeech);
        yield return new WaitForSeconds(Duration);
        controller.Say(EndSpeech);
        if (EndSpeech != null)
            yield return new WaitForSeconds(1);
        foreach (var s in Satisfactions)
            controller.Satisfy(s.Need, s.SatisfactionDelta);
    }
}