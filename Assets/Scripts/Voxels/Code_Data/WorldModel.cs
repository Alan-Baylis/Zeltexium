﻿
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Zeltex.Voxels
{
    /// <summary>
    /// The main polygonal mesh data for a voxel
    /// Also contains rules needed for placing in the voxel grid
    /// </summary>
    [System.Serializable]
    public class WorldModel : Element
    {
        public string VoxelData = "";

        public WorldModel(string NewData)
        {
            VoxelData = NewData;
        }
    }
}
