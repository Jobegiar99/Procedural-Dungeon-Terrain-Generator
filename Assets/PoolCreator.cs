using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolCreator : MonoBehaviour
{
        [SerializeField] GameObject pool;
        [SerializeField] GameObject prefab;
        // Start is called before the first frame update
        void Start()
        {
                for(int i = 0; i < 3125; i++)
                {
                        GameObject block = Instantiate(prefab,new Vector3(0, 0, 0), Quaternion.identity);
                        block.transform.parent = pool.transform;
                        block.SetActive(false);
                }
        }

}
