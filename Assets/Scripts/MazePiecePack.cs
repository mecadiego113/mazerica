using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
[ExecuteInEditMode]
public class MazePiecePack : MonoBehaviour
{
    [SerializeField] private MazePiece[] mazePieceArray = new MazePiece[20];
    [SerializeField] private bool inited = false;
    [SerializeField] private bool inited2 = false;
    [SerializeField] private List<GameObject> dragAndDropPieceGeometryArray = new List<GameObject>();

    private void Awake()
    {
        if (!inited)
        {
            inited = true;
            dragAndDropPieceGeometryArray.Clear();

            AddPiece(MazePieceType.None, true, false, 0.05f, new MazeOutput());
            AddPiece(MazePieceType.Line, true, true, 1,
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.N, MazeOutputDirection.S }));
            AddPiece(MazePieceType.Deadlock, true, true, 1,
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.E }));
            AddPiece(MazePieceType.Triple, true, false, 0.05f,
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.N, MazeOutputDirection.W, MazeOutputDirection.S }));
            AddPiece(MazePieceType.Corner, true, true, 1,
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.E, MazeOutputDirection.S }));
            AddPiece(MazePieceType.Crossing, true, false, 0.05f,
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.N, MazeOutputDirection.E, MazeOutputDirection.S, MazeOutputDirection.W }));

            AddPiece(MazePieceType.Start, false, true, 1,
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.N }));
            AddPiece(MazePieceType.Finish, false, true, 1,
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.N }));

            AddPiece(MazePieceType.DoubleCorner, false, false, 0.5f,
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.N, MazeOutputDirection.W }),
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.E, MazeOutputDirection.S }));
            AddPiece(MazePieceType.Intersection, false, false, 0.2f,
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.N, MazeOutputDirection.S }),
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.W, MazeOutputDirection.E }));

            AddPiece(MazePieceType.DeadlockCorner, false, false, 0.1f,
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.S }),
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.E }));
            AddPiece(MazePieceType.DeadlockLine, false, false, 0.1f,
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.N }),
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.S }));

            AddPiece(MazePieceType.DeadlockTriple, false, false, 0.1f,
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.E }),
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.W }),
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.S }));
            AddPiece(MazePieceType.DeadlockCrossing, false, false, 0.1f,
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.N }),
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.S }),
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.E }),
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.W }));

            AddPiece(MazePieceType.TripleDeadlock, false, false, 0.1f,
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.N, MazeOutputDirection.E, MazeOutputDirection.S }),
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.W }));
            AddPiece(MazePieceType.LineDeadlock, false, false, 0.1f,
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.N, MazeOutputDirection.S }),
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.W }));

            AddPiece(MazePieceType.LineDeadlockLine, false, false, 0.1f,
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.N, MazeOutputDirection.S }),
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.E }),
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.W }));
            AddPiece(MazePieceType.CornerDeadlockLeft, false, false, 0.1f,
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.E, MazeOutputDirection.S }),
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.W }));

            AddPiece(MazePieceType.CornerDeadlockRight, false, false, 0.1f,
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.E, MazeOutputDirection.S }),
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.N }));
            AddPiece(MazePieceType.CornerDeadlockCorner, false, false, 0.1f,
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.E, MazeOutputDirection.S }),
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.W }),
                     new MazeOutput(new List<MazeOutputDirection> { MazeOutputDirection.N }));
        }

        if (!inited2)
        {
            inited2 = true;

            GetPiece(MazePieceType.None).use = false;
            GetPiece(MazePieceType.None).frequency = 0.05f;

            GetPiece(MazePieceType.Triple).use = false;
            GetPiece(MazePieceType.Triple).frequency = 0.05f;

            GetPiece(MazePieceType.Crossing).use = false;
            GetPiece(MazePieceType.Crossing).frequency = 0.05f;
        }
    }

    public MazePiece GetPiece(MazePieceType type)
    {
        return mazePieceArray[(int)type];
    }

    public List<MazePiece> GetMazePieceList()
    {
        List<MazePiece> result = new List<MazePiece>();
        foreach (MazePiece piece in mazePieceArray)
        {
            CheckGeometryList(piece);
            result.Add(piece);
        }
        return result;
    }

    private void CheckGeometryList(MazePiece piece)
    {
        List<GameObject> geometryList = piece.geometryList;
        for (int i = 0; i < geometryList.Count; i++)
        {
            if (geometryList[i] == null)
            {
                geometryList.RemoveAt(i);
                i--;
            }
        }
    }

    private void AddPiece(MazePieceType type, bool require, bool use, float frequency, params MazeOutput[] mazeOutput)
    {
        List<MazeOutput> outputs = new List<MazeOutput>();
        for (int i = 0; i < mazeOutput.Length; i++) outputs.Add(mazeOutput[i]);
        mazePieceArray[(int)type] = new MazePiece(type, require, use, frequency, outputs);
    }
}