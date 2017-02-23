using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using NCalc;

[CustomEditor(typeof(FormulaFork))]
public class FormulaForkEditor : Editor {
    
    private object lastValue;
    private List<string> warnings;

    public string Name { get; set; }

    void OnEnable()
    {
        warnings = new List<string>();
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
