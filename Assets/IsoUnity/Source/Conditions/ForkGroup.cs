using UnityEngine;
using System.Collections.Generic;
using System;

public abstract class ForkGroup : Checkable {

    [SerializeField]
    protected List<Checkable> forks = new List<Checkable>();

    public List<Checkable> List { get { return forks; } }

    public void AddFork(Checkable condition)
    {
        this.forks.Add(condition);

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

    public void RemoveFork(Checkable fork)
    {
        this.forks.Remove(fork);
#if UNITY_EDITOR
        if (Application.isEditor && !Application.isPlaying)
        {
            // If this is an asset and the condition isnt
            if (UnityEditor.AssetDatabase.IsSubAsset(fork))
            {
                // Capture it inside me
                ScriptableObject.DestroyImmediate(fork, true);
            }
        }
#endif
    }

    void OnDestroy()
    {
        new List<Checkable>(List).ForEach(f => RemoveFork(f));
    }
}
