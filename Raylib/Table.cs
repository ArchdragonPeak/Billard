using System.Numerics;
using GameCore;
using Raylib_cs;

namespace Table
{
    public class Table
    {
        Vector2 anchor;
        private readonly int length;
        private readonly int width;

        private readonly float pocketRadius;
        private readonly Vector2[] pockets = new Vector2[6];

        private readonly PoolBall[] balls = new PoolBall[16];

        private readonly string[] players = new string[2];

        readonly Color[] poolColors =
        [
            new Color(245, 245, 245, 255), // 0      - white
            new Color(230, 200,  40, 255), // 1 / 9  - yellow
            new Color( 40,  90, 180, 255), // 2 / 10 - blue
            new Color(200,  50,  50, 255), // 3 / 11 - red
            new Color(140,  60, 160, 255), // 4 / 12 - purple
            new Color(235, 120,  40, 255), // 5 / 13 - orange
            new Color( 30, 110,  60, 255), // 6 / 14 - green
            new Color(110,  40,  40, 255), // 7 / 15 - Maroon
            new Color( 25,  25,  25, 255), // 8      - black
        ];

        Table(int length, int width, float pocketRadius, Vector2 anchor)
        {
            this.length = length;
            this.width = width;
            this.pocketRadius = pocketRadius;
            this.anchor = anchor;
            initPool();
        }

        // dimensions - LxH: 1000x500
        private void initPool()
        {
            pockets[0] = new(anchor.X, anchor.Y);
            pockets[1] = new(anchor.X + 500, anchor.Y);
            pockets[2] = new(anchor.X + 1000, anchor.Y);
            pockets[3] = new(anchor.X, anchor.Y + 500);
            pockets[4] = new(anchor.X + 500, anchor.Y + 500);
            pockets[5] = new(anchor.X + 1000, anchor.Y + 500);

        }

    }
}