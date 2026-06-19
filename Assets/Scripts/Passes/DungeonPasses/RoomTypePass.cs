using System;
using System.Collections.Generic;
using System.Linq;
using Cells;
using Partitions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Passes.DataPasses
{
    public class RoomTypePass : IGenerationPass
    {
        private readonly Generator generator;
        
        public RoomTypePass(Generator generator)
        {
            this.generator =  generator;
        }

        public void AssignPartitionType()
        {
            //It should have an input for type and amount
            ApplyType(PartitionType.END, 1, EndRoomConditions);
            ApplyType(PartitionType.START, 1, StartRoomConditions);
            ApplyType(PartitionType.LOOT, 1, LootRoomConditions, 50);

            foreach (var partition in this.generator.partitions)
            {
                switch (partition.Type)
                {
                    case PartitionType.END:
                    {
                        foreach (Cell cells in partition.Cells.Where(cells => cells.Type == CellType.GROUND))
                            cells.GameObject.GetComponent<SpriteRenderer>().color = Color.red;
                        break;
                    }
                    case PartitionType.START:
                    {
                        foreach (Cell cells in partition.Cells.Where(cells => cells.Type == CellType.GROUND))
                            cells.GameObject.GetComponent<SpriteRenderer>().color = Color.green;
                        break;
                    }
                    
                    case PartitionType.LOOT:
                    {
                        foreach (Cell cells in partition.Cells.Where(cells => cells.Type == CellType.GROUND))
                            cells.GameObject.GetComponent<SpriteRenderer>().color = Color.yellow;
                        break;
                    }
                }
            }
            
        }

        private void ApplyType(PartitionType type, int desiredAmount, Func<Partition, bool> conditions, int chanceAmount = 100)
        {
            List<Partition> candidates = this.generator.partitions.Where(conditions).ToList();

            for (int i = 0; i < desiredAmount; i++)
            {
                Partition partition = candidates[Random.Range(0, candidates.Count)];
                candidates.Remove(partition);
                
                if(Random.Range(0, 100) > chanceAmount) continue;
                
                partition.Type = type;
            }
        }

        private bool EndRoomConditions(Partition partition)
        {
            if (partition.distanceValue > 0.7f && partition.Type == PartitionType.NONE)
                return true;
            
            return false;
        }
        
        private bool StartRoomConditions(Partition partition) => partition.distanceValue < 0.1f && partition.Type == PartitionType.NONE;
        private bool LootRoomConditions(Partition partition) => partition.distanceValue > 0.2f && partition.Type == PartitionType.NONE;
    }
}