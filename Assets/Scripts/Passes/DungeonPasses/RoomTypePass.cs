using System;
using System.Collections.Generic;
using System.Linq;
using Cells;
using Partitions;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Passes.DungeonPass
{
    [CreateAssetMenu(fileName = "Pass", menuName = "Pass/DungeonPass/RoomType", order = 1)]
    public class RoomTypePass : BaseDungeonPass
    {
        [Header("Room Info")]
        [SerializeField] private PartitionType partitionType;
        [SerializeField] private Color roomColor;

        [Header("Conditions")] [SerializeField]
        private bool moreThenDistance = true;
        [SerializeField, Range(0.01f,0.9f)] private float distanceFromStart;
        [SerializeField] private PartitionType replacePartition;
        
        [Header("Spawning parameters")]
        [SerializeField] private int minAmount = 0;
        [SerializeField] private int maxAmount = 3;
        [SerializeField, Range(1, 100)] private int chanceAmount = 100; 
        
        public override bool SetPass()
        {
            return AssignPartitionType();
        }

        public bool AssignPartitionType()
        {

            List<Partition> candidates;
            //Closer then.
            if(moreThenDistance) candidates = this.generator.partitions
                .Where(partition => partition.distanceValue < distanceFromStart && partition.Type == replacePartition).ToList();
            //Further then.
            else candidates = this.generator.partitions
                .Where(partition => partition.distanceValue > distanceFromStart && partition.Type == replacePartition).ToList();
            
            int desiredAmount = Random.Range(minAmount, maxAmount);
            for (int i = 0; i < desiredAmount; i++)
            {
                if (candidates.Count == 0)
                {
                    Debug.LogWarning($"There where no candidates found for {partitionType} partition");
                    return false;
                }
                
                Partition partition = candidates[Random.Range(0, candidates.Count)];
                candidates.Remove(partition);

                if (Random.Range(0, 100) > chanceAmount && !isRequired)
                    continue;
                
                partition.Type = this.partitionType;
                
                foreach (Cell cells in partition.Cells.Where(cells => cells.Type == CellType.GROUND))
                    cells.GameObject.GetComponent<SpriteRenderer>().color = roomColor;
            }

            return true;
        }
    }
}