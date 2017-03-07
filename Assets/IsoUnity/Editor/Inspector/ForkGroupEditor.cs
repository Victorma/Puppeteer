using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections;

[CustomEditor(typeof(ForkGroup), true)]
public class ForkGroupEditor : Editor
{
    private ReorderableList forkList;
    private ForkGroup forkGroup;

    void OnEnable()
    {
        if (!target)
            return;

        forkGroup = target as ForkGroup;
        forkList = new ReorderableList(forkGroup.List, typeof(Checkable));
        forkList.drawHeaderCallback += (rect) =>
        {
            EditorGUI.LabelField(rect, forkGroup.GetType().ToString().Replace("Forks", "FForkorks").Replace("Fork",""));
        };
        forkList.drawElementCallback += (rect, index, focus, active) =>
        {
            EditorGUI.LabelField(rect, forkGroup.List[index].GetType().ToString().Replace("Fork", "") + ": " + forkGroup.List[index].ToString());
        };

        forkList.onAddDropdownCallback += (rect, list) =>
        {
            var menu = new GenericMenu();

            menu.AddItem(new GUIContent("Group/Any"), false, (o) => forkGroup.AddFork(CreateInstance<AnyFork>()), null);
            menu.AddItem(new GUIContent("Group/All"), false, (o) => forkGroup.AddFork(CreateInstance<AllFork>()), null);
            menu.AddItem(new GUIContent("Single/Switch"), false, (o) => forkGroup.AddFork(CreateInstance<ISwitchFork>()), null);
            menu.AddItem(new GUIContent("Single/Formula"), false, (o) => forkGroup.AddFork(CreateInstance<FormulaFork>()), null);
            //menu.AddItem(new GUIContent("Single/Function"), false, (o) => AddFunction(), null);

            menu.ShowAsContext();
        };

        forkList.onRemoveCallback += (list) =>
        {
            forkGroup.RemoveFork(forkGroup.List[list.index]);
            DestroyImmediate(editor);
        };

        forkList.onSelectCallback += (list) =>
        {
            if (list.index != -1 && forkGroup.List[list.index] is UnityEngine.Object)
                editor = Editor.CreateEditor(forkGroup.List[list.index] as UnityEngine.Object);
            else
                editor = null;
        };
    }

    private Editor editor;
    public override void OnInspectorGUI()
    {
        forkList.DoLayoutList();
        if (editor != null)
        {
            editor.OnInspectorGUI();
        }
    }
    
}
