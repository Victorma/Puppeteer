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
    private string paramError;

    public bool IsValidExpression
    {
        get {
            return !string.IsNullOrEmpty(formula.Trim()) && string.IsNullOrEmpty(paramError) && !expression.HasErrors() && (expresionResult is bool); }
    }

    public string Error
    {
        get { return 
                string.IsNullOrEmpty(formula.Trim())
                ? "The formula can't be empty"
                : !string.IsNullOrEmpty(paramError) 
                    ? paramError 
                    : !(expresionResult is bool) 
                        ? "The formula doesn't result in a boolean value." 
                        : expression.Error; }
    }

    void Awake()
    {
        RegenerateExpression();
    }

    private void RegenerateExpression()
    {
        paramError = string.Empty;
        if (!string.IsNullOrEmpty(formula))
        {
            try
            {
                expression = new Expression(this.formula);
                expression.EvaluateParameter += CheckParameter;
                expresionResult = expression.Evaluate();
            }
            catch { }
        }
    }

    private void CheckParameter(string param, ParameterArgs args)
    {
        if (!IsoSwitchesManager.getInstance().getIsoSwitches().containsSwitch(param))
        {
            args.HasResult = false;
            paramError = "Missing parameter \"" + param + "\"";
        }
        else
        {
            args.HasResult = true;
            args.Result = IsoSwitchesManager.getInstance().getIsoSwitches().consultSwitch(param);
        }
    }

    public override bool check()
    {
        var r = expression.Evaluate();
        return r is bool ? (bool) r : false;
    }
}
