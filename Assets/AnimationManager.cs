using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationManager : EventManager {

    private IGameEvent processing;

    public override void Tick() { }
    public override void ReceiveEvent(IGameEvent ev)
    {
        if (ev.Name == "show animation" && ev.getParameter("entity").Equals(this.gameObject.name))
        {
            processing = ev;
            StartCoroutine(ShowAnimation());
            Debug.Log("Recibido" + ev.getParameter("animation"));
        }
    }

    private IEnumerator ShowAnimation()
    {
        yield return new WaitForSeconds(2);
        Game.main.eventFinished(processing);
    }

    void Feliz()
    {
        Debug.Log("feliz");
    }
}
