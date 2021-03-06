using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Zeltex.Voxels
{
    /// <summary>
    /// ? What does this do?
    /// Oh i used this for PCG algorithms
    /// need to update them
    /// </summary>
	public class WorldEditor : MonoBehaviour
    {
		private World MyWorld; 
		[Header("Debug")]
		[SerializeField] protected bool IsDebugMode = false;
		[Header("Voxel Options")]
		[SerializeField] protected  bool bIsMirrorX = false;
		[SerializeField] protected  bool bIsMirrorY = true;
		[SerializeField] protected  bool bIsMirrorZ = false;
		protected float CurrentSparce = 0;
		
		void Start()
		{
			MyWorld = gameObject.GetComponent<World> ();
		}
		
		/// <summary>
        /// returns block size of world
        /// </summary>
		protected Int3 GetSize()
        {
            return gameObject.GetComponent<World>().GetWorldSizeChunks() * Chunk.ChunkSize;
		}

		protected void UpdateBlock(Int3 BlockPosition, int Type)
        {
			if (Type != 0 && MyWorld.GetVoxelType (new Int3(BlockPosition)) != 0)
            {
                return;
            }
			MyWorld.UpdateBlockTypeMass(Type, new Int3(BlockPosition));
		}

		protected int GetBlockType(Int3 BlockPosition)
        {
			return MyWorld.GetVoxelType (new Int3(BlockPosition));
		}

		protected void UpdateBlock(Int3 BlockPosition, int BlockType, Vector2 BlockSize) 
		{
			for (int i = 0; i < BlockSize.x; i++)
				for (int j = 0; j < BlockSize.y; j++)
			{
				UpdateBlock(BlockPosition + new Int3(i,0,j),
				            BlockType);
			}
		}

        protected void CheckSparce(int NewType)
        {
            if (NewType != 0)
                CurrentSparce++;
            else
                CurrentSparce--;
        }

		protected void UpdateBlock(Int3 Position, Int3 Size, int BlockType)
        {
            UpdateBlock(Position, BlockType);
            //DebugPositions.Add (Position);
            CheckSparce(BlockType);
            List<Int3> BlockPlacementPositions = new List<Int3>();
			BlockPlacementPositions.Add(Position);
			Vector3 MirrorAxis = new Vector3(Size.x / 2, Size.y / 2, Size.z / 2);
			if (bIsMirrorX) {
				int DifferenceX = (int)Mathf.Abs(Position.x - MirrorAxis.x);	// difference from mid
				if (Position.x != MirrorAxis.x)
                {
					int MaxBlocks = BlockPlacementPositions.Count;
					for (int i = 0; i < MaxBlocks; i++)
                    {
                        Int3 NewPosition = new Int3(BlockPlacementPositions[i].x, BlockPlacementPositions[i].y, BlockPlacementPositions[i].z);
						if (NewPosition.x > MirrorAxis.x)
							NewPosition.x -= DifferenceX * 2;
						else if (NewPosition.x < MirrorAxis.x)
							NewPosition.x += DifferenceX * 2;
						BlockPlacementPositions.Add(NewPosition);
					}
				}
			}
			if (bIsMirrorY)
            {
				int DifferenceY = (int)Mathf.Abs(Position.y - MirrorAxis.y);	// difference from mid
				if (Position.y != MirrorAxis.y)
                {
					int MaxBlocks = BlockPlacementPositions.Count;
					for (int i = 0; i < MaxBlocks; i++)
                    {
                        Int3 NewPosition = new Int3(BlockPlacementPositions[i].x, BlockPlacementPositions[i].y, BlockPlacementPositions[i].z);
						if (NewPosition.y > MirrorAxis.y)
							NewPosition.y -= DifferenceY * 2;
						else if (NewPosition.y < MirrorAxis.y)
							NewPosition.y += DifferenceY * 2;
						BlockPlacementPositions.Add(NewPosition);
					}
				}
			}

			if (bIsMirrorZ)
            {
				int DifferenceZ = (int)Mathf.Abs(Position.z - MirrorAxis.z);	// difference from mid
				if (Position.z != MirrorAxis.z)
                {
					int MaxBlocks = BlockPlacementPositions.Count;
					for (int i = 0; i < MaxBlocks; i++)
                    {
						Int3 NewPosition = new Int3(BlockPlacementPositions[i].x, BlockPlacementPositions[i].y, BlockPlacementPositions[i].z);
						if (NewPosition.z > MirrorAxis.z)
							NewPosition.z -= DifferenceZ * 2;
						else if (NewPosition.z < MirrorAxis.z)
							NewPosition.z += DifferenceZ * 2;
						BlockPlacementPositions.Add(NewPosition);
					}
				}
			}
			for (int i = 0; i < BlockPlacementPositions.Count; i++)
            {
                UpdateBlock(BlockPlacementPositions[i], BlockType);
                CheckSparce(BlockType);
            }
		}

		
		// needs to use is void axis to find out how to build walls
		// for every path block, build a wall in the empty spot next to it
		public void GeneratePathWalls(int PathBlockType, float MazeWallHeight,  int MazeWallType, int MazeRoofType) 
		{
			//Debug.LogError ("Generating hallway walls.");
			Int3 Size = GetSize();
			for (int i = 0; i < Size.x; i++)
				for (int j = 0; j < Size.y; j++)
					for (int k = 0; k < Size.z; k++)
				{
					if (GetBlockType(new Int3(i, j, k)) == PathBlockType)
					{
						UpdateBlock(new Int3(i, j + MazeWallHeight, k), MazeRoofType);
					}
					if (GetBlockType(new Int3(i, j, k)) == 0
					    && (GetBlockType(new Int3(i + 1, j, k)) == PathBlockType 
					    || GetBlockType(new Int3(i - 1, j, k)) == PathBlockType
					    || GetBlockType(new Int3(i, j, k + 1)) == PathBlockType 
					    || GetBlockType(new Int3(i, j, k - 1)) == PathBlockType)) 
					{
						for (int z = 0; z < MazeWallHeight + 1; z++)
                            {
							UpdateBlock(new Int3(i, j + z, k), MazeWallType);
						}
					}
				}

			/*if (bIsEdge) {
				for (int i = 0; i < Size.x; i++)
					for (int j = 0; j < Size.y; j++)
						for (int k = 0; k < Size.z; k++)
					{
						if (IsOnEdge(new Vector3(i, j, k), Size) && GetBlockType(new Vector3(i, j, k)) == PathBlockType) {
							for (int z = 0; z < MazeWallHeight + 1; z++) {
								UpdateBlock(new Vector3(i, j + z, k), MazeWallType);
							}
						}
					}
			}*/
		}
	}
}