using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;

[CustomEditor(typeof(Conditions))]
public class ConditionsEditor : Editor {

    private ReorderableList conditionslist;
    private Conditions conditions;

    void OnEnable()
    {
        conditions = target as Conditions;
        conditionslist = new ReorderableList(conditions.List, typeof(Condition));
        conditionslist.drawHeaderCallback += (rect) =>
        {
            EditorGUI.LabelField(rect, "Conditions");
        };
        conditionslist.drawElementCallback += (rect, index, focus, active) =>
        {
            EditorGUI.LabelField(rect, conditions.List[index].name);
        };

        conditionslist.onAddDropdownCallback += (rect, list) =>
        {
            var menu = new GenericMenu();
            
            menu.AddItem(new GUIContent("Switch"), false, (o) => AddSwitch(), null);
            menu.AddItem(new GUIContent("Formula"), false, (o) => AddFormula(), null);
            menu.AddItem(new GUIContent("Function"), false, (o) => AddFunction(), null);

            menu.ShowAsContext();
        };

        conditionslist.onRemoveCallback += (list) =>
        {
            conditions.RemoveCondition(conditions.List[list.index]);
        };

        conditionslist.onSelectCallback += (list) =>
        {
            if (list.index != -1)
                editor = Editor.CreateEditor(conditions.List[list.index]);
            else
                editor = null;
        };
    }

    private Editor editor;
    public override void OnInspectorGUI()
    {
        conditionslist.DoLayoutList();
        if(editor != null)
        {
            editor.OnInspectorGUI();
        }
    }

    private void AddSwitch()
    {
        conditions.AddCondition(ScriptableObject.CreateInstance<ISwitchFork>());
    } 

    private void AddFormula()
    {
        conditions.AddCondition(ScriptableObject.CreateInstance<FormulaFork>());
    }

    private void AddFunction()
    {

    }

	
}
