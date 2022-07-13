
using System.Collections.Generic;
using UnityEngine;

public class DungeonNode2 
{
        #region Parameters

        /// <summary>
        /// A list to control which dungeon nodes have been connected.
        /// </summary>
        private List<bool> arcs;

        private List<BiomeInformation> biomes;
 
        /// <summary>
        /// A flag to know if the node's level has been created in the game's world.
        /// </summary>
        private bool built = false;

        /// <summary>
        /// The amount of columns per room
        /// </summary>
        private int columns;

        /// <summary>
        /// A flag to know if the current node's neighbors have been created.
        /// </summary>
        bool createdNeighbors = false;

        BiomeInformation currentBiome;

        int currentBiomeIndex;

        string currentBiomeName;

        bool hasBiome = false;

        /// <summary>
        /// The Dungeon Nodes that are connected to this one.
        /// </summary>
        public List<DungeonNode2> neighbors;

        /// <summary>
        /// Contains the nodes that are part of the node in the game' world.
        /// </summary>
        private List<GameObject> nodeBlocks;

        /// <summary>
        /// The terrain's information of the current room.
        /// </summary>
        private DungeonRoomCreator room;

        /// <summary>
        /// The amount of rows per room
        /// </summary>
        private int rows;

        /// <summary>
        /// X position to origin
        /// </summary>
        private int xOffset;


        /// <summary>
        /// Y position to origin
        /// </summary>
        private int zOffset;
        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new dungeon node.
        /// </summary>
        /// <param name="wanderer"> Room's entrance</param>
        /// <param name="seeker">Room's exit</param>
        /// <param name="xOffset">X position to origin</param>
        /// <param name="zOffset">Y position to origin</param>
        /// <param name="previous">Previous node</param>
        /// <param name="rows">The amount of rows per room</param>
        /// <param name="columns">The amount of columns per room</param>
        public DungeonNode2( int xOffset, int zOffset, int rows, int columns,int biomeIndex, List<BiomeInformation> biomeList)
        {
                this.xOffset = xOffset;
                this.zOffset = zOffset;
                nodeBlocks = new List<GameObject>();
                this.rows = rows;
                this.columns = columns;
                neighbors = new List<DungeonNode2>();
                this.biomes = biomeList;
                this.currentBiomeIndex = biomeIndex;
                arcs = new List<bool>() { false, false, false, false, false, false, false, false, false, false, false, false, false, false,false,false };
                SetBiome();
                CreateRoom();
               
        }

        #endregion

        #region Functions

        /// <summary>
        /// Builds the room in the game space with the information that the DungeonRoomCreator object has.
        /// </summary>
        public void BuildRoom()
        {
                if (built)
                        return;
                built = true;
                DungeonRoomBuilder builder = new DungeonRoomBuilder();
               for(int index = 0; index < neighbors.Count; index++)
                {
                        DungeonNode2 node = neighbors[index];
                        if (!node.built)
                        {
                                builder.BuildLevel(node.xOffset, node.zOffset, node.room.level, rows, columns, ref node.nodeBlocks, node.currentBiome);
                                neighbors[index].built = true;
                        }
                }
                builder.BuildLevel(xOffset, zOffset, room.level, rows, columns, ref nodeBlocks,currentBiome);
        }

        /// <summary>
        /// Checks if the player reached the current room.
        /// </summary>
        /// <param name="playerPosition">Current player's position</param>
        /// <returns></returns>
        public bool CheckIfPlayerInNode(Vector3 playerPosition)
        {
                bool xPosition = (playerPosition.x >= (rows * xOffset) && playerPosition.x < ((rows * xOffset) + rows));
                bool zPosition = (playerPosition.z >= (columns * zOffset) && playerPosition.z < ((columns * zOffset) + columns));
                return xPosition && zPosition;
        }


        /// <summary>
        /// Creates and connects the neighbor nodes to this one.
        /// </summary>
        /// <param name="worldInfo">Dictionary that contains the nodes per position</param>
        public void CreateNeighbors(ref Dictionary<Vector2Int, DungeonNode2> worldInfo)
        {

                if (createdNeighbors)
                        return;

                /*
                 *  Letter --> The node
                 *  Number --> The arc
                 *  
                 *                      A     0    B      1      C
                 *                      2     3    4      5      6
                 *                      D     7    E      8      F
                 *                      9   10   11   12  13
                 *                      G   14   H     15    I
                 *                      
                 *                      
                 */

                #region Node Creation
                float spreadResult =  Random.Range(0f, 1f);
                Debug.Log(spreadResult);
                int nextBiomeIndex = (biomes[currentBiomeIndex].SpreadProbabilty <= spreadResult) ? currentBiomeIndex : -1;
                Debug.Log(nextBiomeIndex);
                DungeonNode2 A = new DungeonNode2(xOffset - 1, zOffset + 1, rows, columns, nextBiomeIndex, biomes);
                DungeonNode2 B = new DungeonNode2(xOffset, zOffset + 1, rows, columns, nextBiomeIndex, biomes);
                DungeonNode2 C = new DungeonNode2(xOffset + 1, zOffset + 1, rows, columns, nextBiomeIndex, biomes);
                DungeonNode2 D = new DungeonNode2(xOffset - 1, zOffset, rows, columns, nextBiomeIndex, biomes);
                DungeonNode2 E = this;
                DungeonNode2 F = new DungeonNode2(xOffset + 1, zOffset, rows, columns, nextBiomeIndex, biomes);
                DungeonNode2 G = new DungeonNode2(xOffset - 1, zOffset - 1, rows, columns, nextBiomeIndex, biomes);
                DungeonNode2 H = new DungeonNode2(xOffset, zOffset - 1, rows, columns, nextBiomeIndex, biomes);
                DungeonNode2 I = new DungeonNode2(xOffset + 1, zOffset - 1, rows, columns, nextBiomeIndex, biomes);
                CreateNeighborInitHelper(ref worldInfo, ref A);
                CreateNeighborInitHelper(ref worldInfo, ref B);
                CreateNeighborInitHelper(ref worldInfo, ref C);
                CreateNeighborInitHelper(ref worldInfo, ref D);
                CreateNeighborInitHelper(ref worldInfo, ref E);
                CreateNeighborInitHelper(ref worldInfo, ref F);
                CreateNeighborInitHelper(ref worldInfo, ref G);
                CreateNeighborInitHelper(ref worldInfo, ref H);
                CreateNeighborInitHelper(ref worldInfo, ref I);

                #endregion

                #region Arc Connection

                #region A
                CreateNeighborsHelper(ref A, ref B, 8);
                CreateNeighborsHelper(ref A, ref D, 11);
                CreateNeighborsHelper(ref A, ref E, 12);
                #endregion

                #region B
                CreateNeighborsHelper(ref B, ref A, 7);
                CreateNeighborsHelper(ref B, ref C, 8);
                CreateNeighborsHelper(ref B, ref D, 10);
                CreateNeighborsHelper(ref B, ref E, 11);
                CreateNeighborsHelper(ref B, ref F, 12);
                #endregion

                #region C
                CreateNeighborsHelper(ref C, ref B, 7);
                CreateNeighborsHelper(ref C, ref E, 10);
                CreateNeighborsHelper(ref C, ref F, 11);
                #endregion

                #region D
                CreateNeighborsHelper(ref D, ref A, 4);
                CreateNeighborsHelper(ref D, ref B, 5);
                CreateNeighborsHelper(ref D, ref E, 8);
                CreateNeighborsHelper(ref D, ref G, 11);
                CreateNeighborsHelper(ref D, ref H, 12);
                #endregion

                #region E
                CreateNeighborsHelper(ref E, ref A, 3);
                CreateNeighborsHelper(ref E, ref B, 4);
                CreateNeighborsHelper(ref E, ref C, 5);
                CreateNeighborsHelper(ref E, ref D, 7);
                CreateNeighborsHelper(ref E, ref F, 8);
                CreateNeighborsHelper(ref E, ref G, 10);
                CreateNeighborsHelper(ref E, ref H, 11);
                CreateNeighborsHelper(ref E, ref I, 12);
                #endregion

                #region F
                CreateNeighborsHelper(ref F, ref B, 3);
                CreateNeighborsHelper(ref F, ref C, 4);
                CreateNeighborsHelper(ref F, ref E, 7);
                CreateNeighborsHelper(ref F, ref H, 10);
                CreateNeighborsHelper(ref F, ref I, 11);
                #endregion

                #region G
                CreateNeighborsHelper(ref G, ref D, 4);
                CreateNeighborsHelper(ref G, ref E, 5);
                CreateNeighborsHelper(ref G, ref H, 8);
                #endregion

                #region H
                CreateNeighborsHelper(ref H, ref D, 3);
                CreateNeighborsHelper(ref H, ref E, 4);
                CreateNeighborsHelper(ref H, ref F, 5);
                CreateNeighborsHelper(ref H, ref G, 7);
                CreateNeighborsHelper(ref H, ref I, 8);
                #endregion

                #region I
                CreateNeighborsHelper(ref I, ref E, 3);
                CreateNeighborsHelper(ref I, ref F, 4);
                CreateNeighborsHelper(ref I, ref H, 7);
                #endregion

                #endregion

                createdNeighbors = true;

        }

        /// <summary>
        /// Checks
        /// </summary>
        /// <param name="node">The node that will be connected to another one</param>
        /// <param name="target">The target node to connect the one to</param>
        /// <param name="arcIndex">The arc index of this node. If true it means that those nodes are connected</param>
        private void CreateNeighborsHelper(ref DungeonNode2 node, ref DungeonNode2 target, int arcIndex)
        {
                if (!node.neighbors.Contains(target))
                        node.neighbors.Add(target);
                node.arcs[arcIndex] = true;
        }

        /// <summary>
        /// Adds or retrieves a node for a given position from teh worldInfo dictionary
        /// </summary>
        /// <param name="worldInfo">Dictionary that contains the nodes per position</param>
        /// <param name="node">The node to perform the operation to</param>
        private void CreateNeighborInitHelper(ref Dictionary<Vector2Int,DungeonNode2> worldInfo, ref DungeonNode2 node) 
        {
                Vector2Int key = new Vector2Int(node.xOffset, node.zOffset);
                if (worldInfo.ContainsKey(key))
                {
                        node = worldInfo[key];
                }
                else
                {
                        worldInfo[key] = node;
                }
        }

        /// <summary>
        /// Creates the object that will contain the terrain information.
        /// </summary>
        public void CreateRoom()
        {
                if (room != null)
                        return;

                List<Vector2Int> options = new List<Vector2Int>() {
                                new Vector2Int(0 , Random.Range(0,columns)),
                                new Vector2Int(rows - 1, Random.Range(0,columns)),
                                new Vector2Int(Random.Range(0,rows), 0),
                                new Vector2Int(Random.Range(0,rows), columns - 1)
                         };
                Vector2Int wanderer = options[Random.Range(0, 4)];
                options.Remove(wanderer);
                Vector2Int seeker = options[Random.Range(0, 3)];
                room = new DungeonRoomCreator(wanderer, seeker, rows, columns);

        }

        /// <summary>
        /// Destroys each created block in the game's world.
        /// </summary>
        public void DestroyBlocks(List<DungeonNode2> otherNeighbors)
        {
                for(int i =0;  i < neighbors.Count; i++)
                {
                        if(!otherNeighbors.Contains(neighbors[i]))
                        {
                               
                                while (neighbors[i].nodeBlocks.Count > 0)
                                {
                                        
                                        neighbors[i].nodeBlocks[0].SetActive(false);
                                        neighbors[i].nodeBlocks.RemoveAt(0);
                                }
                                neighbors[i].built = false;
                        }
                }

        }


        private void SetBiome()
        {
                if (hasBiome)
                        return;

                if(currentBiomeIndex == -1)
                        currentBiomeIndex = Random.Range(0, biomes.Count);

                hasBiome = true;
                currentBiomeName = biomes[currentBiomeIndex].BiomeName;
                currentBiome = biomes[currentBiomeIndex];
        }
        #endregion
}
