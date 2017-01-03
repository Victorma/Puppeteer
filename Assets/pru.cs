using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class pru : MonoBehaviour {
    public Sequence s;
    [SerializeField]
    public CustomCheck check = new CustomCheck();
	// Use this for initialization
	void Start () {
        if (s != null)
            return;


        s = new Sequence();
        s.Root = s.createChild(new Dialog(new List<Fragment>()
        {
            new Fragment("Pepito", "Hola, qué tal andas?"),
            new Fragment("José", "Yo bien"),
            new Fragment("Pepito", "Juegas a la play?")
        }, new List<DialogOption>()
        {
            new DialogOption("Sí"),
            new DialogOption("No")
        }), 2);

        s.Root.Childs[0] = s.createChild(new Dialog(new List<Fragment>()
        {
            new Fragment("José", "Tengo mazo de ganas de jugar!"),
            new Fragment("Pepito", "Pues vamos a enchufarla")
        }), 1);

        s.Root.Childs[1] = s.createChild(new Dialog(new List<Fragment>()
        {
            new Fragment("José", "No me apetece nada..."),
            new Fragment("Pepito", "Bueno... no pasa nada...")
        }));

        s.Root.Childs[0].Childs[0] = s.createChild(new CheckableWrapper(new CustomCheck()), 2);
        s.Root.Childs[0].Childs[0].Childs[0] = s.createChild(new Dialog(new List<Fragment>()
        {
            new Fragment("Play", "*La play se enciende*"),
            new Fragment("Pepito", "Weeeeh")
        }));
        s.Root.Childs[0].Childs[0].Childs[1] = s.createChild(new Dialog(new List<Fragment>()
        {
            new Fragment("Play", "*La play no responde*"),
            new Fragment("Pepito", "Jope")
        }));

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
