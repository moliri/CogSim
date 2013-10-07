using UnityEditor;

[CustomEditor(typeof(RowAnimationSheet))]
// ReSharper disable CheckNamespace
internal class RowAnimationSheetEditor : Editor
// ReSharper restore CheckNamespace
{
    public override void OnInspectorGUI()
    {
        var sheet = (RowAnimationSheet)target;
        if (sheet.Animations.Count>sheet.Rows)
            sheet.Animations.RemoveRange(sheet.Rows, sheet.Animations.Count-sheet.Rows);
        for (int i = sheet.Animations.Count; i < sheet.Rows; i++)
            sheet.Animations.Add(new RowAnimationSheet.RowSpriteAnimation { row = i });
        foreach (var a in sheet.Animations)
            a.AnimationSheet = sheet;
        base.OnInspectorGUI();
    }
}

