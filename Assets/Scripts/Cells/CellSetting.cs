using UnityEngine;

namespace Cells
{
    [CreateAssetMenu(fileName = "CellSetting", menuName = "Cells/Setting", order = 1)]
    public class CellSetting : ScriptableObject
    {
        public CellType cellType = CellType.NONE;
        
        [Space]
        public GameObject tilePrefab;
        public Color cellColor = Color.white;
        [Space]
        public float zOffset = 0;
    }
}