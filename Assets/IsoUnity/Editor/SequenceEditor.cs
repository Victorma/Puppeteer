using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Secuence))]
public class SequenceEditor : Editor {

    SecuenceWindow editor = null;
    public override void OnInspectorGUI()
    {

        if (GUILayout.Button("Open editor"))
        {
            if (editor == null)
            {
                editor = EditorWindow.GetWindow<SecuenceWindow>();
                editor.Secuence = (target as Secuence);
            }
        }
        if (GUILayout.Button("Close editor"))
        {
            if (editor != null)
            {
                editor.Close();

                foreach (var node in editor.Secuence.Nodes)
                {
                    if (!AssetDatabase.IsSubAsset(node))
                    {
                        AssetDatabase.AddObjectToAsset(node, AssetDatabase.GetAssetPath(target));
                        if (node.Content != null)
                            AssetDatabase.AddObjectToAsset(node.Content, AssetDatabase.GetAssetPath(target));
                    }
                }

                AssetDatabase.SaveAssets();
            }
        }

    }
}
