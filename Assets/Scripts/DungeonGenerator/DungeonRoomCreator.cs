using System.Collections.Generic;
using UnityEngine;

public class DungeonRoomCreator
{
        #region Parameters

        /// <summary>
        /// The amount of  columns that the levels will contain.
        /// </summary>
        int columns;

        /// <summary>
        /// The list of cells visited either by Wanderer or Seeker.
        /// </summary>
        List<Vector2Int> generalVisited = new List<Vector2Int>();

        /// <summary>
        /// The set that will contain the information of the visited positions within the level
        /// </summary>
        public HashSet<Vector2Int> generalVisitedSet = new HashSet<Vector2Int>();

        /// <summary>
        /// The matrix that will contain the information of the terrain
        /// </summary>
        public List<List<byte>> level = new List<List<byte>>();

        /// <summary>
        /// Starting Seeker's position
        /// </summary>
        Vector2Int seeker;

        /// <summary>
        /// The list of current cells visited by Seeker
        /// </summary>
        List<Vector2Int> seekerVisited = new List<Vector2Int>();

        /// <summary>
        /// The amount of rows that the level will contain
        /// </summary>
        int rows;

        /// <summary>
        /// Starting Wanderer's position
        /// </summary>
        Vector2Int wanderer;

        /// <summary>
        /// The percentage of visited cells compared to the total amount of cells
        /// </summary>
        float roomSize;

        #endregion

        #region Constructor
        public DungeonRoomCreator(Vector2Int wanderer, Vector2Int seeker, int rows, int columns)
        {
                roomSize = Random.Range(0.4f, 0.7f);
                this.wanderer = wanderer;
                this.seeker = seeker;
                this.rows = rows;
                this.columns = columns;
                GenerateLevel();
        }
        #endregion

        #region Functions

        /// <summary>
        /// Wanderer and Seeker Algorithm
        /// </summary>
        void CreateLevel()
        {
                Vector2Int s = seeker;
                Vector2Int w = wanderer;
                float size = rows * columns;
                float currentRoomSize = 0;

                while ((!generalVisitedSet.Contains(s)) || (currentRoomSize < this.roomSize) || (seekerVisited.Count > 0))
                {

                        level[w.x][w.y] = 1;
                        seekerVisited.Add(s);
                        generalVisited.Add(w);
                        generalVisitedSet.Add(w);
                        s = GetSeekerNextMove(s, w);
                        w = GetWandererNextMove(w);
                        currentRoomSize = generalVisitedSet.Count / size;
                }
        }

        /// <summary>
        /// Calculates the Manhattan's distance between a point P and wanderer
        /// </summary>
        /// <param name="p">A point's position</param>
        /// <param name="w">Wanderer's position</param>
        /// <returns>The Manhattan's distance between point P and Wanderer</returns>
        float Distance(Vector2Int p, Vector2Int w)
        {
                //Manhattan Distance
                return Mathf.Abs(p.x - w.x) + Mathf.Abs(p.y - w.y);
        }

        /// <summary>
        /// Iterates through the amount of rows and columns to fill the empty level matrix
        /// </summary>
        void FillLevel()
        {
                for (int r = 0; r < rows; r++)
                {
                        List<byte> row = new List<byte>();

                        for (int c = 0; c < columns; c++)
                        {
                                row.Add(0);
                        }
                        level.Add(row);
                }
        }

        /// <summary>
        /// The main function that controls the flow of generating the level using the matrix
        /// </summary>
        void GenerateLevel()
        {

                FillLevel();

                CreateLevel();

        }

        /// <summary>
        /// Gets a list of valid movement options for a given point
        /// </summary>
        /// <param name="point">The point from which the function will try to obtain the valid movement list</param>
        /// <returns>A list of valid points to which the current point can move to</returns>
        List<Vector2Int> GetMoves(Vector2Int point)
        {
                Vector2Int up = new Vector2Int(point.x - 1, point.y);
                Vector2Int down = new Vector2Int(point.x + 1, point.y);
                Vector2Int left = new Vector2Int(point.x, point.y - 1);
                Vector2Int right = new Vector2Int(point.x, point.y + 1);
                List<Vector2Int> moves = new List<Vector2Int>() { up, down, left, right };

                return moves.FindAll(p =>
                                0 <= p.x &&
                                p.x < rows &&
                                0 <= p.y &&
                                p.y < columns &&
                                !generalVisitedSet.Contains(p)
                        );

        }

        /// <summary>
        /// Obtains the next position for Seeker
        /// </summary>
        /// <param name="s">Current seeker's position</param>
        /// <param name="w">Current wanderer's position</param>
        /// <returns></returns>
        Vector2Int GetSeekerNextMove(Vector2Int s, Vector2Int w)
        {
                float distance = Distance(s, w);

                List<Vector2Int> moves = GetMoves(s).FindAll
                        (p =>
                                Distance(p, w) < distance &&
                                !seekerVisited.Contains(p)
                        );

                if (moves.Count > 0)
                        return moves[Random.Range(0, moves.Count)];

                if (generalVisitedSet.Contains(s))
                {
                        foreach (Vector2Int p in seekerVisited)
                        {
                                level[p.x][p.y] = 1;
                                generalVisited.Add(p);
                                generalVisitedSet.Add(p);
                        }

                }
                seekerVisited.Clear();
                return new Vector2Int(Random.Range(0, rows), Random.Range(0, columns - 1));
        }

        /// <summary>
        /// Obtains the next position for wanderer
        /// </summary>
        /// <param name="w">Current wanderer's position</param>
        /// <returns>Wanderer's next position</returns>
        Vector2Int GetWandererNextMove(Vector2Int w)
        {
                List<Vector2Int> moves = GetMoves(w);

                if (moves.Count > 0)
                {
                        return moves[Random.Range(0, moves.Count)];
                }

                return generalVisited[Random.Range(0, generalVisited.Count)];
        }

        #endregion
}
