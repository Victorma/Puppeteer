using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using Isometra;
using Isometra.Sequences;

public class pru : MonoBehaviour {
    public Sequence s;
    [SerializeField]
    public CustomCheck check = new CustomCheck();
	// Use this for initialization
	void Start () {
        if (s != null)
            return;

        s = ScriptableObject.CreateInstance<Sequence>();
        s.Root = s.CreateNode(Dialog.Create(
            new Fragment("Pepito", "Hola, qué tal andas?"),
            new Fragment("José", "Yo bien"),
            new Fragment("Pepito", "Juegas a la play?")
        ));

        s.Root.Childs[0] = s.CreateNode("options1", Options.Create(
            new Option("Sí"),
            new Option("No"),
            new Option("Da igual lo que diga, no funciona la play", string.Empty, FormulaFork.Create("playProbada == true && playRota == true"))
        ));

        // Async child setting (setting child that doesnt have a content yet or have been created)
        // By accessing to s[$name] you access the node or, if it doesnt exist, create a new node with that id
        s["options1"][0] = s["chose1"];
        s["options1"][1] = s["chose2"];
        s["options1"][2] = s["chose3"];

        // Async child content set
        s["chose1"].Content = Dialog.Create(new List<Fragment>()
        {
            new Fragment("José", "Tengo mazo de ganas de jugar!"),
            new Fragment("Pepito", "Pues vamos a enchufarla")
        });

        s["chose2"].Content = Dialog.Create(new List<Fragment>()
        {
            new Fragment("José", "No me apetece nada..."),
            new Fragment("Pepito", "Bueno... no pasa nada...")
        });

        s["chose3"].Content = Dialog.Create(new List<Fragment>()
        {
            new Fragment("José", "Sería absurdo volver a probar, está rota...")
        });


        // Multifork chooses the content by priority and allows it to have multiple childs. 
        // By default, if any of the checkables is working, it launches last child
        s.SetObject("objectname", "asd");
        s["chose1"][0] = s.CreateNode("multiFork", MultiFork.Create(new List<Checkable>()
        {
            FormulaFork.Create("playProbada == true && playRota == true && var('objectname', 'component', 'property') == 123 && varObject('objectname', 'property') == 123"),
            // AnyFork can be also AllFork. Any combines checkables with an "or" and All uses an "and"
            ForkGroup.Create<AnyFork>(new List<Checkable>() // This is made like this for the sake of exampling
            {
                ISwitchFork.Create("playRota", ISwitchFork.ComparationType.Equal, false)
            })
        }));



        // Anonymous nodes (no id is required for those)
        s["multiFork"][0] = s.CreateNode(Dialog.Create(new List<Fragment>()
        {
            new Fragment("Vida", "*Nota mental, ya probaste la play y sí, sigue rota*")
        }));

        s["multiFork"][1] = s.CreateNode(Dialog.Create(new List<Fragment>()
        {
            new Fragment("Play", "*La play se enciende*"),
            new Fragment("Pepito", "Weeeeh")
        }));

        s["multiFork"][2] = s.CreateNode("caseRota", Dialog.Create(new List<Fragment>()
        {
            new Fragment("Play", "*La play no responde*"),
            new Fragment("Pepito", "Jope")
        }));

        s["caseRota"][0] = s.CreateNode(new GameEvent("change switch", new Dictionary<string, object>()
        {
            { "switch", "playProbada" },
            { "value", true }
        }))
        ;
        // this will do the same as the last gameevent
        s["caseRota"][0][0] = s.CreateNode(FormulaSetter.Create("playProbada", "true"));

    }

    [System.Serializable]
    public class CustomCheck : IFork
    {
        public bool boolValue;
        public bool check()
        {
            return boolValue;
        }
    }
    bool launched = false;
    // Update is called once per frame
    void Update () {

        //IsoSwitchesManager.getInstance().getIsoSwitches().consultSwitch("BagPicked");

        if (Input.GetMouseButtonDown(0) && !launched)
        {
            var ge = new GameEvent();
            ge.Name = "start sequence";
            ge.setParameter("sequence", s);
            Game.main.enqueueEvent(ge);
            launched = true;
        }
	}
}
