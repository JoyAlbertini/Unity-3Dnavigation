//+
using UnityEngine;
using UnityEditor;
using Octree.OctreeGeneration.Utils; 
using System.Collections.Generic;
using System.Linq;
using System;
using Octree.Agent.Pathfinding;
using Octree.Utils;

namespace Octree.OctreeGeneration
{
    [ExecuteAlways]
    public class OctreeManager : MonoBehaviour
    {
        [Header("Octree Parameters")]
        [SerializeField] private float size = 5;
        public Vector2Int depth;
        [SerializeField] private LayerMask obstacleMask;
        [Header("Non Convex Meshes")]
        [SerializeField] private bool nonConvexMeshes;
        [SerializeField] private GameObject OctreeCleanUp;
        [Header("Debug Tools")]
        [SerializeField] private float thickness = 1f;
        [SerializeField] private bool voxels = true; 
        [SerializeField] private bool label = false; 
        [SerializeField] private bool neighboursEdges = false;
        [SerializeField] private bool tree = false;
        [SerializeField] private bool contourNodes = false;
        [SerializeField] private bool collisionNodes = false;
        [Header("Save Tools")]
        [Space(3)]
        public uint numberOfNodes;
        [Space(15)]
        [Button("Update Octree", "updatePrev")] public bool button_1;
        private Dictionary<Collider, Color> collisionsColors;
        private Octree octree = null; 

        public void updatePrev()
        {
            updateOctree();
        }

        private void OnEnable()
        {
            octree = SingletonOctree.Instance.octree;
            collisionsColors = new Dictionary<Collider, Color>();

            if (octree == null)
            {
                updateOctree();
            }
        }

        private void Start()
        {
            octree = SingletonOctree.Instance.octree;
            Debug.Log(octree);
            if (octree == null)
            {
                updateOctree();
            }
        }

        private void NonConvexCheck()
        {
            if (nonConvexMeshes)
            {
                if (OctreeCleanUp == null)
                {
                    throw new ArgumentNullException(gameObject.name + " Need a OctreeCleanAgent if some mesh are non convex");
                }
            }
        }

        public void updateOctree()
        {
            NonConvexCheck();
            octree = SingletonOctree.Instance.generateOctree(gameObject.transform.position, size, (uint) depth.x, (uint) depth.y, obstacleMask, nonConvexMeshes, OctreeCleanUp);
            SetOctreeMesh();
            RetrieveNumberOfNodes();
        }

#if UNITY_EDITOR
        private void OnValidate()
        {
            ValidationBugFix.SafeValidate(() =>
            {
                SetOctreeMesh();
            });

        }
#endif

        private void RetrieveNumberOfNodes()
        {
            if (octree != null)
            {
                numberOfNodes = (uint)octree.graphNodes.Count;
            }
        }

        private void Update()
        {
            if (octree == null)
            {
                octree = SingletonOctree.Instance.octree;
            }
        }

        private void SetOctreeMesh()
        {
            if (voxels)
            {
                transform.GetComponent<MeshFilter>().sharedMesh = SingletonOctree.Instance.octreeMesh;
            }
            else
            {
                transform.GetComponent<MeshFilter>().sharedMesh = null;
            }
        }

        private void OnDrawGizmos()
        {
            DrawVolumeCube();
            if (octree != null)
            {
                if (label || neighboursEdges)
                {
                    DrawOctreeLabelOrNeighbours();
                }

                if (tree)
                {
                    DrawTree(SingletonOctree.Instance.octree.rootNode);
                }

                if (contourNodes)
                {
                    DrawCountuor();
                }

                if (collisionNodes)
                {
                    DrawCollisions();
                }
            }
        } 

        private void DrawVolumeCube()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position, Vector3.one * size); 
        }

        private void DrawOctreeLabelOrNeighbours()
        {
            foreach (OctreeNode node in octree.graphNodes)
            {
                if (node.validNode)
                {
                    if (label)
                    {
                        #if UNITY_EDITOR
                        Handles.Label(node.position, string.Join("", node.nodeID.Select(a => "." +a.BitsToString())));
                        #endif
                    }
                    if (neighboursEdges)
                    {
                        foreach (OctreeNode neighbour in node.neighbourNodes)
                        {
                            Gizmos.color = Color.blue;
                            Gizmos.DrawLine(node.position, neighbour.position);
                        }
                    }
                }
            }
        }

        private void DrawTree(OctreeNode parent)
        {
            if (parent.hasValidDescendant)
            {
                foreach (OctreeNode child in parent.childNodes)
                {
                    if (child != null)
                    {
                        DrawTree(child);
                        if (child.validNode || child.hasValidDescendant)
                        {
                            GizmoUtils.thickLine(parent.position, child.position, Gizmos.color = new Color(0, 0, 204), thickness);
                        }
                    }
                }
            }
        }
        private void DrawCountuor()
        {
            foreach (Collider col in octree.collisionContourNode.Keys)
            { 

                if (!collisionsColors.ContainsKey(col))
                {
                    collisionsColors.Add(col, new Color(
                      UnityEngine.Random.Range(0f, 1f),
                      UnityEngine.Random.Range(0f, 1f),
                      UnityEngine.Random.Range(0f, 1f)
                  )); 
                }

                foreach (PriorityNode node in octree.collisionContourNode[col])
                {
                    Gizmos.color = collisionsColors[col];
                    Gizmos.DrawWireCube(node.position, Vector3.one * node.node.size);

                    //Handles.Label(node.position, ((int)(node.node.contourScore/node.node.size * 100)).ToString());
                    /*
                    foreach (OctreeNode neighbour in node.neighbourNodes)
                    {
                        if (all.Contains(neighbour))
                        {
                            //print("update");
                            Gizmos.color = Color.green;
                            Gizmos.DrawLine(node.position, neighbour.position);
                        }
                    } */
                }
            }
        }
   
        private void DrawCollisions()
        {
            foreach (OctreeNode node in octree.collisionNodes)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireCube(node.position, Vector3.one * node.size);
            }
        }

    }
}
