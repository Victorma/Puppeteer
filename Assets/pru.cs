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

        // SEQUENCE
        /* Sequences are ScriptableObjects and can be created with the normal CreateInstance method.
         * + They hold a set of SequenceNode that are linked.
         * + Each SequenceNode contains a UnityEngine.Object (ScriptableObject, Monobehaviour, etc)
         * + The Sequence can also hold Variables and Object references (explained later).
         */

        s = ScriptableObject.CreateInstance<Sequence>();
        
        // VARIABLES -> IsoSwitches
        // Acceses happen in: FormulaFork and FormulaSetter by the formula and with the "change switch" GameEvent
        
        /* Global variables are managed in the IsoSwitchesManager:
         * + Are read/written if no local variable is found
         * + To access the global IsoSwitches container use: IsoSwitchesManager.getInstance().getIsoSwitches()
         */

        // The play is globally broken
        IsoSwitchesManager.getInstance().getIsoSwitches().getSwitch("playStationBroken").State = true;

        /* Local variables are managed locally in the Sequence:
         * + Are read/written in two cases: if local variable exists OR if no global exists (with the same name)
         * + To access the local IsoSwitches contanier use: <sequence>.LocalVariables or use method getVariable and setVariable
         */
        
        // The test hasn't been locally made
        s.SetVariable("playStationTested", false);

        // SEQUENCENODE
        /* SequenceNodes are preferably created by using the method CreateNode.
         * + This method allows for the creation of anonymous and named nodes contained in the Sequence.
         * + You can set the content straight up in the node creation passing it as argument.
         * + The special node Root is the starting point of the Sequence.
         * + Childs can be accessed by using the <sequence-node>[<int>] accessor or the <sequence-node>.Childs[<int>] using the child number.
         * + All the Sequence nodes can be accesed by identifier by using <sequence>[<string>] or also by using the <sequence>.Nodes list.
         * 
         * SequenceNode contents:
         * + SequenceNode contents can be any UnityEngine.Object. By default, all Object types have 1 node child.
         * + To manually increase the amount of childs this can be modified using <sequence-node>.ChildSlots
         * + If the Object content has the [NodeContentAttribute] it can fixedly stabilish the number of childs and node content name.
         * + If it has the [NodeContentAttribute] it will also appear in the Sequence Editor Window.
         * + If the Object content inherits from NodeContent it has the capability of stabilish more childs through metods (dinamically).
         * + Inheritance of NodeContent will override anything on the [NodeContentAttribute].
         * 
         * Already implemented contents are: Dialogs, Options, GameEvents and several types of Forks.
         */

        // DIALOGS
        /* Dialogs are conversations divided by fragments. Each fragment has an actor name and a text.
         * + Dialogs are easily created using Dialog.Create(<list-fragment>)
         * + Two extra string attributes could be used as needed. (Character and Parameter) 
         */ 
        s.Root = s.CreateNode(Dialog.Create(
            new Fragment("Pepito", "Hola, qué tal andas?"),
            new Fragment("José", "Yo bien"),
            new Fragment("Pepito", "Juegas a la play?")
        ));

        // OPTIONS
        /* Options are textual choices given to a player through a contextual menu.
         * + Options are easily created using Options.Create(<list-options>)
         * + An option can contain text, a parameter and Fork (Checkable).
         */
        s.Root.Childs[0] = s.CreateNode("options1", Options.Create(
            new Option("Sí"),
            new Option("No"),
            // This option uses Forks. If the fork is false the option wont be shown.
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

        // OBJECT REFERECES
        /* Sequences can store object references.
         * + Can be setted using <sequence>.SetObject(<string>, <UnityEngine.Object>)
         * + Can be accessed using <sequence>.GetObject(<string>)
         * 
         * Object References are meant to be used in FormulaForks by using the function varObject (explained later).
         */ 
        s.SetObject("object-name", new pru());


        // FORKS
        /* Forks are not interactive (resolved automatically) node bifurcations.
         * Its parent class is the Checkable that implements the IFork interface.
         * + Are interpreted by the CheckableInterpreter by using the check method.
         * + Can hanlde only two childs, one for the true (0) and one for the false (1).
         * 
         * ISwitchFork is the fork that is designed to compare a single fork.
         * + It's more efficient than formulas for simple checks.
         * + Compares the value with the local and global switches.
         * 
         * FormulaFork is the fork that handles formulas in string, interpreted during execution.
         * + Uses NCalc to resolve the function.
         * + Uses IsoSwitches as variables that can be stored locally and globaly.
         * + Can handle comparations as == or !=
         * + Can handle operations as +,- or *
         * + It has to return a boolean value, otherwise it will be allways false.
         * + Can handle simple functions such as sqrt and if.
         * + Can access objects in the scene by using: var('objectname', 'component', 'property')
         * + Can access sequence stored references by using: varObject('storedreference', 'property')
         * 
         * ForkGroup: AnyFork or AllFork. Are meant to be used to compare multiple fork results effitiently (x2 speed).
         * + Can be created using ForkGroup.Create< AnyFork | AllFork >().
         * + AnyFork performs the || operator between all the Forks.
         * + AllFork performs the && operator between all the Forks.
         */

        // MULTIFORK
        /* MultiForks allows for the creation of a fork with more than two childs.
         * + It holds as many childs as forks it contains + default.
         * + It will choose the child of the first Fork that is resolved as true.
         * + If no fork returns true, it will choose the last child (the default).
         * + Multifork is created by using MultiFork.Create(<fork-list>)
         */

        // Multifork chooses the content by priority and allows it to have multiple childs. 
        // By default, if none of the checkables is working, it launches last child
        s["chose1"][0] = s.CreateNode("multiFork", MultiFork.Create(new List<Checkable>()
        {
            FormulaFork.Create("var('objectname', 'component', 'property') == varObject('storedreference', 'property')"),
            FormulaFork.Create("playProbada == true && playRota == true"),
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

        // GAME EVENTS
        /* Game Events are the easiest way to send messages outside of the Sequence.
         * + GameEvents are created with a name and a dictionary of parameters.
         * + Parameters can also be manipulated with setted, getter and remove.
         * 
         * Existing events:
         * + "change switch": parameters (string)switch, (any)value
         * + "show dialog fragment": parameters (Fragment)fragment
         * + "show dialog options": parameters (Options)options
         * 
         * All the game events will be received by the EventManagers.
         */
        s["caseRota"][0] = s.CreateNode(new GameEvent("change switch", new Dictionary<string, object>()
        {
            { "switch", "playProbada" },
            { "value", true }
        }))
        ;
        // this will do the same as the last gameevent

        // FORMULA SETTER
        /* Formula setter allows for the setting of variables from nodes by using formulas.
         * It's the more complex mode of the "change switch" game event, that, instead of using a fixed value
         *  uses runtime variables and operations.
         *  
         * + It can be created using FormulaSetter.Create(<variable-name>, <formula>)
         * + The variable will be stored in (by priority):
         *   - A Sequence local variable, if there is a local variable with the same name.
         *   - A global variable, if there is a global variable with the same name.
         *   - A new local variable, otherwise.
         *   -> NOTICE that it will NEVER create a new global variable.
         */
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
