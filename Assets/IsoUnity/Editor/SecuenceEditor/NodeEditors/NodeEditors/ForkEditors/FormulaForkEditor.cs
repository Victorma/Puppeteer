using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using NCalc;

[CustomEditor(typeof(FormulaFork))]
public class FormulaForkEditor : Editor {

    private Expression lastExpression;
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
        f.formula = EditorGUILayout.TextField(f.formula);

        if (EditorGUI.EndChangeCheck())
        {
            warnings.Clear();
            lastExpression = new Expression(f.formula);
            lastExpression.EvaluateParameter += LastExpression_EvaluateParameter;
            lastExpression.EvaluateFunction += LastExpression_EvaluateFunction;
            lastValue = lastExpression.Evaluate();
        }

        if (lastExpression != null)
        {
            if(lastExpression.HasErrors())
                EditorGUILayout.LabelField(lastExpression.Error);
            else if (lastValue != null)
                EditorGUILayout.LabelField(lastValue != null ? lastValue.ToString() : "null");

            foreach(var w in warnings)
            {
                EditorGUILayout.HelpBox(w, MessageType.Warning);
            }
        } 
    }

    private void LastExpression_EvaluateFunction(string name, FunctionArgs args)
    {
        throw new System.NotImplementedException();
    }

    private void LastExpression_EvaluateParameter(string name, ParameterArgs args)
    {

        var iSwitches = IsoSwitchesManager.getInstance().getIsoSwitches();

        if(iSwitches.containsSwitch(name))
        {
            args.HasResult = true;
            args.Result = iSwitches.consultSwitch(name);
        }
        else
        {
            args.HasResult = false;
            warnings.Add("Switch not found: \"" + name +"\"");
        }
    }
}
