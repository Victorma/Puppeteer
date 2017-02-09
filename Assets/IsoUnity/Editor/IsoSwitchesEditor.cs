using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;

[CustomEditor(typeof(IsoSwitches))]
public class IsoSwitchesEditor : Editor{

	private Vector2 scrollposition = new Vector2(0,0);

    private ReorderableList switchList;
    private List<Rect> rects;

	IsoSwitches isoSwitches;
	public void OnEnable(){

        isoSwitches = target as IsoSwitches;

        switchList = new ReorderableList(isoSwitches.switches, typeof(ISwitch), true, false, true, true);

        switchList.elementHeight = 50;

        switchList.drawElementCallback += (rect, index, isActive, isFocused) =>
        {
            var isw = isoSwitches.switches[index];
            while (rects.Count <= index)
                rects.Add(new Rect(0, 0, 0, 0));
            if(Event.current.type == EventType.repaint)
                rects[index] = rect;
        };

        switchList.onRemoveCallback += (list) =>
        {
            isoSwitches.removeSwitch(isoSwitches.switches[list.index]);
        };

        switchList.onAddCallback += (list) =>
        {
            isoSwitches.addSwitch();
        };
	}
	
	
	public override void OnInspectorGUI(){

		isoSwitches = target as IsoSwitches;
		
		GUIStyle style = new GUIStyle();
		style.padding = new RectOffset(5,5,5,5);

		isoSwitches = target as IsoSwitches;
		
		EditorGUILayout.HelpBox("List of switches that represent the state of the game.", MessageType.None);

        switchList.DoLayoutList();

        for(int i = 0; i < rects.Count; i++)
        {
            var isw = isoSwitches.switches[i];
            GUILayout.BeginArea(rects[i]);
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("ID: ", GUILayout.Width(27));
                isw.id = EditorGUILayout.TextField(isw.id);
                isw.State = ParamEditor.editorFor("Initial State: ", isw.State);
                EditorGUILayout.EndHorizontal();
            GUILayout.EndArea();
        }

		/*ISwitch[] switches = isoSwitches.getList ();
		if(switches != null){
			int i = 0;
			scrollposition = EditorGUILayout.BeginScrollView(scrollposition, GUILayout.ExpandHeight(true));
			foreach(ISwitch isw in switches){
				
				i++;
			}
			EditorGUILayout.EndScrollView();
		}

		EditorGUILayout.BeginHorizontal();
		GUIContent bttext = new GUIContent("Add Switch");
		Rect btrect = GUILayoutUtility.GetRect(bttext, style);		
		if(GUI.Button(btrect,bttext)){
		};
		EditorGUILayout.EndHorizontal();*/
	}
}