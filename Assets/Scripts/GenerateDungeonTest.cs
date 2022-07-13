using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class GenerateDungeonTest : MonoBehaviour
{
        [SerializeField] GameObject blockPrefab;
        [SerializeField] GameObject roofPrefab;
        [SerializeField] Transform cubeContainer;

        public List<List<byte>> level = new List<List<byte>>();
        
        List<Vector2Int> generalVisited = new List<Vector2Int>();
        List<Vector2Int> seekerVisited = new List<Vector2Int>();
        HashSet<Vector2Int> generalVisitedSet = new HashSet<Vector2Int>();
        
        public Vector2Int wanderer;
        Vector2Int seeker;
        [SerializeField] int rows = 50;
        [SerializeField] int columns = 50;
        [SerializeField] int height = 3;
        [SerializeField] int seed;

        float levelSize;

        // Start is called before the first frame update
        void Start()
        {

                Random.InitState( seed ) ;
                wanderer = new Vector2Int(1, Random.Range(0, columns - 1));
                seeker = new Vector2Int(rows - 2, Random.Range(0, columns - 1));
                levelSize = Random.Range(0.3f, 0.6f);
                GenerateLevel();
                BuildLevel();
        }


        private void BuildLevel()
        {
                foreach(Vector2Int point in generalVisitedSet)
                {
                        BuildHelper(point);
                }
        }

        private void BuildHelper(Vector2Int point)
        {
                List<Vector2Int> neighbors = GetNeighbors(point);


                foreach (Vector2Int v in neighbors)
                {
                        Vector3 pos = new Vector3(v.x,  0.5f, v.y);
                        for (float i = 0; i < height - 1; i++)
                        {
                                pos.y += i;
                                Instantiate(blockPrefab, pos, Quaternion.identity).
                                transform.parent = cubeContainer;
                        }
                        level[v.x][v.y] = 2;
                } 
        }

        private List<Vector2Int> GetNeighbors(Vector2Int point)
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
                        level[p.x][p.y] == 0
                ).ToList();
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
                        List<byte> row = new List<byte>();

                        for (int c = 0; c < columns; c++)
                        {
                                row.Add(0);
                        }
                        level.Add(row);
                }
        }



        void CreateLevel() 
        {
                Vector2Int s = seeker;
                Vector2Int w = wanderer;
                float size = rows * columns;
                float levelSize = 0;

                while ( (!generalVisitedSet.Contains(s)) || (levelSize < this.levelSize) || (seekerVisited.Count > 0) )
                {
                        
                        level[w.x][w.y] = 1;
                        seekerVisited.Add(s);
                        generalVisited.Add(w);
                        generalVisitedSet.Add(w);
                        s = GetSeekerNextMove(s, w);
                        w = GetWandererNextMove(w);
                        levelSize =  generalVisitedSet.Count / size;
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
                                !generalVisitedSet.Contains(p)
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
                //Manhattan Distance
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
                                level[p.x][p.y] = 1;
                                generalVisited.Add(p);
                                generalVisitedSet.Add(p);
                        }

                }
                seekerVisited.Clear();
                return new Vector2Int(Random.Range(1, rows - 1), Random.Range(1, columns - 1));
        }
}