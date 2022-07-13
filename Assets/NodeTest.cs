using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NodeTest : MonoBehaviour
{
        DungeonNode2 node;
        int rows;
        int columns;
        Dictionary<Vector2Int, DungeonNode2> worldInfo = new Dictionary<Vector2Int, DungeonNode2>();
        [SerializeField] List<BiomeInformation> biomes;
        // Start is called before the first frame update
        void Start()
        {
                rows = 25;
                columns = 25;
                node = new DungeonNode2(0, 0, rows, columns,-1,biomes); 
                InitializeNode();
        }

        public void InitializeNode()
        {
                node.CreateNeighbors(ref worldInfo);
                node.BuildRoom();
        }

        void Update()
        {
                for (int i = 0; i < node.neighbors.Count; i++)
                {
                        if (node.neighbors[i].CheckIfPlayerInNode(transform.position))
                        {
                                UpdatePlayerNode(node.neighbors[i]);
                        }
                }
        }

        public void UpdatePlayerNode(DungeonNode2 targetNode)
        {
                node.DestroyBlocks(targetNode.neighbors);
                node = targetNode;
                InitializeNode();
        }
}
