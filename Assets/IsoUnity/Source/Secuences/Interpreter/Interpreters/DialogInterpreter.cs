using UnityEngine;
using System.Collections.Generic;

public class DialogInterpreter : ScriptableObject, ISequenceInterpreter
{

	private bool launched = false;
	private bool finished = false;
	private SequenceNode node;
	private SequenceNode nextNode;
	private Queue<Fragment> fragments;
	private int chosen;
	private bool next;

	
	public bool CanHandle(SequenceNode node)
	{
		return node!= null && node.Content != null && node.Content is Dialog;
	}
	
	public void UseNode(SequenceNode node){
		this.node = node;
	}
	
	public bool HasFinishedInterpretation()
	{
		return finished;
	}
	
	public SequenceNode NextNode()
	{
		return nextNode;
	}

    IGameEvent eventLaunched;
	public void EventHappened(IGameEvent ge)
	{
        if (ge.Name == "event finished" && ge.getParameter("event") == eventLaunched) {
            switch (eventLaunched.Name.ToLower())
            {
                case "show dialog fragment":
                    next = true;
                    break;

                case "show dialog options":
                    chosen = (int)ge.getParameter("option");
                    break;
            }
        }
	}
	
	public void Tick()
	{
		Dialog dialog = node.Content as Dialog;

		if(!launched){
			fragments = new Queue<Fragment>(dialog.Fragments);
			launched = true;
			next = true;
			chosen = -1;
		}

		if(next){
			if(fragments.Count > 0){
                // Launch next fragment event
                var nextFragment = fragments.Dequeue();
                var ge = new GameEvent();
                ge.name = "show dialog fragment";
                ge.setParameter("fragment", nextFragment);
                ge.setParameter("launcher", this);
                ge.setParameter("synchronous", true);
                eventLaunched = ge;
                Game.main.enqueueEvent(ge);
			}else{
				if(dialog.Options != null && dialog.Options.Count>1){
                    // Launch options event
                    var ge = new GameEvent();
                    ge.name = "show dialog options";
                    ge.setParameter("options", dialog.Options);
                    ge.setParameter("message", dialog.Fragments[dialog.Fragments.Count - 1]);
                    ge.setParameter("launcher", this);
                    ge.setParameter("synchronous", true);
                    eventLaunched = ge;
                    Game.main.enqueueEvent(ge);
                }
				else chosen = 0;
			}
			next = false;
		}

		if(chosen != -1){
			finished = true;
            if (node.Childs.Length > chosen)
                nextNode = node.Childs[chosen];
			chosen = -1;
		}
	}

	public ISequenceInterpreter Clone(){
		return ScriptableObject.CreateInstance<DialogInterpreter>();
	}
}
