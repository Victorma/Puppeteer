using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using NCalc;

[CustomEditor(typeof(FormulaFork))]
public class FormulaForkEditor : Editor {
    
    private object lastValue;

    public string Name { get; set; }

    void OnEnable()
    {
    }

    public override void OnInspectorGUI()
    {
        var f = target as FormulaFork;

        EditorGUI.BeginChangeCheck();
        f.Formula = EditorGUILayout.TextField(f.Formula);

        if (!f.SequenceFormula.IsValidExpression)
        {
            EditorGUILayout.LabelField(f.SequenceFormula.Error);
        } 
    }
}
