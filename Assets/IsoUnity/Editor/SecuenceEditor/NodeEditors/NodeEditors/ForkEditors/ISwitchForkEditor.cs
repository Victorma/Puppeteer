using UnityEngine;
using UnityEditor;
using System.Collections;

public class ISwitchForkEditor : ForkEditor {

	private ISwitchFork isf;

	public ISwitchForkEditor(){
		//isf = ScriptableObject.CreateInstance<ISwitchFork>();
	}

	public Checkable Result { 
		get{ return isf;} 
	}

	public string ForkName{ 
		get{ return "Iso Switch Fork"; } 
	}

	public bool manages(Checkable c){
		return c!=null && c is ISwitchFork;
	}

	public ForkEditor clone(){
		return new ISwitchForkEditor();
	}

	public void useFork(Checkable c){
		if(c is ISwitchFork)
			isf = c as ISwitchFork;
	}

	public void draw(){
		isf.id = EditorGUILayout.TextField("ID", isf.id);
		isf.comparationType = (ISwitchFork.ComparationType) EditorGUILayout.EnumPopup("Comparation Type", isf.comparationType);
		isf.Value = ParamEditor.LayoutEditorFor("Value", isf.Value, false);
        
        isf.name = isf.id + GetComparationString(isf.comparationType) + isf.Value;
    }

    private string GetComparationString(ISwitchFork.ComparationType comparationType)
    {
        switch (comparationType)
        {
            case ISwitchFork.ComparationType.Equal: return "=";
            case ISwitchFork.ComparationType.Greather: return ">";
            case ISwitchFork.ComparationType.Less: return "<";
            case ISwitchFork.ComparationType.Distinct: return "!=";
            case ISwitchFork.ComparationType.GreatherEqual: return ">=";
            case ISwitchFork.ComparationType.LessEqual: return "<=";
        }

        return string.Empty;
    }
}
