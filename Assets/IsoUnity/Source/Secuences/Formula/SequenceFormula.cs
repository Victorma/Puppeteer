using UnityEngine;
using NCalc;
using System.Reflection;

public class SequenceFormula {

    private Expression expression;
    private string formula;
    private string paramError;
    private object expresionResult;

    public SequenceFormula() : this(string.Empty) { }
    public SequenceFormula(string formula)
    {
        this.Formula = formula;
    }

    public string Formula
    {
        get
        {
            return formula;
        }
        set
        {
            formula = value;
            RegenerateExpression();
        }
    }

    private System.Type desiredReturnType;
    public System.Type DesiredReturnType
    {
        get
        {
            return desiredReturnType;
        }
        set
        {
            desiredReturnType = value;
            RegenerateExpression();
        }
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
                expression.EvaluateFunction += EvaluateFunction;
                expresionResult = expression.Evaluate();
            }
            catch { }
        }
    }

    public bool IsValidExpression
    {
        get
        {
            return !string.IsNullOrEmpty(formula.Trim()) && string.IsNullOrEmpty(paramError) && !expression.HasErrors() && (desiredReturnType == null || expresionResult != null && expresionResult.GetType().Equals(desiredReturnType));
        }
    }


    public string Error
    {
        get
        {
            return
              string.IsNullOrEmpty(formula.Trim())
              ? "The formula can't be empty"
              : !string.IsNullOrEmpty(paramError)
                  ? paramError
                  : desiredReturnType != null && !(expresionResult.GetType().Equals(desiredReturnType))
                      ? "The formula doesn't result in a " + desiredReturnType.ToString() + " value."
                      : expression.Error;
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

    private void EvaluateFunction(string name, FunctionArgs args)
    {
        switch (name)
        {
            case "var":
                {
                    GameObject go = GameObject.Find((string)args.Parameters[0].Evaluate());
                    Component co = null;
                    PropertyInfo p = null;

                    if (go) co = go.GetComponent((string)args.Parameters[1].Evaluate());
                    if (co) p = co.GetType().GetProperty((string)args.Parameters[2].Evaluate());

                    // Result
                    args.HasResult = go != null && co != null && p != null;
                    if (args.HasResult) args.Result = p.GetValue(co, null);
                }

                break;
            case "objectVar":
                {
                    object o = Sequence.current.GetObject((string)args.Parameters[0].Evaluate());
                    PropertyInfo p = null;

                    if (o != null) p = o.GetType().GetProperty((string)args.Parameters[1].Evaluate());

                    args.HasResult = o != null && p != null;
                    if (args.HasResult) args.Result = p.GetValue(o, null);
                }
                break;
        }
    }

    public object Evaluate()
    {
        return expression.Evaluate();
    }
}
