using UnityEngine;
using System.Collections.Generic;

namespace qtools.qmaze
{
    public class MazePieceData
    {
        public int x;
        public int y;

        public List<MazeOutput> outputs;
        public GameObject geometry;
        public MazePieceType type;
        public float rotation;

        public MazePieceData(int x, int y)
        {
            this.x = x;
            this.y = y;
            type = MazePieceType.None;
        }
    }
}