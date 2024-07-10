//+
using Octree.Agent;
using Octree.Agent.Pathfinding;
using Octree.OctreeGeneration.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Octree.OctreeGeneration
{
    public class OctreeNode 
    {
        public Vector3 position { get; private set; }
        public float size { get; private set; }
        public int depth { get; private set; }
        private uint maxDepth;
        private uint minDepth;
        private bool leaf = true;
        public bool validNode = true;
        public bool hasValidDescendant  = false;
        public bool contourNode { get; private set; } = false;
        public uint contourScore { get; private set; } = 0; 
        public List<OctreeNode> childNodes { get; private set; }
        public List<BitArray> nodeID { get; private set; }
        public PriorityNode priorityNode { get; private set; } = null;
        public HashSet<OctreeNode> neighbourNodes { get; private set; } = null;
        public Collider[] collision { get; private set; } 
        private OctreeNodeReferences ONR;

        public OctreeNode(Vector3 position, float size, int depth, List<BitArray> nodeID, ref OctreeNodeReferences ONR)
        {
            this.position = position;
            this.size = size;
            this.minDepth = ONR.minDepth;
            this.maxDepth = ONR.maxDepth;
            this.ONR = ONR;
            this.depth = depth; 
            this.nodeID = nodeID;
            this.childNodes = new List<OctreeNode>();
            neighbourNodes = new HashSet<OctreeNode>();
            collision = new Collider[0];
            priorityNode = new PriorityNode(this.position, this);
            bool collides = CheckForCollision();
         
            if (depth <= minDepth && !collides)
            {
                leaf = false;
                Divide(ref ONR);
            } else if (depth <= maxDepth && collides)
            {
                leaf = false;
                Divide(ref ONR);
            }
            
            if (collides || !leaf)
            {
                validNode = false;
            }

            if (validNode)
            {
                ONR.graphNodes.Add(nodeID, this);
            }

            if (collides && leaf)
            {
                ONR.collisionNodes.Add(nodeID, this);
            }
        }

        private void Divide(ref OctreeNodeReferences ONR)
        {
            bool hasValidChildrenTmp = false; 
            for (int i = 0; i < 8; i++)
            {
                BitArray suffixChild = null;
                Vector3 childPos = Vector3.zero;
                float sizeIncrement = (size / 4);

                if (i == 0)
                {
                    childPos = new Vector3(position.x - sizeIncrement, position.y - sizeIncrement, position.z - sizeIncrement);
                    suffixChild = new BitArray(new bool[3] { false, false, false });
                }
                else if (i == 1)
                {
                    childPos = new Vector3(position.x - sizeIncrement, position.y - sizeIncrement, position.z + sizeIncrement);
                    suffixChild = new BitArray(new bool[3] { false, false, true });
                }
                else if (i == 2)
                {
                    childPos = new Vector3(position.x + sizeIncrement, position.y - sizeIncrement, position.z - sizeIncrement);
                    suffixChild = new BitArray(new bool[3] { true, false, false });
                }
                else if (i == 3)
                {
                    childPos = new Vector3(position.x + sizeIncrement, position.y - sizeIncrement, position.z + sizeIncrement);
                    suffixChild = new BitArray(new bool[3] { true, false, true });
                }
                else if (i == 4)
                {
                    childPos = new Vector3(position.x - sizeIncrement, position.y + sizeIncrement, position.z - sizeIncrement);
                    suffixChild = new BitArray(new bool[3] { false, true, false });
                }
                else if (i == 5)
                {
                    childPos = new Vector3(position.x - sizeIncrement, position.y + sizeIncrement, position.z + sizeIncrement);
                    suffixChild = new BitArray(new bool[3] { false, true, true });
                }
                else if (i == 6)
                {
                    childPos = new Vector3(position.x + sizeIncrement, position.y + sizeIncrement, position.z - sizeIncrement);
                    suffixChild = new BitArray(new bool[3] { true, true, false });
                }else if (i == 7)
                {
                    childPos = new Vector3(position.x + sizeIncrement, position.y + sizeIncrement, position.z + sizeIncrement);
                    suffixChild = new BitArray(new bool[3] { true, true, true });
                }

                List<BitArray> childID = new List<BitArray>(nodeID);
                childID.Add(suffixChild);
                OctreeNode child  = new OctreeNode(childPos, size/2, depth + 1, childID, ref ONR);
                childNodes.Add(child); 

                // recursion up; 
                if (child.validNode || child.hasValidDescendant && !hasValidChildrenTmp)
                {
                    hasValidChildrenTmp = true; 
                }

            }
            hasValidDescendant = hasValidChildrenTmp; 
        }

        public bool CheckForCollision()
        {
            Collider[] hitColliders = Physics.OverlapBox(position, (Vector3.one * (size)) / 2,
                Quaternion.identity, GlobalNavigationParameters.obstructionMask);
            if (hitColliders.Length > 0)
            {
                collision = hitColliders;
                return true; 
            } else
            {
                return false; 
            }
        }

        public void DefineNeighboursNode(bool type)
        {
            List<BitArray> neighboursID = GenerateNeighboursID(nodeID[nodeID.Count - 1]);
            SameSizeSameParent(neighboursID, type);
            SameSizeDifferentParent(type);
        }

        private List<BitArray> GenerateNeighboursID(BitArray ID)
        {
            List<BitArray> neighboursID = new List<BitArray>();
            neighboursID.Add(InvertBit(ID, 0));
            neighboursID.Add(InvertBit(ID, 1));
            neighboursID.Add(InvertBit(ID, 2));
            return neighboursID;
        }

        private BitArray InvertBit(BitArray bits, int index)
        {
            BitArray tmp = new BitArray(bits);
            tmp[index] = !tmp[index];
            return tmp;
        }

        private void SameSizeSameParent(List<BitArray> suffixID, bool type)
        {
            List<BitArray> neighbourID = new List<BitArray>(nodeID);
            foreach (BitArray key in suffixID)
            {
                neighbourID[neighbourID.Count - 1] = key;
                AddIfExistInDict(neighbourID, type);
            }
        }

        private bool AddIfExistInDict(List<BitArray> ID, bool type)
        {
            // graphNodes 
            if (ONR.graphNodes.ContainsKey(ID))
            {
                OctreeNode neighbour = ONR.graphNodes[ID];
                // true find dual graph nodes 
                // false find the margin of a collision 
                if (type)
                {
                    neighbourNodes.Add(neighbour);
                    neighbour.neighbourNodes.Add(this);
                    return true;
                }
                else
                {
                    for (int i = 0; i < collision.Length; i++)
                    {
                        if (!ONR.collisionContourNode.ContainsKey(collision[i]))
                        {
                            HashSet<PriorityNode> contourNodes = new HashSet<PriorityNode>();
                            contourNodes.Add(neighbour.priorityNode);
                           
                            ONR.collisionContourNode.Add(collision[i], contourNodes);
                        }
                        else
                        {
                            ONR.collisionContourNode[collision[i]].Add(neighbour.priorityNode);
                            neighbour.contourNode = true;
                            neighbour.contourScore++;
                        }
                    }
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private List<BitArray> BitShiftLevelArray(int cheangeBit)
        {
            List<BitArray> invertedArray = null;
            if (nodeID.Count - 1 > 0)
            {
                invertedArray = new List<BitArray>(nodeID);
                bool firstBit = nodeID[nodeID.Count - 1][cheangeBit];
                for (int i = nodeID.Count - 1; i >= 0; i--)
                {
                    bool nodeBit = nodeID[i][cheangeBit];
                    if (nodeBit != firstBit)
                    {
                        invertedArray[i] = InvertBit(invertedArray[i], cheangeBit);
                        break;
                    }
                    invertedArray[i] = InvertBit(invertedArray[i], cheangeBit);
                }
            }
            return invertedArray;
        }

        private void FindAncestor(List<BitArray> ID, bool type)
        {
            List<BitArray> ancestorID = new List<BitArray>(ID);
            for (int i = ID.Count - 1; i > 0; i--)
            {
                ancestorID.RemoveAt(i);
                if (AddIfExistInDict(ancestorID, type))
                {
                   break; 
                } 
            }
        }

        private void SameSizeDifferentParent(bool type)
        {
            List<BitArray> invertedArrayX = BitShiftLevelArray(0);
            List<BitArray> invertedArrayY = BitShiftLevelArray(1);
            List<BitArray> invertedArrayZ = BitShiftLevelArray(2);

            if (invertedArrayX != null)
            {
                if (!AddIfExistInDict(invertedArrayX, type))
                {
                    FindAncestor(invertedArrayX, type);
                }
            }

            if (invertedArrayY != null)
            {
                if (!AddIfExistInDict(invertedArrayY, type))
                {
                    FindAncestor(invertedArrayY, type);
                }
            }

            if (invertedArrayZ != null)
            {
                if (!AddIfExistInDict(invertedArrayZ, type))
                {
                    FindAncestor(invertedArrayZ, type);
                }
            }
        }


    }
}
