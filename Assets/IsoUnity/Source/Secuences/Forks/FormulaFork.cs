using UnityEngine;
using System.Collections;
using System;
using NCalc;

public class FormulaFork : Checkable {

    public string formula;

    public override bool check()
    {
        Expression ex = new Expression(formula);
        ex.EvaluateParameter += (param,args) =>
        {
            args.HasResult = true;
            args.Result = IsoSwitchesManager.getInstance().getIsoSwitches().consultSwitch(param);
        };

        var r = ex.Evaluate();
        return r is bool ? (bool) r : false;
    }
}
