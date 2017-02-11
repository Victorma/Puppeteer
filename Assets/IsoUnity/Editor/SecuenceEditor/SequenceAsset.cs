using UnityEditor;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class SequenceAsset : Sequence {
    

    public override SequenceNode createChild(object content = null, int childSlots = 0)
    {
        var node = ScriptableObject.CreateInstance<SequenceNodeAsset>();

        node.init(this);
        this.nodes.Add(node);

        AssetDatabase.AddObjectToAsset(node, this);

        node.Content = content;
        node.ChildSlots = childSlots;

        AssetDatabase.SaveAssets();

        return node;
    }
    
    public override bool removeChild(SequenceNode node)
    {
        var r = base.removeChild(node);
        if (r)
            AssetDatabase.SaveAssets();
        return r;
    }
}
