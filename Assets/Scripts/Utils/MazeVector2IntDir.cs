using System;

[Serializable]
public class MazeVector2IntDir : MazeVector2Int
{
    public MazeOutputDirection direction;

    public MazeVector2IntDir(int x, int y, MazeOutputDirection direction) : base(x, y)
    {
        this.direction = direction;
    }

    public override bool Equal(MazeVector2Int otherPoint)
    {
        return x == otherPoint.x && y == otherPoint.y;
    }

    public bool Equal(MazeVector2IntDir otherPoint)
    {
        return x == otherPoint.x && y == otherPoint.y && (direction == otherPoint.direction || direction == MazeOutputDirection.NotSpecified || otherPoint.direction == MazeOutputDirection.NotSpecified);
    }
}