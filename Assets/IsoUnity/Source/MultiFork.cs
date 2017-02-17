using UnityEngine;
using System.Collections.Generic;
using System;

[NodeContent("Fork/MultiFork")]
public class MultiFork : ScriptableObject, NodeContent, IAssetSerializable {

    public static MultiFork Create(List<Checkable> forks)
    {
        var r = ScriptableObject.CreateInstance<MultiFork>();
        var forkGroup = ScriptableObject.CreateInstance<ForkGroup>();
        r.forkGroup = forkGroup;
        return r;
    }

    /***************
     * Properties
     * *************/

    [SerializeField]
    private ForkGroup forkGroup;
    public ForkGroup ForkGroup { get { return forkGroup; } }
    public List<Checkable> Forks { get { return forkGroup.List; } }

    /****************
     *  NODECONTENT
     * **************/
    public string[] ChildNames { get { return null; } }
    public int ChildSlots { get { return forkGroup == null ? 0 : forkGroup.List.Count; } }
    
    void Awake()
    {
        if (forkGroup == null)
            forkGroup = ScriptableObject.CreateInstance<Forks>();
    }

    public void AddFork(Checkable fork)
    {
        this.forkGroup.AddFork(fork);
    }

    public void RemoveFork(Checkable fork)
    {
        this.forkGroup.RemoveFork(fork);
    }

    public void SerializeInside(UnityEngine.Object assetObject)
    {
#if UNITY_EDITOR
        if (!UnityEditor.AssetDatabase.IsSubAsset(this))
        {
            UnityEditor.AssetDatabase.AddObjectToAsset(this, assetObject);
            UnityEditor.AssetDatabase.AddObjectToAsset(forkGroup, this);
        }
#endif
    }

    void OnDestroy()
    {
        ScriptableObject.DestroyImmediate(this.ForkGroup, Application.isEditor && !Application.isPlaying);
    }
}

internal class Forks : ForkGroup
{
    public override bool check()
    {
        throw new NotImplementedException();
    }
}
