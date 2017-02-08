using UnityEngine;
using UnityEditor;
using System.Collections;
using System;

public class ForkNodeEditor : NodeEditor {

	private SequenceNode myNode;

	public void draw(){
		Checkable c = (Checkable) myNode.Content;
		string[] editors = ForkEditorFactory.Intance.CurrentForkEditors;
		int editorSelected = EditorGUILayout.Popup (
			ForkEditorFactory.Intance.ForkEditorIndex(c),
			ForkEditorFactory.Intance.CurrentForkEditors
			);
		
		ForkEditor editor = ForkEditorFactory.Intance.createForkEditorFor (editors[editorSelected]);
		
		editor.useFork (c);		
		
		editor.draw ();
		
		myNode.Content = editor.Result;
		
		if(Event.current.type != EventType.Layout){
            myNode.ChildSlots = 2;
			/*int l = myNode.Childs.Length;
			if (l != 2) {
				while(l < 2){
					myNode.addNewChild ();
					l++;
				}
				while(l > 2){
					myNode.removeChild(l);
					l--;
				}
				myNode.Childs[0].Name = "Case fork True";
				myNode.Childs[1].Name = "Case fork False";
				//this.Repaint ();
			}*/
		}
	}
	
	public SequenceNode Result { get{ return myNode; } }
	public string NodeName{ get { return "Fork"; } }

    public string[] ChildNames { get{ return new string[] { "case true", "case false" }; } }

    public NodeEditor clone(){ return new ForkNodeEditor(); }
	
	public bool manages(SequenceNode c) { return c.Content != null && c.Content is Checkable; }
	public void useNode(SequenceNode c) {
		if(!(c.Content is Checkable))
			c.Content = null;
		myNode = c;
	}
}