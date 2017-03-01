using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using UnityEditor.Callbacks;
using System;
using System.Linq;

public class SequenceWindow : EditorWindow
{
    [OnOpenAsset(1)]
    public static bool Open(int instanceID, int line)
    {
        var o = EditorUtility.InstanceIDToObject(instanceID);
		if (o is SequenceAsset)
        {
            var newWindow = ScriptableObject.CreateInstance<SequenceWindow>();
            newWindow.sequence = o as SequenceAsset;
            newWindow.Show();
            return true;
        }
        return false;
    }

    private Sequence sequence;

    public Sequence Sequence
    {
        get { return sequence; }
        set { this.sequence = value; }
    }

	/*******************
	 *  ATTRIBUTES
	 * *****************/

    private Dictionary<int, SequenceNode> nodes = new Dictionary<int, SequenceNode>();
    private Dictionary<SequenceNode, NodeEditor> editors = new Dictionary<SequenceNode, NodeEditor>();
    private GUIStyle closeStyle, collapseStyle, selectedStyle;

	private int hovering = -1;
	private SequenceNode hoveringNode = null;
    private int focusing = -1;

    private int lookingChildSlot;
	private SequenceNode lookingChildNode;

	private Rect scrollRect = new Rect(0, 0, 1000, 1000);
	private Vector2 scroll;

	private bool toSelect = false;

    void nodeWindow(int id)
    {
        SequenceNode myNode = nodes[id];

        // Editor selection
        //string[] editorNames = NodeEditorFactory.Intance.CurrentNodeEditors;

        GUILayout.BeginHorizontal();
        EditorGUI.BeginChangeCheck();
        //var editorSelected = EditorGUILayout.Popup(NodeEditorFactory.Intance.NodeEditorIndex(myNode), editorNames);
        
        if (!editors.ContainsKey(myNode) || EditorGUI.EndChangeCheck())
        {
               var editor = NodeEditorFactory.Intance.createNodeEditorFor(
                NodeEditorFactory.Intance.CurrentNodeEditors[
                    NodeEditorFactory.Intance.NodeEditorIndex(myNode)
                ]);
            editor.useNode(myNode);

            if (!editors.ContainsKey(myNode)) editors.Add(myNode, editor);
            else
            {
                ScriptableObject.DestroyImmediate(editors[myNode] as ScriptableObject);
                editors[myNode] = editor;
            }
        }

        // Drawing

        if (myNode.Collapsed)
        {
            if (GUILayout.Button(myNode.ShortDescription))
                myNode.Collapsed = false;
        }
        else
        {
            GUILayout.FlexibleSpace();
        }
        if (GUILayout.Button(myNode.Collapsed ? "+" : "-", collapseStyle, GUILayout.Width(15), GUILayout.Height(15)))
            myNode.Collapsed = !myNode.Collapsed;
        if (GUILayout.Button("X", closeStyle, GUILayout.Width(15), GUILayout.Height(15)))
        {
            sequence.RemoveNode(myNode);
            return;
        }

        GUILayout.EndHorizontal();

        if (!myNode.Collapsed)
        {
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            editors[myNode].draw();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();

            nodes[id] = editors[myNode].Result;
        }

        // Event management
        if (Event.current.type != EventType.layout)
        {
            Rect lastRect = GUILayoutUtility.GetLastRect();
            Rect myRect = myNode.Position;
            myRect.height = lastRect.y + lastRect.height;
            myNode.Position = myRect;
            this.Repaint();
        }

        switch (Event.current.type)
        {
            case EventType.MouseDown:

                // Left button
                if (Event.current.button == 0)
                {
					if (hovering == id) {
						toSelect = false;
						focusing = hovering;
						if (Event.current.control) {
							if (selection.Contains (myNode))
								selection.Remove (myNode);
							else
								selection.Add (myNode);
						} else {
							toSelect = true;
							if (!selection.Contains (myNode)) {
								selection.Clear ();
								selection.Add (myNode);
							}
						}
					}
                    if (lookingChildNode != null)
                    {
                        // link creation between nodes
                        lookingChildNode.Childs[lookingChildSlot] = myNode;
                        // finishing search
                        lookingChildNode = null;
                    }
                    if(myNode.Content is UnityEngine.Object)
                        Selection.activeObject = myNode.Content as UnityEngine.Object;
                }

                // Right Button
                if (Event.current.button == 1)
                {
                    var menu = new GenericMenu();
                    var i = 0;
                    string text = string.Empty;
                    menu.AddItem(new GUIContent("Set sequence root"), false, (node) => sequence.Root = node as SequenceNode, myNode);
                    foreach (var a in editors[myNode].ChildNames)
                    {
                        text = (a == "") ? (i + "") : a;
                        menu.AddItem(new GUIContent("Set node for " + text), false, (t) => {
                            // Detach		
                            myNode.Childs[(int)t] = null;
                            lookingChildNode = myNode;
                            lookingChildSlot = (int)t;
                        }, i);
                        i++;
                    }

                    menu.ShowAsContext();
                }

                break;
			case EventType.MouseMove:

				if (new Rect (0, 0, myNode.Position.width, myNode.Position.height).Contains (Event.current.mousePosition)) {
					hovering = id;
					hoveringNode = myNode;
				}
				break;
		case EventType.MouseDrag:
			toSelect = false;
			break;
		case EventType.MouseUp:
			{
				if(toSelect) {
					selection.Clear ();
					selection.Add (myNode);
				}
			}
                break;
        }

        var resizeRect = new Rect(new Vector2(myNode.Position.width - 10, 0), new Vector2(10, myNode.Position.height));
        EditorGUIUtility.AddCursorRect(resizeRect,MouseCursor.ResizeHorizontal, myNode.GetHashCode());
        if (EditorGUIUtility.hotControl == 0 
            && Event.current.type == EventType.MouseDown 
            && Event.current.button == 0 
            && resizeRect.Contains(Event.current.mousePosition))
        {
            EditorGUIUtility.hotControl = myNode.GetHashCode();
            Event.current.Use();
        }
        
        if(GUIUtility.hotControl == myNode.GetHashCode())
        {
            //Debug.Log("hotcontrol");
            myNode.Position = new Rect(myNode.Position.x, myNode.Position.y, Event.current.mousePosition.x + 5, myNode.Position.height);
            this.Repaint();
            //Event.current.Use();
            if (Event.current.type == EventType.MouseUp)
                EditorGUIUtility.hotControl = 0;
            //if(Event.current.type != EventType.layout)*/
        }


        GUI.DragWindow();

    }

    void curveFromTo(Rect wr, Rect wr2, Color color)
    {
        Vector2 start = new Vector2(wr.x + wr.width, wr.y + 3 + wr.height / 2),
            startTangent = new Vector2(wr.x + wr.width + Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2, wr.y + 3 + wr.height / 2),
            end = new Vector2(wr2.x, wr2.y + 3 + wr2.height / 2),
            endTangent = new Vector2(wr2.x - Mathf.Abs(wr2.x - (wr.x + wr.width)) / 2, wr2.y + 3 + wr2.height / 2);

        Handles.BeginGUI();
        Handles.color = color;
        Handles.DrawBezier(start, end, startTangent, endTangent, color, null, 3);
        Handles.EndGUI();
    }

    private Rect sumRect(Rect r1, Rect r2)
    {
        return new Rect(r1.x + r2.x, r1.y + r2.y, r1.width + r2.width, r1.height + r2.height);
    }

    private Dictionary<SequenceNode, bool> loopCheck = new Dictionary<SequenceNode, bool>();


    bool drawSlot(Vector2 center)
    {
        return GUI.Button(new Rect(center.x - 10, center.y - 10, 20, 20), "");
    }

    void drawSlots(Sequence sequence)
    {

        // Draw the rest of the lines in red
        foreach (var n in sequence.Nodes)
        {
            // InputSlot
            drawSlot(new Vector2(n.Position.x, n.Position.y + 3 + n.Position.height / 2));

            // OutputSlots
            float h = n.Position.height / (n.Childs.Length * 1.0f);
            for (int i = 0; i < n.Childs.Length; i++)
                if (drawSlot(new Vector2(n.Position.x + n.Position.width, n.Position.y + h * i + h / 2f)))
                {
                    // Detach		
                    n.Childs[i] = null;
                    lookingChildNode = n;
                    lookingChildSlot = i;
                }
        }
    }

    void drawLines(Sequence sequence)
    {
        loopCheck.Clear();

		// Draw the main nodes in green
		drawLines(new Rect(0, 0, 0, position.height), sequence.Root, 
			Color.green, 
			new Color(Color.green.r, Color.green.g, Color.green.b, .2f));

        // Draw the rest of the lines in red
        foreach (var n in sequence.Nodes)
		{
			drawLines(new Rect(-1,-1,-1,-1), n, 
				Color.red, 
				new Color(Color.red.r, Color.red.g, Color.red.b, .2f));
        }
    }


	void drawLines(Rect from, SequenceNode to, Color c, Color notHoveringColor, bool parentHovered = false)
	{
		if (to == null)
			return;

		var hoveringMe = hoveringNode != null && hoveringNode == to;
		var useColor = parentHovered || hoveringNode == null || hoveringMe ? c : notHoveringColor;

		// Visible loop line
		if(from.width != -1 && from.height != -1)
			curveFromTo(from, to.Position, useColor);

		if (!loopCheck.ContainsKey(to))
		{
			loopCheck.Add(to, true);
			float h = to.Position.height / (to.Childs.Length * 1.0f);
			for (int i = 0; i < to.Childs.Length; i++)
			{
				Rect fromRect = sumRect(to.Position, new Rect(0, h * i, 0, h - to.Position.height));
				// Looking child line
				if (lookingChildNode == to && i == lookingChildSlot)
				{
					if (hovering != -1) curveFromTo(fromRect, nodes[hovering].Position, useColor);
					else curveFromTo(fromRect, new Rect(Event.current.mousePosition.x, Event.current.mousePosition.y, 1, 1), useColor);
				}
				else drawLines(fromRect, to.Childs[i], c, notHoveringColor, hoveringMe);
			}
		}

	}

    /**
     *  Rectangle backup code calculation
     **/

    void createWindows(Sequence sequence)
    {
        foreach (var node in sequence.Nodes)
        {
            nodes.Add(node.GetInstanceID(), node);
            var finalPosition = GUILayout.Window(node.GetInstanceID(), node.Position, nodeWindow, node.Name);
			var diff = finalPosition.position - node.Position.position;

			// If the window has been moved, lets move the others too
			if (diff != Vector2.zero) {

				if (selection.Contains (node)) {
					selection.ForEach (n => {
						if(n != node) {
							n.Position = new Rect(n.Position.position + diff, n.Position.size);
							// Redo the window
							GUILayout.Window(n.GetInstanceID(), n.Position, nodeWindow, n.Name);
						}
					});
				}
			}
			node.Position = finalPosition;
        }
    }

    /*Color s = new Color(0.4f, 0.4f, 0.5f),
        l = new Color(0.3f, 0.7f, 0.4f),
        r = new Color(0.8f, 0.2f, 0.2f);*/

	List<SequenceNode> selection = new List<SequenceNode>();
	Vector2 startPoint;

    void OnGUI()
    {
        if (sequence == null)
            this.Close();

		Sequence.current = sequence;

        this.wantsMouseMove = true;

        if (closeStyle == null)
        {
            closeStyle = new GUIStyle(GUI.skin.button);
            closeStyle.padding = new RectOffset(0, 0, 0, 0);
            closeStyle.margin = new RectOffset(0, 5, 2, 0);
            closeStyle.normal.textColor = Color.red;
            closeStyle.focused.textColor = Color.red;
            closeStyle.active.textColor = Color.red;
            closeStyle.hover.textColor = Color.red;
        }

        if (collapseStyle == null)
        {
            collapseStyle = new GUIStyle(GUI.skin.button);
            collapseStyle.padding = new RectOffset(0, 0, 0, 0);
            collapseStyle.margin = new RectOffset(0, 5, 2, 0);
            collapseStyle.normal.textColor = Color.blue;
            collapseStyle.focused.textColor = Color.blue;
            collapseStyle.active.textColor = Color.blue;
            collapseStyle.hover.textColor = Color.blue;
        }

		if (selectedStyle == null) {
			selectedStyle = Resources.Load<GUISkin> ("resplandor").box;
		}

        GUILayout.BeginVertical(GUILayout.Height(17));
        GUILayout.BeginHorizontal("toolbar");

        using (new EditorGUI.DisabledScope())
        {
            if (GUILayout.Button("Globals", "toolbarButton", GUILayout.Width(100)))
            {
                var o = SwitchesMenu.ShowAtPosition(GUILayoutUtility.GetLastRect().Move(new Vector2(5,16)));
                if (o) GUIUtility.ExitGUI();
            }
            if (GUILayout.Button("Locals", "toolbarButton", GUILayout.Width(100)))
            {
                var o = SwitchesMenu.ShowAtPosition(GUILayoutUtility.GetLastRect().Move(new Vector2(105, 16)), sequence.LocalVariables);
                if (o) GUIUtility.ExitGUI();
            }
        }

        GUILayout.Space(5);

        if (GUILayout.Button("New Node", "toolbarButton"))
        {
            var node = sequence.CreateNode();
            node.Position = new Rect(scroll + position.size / 2 - node.Position.size / 2, node.Position.size);
            node.Position = new Rect(new Vector2((int)node.Position.x, (int)node.Position.y), node.Position.size);
        }
        if (GUILayout.Button("Set Root", "toolbarButton"))
        {
            if (nodes.ContainsKey(focusing))
            {
                sequence.Root = nodes[focusing];
            }
        }

        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
        var lastRect = GUILayoutUtility.GetLastRect();

        var rect = new Rect(0, lastRect.y + lastRect.height, position.width, position.height - lastRect.height);

        float maxX = rect.width, maxY = rect.height;
        foreach (var node in sequence.Nodes)
        {
            var px = node.Position.x + node.Position.width + 50;
            var py = node.Position.y + node.Position.height + 50;
            maxX = Mathf.Max(maxX, px);
            maxY = Mathf.Max(maxY, py);
        }
        scrollRect = new Rect(0, 0, maxX, maxY);

        scroll = GUI.BeginScrollView(rect, scroll, scrollRect);
        // Clear mouse hover
		if (Event.current.type == EventType.MouseMove) { hovering = -1; hoveringNode = null; }
        GUI.Box(scrollRect, "", "preBackground");
		
        BeginWindows();
        {
            nodes.Clear();
            createWindows(sequence);

			if(Event.current.type == EventType.Repaint)
				foreach (var n in selection)
					GUI.Box (new Rect(
						n.Position.position - new Vector2(0,0), 
						n.Position.size + new Vector2(0	,0)), 
						"", selectedStyle);
				
            drawSlots(sequence);

            if (Event.current.type == EventType.Repaint)
            {
                drawLines(sequence);
            }
        }
        EndWindows();	

        if (Event.current.type == EventType.MouseDrag && EditorGUIUtility.hotControl == 0)
        {
            scroll -= Event.current.delta;
        }

		switch (Event.current.type) {
			case EventType.MouseDown: 
				{
					if (Event.current.button == 0) {
						// Selecting
						if (GUIUtility.hotControl == 0) {
							// Start selecting
							GUIUtility.hotControl = this.GetHashCode();
							startPoint = Event.current.mousePosition;
							selection.Clear ();
							Event.current.Use ();
						}
					} 
				}
				break;
			case EventType.MouseUp:
				{
					if (Event.current.button == 0) {
						if (GUIUtility.hotControl == this.GetHashCode()) {
							GUIUtility.hotControl = 0;

							UpdateSelection ();
							Event.current.Use ();
						}

					} else if (Event.current.button == 1) {
						// Right click

						var menu = new GenericMenu();
						var mousePos = Event.current.mousePosition;
						int i = 0;
						foreach (var a in GetPossibleCreations())
						{

							menu.AddItem(new GUIContent("Create/" + a.Key), false, (t) => {
								var kv = (KeyValuePair < string, Type>)t;
								var newObject = CreateInstance(kv.Value);
								var child = sequence.CreateNode(newObject);
								child.Position = new Rect(mousePos, child.Position.size);
							}, a);
							i++;
						}

						menu.ShowAsContext();
					}
				}
				break;
		case EventType.Repaint: 
			// Draw selection rect 
			if (GUIUtility.hotControl == GetHashCode ()) {
				UpdateSelection ();
				Handles.BeginGUI();
				Handles.color = Color.white;
				Handles.DrawSolidRectangleWithOutline (
					Rect.MinMaxRect (startPoint.x, startPoint.y, Event.current.mousePosition.x, Event.current.mousePosition.y), 
					new Color (.3f, .3f, .3f, .3f),
					Color.gray);
				Handles.EndGUI ();
			}
			break;
		}

        GUI.EndScrollView();

		Sequence.current = null;
    }


	/*************************
	 *  Selection
	 * **********************/

	void UpdateSelection(){

		var xmin = Math.Min (startPoint.x, Event.current.mousePosition.x);
		var ymin = Math.Min (startPoint.y, Event.current.mousePosition.y);
		var xmax = Math.Max (startPoint.x, Event.current.mousePosition.x);
		var ymax = Math.Max (startPoint.y, Event.current.mousePosition.y);
		selection = sequence.Nodes.ToList().FindAll (node => 
			RectContains(Rect.MinMaxRect (xmin, ymin, xmax, ymax), node.Position)
		);
		Repaint ();
	}

    /**************************
     * Possible node contents *
     **************************/

    private Dictionary<string, Type> possibleCreationsCache;
    private Dictionary<string, Type> GetPossibleCreations()
    {
        if (possibleCreationsCache == null)
        {
            possibleCreationsCache = new Dictionary<string, Type>();
            // Make sure is a DOMWriter
            var contents = GetTypesWith<NodeContentAttribute>(true).Where(t => (typeof(UnityEngine.Object)).IsAssignableFrom(t));
            foreach (var content in contents)
            {
                foreach (var attr in content.GetCustomAttributes(typeof(NodeContentAttribute), true))
                {
                    var nodeContent = attr as NodeContentAttribute;
                    var name = nodeContent.Name == string.Empty ? content.ToString() : nodeContent.Name;
                    possibleCreationsCache.Add(name, content);
                }
            }
        }
        return possibleCreationsCache;
    }

	bool RectContains (Rect r1, Rect r2){
		var intersection = Rect.MinMaxRect (Mathf.Max (r1.xMin, r2.xMin), Mathf.Max (r1.yMin, r2.yMin), Mathf.Min (r1.xMax, r2.xMax), Mathf.Min (r1.yMax, r2.yMax));

		return Event.current.shift 
			? r1.xMin < r2.xMin && r1.xMax > r2.xMax && r1.yMin < r2.yMin && r1.yMax > r2.yMax // Completely inside
				:  intersection.width > 0 && intersection.height > 0;
	}

    static IEnumerable<Type> GetTypesWith<TAttribute>(bool inherit)
                          where TAttribute : System.Attribute
    {
        return from a in AppDomain.CurrentDomain.GetAssemblies()
               from t in a.GetTypes()
               where System.Attribute.GetCustomAttributes(t, typeof(TAttribute), true).Length > 0 && !t.IsAbstract
               select t;
    }
}