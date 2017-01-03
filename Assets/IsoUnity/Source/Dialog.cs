using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;

[System.Serializable]
public class Dialog : ScriptableObject
{
    public Dialog()
    {

    }

    public Dialog(List<Fragment> fragments)
    {
        Fragments = fragments;
    }

    public Dialog(List<Fragment> fragments, List<DialogOption> options)
    {
        Fragments = fragments;
        Options = options;
    }

	
	[SerializeField]
	public string id;
    [SerializeField]
    private List<Fragment> fragments = new List<Fragment>();
    [SerializeField]
    private List<DialogOption> options = new List<DialogOption>();

    public List<Fragment> Fragments {
        get
        {
            if (fragments == null)
                fragments = new List<Fragment>();
            return this.fragments;
        }
        set { this.fragments = value; }
    }
		
	public void AddFragment(string name = "", string msg = "", string character = "", string parameter = "")
    {
        Fragments.Add(new Fragment(name, msg, character, parameter));
	}

	public void RemoveFragment(Fragment frg){
        Fragments.Remove(frg);
	}

    public List<DialogOption> Options
    {
        get
        {
            if (options == null)
                options = new List<DialogOption>();

            return this.options;
        }
        set { this.options = value; }
    }

	public void AddOption(string option = "", string parameter = ""){
        this.Options.Add(new DialogOption(option, parameter));
	}

	public void removeOption(DialogOption option){
        this.Options.Remove(option);
    }
	
}

[System.Serializable]
[StructLayout(LayoutKind.Sequential)]
public class Fragment
{
    [SerializeField]
    private string name;
    [SerializeField]
    private string msg;
    [SerializeField]
    private string character;
    [SerializeField]
    private string parameter;

    public string Name
    {
        get { return name; }
        set { this.name = value; }
    }

    public string Msg
    {
        get { return msg; }
        set { msg = value; }
    }

    public string Character
    {
        get { return character; }
        set { character = value; }
    }

    public string Parameter
    {
        get { return parameter; }
        set { parameter = value; }
    }

    public Fragment(string name = "", string msg = "", string character = "", string parameter = "")
    {
        this.name = name;
        this.msg = msg;
        this.character = character;
        this.parameter = parameter;
    }
}

[System.Serializable]
[StructLayout(LayoutKind.Sequential)]
public class DialogOption
{

    [SerializeField]
    private string text;
    [SerializeField]
    private string parameter;

    public DialogOption(string text = "", string parameter = "")
    {
        this.text = text;
        this.parameter = parameter;
    }

    public string Text { get { return text; } set { this.text = value; } }
    public string Parameter { get { return parameter; } set { this.parameter = value; } }
}


