using UnityEngine;
using UnityEngine.Events;

[NodeContent("UnityEvent")]
public class UnityEventNodeContent : ScriptableObject {
    
    [SerializeField]
    public UnityEvent unityEvent;
    
    void Awake()
    {
    }
    
}

public class UnityEventInterpreter : ISequenceInterpreter
{
    private bool finished = false;
    private SequenceNode node;
    private UnityEventNodeContent content;

    public bool CanHandle(SequenceNode node)
    {
        return node.Content != null && node.Content is UnityEventNodeContent;

    }

    public ISequenceInterpreter Clone()
    {
        return new UnityEventInterpreter();
    }

    public void EventHappened(IGameEvent ge){}
    public bool HasFinishedInterpretation()
    {
        return finished;
    }

    public SequenceNode NextNode()
    {
        return node.Childs.Length > 0 ? node[0] : null;
    }

    public void Tick()
    {
        content.unityEvent.Invoke();
    }

    public void UseNode(SequenceNode node)
    {
        this.node = node;
        this.content = node.Content as UnityEventNodeContent;
    }
}
