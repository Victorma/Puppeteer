using UnityEngine;
using System.Collections;
using UnityEditor;

public class SequenceNodeAsset : SequenceNode {

    public override Object Content
    {
        get
        {
            return base.Content;
        }

        set
        {
            if (value != null)
            {
                if (value != Content && value is IAssetSerializable)
                {
                    (value as IAssetSerializable).SerializeInside(this);
                    AssetDatabase.SaveAssets();
                }
                else
                {
                    if (!AssetDatabase.IsMainAsset(value) && !AssetDatabase.IsSubAsset(value))
                    {
                        AssetDatabase.AddObjectToAsset(value, this);
                        AssetDatabase.SaveAssets();
                    }
                }
            }
                

            if (value != Content && Content != null && AssetDatabase.IsSubAsset(Content))
            {
                ScriptableObject.DestroyImmediate(Content, true);
                AssetDatabase.SaveAssets();
            }


            base.Content = value;
        }
    }

    protected override void OnDestroy()
    {
        if(Content != null)
            ScriptableObject.DestroyImmediate(Content, true);
    }
}
