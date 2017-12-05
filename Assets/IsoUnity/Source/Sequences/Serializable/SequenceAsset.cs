using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace IsoUnity.Sequences {

    [CreateAssetMenu(fileName = "new Sequence", menuName = "Puppeteer/Sequence")]
	public class SequenceAsset : Sequence {

	    [SerializeField]
	    private bool assetinited = false;

	    public void InitAsset()
	    {
	        if(this.localVariables == null){
	            this.localVariables = ScriptableObject.CreateInstance<IsoSwitches>();
	        }

			#if UNITY_EDITOR
	        if (!UnityEditor.AssetDatabase.IsSubAsset(this.localVariables))
	        {
				UnityEditor.AssetDatabase.AddObjectToAsset(this.localVariables, this);
				UnityEditor.AssetDatabase.SaveAssets();
	        }
			#endif
	        assetinited = true;
        }

        public override SequenceNode CreateNode(object content = null, int childSlots = 0) { return CreateNode(null, content, childSlots); }

        public override SequenceNode CreateNode(string id, object content = null, int childSlots = 0)
		{
            if (!assetinited)
            {
                InitAsset();
            }

			#if UNITY_EDITOR
			var node = CreateInstance<SequenceNodeAsset>();
			UnityEditor.AssetDatabase.AddObjectToAsset(node, this);
			#else
			var node = CreateInstance<SequenceNode>();
			#endif

	        node.init(this);
	        this.nodeDict.Add(id != null ? id : node.GetInstanceID().ToString(), node);
	        node.Content = content;

			#if UNITY_EDITOR
			UnityEditor.AssetDatabase.SaveAssets();
			#endif

	        return node;
	    }
	    
	    public override bool RemoveNode(SequenceNode node)
	    {
			var r = base.RemoveNode(node);

			#if UNITY_EDITOR
			if (r)
				UnityEditor.AssetDatabase.SaveAssets();
			#endif

	        return r;
	    }

        public override IsoSwitches LocalVariables
        {
            get
            {
                if (!assetinited) InitAsset();
                return base.LocalVariables;
            }
        }

        public override bool ContainsVariable(string id)
        {
            if (!assetinited) InitAsset();
            return base.ContainsVariable(id);
        }

        public override object GetVariable(string id)
        {
            if (!assetinited) InitAsset();
            return base.GetVariable(id);
        }

        public override void SetVariable(string id, object value)
        {
            if (!assetinited) InitAsset();
            base.SetVariable(id, value);
        }

#if UNITY_EDITOR
        public static Sequence FindSequenceOf(Object content)
	    {
	        Sequence r = null;
			var sequences = UnityEditor.AssetDatabase.FindAssets("t:Sequence").ToList().ConvertAll(o => UnityEditor.AssetDatabase.GUIDToAssetPath(o));

	        foreach (var s in sequences)
	        {
				Object[] assets = UnityEditor.AssetDatabase.LoadAllAssetsAtPath(s);
	            for (int i = 0; i < assets.Length; i++)
	            {
	                Object asset = assets[i];
	                if (asset == content)
	                {
						return UnityEditor.AssetDatabase.LoadAssetAtPath(s, typeof(Sequence)) as Sequence;
	                }
	            }
	        }

	        return r;
		}
		#endif
	}
}