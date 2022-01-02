using UnityEngine;
using System.Collections.Generic;
public class Brain : MonoBehaviour
{
        public DNA dna;
        public bool alive = true;
        public int dnaLength;
        List<List<bool>> level;
        [SerializeField] MeshRenderer mesh;
        private Vector3 goalPosition;
        // Start is called before the first frame update
        public void Init(Point goal)
        {
                
                mesh.material.color = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
                dna = new DNA(dnaLength, 4);
                level = GameObject.Find("Main Camera").GetComponent<GenerateDungeon>().level;
                goalPosition = new Vector3(goal.row, transform.position.y, goal.column);

                StartCoroutine(Move());
               
                dna.furtherstDistance = Vector3.Distance(transform.position, goalPosition);
        }

        private void OnCollisionEnter(Collision collision)
        {
                if(collision.gameObject.tag == "wall")
                {
                        alive = false;
                }
        }

        private System.Collections.IEnumerator Move()
        {
                Point position = new Point((int)transform.position.x, (int)transform.position.z);
                if (!dna.visitedCells.Contains(position))
                        dna.visitedCells.Add(position);
                else
                        dna.repeatedCells++;

                while (alive)
                {
                        if (!alive)
                                continue;

                        int gene = dna.GetGene();
                        if (gene == -1)
                        {
                                alive = false;
                                GameObject.Find("PM(Clone)").GetComponent<PopulationManager>().bots--;
                                continue;
                        }

                        Vector3 nextPosition = new Vector3(0, 0, 0);
                        switch (gene)
                        {
                                case 0:
                                        {
                                                nextPosition.x = 1;
                                                break;
                                        }
                                case 1:
                                        {
                                                nextPosition.x = -1;
                                                break;
                                        }
                                case 2:
                                        {
                                                nextPosition.z = 1;
                                                break;
                                        }
                                case 3:
                                        {
                                                nextPosition.z = -1;
                                                break;
                                        }
                        }

                        RaycastHit hit;
                        if (Physics.Raycast(transform.position, nextPosition, out hit, 1f))
                        {
                                if (hit.collider.gameObject.tag != "wall")
                                        transform.position += nextPosition;
                        }
                        else
                        {
                                transform.position += nextPosition;
                        }
                        float distanceA = Vector3.Distance(transform.position, goalPosition);

                        if (distanceA < dna.furtherstDistance)
                                dna.furtherstDistance = distanceA;
                        if( distanceA == 0)
                        {
                                alive = false;
                                GameObject.Find("PM(Clone)").GetComponent<PopulationManager>().bots--;
                                dna.furtherstDistance = 0;
                        }
                        yield return new WaitForSecondsRealtime(0.01f);
                }
        }
}
