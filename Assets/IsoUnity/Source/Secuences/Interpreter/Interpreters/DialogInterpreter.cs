using UnityEngine;
using System.Collections.Generic;
using System.Text.RegularExpressions;

public class DialogInterpreter : ScriptableObject, ISequenceInterpreter
{

	private bool launched = false;
	private bool finished = false;
	private SequenceNode node;
	private SequenceNode nextNode;
	private Queue<Fragment> fragments;
    private List<Option> optionsList, launchedOptionsList;
	private int chosen;
	private bool next;
    
	public bool CanHandle(SequenceNode node)
	{
		return node!= null && node.Content != null && (node.Content is Dialog || node.Content is Options);
	}
	
	public void UseNode(SequenceNode node){
		this.node = node;
        launched = false;
        chosen = -1;

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
                    var optionchosen = (int)ge.getParameter("option");
                    chosen = optionsList.FindIndex(o => o == launchedOptionsList[optionchosen]);
                    break;
            }
        }
	}
	
	public void Tick()
	{
        if (node.Content is Dialog)
        {
            Dialog dialog = node.Content as Dialog;
            if (!launched)
            {
                fragments = new Queue<Fragment>(dialog.Fragments);
                launched = true;
                next = true;
                chosen = -1;
            }
            if (next)
            {
                if (fragments.Count > 0)
                {
                    // Launch next fragment event
                    var nextFragment = fragments.Dequeue().Clone();

                    nextFragment.Msg = Regex.Replace(nextFragment.Msg, @"\<\$(.+)\$\>", m => {
                        var formula = new SequenceFormula(m.Groups[1].Value);
                        return formula.IsValidExpression ? formula.Evaluate().ToString() : formula.Error;
                    }, RegexOptions.Multiline);
                    nextFragment.Msg = Regex.Replace(nextFragment.Msg, @"\$(\w+)", m => {
                        var formula = new SequenceFormula(m.Groups[1].Value);
                        return formula.IsValidExpression ? formula.Evaluate().ToString() : formula.Error;
                    }, RegexOptions.Multiline);

                    var ge = new GameEvent();
                    ge.name = "show dialog fragment";
                    ge.setParameter("fragment", nextFragment);
                    ge.setParameter("launcher", this);
                    ge.setParameter("synchronous", true);
                    eventLaunched = ge;
                    Game.main.enqueueEvent(ge);
                    next = false;
                }
                else
                    chosen = 0;
            }
        }
        else if (node.Content is Options)
        {
            if (!launched)
            {
                chosen = -1;
                Options options = node.Content as Options;

                // Launch options event
                var ge = new GameEvent();
                ge.name = "show dialog options";
                optionsList = options.Values;
                launchedOptionsList = optionsList.FindAll(o => o.Fork == null || o.Fork.check());
                ge.setParameter("options", launchedOptionsList);
                ge.setParameter("message", options.Question);
                ge.setParameter("launcher", this);
                ge.setParameter("synchronous", true);
                eventLaunched = ge;
                Game.main.enqueueEvent(ge);
                launched = true;
            }
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
