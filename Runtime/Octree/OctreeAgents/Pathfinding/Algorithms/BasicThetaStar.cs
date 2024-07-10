//+
namespace Octree.Agent.Pathfinding
{
    public class BasicThetaStar : Astar
    {
        public BasicThetaStar() : base() {}
       
        protected override void UpdateNodePriority(PriorityNode currentNode, PriorityNode neighbourNode)
        {
            if (agent.LineOfSight(currentNode.parent.position, neighbourNode.position))
            {
                lineOfSightCheck++;
                float gScoreTmp = currentNode.parent.gScore + G(currentNode.parent, neighbourNode);

                if (gScoreTmp < neighbourNode.gScore)
                {
                    neighbourNode.gScore = gScoreTmp;
                    neighbourNode.parent = currentNode.parent;
                    neighbourNode.fScore = F(currentNode);
                    replaceOrAdd(neighbourNode);
                }
            } else
            {
                base.UpdateNodePriority(currentNode, neighbourNode);
            }
        }
    }
}