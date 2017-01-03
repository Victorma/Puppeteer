using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;
using System.Collections.Generic;

public class DialogNodeEditor : NodeEditor {

	private SequenceNode myNode;
	private Vector2 scroll = new Vector2(0,0);

    private ReorderableList fragmentsReorderableList, optionsReorderableList;
    private Dialog dialog;

	public void draw(){

		dialog = myNode.Content as Dialog;
		
		GUIStyle style = new GUIStyle();
		style.padding = new RectOffset(5,5,5,5);
		dialog.id = UnityEditor.EditorGUILayout.TextField("Name", dialog.id);
        
        fragmentsReorderableList.list = dialog.Fragments;
        optionsReorderableList.list = dialog.Options;
		
		EditorGUILayout.HelpBox("You have to add at least one", MessageType.None);
        if (fragmentsReorderableList.list != null)
        {
			bool isScrolling = false;
            if (fragmentsReorderableList.list.Count > 3)
            {
				scroll = EditorGUILayout.BeginScrollView(scroll, GUILayout.ExpandWidth(true), GUILayout.Height(250));
				isScrolling = true;
			}

            fragmentsReorderableList.elementHeight = fragmentsReorderableList.list.Count == 0 ? 20 : 70;
            fragmentsReorderableList.DoLayoutList();

			if(isScrolling)
				EditorGUILayout.EndScrollView();
		}
		
		EditorGUILayout.HelpBox("Options are the lines between you have to choose at the end of the dialog. Leave empty to do nothing, put one to execute this as the dialog ends, or put more than one to let the player choose between them.", MessageType.None);
        if (optionsReorderableList.list != null)
        {
            int i = optionsReorderableList.count;
		}

        optionsReorderableList.DoLayoutList();
		
		if (Event.current.type != EventType.layout)
			if (myNode.Childs.Length < 1) {
				myNode.addNewChild ();
				//this.Repaint ();
			}
	}
	
	public SequenceNode Result { get{ return myNode; } }
	public string NodeName{ get { return "Dialog"; } }
	public NodeEditor clone(){ return new DialogNodeEditor(); }
	
	public bool manages(SequenceNode c) { return c.Content != null && c.Content is Dialog; }
	public void useNode(SequenceNode c) {
		if(c.Content == null || !(c.Content is Dialog))
			c.Content = ScriptableObject.CreateInstance<Dialog>();

		myNode = c;


        // This could be used aswell, but I only advise this your class inherrits from UnityEngine.Object or has a CustomPropertyDrawer
        // Since you'll find your item using: serializedObject.FindProperty("list").GetArrayElementAtIndex(index).objectReferenceValue
        // which is a UnityEngine.Object
        // reorderableList = new ReorderableList(serializedObject, serializedObject.FindProperty("list"), true, true, true, true);

        // Add listeners to draw events

        fragmentsReorderableList = new ReorderableList(new ArrayList(), typeof(Fragment), true, true, true, true);
        fragmentsReorderableList.drawHeaderCallback  += DrawFragmentsHeader;
        fragmentsReorderableList.drawElementCallback += DrawFragment;
        fragmentsReorderableList.onAddCallback       += AddFragment;
        fragmentsReorderableList.onRemoveCallback    += RemoveFragment;
        fragmentsReorderableList.onReorderCallback   += ReorderFragments;


        optionsReorderableList = new ReorderableList(new ArrayList(), typeof(DialogOption), true, true, true, true);
        //optionsReorderableList.elementHeight = 70;
        optionsReorderableList.drawHeaderCallback  += DrawOptionsHeader;
        optionsReorderableList.drawElementCallback += DrawOption;
        optionsReorderableList.onAddCallback       += AddOption;
        optionsReorderableList.onRemoveCallback    += RemoveOption;
        optionsReorderableList.onReorderCallback   += ReorderOptions;
	}


    private Rect moveRect(Rect target, Rect move)
    {
        Rect r = new Rect(move.x + target.x, move.y + target.y, target.width, target.height);

        if (r.x + r.width > move.x + move.width)
        {
            r.width = (move.width+25) - r.x;
        }

        return r;
    }

    /*****************************
     * FRAGMENTS LIST OPERATIONS
     *****************************/

    Rect entityRect = new Rect(0, 2, 40, 15);
    Rect characterRect = new Rect(0, 2, 95, 15);
    Rect parameterRect = new Rect(100, 2, 190, 15);
    Rect nameRect = new Rect(0, 20, 190, 15);
    Rect textRect = new Rect(0, 35, 190, 30);
    private void DrawFragmentsHeader(Rect rect)
    {
        GUI.Label(rect, "Dialog fragments");
    }

    private void DrawFragment(Rect rect, int index, bool active, bool focused)
    {
        Fragment frg = (Fragment)fragmentsReorderableList.list[index];

        EditorGUI.LabelField(moveRect(entityRect, rect), "Target: ");
        frg.Character = EditorGUI.TextField(moveRect(characterRect, rect), frg.Character);
        frg.Parameter = EditorGUI.TextField(moveRect(parameterRect, rect), frg.Parameter);
        frg.Name = EditorGUI.TextField(moveRect(nameRect,rect), frg.Name);
        frg.Msg = EditorGUI.TextArea(moveRect(textRect, rect), frg.Msg);

        // If you are using a custom PropertyDrawer, this is probably better
        // EditorGUI.PropertyField(rect, serializedObject.FindProperty("list").GetArrayElementAtIndex(index));
        // Although it is probably smart to cach the list as a private variable ;)
    }

    private void AddFragment(ReorderableList list)
    {
        dialog.AddFragment();
    }

    private void RemoveFragment(ReorderableList list)
    {
        dialog.RemoveFragment(dialog.Fragments[list.index]);

    }

    private void ReorderFragments(ReorderableList list)
    {
        List<Fragment> l = (List<Fragment>)fragmentsReorderableList.list;
        dialog.Fragments = l;
    }


    /**************************
     * OPTIONS LIST OPERATIONS
     ***************************/

    Rect labelRect = new Rect(0, 2, 35, 15);
    Rect optionRect = new Rect(40, 2, 185, 15);
    private void DrawOptionsHeader(Rect rect)
    {
        GUI.Label(rect, "Dialog options");
    }

    private void DrawOption(Rect rect, int index, bool active, bool focused)
    {
        DialogOption opt = (DialogOption)optionsReorderableList.list[index];

        EditorGUI.LabelField(moveRect(labelRect, rect), "Text: ");
        opt.Text = EditorGUI.TextField(moveRect(optionRect, rect), opt.Text);

        if (myNode.Childs[index] != null)
            myNode.Childs[index].Name = dialog.Options[index].Text;
    }

    private void AddOption(ReorderableList list)
    {
        dialog.AddOption();
        if (myNode.Childs.Length < dialog.Options.Count)
            myNode.addNewChild();
    }

    private void RemoveOption(ReorderableList list)
    {
        dialog.removeOption(dialog.Options[list.index]);
        if (myNode.Childs.Length > 1)
        {
            myNode.removeChild(list.index);
        }
    }

    private void ReorderOptions(ReorderableList list)
    {
        List<DialogOption> l = (List<DialogOption>)optionsReorderableList.list;
        dialog.Options = l;
    }
}