using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Generate3DDungeon : MonoBehaviour
{
        [SerializeField] GameObject blockPrefab;
        [SerializeField] Transform cubeContainer;
        public List<List<List<bool>>> level = new List< List<List<bool>>>();

        List<Vector3Int> generalVisited = new List<Vector3Int>();
        List<Vector3Int> seekerVisited = new List<Vector3Int>();
        int maxIterations = 1000;
        public  Vector3Int wanderer;
        Vector3Int seeker;

        int X;
        int Y;
        int Z;

        // Start is called before the first frame update
        void Start()
        {
                X = Random.Range(10,20);
                Y = Random.Range(10,20);
                Z = Random.Range(10,20);
                maxIterations = X * Y * Z;

                wanderer = new Vector3Int(Random.Range(0,X-1), 0,1);
                seeker = new Vector3Int(Random.Range(0, X - 1), Y - 1, Z - 1);
                Debug.Log(X + " " + Y + " " + Z);
                GenerateLevel();
                BuildLevel();
        }
        
        private void BuildLevel()
        {
                
                for (int x = 0; x < X; x++)
                {
                        for (int y = 0; y < Y; y++)
                        {
                                for (int z = 0; z < Z; z++)
                                {
                                        if (!level[x][y][z])
                                        {
                                                Vector3Int position = new Vector3Int(x, y, z);


                                                GameObject cube = Instantiate(blockPrefab, position, Quaternion.identity);
                                                cube.transform.parent = cubeContainer;
                                               



                                        }
                                }
                        }
                }
        }
        
         void GenerateLevel()
        {
                FillLevel();
                
                Debug.Log(level.Count + " " + level[0].Count + " " + level[0][0].Count);
               CreateLevel();
        }

         void FillLevel()
        {
                for (int x = 0; x < X; x++)
                {
                        List<List<bool>> element = new List<List<bool>>();
                        for (int y = 0; y < Y; y++)
                        {
                                List<bool> row = new List<bool>();
                                for (int z = 0; z < Z; z++)
                                {
                                        row.Add(false);
                                }
                                element.Add(row);
                        }
                        level.Add(element);
                }
        }
        
         void CreateLevel()
        {

                Vector3Int s = seeker;
                Vector3Int w = wanderer;

                while (!generalVisited.Contains(s) && maxIterations > 0)
                {
                        maxIterations--;
                        level[(int)w.x][(int)w.y][(int)w.z] = true;
                        level[(int)s.x][(int)s.y][(int)s.z] = true;

                        seekerVisited.Add(s);
                        generalVisited.Add(w);
                        s = GetSeekerNextMove(s,w);
                        w = GetWandererNextMove(w);
                }

        }

        
        List<Vector3Int> GetMoves(Vector3Int point)
        {
                Vector3Int front = new Vector3Int(point.x, point.y, point.z + 1);
                Vector3Int back = new Vector3Int(point.x, point.y, point.z + -1);
                Vector3Int left = new Vector3Int(point.x - 1, point.y, point.z + 1);
                Vector3Int right = new Vector3Int(point.x + 1, point.y, point.z + 1);
                Vector3Int up = new Vector3Int(point.x, point.y + 1, point.z );
                Vector3Int down = new Vector3Int(point.x, point.y - 1, point.z);

                List<Vector3Int> moves = new List<Vector3Int>(){ up, down, left, right, front, back };

                return moves.FindAll(p =>
                                1 <= p.x &&
                                p.x < X - 1 &&
                                1 <= p.y &&
                                p.y < Y - 1 &&
                                1 <= p.z &&
                                p.z < Z - 1 &&
                                !generalVisited.Contains(p) &&
                                !level[p.x][p.y][p.z]
                        ).ToList();

        }

        Vector3Int GetWandererNextMove(Vector3Int w)
        {
                List<Vector3Int> moves = GetMoves(w);

                if (moves.Count > 0)
                {
                        return moves[Random.Range(0, moves.Count)];
                }

                return generalVisited[Random.Range(0, generalVisited.Count)];
        }

        float Distance(Vector3Int p, Vector3Int w)
        {
                return Mathf.Abs(p.x - w.x) + Mathf.Abs(p.y - w.y) + Mathf.Abs(p.z - w.z);
        }

        Vector3Int GetSeekerNextMove(Vector3Int s, Vector3Int w)
        {
                float distance = Distance(s, w);

                List<Vector3Int> moves = GetMoves(s).FindAll
                        (p =>
                                Distance(p, w)  < distance &&
                                !seekerVisited.Contains(p)
                        );

                if (moves.Count > 0)
                        return moves[Random.Range(0, moves.Count)];

                if(generalVisited.Contains(s))
                {
                        foreach( Vector3Int p in seekerVisited)
                        {
                                generalVisited.Add(p);
                        }

                }
                seekerVisited.Clear();
                return new Vector3Int(Random.Range(1, X - 1), Random.Range(1, Y - 1), Random.Range(1,Z-1));
        }
        
}
