using System.Collections.Generic;
using System.Linq;
using Random = UnityEngine.Random;
using UnityEngine;
using Cells;
using Entities;
using Partitions;

namespace Passes.DungeonPass
{
    [CreateAssetMenu(fileName = "Pass", menuName = "Pass/DungeonPass/RoomEntities", order = 1)]
    public class RoomEntitiesPass : BaseDungeonPass
    {
        [Header("Types")]
        [SerializeField] private EntitySettings entity;
        [SerializeField] private List<PartitionType> roomTypes;
        
        [Header("Spawning parameters")]
        [SerializeField] private int minAmount = 0;
        [SerializeField] private int maxAmount = 3;
        [SerializeField, Range(1, 100)] private int chanceAmount = 100; 
        
        private GridManager gridManager;
        
        public override bool SetPass()
        {
            this.gridManager ??= this.generator.gridManager;
            
            GenerateType();
            return true;
        }

        private void GenerateType()
        {
            EntitySettings settings = entity;
            
            foreach (var partition in this.generator.partitions)
            {
                if(!roomTypes.Contains(partition.Type)) continue;
                
                int amountOfEntities = Random.Range(this.minAmount, this.maxAmount+1);
                bool willSpawn = Random.Range(0, 100) < chanceAmount;
                if (!willSpawn) continue;

                for (int i = 0; i < amountOfEntities; )
                {
                    Cell randomCell = partition.Cells[Random.Range(0, partition.Cells.Count)];
                    if(randomCell.Type != CellType.GROUND) continue;
                    
                    GameObject instance = Object.Instantiate(settings.tilePrefab, new Vector3(randomCell.Position.x, randomCell.Position.y, settings.zOffset), Quaternion.identity);
                    instance.GetComponent<SpriteRenderer>().color = settings.cellColor;
                    instance.transform.SetParent(generator.transform);

                    i++;
                }
            }
        }
    }
}