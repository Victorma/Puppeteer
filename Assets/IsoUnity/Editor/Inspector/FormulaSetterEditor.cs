using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(FormulaSetter))]
public class FormulaSetterEditor : Editor {
    

    public override void OnInspectorGUI()
    {
        var isoSwitches = IsoSwitchesManager.getInstance().getIsoSwitches();
        var fs = target as FormulaSetter;
        
        EditorGUILayout.BeginHorizontal();
        fs.iswitch = EditorGUILayout.TextField(fs.iswitch, GUILayout.Width(100));
        if(GUILayout.Button("v", GUILayout.Width(15), GUILayout.Height(15)))
        {
            var menu = new GenericMenu();
            var i = 0;
            string text = string.Empty;
            var mousePos = Event.current.mousePosition;
            var possibles = isoSwitches.switches.ConvertAll(s => s.id);

            if (!string.IsNullOrEmpty(fs.iswitch))
                possibles = isoSwitches.switches.FindAll(s => s.id.Contains(fs.iswitch)).ConvertAll(s => s.id);

            possibles.Sort();

            foreach (var p in possibles)
            {
                menu.AddItem(new GUIContent(p), false, (n) => fs.iswitch = (string)n, p);
            }

            menu.ShowAsContext();
        }

        if (!string.IsNullOrEmpty(fs.iswitch) && isoSwitches.containsSwitch(fs.iswitch))
        {
            fs.Formula = EditorGUILayout.TextField(fs.Formula);
            EditorGUILayout.EndHorizontal();
            if (!fs.SequenceFormula.IsValidExpression)
            {
                EditorGUILayout.LabelField(fs.SequenceFormula.Error);
            }
        }
        else
        {
            EditorGUILayout.LabelField("Variable is not a valid Switch");
            EditorGUILayout.EndHorizontal();
        }


    }
}
