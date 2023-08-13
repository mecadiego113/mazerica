using System;
using UnityEngine;

[Serializable]
public class MazeVector2Int
{
    public int x;
    public int y;

    public MazeVector2Int(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public virtual bool Equal(MazeVector2Int otherPoint)
    {
        return x == otherPoint.x && y == otherPoint.y;
    }

    public virtual MazeVector2Int clone()
    {
        return new MazeVector2Int(x, y);
    }

    public virtual void set(int nx, int ny)
    {
        this.x = nx;
        this.y = ny;
    }

    public Vector2 toVector2()
    {
        return new Vector2(x, -y);
    }

    public Vector3 toVector3()
    {
        return new Vector3(x, 0, -y);
    }

    public float sqrMagnitude(MazeVector2Int otherPoint)
    {
        float tx = x - otherPoint.x;
        float ty = y - otherPoint.y;
        return tx * tx + ty * ty;
    }
}