using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GenerateDungeon : MonoBehaviour
{
        [SerializeField] GameObject blockPrefab;
        [SerializeField]  GameObject diamondBlock;
        [SerializeField] Transform cubeContainer;
        [SerializeField] GameObject populationManager;
        public List<List<bool>> level = new List<List<bool>>();
        List<Point> generalVisited = new List<Point>();
        List<Point> seekerVisited = new List<Point>();
       public  Point wanderer;
        Point seeker;
        int rows;
        int columns;
        // Start is called before the first frame update
        void Start()
        {
                rows = Random.Range(20, 40);
                columns = Random.Range(rows, rows * 2);
                wanderer = new Point(1, Random.Range(0, columns-1));
                seeker = new Point(rows - 2, Random.Range(0, columns-1));
                Debug.Log(seeker.row + " " + rows);
                GenerateLevel();
                BuildLevel();
                Instantiate(diamondBlock, new Vector3(wanderer.row, 0.5f, wanderer.column),Quaternion.identity);
                Instantiate(populationManager, new Vector3(seeker.row, 0.5f, seeker.column), Quaternion.identity);
        }

        private void BuildLevel()
        {
                for (int row = 0; row < rows; row++)
                {
                        for (int column = 0; column < columns; column++)
                        {
                                if (!level[row][column])
                                {
                                        Vector3 positionA = new Vector3(row, 0.5f, column);
                                        Vector3 positionB = new Vector3(row, 1.5f, column);

                                        Instantiate(blockPrefab, positionA, Quaternion.identity).transform.parent = cubeContainer;
                                        Instantiate(blockPrefab, positionB, Quaternion.identity).transform.parent = cubeContainer;

                                }
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
                for(int r = 0; r < rows; r++)
                {
                        List<bool> row = new List<bool>();
                        for(int c = 0; c < columns; c++)
                        {
                                row.Add(false);
                        }
                        level.Add(row);
                }
        }

         void CreateLevel()
        {

                Point s = seeker;
                Point w = wanderer;

                while (!generalVisited.Contains(s))
                {

                        level[w.row][w.column] = true;
                        level[s.row][s.column] = true;
                        seekerVisited.Add(s);
                        generalVisited.Add(w);
                        s = GetSeekerNextMove(s,w);
                        w = GetWandererNextMove(w);
                }
        }

        List<Point> GetMoves(Point point)
        {
                Point up = new Point(point.row - 1, point.column);
                Point down = new Point(point.row + 1, point.column);
                Point left = new Point(point.row, point.column - 1);
                Point right = new Point(point.row, point.column + 1);
                List<Point> moves = new List<Point>() { up, down, left, right };
                return moves.FindAll(p =>
                                1 <= p.row &&
                                p.row < rows - 1 &&
                                1 <= p.column &&
                                p.column < columns - 1 &&
                                !generalVisited.Contains(p) &&
                                !level[p.row][p.column]
                        ).ToList();

        }

        Point GetWandererNextMove(Point w)
        {
                List<Point> moves = GetMoves(w);

                if (moves.Count > 0)
                {
                        return moves[Random.Range(0, moves.Count)];
                }

                return generalVisited[Random.Range(0, generalVisited.Count)];
        }

        float Distance(Point p, Point w)
        {
                return Mathf.Abs(p.row - w.row) + Mathf.Abs(p.column - w.column);
        }

        Point GetSeekerNextMove(Point s, Point w)
        {
                float distance = Distance(s, w);

                List<Point> moves = GetMoves(s).FindAll
                        (p =>
                                Distance(p, w)  < distance &&
                                !seekerVisited.Contains(p)
                        );

                if (moves.Count > 0)
                        return moves[Random.Range(0, moves.Count)];

                if(generalVisited.Contains(s))
                {
                        foreach( Point p in seekerVisited)
                        {
                                generalVisited.Add(p);
                        }

                }
                seekerVisited.Clear();
                return new Point(Random.Range(1, rows - 1), Random.Range(1, columns - 1));
        }
}
