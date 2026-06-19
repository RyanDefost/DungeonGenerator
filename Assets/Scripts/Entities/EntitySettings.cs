using System.Collections.Generic;
using Cells;
using Partitions;
using UnityEngine;

namespace Entities
{
    [CreateAssetMenu(fileName = "EntitySetting", menuName = "Entities/Setting", order = 1)]
    public class EntitySettings : ScriptableObject
    {
        public EntityType Type;
        public List<PartitionType> roomTypes;
        [Space]
        public GameObject tilePrefab;
        public Color cellColor = Color.white;
        [Space]
        public float zOffset = 0;
    }
}