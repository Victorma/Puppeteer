using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogEventManager : EventManager {
    
    public enum State
    {
        Opening, Showing, Closing, Idle, Options
    }

    // Public variables
    public float timePerCharacter;
    public float fadeSpeed;
    public Text textHolder;
    public Text optionsMessage;
    public CanvasGroup dialogGroup;
    public CanvasGroup optionsGroup;
    public GridLayoutGroup optionsGrid;
    public GameObject optionPrefab;

    // Private variables
    private State state = State.Idle;
    private Dialog.Fragment frg;
	private List<Dialog.DialogOption> opt;
    private int charactersShown;
    private float accumulated;
	private string msg = "";
    private CanvasGroup managingGroup;
    private Dialog.DialogOption optionSelected;
    private IGameEvent gameEvent;

    void Start()
    {
        state = State.Idle;
    }

    public override void ReceiveEvent(IGameEvent ev)
    {
        if(ev.Name == "show dialog fragment")
        {
            frg = ev.getParameter("fragment") as Dialog.Fragment;
            gameEvent = ev;
            msg = frg.Msg;
            charactersShown = 0;
            state = State.Opening;
            managingGroup = dialogGroup;

        }

        if(ev.Name == "show dialog options")
        {
            opt = ev.getParameter("options") as List<Dialog.DialogOption>;
            msg = ev.getParameter("message") as string;
            gameEvent = ev;

            optionsMessage.text = msg;
            managingGroup = optionsGroup;
            foreach (var o in opt)
            {
                // create the options
                var option = GameObject.Instantiate(optionPrefab);
                var text = option.GetComponent<Text>().text = o.Text;
                option.GetComponent<Button>().onClick.AddListener(() => optionSelected = opt.Find(e => e.Text == text));
            }
        }
    }

    public override void Tick()
    {
        switch (state)
        {
            case State.Opening:
                managingGroup.alpha = Mathf.Clamp01(managingGroup.alpha + fadeSpeed * Time.deltaTime);
                if (managingGroup.alpha == 1)
                    state = State.Showing;
                break;
            case State.Showing:
                // IF showing, show more characters untill all of them are displayed
                if (managingGroup == dialogGroup)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        if (charactersShown < msg.Length)
                        {
                            charactersShown = msg.Length;
                            UpdateText();
                        }
                        else state = State.Closing;
                    }

                    accumulated += Time.deltaTime;
                    while (accumulated > timePerCharacter)
                    {
                        charactersShown = (charactersShown + 1) % msg.Length;
                        UpdateText();
                    }
                }
                else if(managingGroup == optionsGroup)
                {
                    if(optionSelected != null) state = State.Closing;
                }

                break;
            case State.Closing:
                managingGroup.alpha = Mathf.Clamp01(managingGroup.alpha - fadeSpeed * Time.deltaTime);
                if (managingGroup.alpha == 0)
                {
                    if (optionSelected != null)
                    {
                        var extraParams = new Dictionary<string, object>();
                        extraParams.Add("option", opt.FindIndex(o => o == optionSelected));
                        Game.main.eventFinished(gameEvent, extraParams);
                        optionSelected = null;
                    }
                    else
                    {
                        Game.main.eventFinished(gameEvent);
                        frg = null;
                    }
                }
                break;
            case State.Idle:
                break;
        }
    }

    private void UpdateText()
    {
        accumulated -= timePerCharacter;
        textHolder.text = msg.Substring(0, charactersShown);
    }
};


