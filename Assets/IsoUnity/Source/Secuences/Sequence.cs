using UnityEngine;
using System.Collections.Generic;
using System;

[System.Serializable]
public class Sequence : ScriptableObject {

    [SerializeField]
    private bool inited = false;

    public Sequence()
    {
        init();
    }

    void Awake()
    {
        init();
    }

	[SerializeField]
    protected SequenceNode root;

    [SerializeField]
    protected List<SequenceNode> nodes;

	public void init()
    {
        if (!inited)
        {
            this.nodes = new List<SequenceNode>();
        }
	}
	public SequenceNode Root
    {
		get{ return root;}
		set{ root = value;}
	}

    public SequenceNode[] Nodes
    {
        get { return nodes.ToArray() as SequenceNode[]; }
    }

    public virtual SequenceNode createChild(UnityEngine.Object content = null, int childSlots = 0)
    {
        var node = CreateInstance<SequenceNode>();
        node.init(this);
        this.nodes.Add(node);
        node.Content = content;
        node.ChildSlots = childSlots;
        return node;
    }

    public virtual bool removeChild(SequenceNode node)
    {
        int pos = nodes.IndexOf(node);

        if (pos != -1)
        {
            nodes.RemoveAt(pos);
            SequenceNode.DestroyImmediate(node, true);
        }

        return pos != -1;
    }

    private void findNodes(SequenceNode node, Dictionary<SequenceNode, bool> checkList)
    {
        if (node == null)
            return;

        if (checkList.ContainsKey(node))
            checkList[node] = true;

        foreach (var c in node.Childs)
            findNodes(c, checkList);
    }

    public int FreeNodes
    {
        get
        {
            Dictionary<SequenceNode, bool> found = new Dictionary<SequenceNode, bool>();
            foreach (SequenceNode sn in nodes)
                found.Add(sn, false);

            findNodes(root, found);

            int free = 0;
            foreach (var v in found.Values) if (!v) free++;

            return free;
        }
    }

    /*public Rect getRectFor(SecuenceNode node)
    {
        int i = nodes.IndexOf(node);
        Rect r = positions[i];
        if (r == null || r.width == 0)
        {
            // TODO reposition
            r = new Rect(10, 10, 300, 0);
            positions[i] = r;
        }

        return r;
    }

    public void setRectFor(SecuenceNode node, Rect rect)
    {
        int i = nodes.IndexOf(node);
        positions[i] = rect;
    }*/
}
