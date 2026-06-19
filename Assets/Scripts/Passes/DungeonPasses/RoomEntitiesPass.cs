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
        private readonly GridManager gridManager;
        
        public RoomEntitiesPass(Generator generator)
        {
            this.generator =  generator;
            this.gridManager = generator.gridManager;
        }

        public override bool SetPass()
        {
            throw new System.NotImplementedException();
        }
        
        public void GenerateEntities()
        {
            GenerateType(EntityType.ENEMY);
            
        }

        private void GenerateType(EntityType type)
        {
            EntitySettings settings = this.generator.EnemySettings;
            
            foreach (var partition in this.generator.partitions)
            {
                if(partition.Type != PartitionType.NONE) continue;
                
                int amountOfEntities = Random.Range(0, 3);
                bool willSpawn = Random.Range(0, 100) < 75;
                if (!willSpawn) continue;

                for (int i = 0; i < amountOfEntities; )
                {
                    Debug.Log("Spawning entity " + i);
                    Cell randomCell = partition.Cells[Random.Range(0, partition.Cells.Count)];
                    if(randomCell.Type != CellType.GROUND) continue;
                    
                    GameObject instance = Object.Instantiate(this.gridManager.baseCellPrefab, new Vector3(randomCell.Position.x, randomCell.Position.y, settings.zOffset), Quaternion.identity);
                    instance.GetComponent<SpriteRenderer>().color = settings.cellColor;
                    instance.transform.SetParent(generator.transform);

                    i++;
                }
            }
        }
    }
}