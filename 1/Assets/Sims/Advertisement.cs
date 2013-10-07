using System;
using System.Collections;
using UnityEngine;
using System.Collections.Generic;

public abstract class Advertisement : BindingBehaviour {
    public NeedSatisfaction[] Satisfactions = new NeedSatisfaction[1];

    public abstract IEnumerator Action(SimController controller);
}

[Serializable]
public class NeedSatisfaction
{
    public Need Need;
    public int SatisfactionDelta;
}
