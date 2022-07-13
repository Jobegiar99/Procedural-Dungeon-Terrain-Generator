using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonManager : MonoBehaviour
{
        List<DungeonNode> unvisitedRooms = new List<DungeonNode>();
        // Start is called before the first frame update
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {
                for(int i = 0; i <  unvisitedRooms.Count; i++)
                {
                        DungeonNode node = unvisitedRooms[i];
                        if( node.CheckIfPlayerInNode( transform.position))
                        {
                                unvisitedRooms.Remove(node);
                               //unvisitedRooms.Add( node.ExpandDungeon());
                        }
                }
        }
}
