using System;

public class MazeMath
{
    private static Random random = new Random();

    public static int GetRandom(int min, int max)
    {
        return random.Next(min, max);
    }

    public static float GetRandom()
    {
        return (float)random.NextDouble();
    }

    public static void SetSeed(int _seed)
    {
        random = new Random(_seed);
    }
}