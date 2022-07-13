using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeeCamera : MonoBehaviour
{

        // Update is called once per frame
        void Update()
        {
                transform.LookAt(GameObject.Find("Main Camera").transform);
        }
}
