using UnityEngine;
using UnityEditor;
using System.Reflection;

[CustomEditor(typeof(MazePiecePack))]
public class MazePiecePackInspector : Editor
{
    private MazePiecePack mazePiecePack;
    private SerializedProperty customPieces;

    private Texture2D pieceIconNone;
    private Texture2D pieceIconLine;
    private Texture2D pieceIconDeadlock;
    private Texture2D pieceIconTriple;
    private Texture2D pieceIconCorner;
    private Texture2D pieceIconCrossing;
    private Texture2D pieceIconStart;
    private Texture2D pieceIconFinish;
    private Texture2D pieceIconDoubleCorner;
    private Texture2D pieceIconIntersection;
    private Texture2D pieceIconDeadlockCorner;
    private Texture2D pieceIconDeadlockLine;
    private Texture2D pieceIconDeadlockTriple;
    private Texture2D pieceIconDeadlockCrossing;
    private Texture2D pieceIconTripleDeadlock;
    private Texture2D pieceIconLineDeadlock;
    private Texture2D pieceIconLineDeadlockLine;
    private Texture2D pieceIconCornerDeadlockLeft;
    private Texture2D pieceIconCornerDeadlockRight;
    private Texture2D pieceIconCornerDeadlockCorner;

    private Texture2D addButton;
    private Texture2D removeButton;

    private GUIStyle imageButton;
    private GUIStyle dragAndDropLabelStyle;
    private Color greyLight;

    private Vector2 scrollPosition;
    private static bool basePiecesFoldout = true;
    private static bool additionalPiecesFoldout = true;

    private SerializedProperty inited;

    private void OnEnable()
    {
        mazePiecePack = (MazePiecePack)target;

        inited = serializedObject.FindProperty("inited");
        greyLight = EditorGUIUtility.isProSkin ? new Color(0.25f, 0.25f, 0.25f) : new Color(0.8f, 0.8f, 0.8f);

        customPieces = serializedObject.FindProperty("dragAndDropPieceGeometryArray");
    }

    private void initStyle()
    {
        imageButton = new GUIStyle(GUI.skin.button);
        imageButton.normal.background = null;

        dragAndDropLabelStyle = new GUIStyle(GUI.skin.label);
        dragAndDropLabelStyle.alignment = TextAnchor.MiddleCenter;
    }

    public override void OnInspectorGUI()
    {
        if (imageButton == null) initStyle();

        serializedObject.Update();
        if (!inited.boolValue)
        {
            MethodInfo awakeMethod = mazePiecePack.GetType().GetMethod("Awake", BindingFlags.NonPublic | BindingFlags.Instance);
            awakeMethod.Invoke(mazePiecePack, null);
        }

        GUI.changed = false;
        Undo.RecordObject(target, "MazePiecePackChnaged");

        GUILayout.Space(5);

        basePiecesFoldout = EditorGUILayout.Foldout(basePiecesFoldout, "Base Pieces");
        if (basePiecesFoldout)
        {
            GUILayout.BeginHorizontal();
            drawPiece(MazePieceType.None, "None", pieceIconNone, true);
            drawPiece(MazePieceType.Line, "Line", pieceIconLine);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            drawPiece(MazePieceType.Deadlock, "Deadlock", pieceIconDeadlock);
            drawPiece(MazePieceType.Triple, "Triple", pieceIconTriple, true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            drawPiece(MazePieceType.Corner, "Corner", pieceIconCorner);
            drawPiece(MazePieceType.Crossing, "Crossing", pieceIconCrossing, true);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            drawPiece(MazePieceType.Start, "Start", pieceIconStart);
            drawPiece(MazePieceType.Finish, "Finish", pieceIconFinish);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
        }

        additionalPiecesFoldout = EditorGUILayout.Foldout(additionalPiecesFoldout, "Additional Pieces");
        if (additionalPiecesFoldout)
        {
            GUILayout.BeginHorizontal();
            drawPiece(MazePieceType.DoubleCorner, "Double Corner", pieceIconDoubleCorner, true);
            drawPiece(MazePieceType.Intersection, "Intersection", pieceIconIntersection, true);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            drawPiece(MazePieceType.DeadlockCorner, "Deadlock Corner", pieceIconDeadlockCorner, true);
            drawPiece(MazePieceType.DeadlockLine, "Deadlock Line", pieceIconDeadlockLine, true);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            drawPiece(MazePieceType.DeadlockTriple, "Deadlock Triple", pieceIconDeadlockTriple, true);
            drawPiece(MazePieceType.DeadlockCrossing, "Deadlock Crossing", pieceIconDeadlockCrossing, true);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            drawPiece(MazePieceType.TripleDeadlock, "Triple Deadlock", pieceIconTripleDeadlock, true);
            drawPiece(MazePieceType.LineDeadlock, "Line Deadlock", pieceIconLineDeadlock, true);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            drawPiece(MazePieceType.LineDeadlockLine, "Line Deadlock Line", pieceIconLineDeadlockLine, true);
            drawPiece(MazePieceType.CornerDeadlockLeft, "Corner Deadlock Left", pieceIconCornerDeadlockLeft, true);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);

            GUILayout.BeginHorizontal();
            drawPiece(MazePieceType.CornerDeadlockRight, "Corner Deadlock Right", pieceIconCornerDeadlockRight, true);
            drawPiece(MazePieceType.CornerDeadlockCorner, "Corner Deadlock Corner", pieceIconCornerDeadlockCorner, true);
            GUILayout.EndHorizontal();

            GUILayout.Space(5);
        }

        checkDragAndDrop();

        if (GUI.changed) EditorUtility.SetDirty(mazePiecePack);
    }

    private void drawPiece(MazePieceType pieceType, string pieceName, Texture2D pieceIcon, bool specOptions = false)
    {
        MazePiece piece = mazePiecePack.GetPiece(pieceType);

        GUILayout.Box(pieceIcon);
        GUILayout.BeginVertical();
        {
            drawPieceGeometryList(pieceName, piece);

            bool found = false;
            bool errorFound = false;
            for (int i = 0; i < piece.geometryList.Count; i++)
            {
                if (piece.geometryList[i] != null)
                {
                    found = true;
                }
                else
                {
                    errorFound = true;
                }
            }

            if (piece.isRequire() && !found) EditorGUILayout.HelpBox("Piece geometry is required", MessageType.Warning);
            else if (errorFound) EditorGUILayout.HelpBox("One of the elements is null", MessageType.Warning);

            if (specOptions)
            {
                GUILayout.BeginHorizontal();
                {
                    GUILayout.Space(20);
                    GUILayout.Label("Use");
                    piece.use = EditorGUILayout.Toggle(piece.use, GUILayout.Width(15));
                }
                GUILayout.EndHorizontal();

                if (piece.use)
                {
                    GUILayout.BeginHorizontal();
                    {
                        GUILayout.Space(20);
                        GUILayout.Label("Frequency", GUILayout.MinWidth(40));
                        piece.frequency = Mathf.Clamp01(EditorGUILayout.FloatField(piece.frequency, GUILayout.Width(32)));
                    }
                    GUILayout.EndHorizontal();
                }
            }
        }
        GUILayout.EndVertical();
    }

    private void drawPieceGeometryList(string pieceName, MazePiece piece)
    {
        GUILayout.BeginHorizontal();
        {
            if (GUILayout.Button(addButton, GUIStyle.none, GUILayout.Width(16), GUILayout.Height(20)))
                piece.geometryList.Add(null);

            Rect rect = GUILayoutUtility.GetRect(50, 150, 10, 20);
            rect.x += 3;
            rect.y += 2;
            EditorGUI.LabelField(rect, pieceName);
        }
        GUILayout.EndHorizontal();
        if (piece.geometryList.Count > 0)
        {
            for (int i = 0; i < piece.geometryList.Count; i++)
            {
                GUILayout.BeginHorizontal();
                {
                    bool remove = GUILayout.Button(removeButton, GUIStyle.none, GUILayout.Width(16), GUILayout.Height(20));
                    piece.geometryList[i] = (GameObject)EditorGUILayout.ObjectField(piece.geometryList[i], typeof(GameObject), true, GUILayout.MinWidth(80));
                    if (remove)
                    {
                        piece.geometryList.RemoveAt(i);
                        i--;
                    }
                }
                GUILayout.EndHorizontal();
            }
        }
        else
        {
            GUILayout.BeginHorizontal();
            {
                GUILayout.Space(20);
                GameObject gameObject = (GameObject)EditorGUILayout.ObjectField(null, typeof(GameObject), true, GUILayout.MinWidth(80));
                if (gameObject != null) piece.geometryList.Add(gameObject);
            }
            GUILayout.EndHorizontal();
        }
    }

    private void checkDragAndDrop()
    {
        for (int i = 0; i < customPieces.arraySize; i++)
        {
            GameObject go = (GameObject)customPieces.GetArrayElementAtIndex(i).objectReferenceValue;
            string goName = go.name.ToLower();

            string[] cornerSplit = goName.Split(new string[] { "corner" }, System.StringSplitOptions.None);
            if (cornerSplit.Length > 1)
            {
                string[] deadlockSplit0 = cornerSplit[0].Split(new string[] { "deadlock" }, System.StringSplitOptions.None);
                string[] deadlockSplit1 = cornerSplit[1].Split(new string[] { "deadlock" }, System.StringSplitOptions.None);
                if (deadlockSplit0.Length > 1)
                {
                    mazePiecePack.GetPiece(MazePieceType.DeadlockCorner).geometryList.Add(go);
                }
                else if (deadlockSplit1.Length > 1)
                {
                    if (cornerSplit.Length == 3)
                    {
                        mazePiecePack.GetPiece(MazePieceType.CornerDeadlockCorner).geometryList.Add(go);
                    }
                    else if (deadlockSplit1[1].Contains("left"))
                    {
                        mazePiecePack.GetPiece(MazePieceType.CornerDeadlockLeft).geometryList.Add(go);
                    }
                    else if (deadlockSplit1[1].Contains("right"))
                    {
                        mazePiecePack.GetPiece(MazePieceType.CornerDeadlockRight).geometryList.Add(go);
                    }
                }
                else
                {
                    if (deadlockSplit0[0].Contains("double"))
                    {
                        mazePiecePack.GetPiece(MazePieceType.DoubleCorner).geometryList.Add(go);
                    }
                    else
                    {
                        mazePiecePack.GetPiece(MazePieceType.Corner).geometryList.Add(go);
                    }
                }
            }
            else
            {
                string[] deadlockSplit = cornerSplit[0].Split(new string[] { "deadlock" }, System.StringSplitOptions.None);
                if (deadlockSplit.Length > 1)
                {
                    if (deadlockSplit[0].Contains("line"))
                    {
                        if (deadlockSplit[1].Contains("line"))
                            mazePiecePack.GetPiece(MazePieceType.LineDeadlockLine).geometryList.Add(go);
                        else
                            mazePiecePack.GetPiece(MazePieceType.LineDeadlock).geometryList.Add(go);
                    }
                    else if (deadlockSplit[0].Contains("line"))
                    {
                        mazePiecePack.GetPiece(MazePieceType.LineDeadlock).geometryList.Add(go);
                    }
                    else if (deadlockSplit[0].Contains("triple"))
                    {
                        mazePiecePack.GetPiece(MazePieceType.TripleDeadlock).geometryList.Add(go);
                    }
                    else if (deadlockSplit[1].Contains("triple"))
                    {
                        mazePiecePack.GetPiece(MazePieceType.DeadlockTriple).geometryList.Add(go);
                    }
                    else if (deadlockSplit[1].Contains("crossing"))
                    {
                        mazePiecePack.GetPiece(MazePieceType.DeadlockCrossing).geometryList.Add(go);
                    }
                    else if (deadlockSplit[1].Contains("line"))
                    {
                        mazePiecePack.GetPiece(MazePieceType.DeadlockLine).geometryList.Add(go);
                    }
                    else
                    {
                        mazePiecePack.GetPiece(MazePieceType.Deadlock).geometryList.Add(go);
                    }
                }
                else
                {
                    if (deadlockSplit[0].Contains("crossing"))
                    {
                        mazePiecePack.GetPiece(MazePieceType.Crossing).geometryList.Add(go);
                    }
                    else if (deadlockSplit[0].Contains("finish"))
                    {
                        mazePiecePack.GetPiece(MazePieceType.Finish).geometryList.Add(go);
                    }
                    else if (deadlockSplit[0].Contains("intersection"))
                    {
                        mazePiecePack.GetPiece(MazePieceType.Intersection).geometryList.Add(go);
                    }
                    else if (deadlockSplit[0].Contains("line"))
                    {
                        mazePiecePack.GetPiece(MazePieceType.Line).geometryList.Add(go);
                    }
                    else if (deadlockSplit[0].Contains("none"))
                    {
                        mazePiecePack.GetPiece(MazePieceType.None).geometryList.Add(go);
                    }
                    else if (deadlockSplit[0].Contains("start"))
                    {
                        mazePiecePack.GetPiece(MazePieceType.Start).geometryList.Add(go);
                    }
                    else if (deadlockSplit[0].Contains("triple"))
                    {
                        mazePiecePack.GetPiece(MazePieceType.Triple).geometryList.Add(go);
                    }
                }
            }
        }

        customPieces.ClearArray();
    }
}