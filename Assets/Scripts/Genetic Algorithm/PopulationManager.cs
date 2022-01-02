using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class PopulationManager : MonoBehaviour
{

        public GameObject botPrefab;
        public int populationSize = 100;
        List<GameObject> population = new List<GameObject>();
        int generation = 1;
        GUIStyle guiStyle = new GUIStyle();
        Point goal;
        float trialTime = 0;
        public int bots = 50;
        // Start is called before the first frame update
        void Start()
        {
                goal = GameObject.Find("Main Camera").GetComponent<GenerateDungeon>().wanderer;
                for (int i = 0; i < populationSize; i++)
                {

                        GameObject bot = Instantiate(
                                        botPrefab,
                                        transform.position,
                                        transform.rotation
                                );

                        bot.GetComponent<Brain>().dnaLength = Random.Range(5, 15);
                        bot.GetComponent<Brain>().Init(goal);
                        
                        population.Add(bot);
                }
        }

        float FitnessFunction(GameObject bot)
        {
                Brain brain = bot.GetComponent<Brain>();
                if (brain.dna.furtherstDistance == 0)
                        return int.MaxValue;

                float fitness = (brain.dna.visitedCells.Count ) / (brain.dna.furtherstDistance );

                

                return fitness;
        }

        GameObject Breed(GameObject parent1, GameObject parent2, bool canMutate)
        {

                GameObject offspring =
                        Instantiate(botPrefab, transform.position, transform.rotation);

                Brain brain = offspring.GetComponent<Brain>();
                brain.dnaLength = (Random.Range(0, 100) % 2 == 0) ? parent1.GetComponent<Brain>().dnaLength : parent2.GetComponent<Brain>().dnaLength;
                

                if (Random.Range(0f, 1f) <= 0.55f && canMutate)
                {
                        brain.dnaLength += Random.Range(1,6);
                        brain.Init(goal);
                        brain.dna.Mutate();
                }
                else
                {
                        brain.Init(goal);
                        brain.dna.Combine
                        (
                                parent1.GetComponent<Brain>().dna,
                                parent2.GetComponent<Brain>().dna
                        );
                }
                        
                

                return offspring;
        }

        void BreedNewPopulation()
        {
                List<GameObject> sortedList =
                        population.OrderBy( bot => FitnessFunction(bot)).ToList();
                //sortedList.Reverse();
                population.Clear();
                int startingPos = sortedList.Count - (int)(sortedList.Count / 2.0f) - 1;
                for (int i = startingPos - 1 ; i < sortedList.Count - 1 ; i++)
                {
                        bool canMutate = (FitnessFunction(sortedList[i]) == int.MaxValue) ? false : true;

                        if (Random.Range(0f, 1f) < 0.3f)
                        {
                                population.Add(Breed(sortedList[i], sortedList[sortedList.Count - 1], canMutate));
                                population.Add(Breed(sortedList[sortedList.Count - 1], sortedList[i], canMutate));
                        }
                        else
                        {
                                population.Add(Breed(sortedList[i], sortedList[i + 1], canMutate));
                                population.Add(Breed(sortedList[i + 1], sortedList[i],canMutate));
                        }


                }
                for (int i = 0; i < sortedList.Count; i++)
                {
                        Destroy(sortedList[i].gameObject);
                }
                generation++;
        }

        private void Update()
        {
                trialTime += Time.deltaTime;
                if(   bots <= 0 )
                {
                        BreedNewPopulation();
                        bots = populationSize;
                        trialTime = 0;
                }
        }

        private void OnGUI()
        {
                guiStyle.fontSize = 25;
                guiStyle.normal.textColor = Color.black;

                GUI.BeginGroup(new Rect(10, 10, 250, 150));
                GUI.Box(new Rect(0, 0, 140, 140), "Stats", guiStyle);
                GUI.color = new Color(0.1f, 0.1f, 0.1f, 0.7f);
                GUI.Label(new Rect(10, 25, 200, 30), "Gen: " + generation, guiStyle);
                GUI.Label(new Rect(10, 50, 200, 30), "Alive: " + bots, guiStyle);
                GUI.Label(new Rect(10, 75, 200, 30), "Population: " + population.Count, guiStyle);
                GUI.EndGroup();
        }
}
