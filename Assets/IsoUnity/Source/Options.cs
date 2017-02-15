using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

[NodeContent("Options")]
public class Options : ScriptableObject, NodeContent {
    
    public string[] ChildNames { get { return options.ConvertAll(o => o.Text).ToArray(); } }
    public int ChildSlots { get { return options.Count; } }
    
    public static Options Create(params Option[] options)
    {
        return Create(new List<Option>(options));
    }

    public static Options Create(List<Option> options)
    {
        var op = CreateInstance<Options>();
        op.options = options;
        return op;
    }

    [SerializeField]
    private List<Option> options = new List<Option>();
    
    public string Question { get; set; }

    public List<Option> Values
    {
        get
        {
            if (options == null)
                options = new List<Option>();

            return this.options;
        }
        set { this.options = value; }
    }

    public void AddOption(string option = "", string parameter = "")
    {
        var conditions = ScriptableObject.CreateInstance<Conditions>();
#if UNITY_EDITOR
        if (Application.isEditor && !Application.isPlaying)
        {
            UnityEditor.AssetDatabase.AddObjectToAsset(conditions, this);
        }
#endif
        this.Values.Add(new Option(option, parameter, conditions));
    }

    public void removeOption(Option option)
    {
        this.Values.Remove(option);

#if UNITY_EDITOR
        if (Application.isEditor && !Application.isPlaying)
        {
            ScriptableObject.DestroyImmediate(option.Conditions, true);
        }
        else
        {
            ScriptableObject.DestroyImmediate(option.Conditions);
        }
#else
        ScriptableObject.DestroyImmediate(option.Conditions);
#endif
    }
}


[System.Serializable]
[StructLayout(LayoutKind.Sequential)]
public class Option
{
    [SerializeField]
    private Conditions conditions;
    [SerializeField]
    private string text;
    [SerializeField]
    private string parameter;

    public Option(string text = "", string parameter = "", Conditions conditions = null)
    {
        this.text = text;
        this.parameter = parameter;
        this.conditions = conditions;
    }

    public string Text { get { return text; } set { this.text = value; } }
    public string Parameter { get { return parameter; } set { this.parameter = value; } }
    public Conditions Conditions { get { return conditions; } }
}
