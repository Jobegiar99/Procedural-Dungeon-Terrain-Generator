using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonNode
{
        #region Parameters
        /// <summary>
        /// X position to origin
        /// </summary>
        private int xOffset;

        /// <summary>
        /// Y position to origin
        /// </summary>
        private int yOffset;

        /// <summary>
        /// Will be the entrance of the current room
        /// </summary>
        private Vector2Int wanderer;

        /// <summary>
        /// Will be the exit of the current room
        /// </summary>
        private Vector2Int seeker;

        /// <summary>
        /// Node that contains the next room that is connected to this one
        /// </summary>
        public DungeonNode nextRoom;

        /// <summary>
        /// Node that contains the previous room that is connected to this one.
        /// </summary>
        public DungeonNode previousRoom;

        /// <summary>
        /// The terrain's information of the current room.
        /// </summary>
        public DungeonRoomCreator room;

        /// <summary>
        /// Contains the nodes that are part of the node in the game' world.
        /// </summary>
        public List<GameObject> nodeBlocks;

        /// <summary>
        /// The amount of rows per room
        /// </summary>
        private int rows;

        /// <summary>
        /// The amount of columns per room
        /// </summary>
        private int columns;

        private Vector2Int nextOffset;

        bool initialized = false;
        bool createdRoom = false;
        bool createdNextRoom = false;
        #endregion

        #region Constructor
        /// <summary>
        /// Initializes a new dungeon node.
        /// </summary>
        /// <param name="wanderer"> Room's entrance</param>
        /// <param name="seeker">Room's exit</param>
        /// <param name="xOffset">X position to origin</param>
        /// <param name="yOffset">Y position to origin</param>
        /// <param name="previous">Previous node</param>
        /// <param name="rows">The amount of rows per room</param>
        /// <param name="columns">The amount of columns per room</param>
        public DungeonNode(
                Vector2Int wanderer, 
                Vector2Int seeker, 
                int xOffset, 
                int yOffset, 
                DungeonNode previous,
                int rows,
                int columns)
        {
                this.wanderer = wanderer;
                this.seeker = seeker;
                this.xOffset = xOffset;
                this.yOffset = yOffset;
                this.previousRoom = previous;
                nodeBlocks = new List<GameObject>();
                this.rows = rows;
                this.columns = columns;
        }

        #endregion

        #region Functions
        /// <summary>
        /// Builds the room in the game space with the information that the DungeonRoomCreator object has.
        /// </summary>
        public void BuildRoom()
        {
                DungeonRoomBuilder builder = new DungeonRoomBuilder();
                
        }

        /// <summary>
        /// Checks if the player reached the current room.
        /// </summary>
        /// <param name="playerPosition">Current player's position</param>
        /// <returns></returns>
        public bool CheckIfPlayerInNode(Vector3 playerPosition)
        {
                bool xPosition = (playerPosition.x >= (rows * xOffset) && playerPosition.x < ((rows * xOffset) + rows));
                bool zPosition = (playerPosition.z >= (columns * yOffset) && playerPosition.z < ((columns * yOffset) + columns));
                return xPosition && zPosition;
        }

        /// <summary>
        /// Makes the exit of the room be empty to avoid any kind of problem
        /// related to not being able to go to the next room.
        /// </summary>
        private void CleanPoints()
        {
                room.level[wanderer.x][wanderer.y] = 2;
                room.level[seeker.x][seeker.y] = 3;
                if (nextRoom == null)
                        return;

                //up
                if ((nextRoom.xOffset < xOffset))
                {
                        room.level[0][columns / 2] = 1;
                }
                
                //left
                 if ((nextRoom.yOffset < yOffset) )
                {
                        room.level[rows / 2][0] = 1;
                }

                //right
                if ((nextRoom.yOffset > yOffset) )
                {
                        room.level[rows / 2][0] = 1;
                }

                if (previousRoom == null)
                        return;

                //down
                if ( (previousRoom.xOffset > xOffset))
                {
                        room.level[rows - 1][columns / 2] = 1;
                }

                if (previousRoom.yOffset < yOffset)
                        previousRoom.room.level[rows / 2][0] = 1;

                if (previousRoom.yOffset > yOffset)
                        previousRoom.room.level[rows / 2][0] = 1;




                /*int holeSize = 2;

                for (int i = 1; i < rows - 1; i++)
                {
                        for (int j = 1; j < columns - 1; j++)
                        {
                                //up;
                                if ((nextRoom.xOffset < xOffset))
                                {
                                        for (int k = 0; k < holeSize; k++)
                                        {
                                                room.level[k][j] = 1;
                                        }

                                }
                                //down
                                if (previousRoom != null && previousRoom.xOffset > xOffset)
                                {
                                        for (int k = 1; k < holeSize; k++)
                                        {
                                                room.level[rows - k][j] = 1;
                                        }
                                }
                        }
                }

                for (int i = 1; i < columns - 1; i++)
                {
                        for (int j = 1; j < rows - 1; j++)
                        {
                                //left
                                if ((nextRoom.yOffset < yOffset) || (previousRoom != null && (previousRoom.yOffset < yOffset) ))
                                {
                                        for (int k = 0; k < holeSize; k++)
                                        {
                                                room.level[j][k] = 1;
                                        }
                                }
                                //right
                                if ((nextRoom.yOffset > yOffset) || (previousRoom != null && (previousRoom.yOffset > yOffset )))
                                {
                                        for (int k = 1; k < holeSize; k++)
                                        {
                                                room.level[j][columns - k] = 1;
                                        }
                                }
                        }
                }
                */
        }

        /// <summary>
        /// Rotates the level to be equal to world's position
        /// </summary>
        private void RotateMatrix(int angles)
        {
                /*
                        | 0  |  1  | 2
                        | 3  | 4  | 5

                        |2 | 5 --> from column - 1 to 0 , then rows
                        | 1 | 4
                        | 0 | 3

                        |0 | 3 | --> from 0 to column - 1, then rows
                        | 1 | 4 |
                        | 2 | 5 |

                        | 2 | 1 | 0 | --> from row - 1 to 0, then columns
                        | 5 | 3 | 2 |
                */
                List<List<byte>> rotatedLevel = new List<List<byte>>();
                
                if( angles == 90)
                {
                        for (int column = 0; column < columns; column++)
                        {
                                List<byte> newRow = new List<byte>();
                                for (int row = 0; row < rows; row++)
                                {
                                        newRow.Add(room.level[row][column]);
                                }
                                rotatedLevel.Add(newRow);
                        }
                }

                if ( angles == 180)
                {
                        for (int row = rows - 1; row >= 0; row--)
                        {
                                List<byte> newRow = new List<byte>();
                                for (int column = 0; column < columns; column++)
                                {
                                        newRow.Add(room.level[row][column]);
                                }
                                rotatedLevel.Add(newRow);
                        }
                }
                
                if( angles == 270)
                {
                        for (int column = columns - 1; column > -1; column--)
                        {
                                List<byte> newRow = new List<byte>();
                                for (int row = 0; row < rows; row++)
                                {
                                        newRow.Add(room.level[row][column]);
                                }
                                rotatedLevel.Add(newRow);
                        }
                }
                room.level = rotatedLevel;
        }

        public void CreateNextNode(ref List<Vector2Int> createdRooms)
        {
                if (createdNextRoom)
                        return;

                createdNextRoom = true;


                Vector2Int nextEntrance = new Vector2Int();
                if (nextOffset.x < xOffset)
                {
                        Debug.Log("Up");
                        nextEntrance = new Vector2Int(rows - 1, columns / 2);
                }
               
                else if (nextOffset.y > yOffset)
                {
                        Debug.Log("Right");
                        nextEntrance = new Vector2Int(rows / 2, columns - 1);
                }
                else if (nextOffset.y < yOffset)
                {
                        Debug.Log("Left");
                        nextEntrance = new Vector2Int(rows / 2, 1);
                }

                nextRoom = new DungeonNode(
                               nextEntrance,
                               new Vector2Int(-1, -1),
                               nextOffset.x,
                               nextOffset.y,
                               this,
                               rows,
                               columns);

               // createdRooms.Add(new Vector2Int(nextOffset.x, nextOffset.y));
        }

        /// <summary>
        /// Creates the object that will contain the terrain information.
        /// </summary>
        public void CreateRoom()
        {
                Debug.Log("Wanderer " + wanderer + " Seeker" + seeker + " rows" + rows + " columns" + columns);
                
                if (!createdRoom)
                {
                        room = new DungeonRoomCreator(wanderer, seeker, rows, columns);
                        CleanPoints();
                        createdRoom = true;
                }
        }

        /// <summary>
        /// Defines the entrance and exit points of the current room
        /// </summary>
        /// <param name="createdRooms">The hashset of created room locations</param>
        public void DefineRoomPoints(ref List<Vector2Int> createdRooms)
        {
                if (!initialized)
                {
                        List<List<Vector2Int>> options = GetMovementOptions(ref createdRooms);
                        int index = Random.Range(0, options.Count);

                        if (wanderer == new Vector2Int(-1, -1))
                        {
                                wanderer = options[index][0];
                                options.RemoveAt(index);
                        }

                        index = Random.Range(0, options.Count);
                        
                        seeker = options[index][0];
                        nextOffset = options[index][1];
                        createdRooms.Add(new Vector2Int(xOffset, yOffset));
                        initialized = true;
                }
        }

        /// <summary>
        /// Destroys each created block in the game's world.
        /// </summary>
        public void DestroyBlocks()
        {
                foreach(GameObject block in nodeBlocks)
                {
                        UnityEngine.MonoBehaviour.Destroy(block.gameObject);
                }
        }

        /// <summary>
        /// Gets the possible directions for the next room.
        /// </summary>
        /// <param name="createdRooms">The vector of created room positions</param>
        /// <param name="rows">The amount of rows per room</param>
        /// <param name="columns">The amount of columns per room</param>
        /// <returns></returns>
        private List<List<Vector2Int>> GetMovementOptions(ref List<Vector2Int> createdRooms)
        {
                List<List<Vector2Int>> options = new List<List<Vector2Int>>();

                options.Add(
                        new List<Vector2Int>() {
                                        new Vector2Int(1, columns / 2),
                                        new Vector2Int(xOffset - 1, yOffset )
                        }
                ); // up

                options.Add(
                        new List<Vector2Int>() {
                                        new Vector2Int(rows / 2, 1),
                                        new Vector2Int(xOffset, yOffset - 1)
                        }
                ); // left

                options.Add(
                        new List<Vector2Int>() {
                                        new Vector2Int( rows / 2, columns - 2),
                                        new Vector2Int(xOffset , yOffset + 1)
                        }
                ); // right


                for (int i = 0; i < options.Count; i++)
                {

                        for (int j = 0; j < createdRooms.Count; j++)
                        {
                                if (createdRooms[j].x == options[i][1].x && createdRooms[j].y == options[i][1].y)
                                {
                                        options.RemoveAt(i);
                                        i = -1;
                                        break;
                                }
                        }
                }

                return options;
        }

        /// <summary>
        /// Defines seeker and wanderer and also creates the room
        /// </summary>
        /// <param name="createdRooms">The hashset of created room locations</param>
        /// <param name="rows">Amount of rows that each room can have</param>
        /// <param name="columns">Amount of columns that each room can have</param>
        public void InitializeNode(ref List<Vector2Int> CreateRooms, int rows, int columns)
        {
                if (nextRoom == null)
                {
                        DefineRoomPoints(ref CreateRooms);
                }
        }

        #endregion
}
