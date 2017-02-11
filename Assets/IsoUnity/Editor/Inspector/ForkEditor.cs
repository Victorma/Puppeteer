using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Checkable),true)]
public class CheckableEditor : Editor {

    private ForkEditor editor;
    private Checkable checkable;

    void OnEnable()
    { 
        checkable = target as Checkable;
        editor = ForkEditorFactory.Intance.createForkEditorFor(
                    ForkEditorFactory.Intance.CurrentForkEditors[
                        ForkEditorFactory.Intance.ForkEditorIndex(checkable)
                        ]);
    }

    public override void OnInspectorGUI()
    {
        editor.useFork(checkable);
        editor.draw();
    }
}
