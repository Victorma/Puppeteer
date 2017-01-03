using UnityEditor;
using UnityEngine;
using System.Collections;

public class SequenceAsset : Sequence {

    public override SequenceNode createChild(Object content = null, int childSlots = 0)
    {
        var node = ScriptableObject.CreateInstance<SequenceNodeAsset>();
        AssetDatabase.AddObjectToAsset(node, this);
        AssetDatabase.SaveAssets();

        node.init(this);
        this.nodes.Add(node);
        node.Content = content;
        node.ChildSlots = childSlots;

        return node;
    }

    public override bool removeChild(SequenceNode node)
    {
        ScriptableObject.DestroyImmediate(node.Content, true);

        var r = base.removeChild(node);
        if(r)
            AssetDatabase.SaveAssets();
        return r;
    }
}
