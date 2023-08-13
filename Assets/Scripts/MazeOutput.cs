using UnityEngine;
using System.Collections.Generic;
using System;

[Serializable]
public enum MazeOutputDirection
{
    NotSpecified = 4,
    N = 0,
    E = 1,
    S = 2,
    W = 3
}

[Serializable]
public class MazeOutput
{
    public static Dictionary<MazeOutputDirection, int> dx = new Dictionary<MazeOutputDirection, int>() { { MazeOutputDirection.N, 0 }, { MazeOutputDirection.E, 1 }, { MazeOutputDirection.S, 0 }, { MazeOutputDirection.W, -1 } };
    public static Dictionary<MazeOutputDirection, int> dy = new Dictionary<MazeOutputDirection, int>() { { MazeOutputDirection.N, -1 }, { MazeOutputDirection.E, 0 }, { MazeOutputDirection.S, 1 }, { MazeOutputDirection.W, 0 } };
    public static Dictionary<MazeOutputDirection, MazeOutputDirection> opposite = new Dictionary<MazeOutputDirection, MazeOutputDirection>() { { MazeOutputDirection.N, MazeOutputDirection.S }, { MazeOutputDirection.E, MazeOutputDirection.W }, { MazeOutputDirection.S, MazeOutputDirection.N }, { MazeOutputDirection.W, MazeOutputDirection.E } };
    public static Dictionary<MazeOutputDirection, MazeOutputDirection> rotateCW = new Dictionary<MazeOutputDirection, MazeOutputDirection>() { { MazeOutputDirection.N, MazeOutputDirection.E }, { MazeOutputDirection.E, MazeOutputDirection.S }, { MazeOutputDirection.S, MazeOutputDirection.W }, { MazeOutputDirection.W, MazeOutputDirection.N } };
    public static Dictionary<MazeOutputDirection, MazeOutputDirection> rotateCCW = new Dictionary<MazeOutputDirection, MazeOutputDirection>() { { MazeOutputDirection.N, MazeOutputDirection.W }, { MazeOutputDirection.W, MazeOutputDirection.S }, { MazeOutputDirection.S, MazeOutputDirection.E }, { MazeOutputDirection.E, MazeOutputDirection.N } };

    public static MazeOutput GetShuffleOutput()
    {
        MazeOutput mazeOutput = new MazeOutput();

        mazeOutput.outputDirList = new List<MazeOutputDirection>
        {
            MazeOutputDirection.N,
            MazeOutputDirection.E,
            MazeOutputDirection.S,
            MazeOutputDirection.W
        };

        ListUtilities.Shuffle(mazeOutput.outputDirList);

        return mazeOutput;
    }

    [SerializeField] public List<MazeOutputDirection> outputDirList;

    public MazeOutput()
    {
        outputDirList = new List<MazeOutputDirection>();
    }

    public MazeOutput(List<MazeOutputDirection> _direction)
    {
        outputDirList = _direction;
    }
}