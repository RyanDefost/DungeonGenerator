using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using Cells;

namespace HelperScripts
{
     public class Astar
    {
        private Dictionary<Vector2Int, StepSpace> OpenSpace;
        private Dictionary<Vector2Int, StepSpace> ClosedSpace;

        private readonly GridManager gridManager;
        
        public Astar(GridManager gridManager)
        {
            this.gridManager = gridManager;
        }
        
        public List<Vector2Int> FindPathToTarget(Vector2Int startPos, Vector2Int endPos)
        {
            OpenSpace = new();
            ClosedSpace = new();

            OpenSpace.Add(startPos, CreateStepSpace(startPos, null, 0, (int)Vector2Int.Distance(startPos, endPos)));

            while (OpenSpace.Count > 0)
            {
                //a Look for lowest F score.
                StepSpace lowestScoreStepSpace = OpenSpace.First().Value;
                foreach (var openNode in OpenSpace)
                {
                    lowestScoreStepSpace = openNode.Value.FScore <= lowestScoreStepSpace.FScore
                    ? openNode.Value : lowestScoreStepSpace;
                }

                //b
                OpenSpace.Remove(lowestScoreStepSpace.position);

                //c
                List<StepSpace> neighbours = GetNeighbours(lowestScoreStepSpace);

                //d
                foreach (var neighbour in neighbours)
                {
                    //i
                    if (neighbour.position == endPos) return GetRoute(neighbour);

                    //ii
                    neighbour.GScore = lowestScoreStepSpace.GScore + Vector2Int.Distance(neighbour.position, lowestScoreStepSpace.position);
                    neighbour.HScore = Vector2Int.Distance(neighbour.position, endPos) + this.gridManager.GetNeighbors(neighbour.position).Count*2;

                    //iii
                    if (OpenSpace.ContainsKey(neighbour.position)
                        && OpenSpace[neighbour.position].FScore < neighbour.FScore) continue;
                    //iv
                    if (ClosedSpace.ContainsKey(neighbour.position)
                        && ClosedSpace[neighbour.position].FScore < neighbour.FScore) continue;

                    OpenSpace[neighbour.position] = neighbour;
                }

                //e
                if (!ClosedSpace.ContainsKey(lowestScoreStepSpace.position))
                    ClosedSpace.Add(lowestScoreStepSpace.position, lowestScoreStepSpace);
            }

            return GetClosesdRoute();
        }

        private List<StepSpace> GetNeighbours(StepSpace stepSpace)
        {
            List<StepSpace> reachable = new();
            List<Cell> cellList = this.gridManager.GetNeighbors(stepSpace.position);
            
            foreach (var cell in cellList)
            {
                if(cell.IsOccupied) continue;
                
                StepSpace currentStepSpace = CreateStepSpace(cell.Position, stepSpace, 0, 0);
                reachable.Add(currentStepSpace);
            }

            return reachable;
        }

        private List<Vector2Int> GetRoute(StepSpace endStepSpace)
        {
            List<Vector2Int> route = new();
            StepSpace currentStepSpace = endStepSpace;


            while (currentStepSpace.parent != null)
            {
                route.Add(currentStepSpace.position);
                currentStepSpace = currentStepSpace.parent;
            }
            route.Reverse();

            return route;
        }

        private List<Vector2Int> GetClosesdRoute()
        {
            //Try get to closesd node.
            StepSpace closesdStepSpace = ClosedSpace.First().Value; ;
            foreach (var node in ClosedSpace)
            {
                closesdStepSpace = node.Value.HScore < closesdStepSpace.HScore
                ? node.Value : closesdStepSpace;
            }

            return GetRoute(closesdStepSpace);
        }

        private StepSpace CreateStepSpace(Vector2Int position, StepSpace parent, int GScore, int HScore)
        {
            return new StepSpace(
                position,
                parent,
                GScore,
                HScore
            );
        }

        /// This is the Node class you can use this class to store calculated FScores for the cells of the grid, you can leave this as it is
        /// </summary>
        public class StepSpace
        {
            public Vector2Int position; //Position on the grid
            public StepSpace parent; //Parent Node of this node

            public float FScore
            { //GScore + HScore
                get { return GScore + HScore; }
            }
            public float GScore; //Current Travelled Distance
            public float HScore; //Distance estimated based on Heuristic

            public StepSpace() { }
            public StepSpace(Vector2Int position, StepSpace parent, int GScore, int HScore)
            {
                this.position = position;
                this.parent = parent;
                this.GScore = GScore;
                this.HScore = HScore;
            }
        }
    }   
}