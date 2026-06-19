using System.Collections.Generic;
using Partitions;
using UnityEngine;

namespace Entities
{
    [CreateAssetMenu(fileName = "EntitySetting", menuName = "Entity/Setting", order = 1)]
    public class EntitySettings : ScriptableObject
    {
        public EntityType Type;
        [Space]
        public GameObject tilePrefab;
        public Color cellColor = Color.white;
        [Space]
        public float zOffset = 0;
    }
}