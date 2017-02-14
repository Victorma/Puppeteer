using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpriteSequenceLauncher : MonoBehaviour {

    public Sequence sequence;

    private void OnMouseDown()
    {
        var ge = new GameEvent();
        ge.Name = "start sequence";
        ge.setParameter("sequence", sequence);
        Game.main.enqueueEvent(ge);
    }
}
