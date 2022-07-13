using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BiomeInformation
{

        [SerializeField]
                List<string> biomeEnemies;

        [SerializeField]
                string biomeName;

        [SerializeField]
                List<string> biomeProps;

        [SerializeField]
                string floor;

        [SerializeField]
                float spreadProbability;

        [SerializeField]
                string wall;
        

        public string BiomeName
        {
                get { return biomeName; }
        }
        public List<string> BiomeProps
        {
                get { return biomeProps; }
        }

        public string Floor
        {
                get { return floor; }
        }

        public float SpreadProbabilty
        {
                get { return spreadProbability; }

                set { spreadProbability = value; }
        }

        public string Wall
        {
                get { return wall; }
        }

       
}
