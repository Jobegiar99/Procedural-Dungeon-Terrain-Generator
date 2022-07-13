using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class DungeonRoomBuilder
{
     
        public void BuildLevel( int xOffset,  int zOffset,  List<List<byte>> room, int rows,int columns,ref  List<GameObject> nodeBlocks, BiomeInformation biome)
        {
                string pool = AssignPool();
                if (pool == "")
                        return;

                Transform blockPool = GameObject.Find(pool).transform;
                int childIndex = 0;
                Material iceWall = Resources.Load<Material>(biome.Wall);
                Material iceFloor = Resources.Load<Material>(biome.Floor);
                Debug.Log(iceFloor);
                for (int i = 0; i < columns; i++)
                {
                        for (int j = 0; j < rows; j++)
                        {
                                Vector3 pos = new Vector3(j + ((rows ) * (xOffset)), 0, i + ((columns ) * (zOffset)));
                                if (room[j][i] == 0)
                                {
                                        for (float h = 0; h < 3; h++)
                                        {
                                                GameObject block = blockPool.GetChild(childIndex).gameObject;
                                                block.GetComponent<MeshRenderer>().material = iceWall;
                                                block.transform.position = pos;
                                                block.SetActive(true);
                                                nodeBlocks.Add(block);
                                                pos.y++;
                                                childIndex++;
                                        }
                                       
                                }
                                //pos.y = 3;
                                //GameObject roof = blockPool.GetChild(childIndex).gameObject;
                                //roof.transform.position = pos;
                                //roof.SetActive(true);
                                //nodeBlocks.Add(roof);
                                //childIndex++;
                                pos.y = -1;
                                GameObject floor = blockPool.GetChild(childIndex).gameObject;
                                floor.GetComponent<MeshRenderer>().material = iceFloor;
                                floor.transform.position = pos;
                                floor.SetActive(true);
                                nodeBlocks.Add(floor);
                                childIndex++;

                        }
                }

        }

        private string AssignPool()
        {
                string pool = "";
                for (int i = 1; i <= 10; i++)
                {
                        pool = "pool" + i.ToString();
                        if (pool == "pool10")
                                pool = "";

                        if (!GameObject.Find(pool).transform.GetChild(0).gameObject.activeInHierarchy)
                        {
                                return pool;
                        }
                }
                return pool;
        }
        
}
