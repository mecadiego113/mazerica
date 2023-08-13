using System.Collections.Generic;

public class ListUtilities
{
    public static void Shuffle<T>(List<T> list)
    {
        int count = list.Count;
        int halfCount = count / 2;

        for (int i = 0; i < halfCount; i++)
        {
            int j = MazeMath.getRandom(i, count);

            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    public static bool Has(List<MazeVector2Int> list, MazeVector2Int element)
    {
        List<MazeVector2Int>.Enumerator listEnumerator = list.GetEnumerator();

        while (listEnumerator.MoveNext())
        {
            if (listEnumerator.Current.Equal(element))
            {
                return true;
            }
        }

        return false;
    }

    public static bool Has(List<MazeVector2IntDir> list, MazeVector2Int element)
    {
        List<MazeVector2IntDir>.Enumerator listEnumerator = list.GetEnumerator();

        while (listEnumerator.MoveNext())
        {
            if (listEnumerator.Current.Equal(element))
            {
                return true;
            }
        }

        return false;
    }

    public static bool Has(List<MazeVector2IntDir> list, MazeVector2IntDir element)
    {
        List<MazeVector2IntDir>.Enumerator listEnumerator = list.GetEnumerator();
        while (listEnumerator.MoveNext())
            if (listEnumerator.Current.Equal(element))
                return true;
        return false;
    }

    public static MazeVector2IntDir Get(List<MazeVector2IntDir> list, MazeVector2IntDir element)
    {
        List<MazeVector2IntDir>.Enumerator listEnumerator = list.GetEnumerator();

        while (listEnumerator.MoveNext())
        {
            if (listEnumerator.Current.Equal(element))
            {
                return listEnumerator.Current;
            }
        }

        return null;
    }

    public static MazeVector2IntDir Get(List<MazeVector2IntDir> list, MazeVector2Int element)
    {
        List<MazeVector2IntDir>.Enumerator listEnumerator = list.GetEnumerator();

        while (listEnumerator.MoveNext())
        {
            if (listEnumerator.Current.Equal(element))
            {
                return listEnumerator.Current;
            }
        }

        return null;
    }

    public static bool has(List<MazeVector2IntDir> list, int ix, int iy)
    {
        List<MazeVector2IntDir>.Enumerator listEnumerator = list.GetEnumerator();

        while (listEnumerator.MoveNext())
        {
            if (listEnumerator.Current.x == ix && listEnumerator.Current.y == iy)
            {
                return true;
            }
        }

        return false;
    }
}