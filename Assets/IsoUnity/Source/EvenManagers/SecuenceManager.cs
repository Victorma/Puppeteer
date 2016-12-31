using UnityEngine;
using System.Collections;

public class SecuenceManager : EventManager {
	
	private SecuenceInterpreter secuenceInterpreter;

	public override void ReceiveEvent (IGameEvent ev)
	{
		if(secuenceInterpreter == null){
			if(ev.Name.ToLower() == "start secuence"){
				Secuence secuence = (ev.getParameter("Secuence") as Secuence);
				secuenceInterpreter = new SecuenceInterpreter(secuence);
			}
		}else secuenceInterpreter.EventHappened(ev);
	}

	public override void Tick(){
		if(secuenceInterpreter != null){
			secuenceInterpreter.Tick();
			if(secuenceInterpreter.SecuenceFinished){
				Debug.Log("Secuence finished");
				this.secuenceInterpreter = null;
			}
		}
	}
}
