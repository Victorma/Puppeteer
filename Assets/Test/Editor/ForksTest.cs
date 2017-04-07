using UnityEngine;
using UnityEditor;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Isometra;
using Isometra.Sequences;
using System.Collections.Generic;

public class ForksTest {
    

	[Test]
	public void ForksTestWithForks()
    {
        Sequence s = ScriptableObject.CreateInstance<Sequence>();

        IsoSwitchesManager.getInstance().getIsoSwitches().getSwitch("global1").State = true;
        IsoSwitchesManager.getInstance().getIsoSwitches().getSwitch("global2").State = "value";
        IsoSwitchesManager.getInstance().getIsoSwitches().getSwitch("global3").State = 0;
        s.SetVariable("variable1", true);
        s.SetVariable("variable2", "value");
        s.SetVariable("variable3", 0);

        var anyfork = ForkGroup.Create<AnyFork>(
            ISwitchFork.Create("global1", ISwitchFork.ComparationType.Equal, false),
            ISwitchFork.Create("global2", ISwitchFork.ComparationType.Equal, "true"),
            ISwitchFork.Create("global3", ISwitchFork.ComparationType.Equal, 0));

        var allfork = ForkGroup.Create<AllFork>(
            ISwitchFork.Create("variable1", ISwitchFork.ComparationType.Equal, true),
            ISwitchFork.Create("variable2", ISwitchFork.ComparationType.Equal, "value"),
            ISwitchFork.Create("variable3", ISwitchFork.ComparationType.Equal, 0));

        s.Root = s.CreateNode(ForkGroup.Create<AllFork>(anyfork, allfork));

        for(int i = 0; i<10000; i++)
        {
            SequenceInterpreter interpreter = new SequenceInterpreter(s);
            while (!interpreter.SequenceFinished)
            {
                interpreter.Tick();
            }
        }
    }

    [Test]
    public void ForksTestWithFormulas()
    {
        Sequence s = ScriptableObject.CreateInstance<Sequence>();

        IsoSwitchesManager.getInstance().getIsoSwitches().getSwitch("global1").State = true;
        IsoSwitchesManager.getInstance().getIsoSwitches().getSwitch("global2").State = "value";
        IsoSwitchesManager.getInstance().getIsoSwitches().getSwitch("global3").State = 0;
        s.SetVariable("variable1", true);
        s.SetVariable("variable2", "value");
        s.SetVariable("variable3", 0);

        s.Root = s.CreateNode(FormulaFork.Create("(global1 == false || global2 == 'true' || global3 == 0) && (variable1 == true && variable2 == 'value' && variable3 == 0)"));

        for (int i = 0; i < 10000; i++)
        {
            SequenceInterpreter interpreter = new SequenceInterpreter(s);
            while (!interpreter.SequenceFinished)
            {
                interpreter.Tick();
            }
        }
    }

    // A UnityTest behaves like a coroutine in PlayMode
    // and allows you to yield null to skip a frame in EditMode
    [UnityTest]
	public IEnumerator ForksTestWithEnumeratorPasses() {
		// Use the Assert class to test conditions.
		// yield to skip a frame
		yield return null;
	}
}
