using UnityEngine;
using NCalc;
using System.Reflection;

[NodeContent("Fork/Single/Formula fork", 2)]
public class FormulaFork : Checkable {

    public static FormulaFork Create(string formula)
    {
        var r = ScriptableObject.CreateInstance<FormulaFork>();
        r.Formula = formula;
        return r;
    }


    Expression expression;

    [SerializeField]
    private string formula = "";
    public string Formula
    {
        get { return formula; }
        set
        {
            this.name = value;
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

    void OnEnable()
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
                expression.EvaluateFunction += EvaluateFunction;
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

    public override bool check()
    {
        var r = expression.Evaluate();
        return r is bool ? (bool)r : false;
    }

    public override string ToString()
    {
        return this.formula;
    }
}