using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonRoomInformation
{
        public int rows;
        public int columns;
        public Vector2Int wandererOffset;
        public Vector2Int seekerOffset;
        public Vector2Int wanderer;
        public Vector2Int seeker;

        public DungeonRoomInformation(
                        int rows,
                        int columns,
                        Vector2Int wandererOffset,
                        Vector2Int seekerOffset,
                        Vector2Int wanderer,
                        Vector2Int seeker
                 )
        {
                this.rows = rows;
                this.columns = columns;
                this.wandererOffset = wandererOffset;
                this.seekerOffset = seekerOffset;
                this.wanderer = wanderer;
                this.seeker = seeker;
        }
};
