using UnityEngine;
using System.Collections.Generic;

public class Conditions : ScriptableObject {

    [SerializeField]
    private List<Checkable> conditions = new List<Checkable>();

    public List<Checkable> List { get { return conditions; } }

    public void AddCondition(Checkable condition)
    {
        this.conditions.Add(condition);

#if UNITY_EDITOR
        if(Application.isEditor && !Application.isPlaying)
        {
            // If this is an asset and the condition isnt
            if((UnityEditor.AssetDatabase.IsMainAsset(condition) 
                || UnityEditor.AssetDatabase.IsSubAsset(condition)) 
                && !UnityEditor.AssetDatabase.IsMainAsset(condition) 
                && !UnityEditor.AssetDatabase.IsSubAsset(condition))
            {
                if(condition is IAssetSerializable)
                {
                    (condition as IAssetSerializable).SerializeInside(this);
                }
                else
                {
                    // Capture it inside me
                    UnityEditor.AssetDatabase.AddObjectToAsset(condition, this);
                }
            }
        }
#endif
    }

    public void RemoveCondition(Checkable condition)
    {
        this.conditions.Remove(condition);
#if UNITY_EDITOR
        if (Application.isEditor && !Application.isPlaying)
        {
            // If this is an asset and the condition isnt
            if (UnityEditor.AssetDatabase.IsSubAsset(condition))
            {
                // Capture it inside me
                ScriptableObject.DestroyImmediate(condition, true);
            }
        }
#endif
    }

    public bool Eval()
    {
        return conditions.TrueForAll(c => c.check());
    }
}
