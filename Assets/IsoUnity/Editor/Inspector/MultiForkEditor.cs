using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;

[CustomEditor(typeof(MultiFork))]
public class MultiForkEditor : Editor {
    
    private MultiFork multifork;
    
    void OnEnable()
    {
        multifork = target as MultiFork;
        editor = CreateEditor(multifork.ForkGroup);
    }

    private Editor editor;
    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Each fork in the list result in a different branch", MessageType.Info);
        if(editor != null)
        {
            editor.OnInspectorGUI();
        }
    }

	
}
