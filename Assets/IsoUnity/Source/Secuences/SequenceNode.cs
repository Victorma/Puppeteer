using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class SequenceNode : ScriptableObject {
	[SerializeField]
	private SequenceNode[] childs;
	[SerializeField]
	private Object content = null;
    [SerializeField]
    protected Sequence secuence = null;
    [SerializeField]
    private Rect position = new Rect(0, 0, 300, 0);
    [SerializeField]
    private bool collapsed = false;

    public Rect Position
    {
        get {
            if (collapsed) return new Rect(position.x, position.y, 50, 30);
            else           return position; 
        }
        set {
            if (collapsed) position = new Rect(value.x, value.y, position.width, position.height);
            else           position = value; 
        }
    }

    public bool Collapsed
    {
        get { return collapsed; }
        set { collapsed = value; }
    }

	public void init(Sequence s){
		childs = new SequenceNode[0];
        this.secuence = s;
		DontDestroyOnLoad (this);
	}
	
	public SequenceNode[] Childs {
		get{ return childs; }
	}

    public string Name{
		get{ return name;} 
		set{ name = value;}
	}
	
	public virtual Object Content{
		get{ return content;}
		set{ content = value;}
	}
	
	public void clearChilds(){
        var aux = ChildSlots;
        ChildSlots = 0;
        ChildSlots = aux;
	}

    private int move<T>(T[] from, T[] to, T empty)
    {
        int l = Mathf.Min(from.Length, to.Length);
        for (int i = 0; i < l; i++)          to[i] = from[i];
        for (int i = l; i < to.Length; i++)  to[i] = empty;
        return l;
    }

    public int ChildSlots
    {
        set
        {
            if (this.childs.Length != value)
            {
                var newChilds = new SequenceNode[value];
                var max = move<SequenceNode>(this.childs, newChilds, null);
                this.childs = newChilds;
            }
        }
        get
        {
            return this.childs.Length;
        }
    }
	
	public SequenceNode addNewChild(Object content = null, int childSlots = 0){
        this.ChildSlots++;
        var r = this.childs[this.ChildSlots - 1] = secuence.createChild(content, childSlots);
        return r;
	}
	
	public void removeChild(int index){
        for (int i = index; i < this.childs.Length - 1; i++)
        {
            this.childs[i] = this.childs[i + 1];
        }
        this.ChildSlots--;
	}
	
	public void removeChild(SequenceNode child){
        for (int i = 0; i < childs.Length; i++)
            if (child == childs[i])
            {
                this.removeChild(i);
                break;
            }
	}

    protected virtual void OnDestroy()
    {
        ScriptableObject.Destroy(this.Content);
    }
}