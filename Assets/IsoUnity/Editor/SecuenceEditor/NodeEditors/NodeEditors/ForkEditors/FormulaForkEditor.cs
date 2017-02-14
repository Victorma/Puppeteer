using UnityEngine;
using System.Collections;
using UnityEditor;
using NCalc;

[CustomEditor(typeof(FormulaFork))]
public class FormulaForkEditor : Editor {

    private Expression lastExpression;
    private object lastValue;

    public override void OnInspectorGUI()
    {
        var f = target as FormulaFork;

        EditorGUI.BeginChangeCheck();
        f.formula = EditorGUILayout.TextField(f.formula);

        if (EditorGUI.EndChangeCheck())
        {
            lastExpression = new Expression(f.formula);
            lastValue = lastExpression.Evaluate();
        }

        if (lastExpression != null && lastExpression.HasErrors())
        {
            EditorGUILayout.LabelField(lastExpression.Error);
            EditorGUILayout.LabelField(lastValue != null ? lastValue.ToString() : "null");
        } 
    }
}
