using UnityEngine;
using NCalc;
using System;

[NodeContent("Switches/Formula setter")]
public class FormulaSetter : ScriptableObject, ISimpleContent {

    public string iswitch;
    [SerializeField]
    private string formula;
    private Expression expression;

    public static FormulaSetter Create(string iswitch, string formula)
    {
        var r = ScriptableObject.CreateInstance<FormulaSetter>();
        r.iswitch = iswitch;
        r.Formula = formula;
        return r;
    }
    
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
    
    private string paramError;

    public bool IsValidExpression
    {
        get
        {
            return !string.IsNullOrEmpty(formula.Trim()) && string.IsNullOrEmpty(paramError) && !expression.HasErrors();
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
                  : expression.Error;
        }
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
                // test
                expression.Evaluate();
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

    public override string ToString()
    {
        return this.formula;
    }

    public int Execute()
    {
        var result = expression.Evaluate();
        if(IsValidExpression && result != null)
        {
            IsoSwitchesManager.getInstance().getIsoSwitches().getSwitch(iswitch).State = result;
        }

        return 0;
    }
}
