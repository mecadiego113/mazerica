using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.EventSystems;

[System.Serializable]
public enum MazePieceType
{
    None = 0,
    Line = 1,
    Deadlock = 2,
    Triple = 3,
    Corner = 4,
    Crossing = 5,
    Start = 6,
    Finish = 7,
    DoubleCorner = 8,
    Intersection = 9,
    DeadlockCorner = 10,
    DeadlockLine = 11,
    DeadlockTriple = 12,
    DeadlockCrossing = 13,
    TripleDeadlock = 14,
    LineDeadlock = 15,
    LineDeadlockLine = 16,
    CornerDeadlockLeft = 17,
    CornerDeadlockRight = 18,
    CornerDeadlockCorner = 19
}

[System.Serializable]
public class MazePiece
{
    [SerializeField] private MazePieceType type;
    [SerializeField] private bool require;

    public MazePieceType getType()
    {
        return type;
    }

    public bool isRequire()
    {
        return require;
    }

    public bool use = false;
    public float frequency = 0.05f;
    public List<GameObject> geometryList = new List<GameObject>();
    public List<MazeOutput> outputList;

    [SerializeField] private float rotation;

    public MazePiece(MazePieceType type, bool require, bool use, float frequency, List<MazeOutput> outputList)
    {
        this.type = type;
        this.require = require;
        this.use = use;
        this.frequency = frequency;
        this.outputList = outputList;
    }

    /// <summary>
    /// Verifies that the passages into parameters coincide with passages in the piece.
    /// </summary>
    /// <returns><c>true</c>, if the piece corresponds to the passages, <c>false</c> otherwise.</returns>
    /// <param name="sourceOutputs">List of passages</param>
    public bool checkFit(List<MazeOutput> sourceOutputs)
    {
        if (sourceOutputs == null)
        {
            if (outputList.Count > 0) return false;
            else return true;
        }

        rotation = 0;
        for (int i = 0; i < 4; i++)
        {
            if (check(sourceOutputs)) return true;
            rotation += 90;
            rotate(sourceOutputs);
        }
        return false;
    }

    /// <summary>
    /// Getting the current rotation angle of the maze piece
    /// </summary>
    /// <returns>Current rotation angle of the maze piece</returns>
    public float getRotation()
    {
        return rotation;
    }

    private void rotate(List<MazeOutput> sourceOutputs)
    {
        int sourceOutputsCount = sourceOutputs.Count;
        for (int i = 0; i < sourceOutputsCount; i++)
        {
            MazeOutput sourceOutput = sourceOutputs[i];
            List<MazeOutputDirection> directions = sourceOutput.outputDirList;
            int directionCount = directions.Count;
            for (int j = 0; j < directionCount; j++)
            {
                directions[j] = MazeOutput.rotateCW[directions[j]];
            }
        }
    }

    private bool check(List<MazeOutput> sourceOutputs)
    {
        if (outputList.Count != sourceOutputs.Count) return false;

        int found = 0;
        int outputCount = outputList.Count;
        int sourceOutputsCount = sourceOutputs.Count;
        for (int oi = 0; oi < outputCount; oi++)
        {
            List<MazeOutputDirection> outputDirections = outputList[oi].outputDirList;
            for (int si = 0; si < sourceOutputsCount; si++)
            {
                List<MazeOutputDirection> sourceOutputDirections = sourceOutputs[si].outputDirList;
                if (outputDirections.Count == sourceOutputDirections.Count)
                {
                    int contains = 0;
                    int outputDirectionsCount = outputDirections.Count;
                    for (int di = 0; di < outputDirectionsCount; di++)
                    {
                        if (outputDirections.Contains(sourceOutputDirections[di])) contains++;
                    }
                    if (contains == outputDirectionsCount) found++;
                }
            }
        }

        if (found == outputList.Count) return true;
        else return false;
    }
}