using UnityEngine;
using System.Collections;
using System;
using NCalc;

[NodeContent("Switches/Formula fork", 2)]
public class FormulaFork : Checkable {

    Expression expression;

    [SerializeField]
    private string formula = "";
    public string Formula
    {
        get { return formula; }
        set
        {
            this.formula = value;
            RegenerateExpression();
        }
    }

    private object expresionResult;

    public bool IsValidExpression
    {
        get { return expression.HasErrors() || !(expresionResult is bool); }
    }

    void Awake()
    {
        RegenerateExpression();
    }

    private void RegenerateExpression()
    {
        expression = new Expression(this.formula);
        expression.EvaluateParameter += CheckParameter;
        expresionResult = expression.Evaluate();
    }

    private void CheckParameter(string param, ParameterArgs args)
    {
        args.HasResult = true;
        args.Result = IsoSwitchesManager.getInstance().getIsoSwitches().consultSwitch(param);
    }

    public override bool check()
    {
        var r = expression.Evaluate();
        return r is bool ? (bool) r : false;
    }
}
