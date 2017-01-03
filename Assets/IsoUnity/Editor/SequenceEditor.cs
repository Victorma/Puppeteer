using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(SequenceAsset))]
public class SequenceEditor : Editor {

    SequenceWindow editor = null;
    public override void OnInspectorGUI()
    {

        if (GUILayout.Button("Open editor"))
        {
            if (editor == null)
            {
                editor = EditorWindow.GetWindow<SequenceWindow>();
                editor.Sequence = (target as Sequence);
                if (editor.Sequence.Root == null)
                    editor.Sequence.init();
            }
        }
        if (GUILayout.Button("Close editor"))
        {
            if (editor != null)
            {
                editor.Close();

                /*foreach (var node in editor.Sequence.Nodes)
                {
                    if (!AssetDatabase.IsSubAsset(node))
                    {
                        AssetDatabase.AddObjectToAsset(node, AssetDatabase.GetAssetPath(target));
                        if (node.Content != null)
                            AssetDatabase.AddObjectToAsset(node.Content, AssetDatabase.GetAssetPath(target));
                    }
                }*/

                AssetDatabase.SaveAssets();
            }
        }

    }
}
