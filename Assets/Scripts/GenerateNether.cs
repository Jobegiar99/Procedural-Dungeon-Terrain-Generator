using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GenerateNether : MonoBehaviour
{
        [SerializeField] GameObject blockPrefab;

        [SerializeField] Transform cubeContainer;
        public List<List<bool>> level = new List<List<bool>>();
        List<Vector2Int> generalVisited = new List<Vector2Int>();
        List<Vector2Int> seekerVisited = new List<Vector2Int>();
        HashSet<Vector2> generalVisitedSet = new HashSet<Vector2>();
        public Vector2Int wanderer;
        Vector2Int seeker;
        int rows;
        int columns;
        // Start is called before the first frame update
        void Start()
        {
                rows = Random.Range(40, 60);
                columns = Random.Range(40, 60);
                wanderer = new Vector2Int(1, Random.Range(0, columns - 1));
                seeker = new Vector2Int(rows - 2, Random.Range(0, columns - 1));
                Debug.Log(seeker.x + " " + rows);
                GenerateLevel();
                BuildLevel();
        }

        private void BuildLevel()
        {
                for (int row = 0; row < rows; row++)
                {
                        for (int column = 0; column < columns; column++)
                        {
                                if (!level[row][column])
                                {
                                        for (float i = 0; i < 5; i++)
                                        {
                                                Vector3 pos = new Vector3(row, i + 0.5f, column);

                                                Instantiate(blockPrefab, pos, Quaternion.identity).
                                                        transform.parent = cubeContainer;

                                        }
                                }
                                Vector3 position = new Vector3(row, 4.5f, column);
                        }
                }
        }

        void GenerateLevel()
        {
                FillLevel();

                CreateLevel();
        }

        void FillLevel()
        {
                for (int r = 0; r < rows; r++)
                {
                        List<bool> row = new List<bool>();
                        for (int c = 0; c < columns; c++)
                        {
                                row.Add(false);
                        }
                        level.Add(row);
                }
        }

        void CreateLevel()
        {

                Vector2Int s = seeker;
                Vector2Int w = wanderer;

                while (!generalVisited.Contains(s))
                {

                        level[w.x][w.y] = true;
                        level[s.x][s.y] = true;
                        seekerVisited.Add(s);
                        generalVisited.Add(w);
                        generalVisitedSet.Add(w);
                        s = GetSeekerNextMove(s, w);
                        w = GetWandererNextMove(w);
                }
        }

        List<Vector2Int> GetMoves(Vector2Int point)
        {
                Vector2Int up = new Vector2Int(point.x - 1, point.y);
                Vector2Int down = new Vector2Int(point.x + 1, point.y);
                Vector2Int left = new Vector2Int(point.x, point.y - 1);
                Vector2Int right = new Vector2Int(point.x, point.y + 1);
                List<Vector2Int> moves = new List<Vector2Int>() { up, down, left, right };
                return moves.FindAll(p =>
                                1 <= p.x &&
                                p.x < rows - 1 &&
                                1 <= p.y &&
                                p.y < columns - 1 &&
                                !generalVisitedSet.Contains(p) &&
                                !level[p.x][p.y]
                        ).ToList();

        }

        Vector2Int GetWandererNextMove(Vector2Int w)
        {
                List<Vector2Int> moves = GetMoves(w);

                if (moves.Count > 0)
                {
                        return moves[Random.Range(0, moves.Count)];
                }

                return generalVisited[Random.Range(0, generalVisited.Count)];
        }

        float Distance(Vector2Int p, Vector2Int w)
        {
                return Mathf.Abs(p.x - w.x) + Mathf.Abs(p.y - w.y);
        }

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
                                generalVisited.Add(p);
                                generalVisitedSet.Add(p);
                        }

                }
                seekerVisited.Clear();
                return new Vector2Int(Random.Range(1, rows - 1), Random.Range(1, columns - 1));
        }
}
