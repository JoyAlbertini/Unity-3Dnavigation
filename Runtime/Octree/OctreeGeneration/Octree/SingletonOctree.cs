using UnityEngine;
using System.IO;

namespace Octree.OctreeGeneration
{
    
    [ExecuteAlways]
    public class SingletonOctree : MonoBehaviour
    {

        private static SingletonOctree instance = null;
        // Code source singleton Unity https://gamedev.stackexchange.com/questions/116009/in-unity-how-do-i-correctly-implement-the-singleton-pattern
        public static SingletonOctree Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = (SingletonOctree)FindObjectOfType(typeof(SingletonOctree));
                }
                return instance;
            }
        }
        public Octree octree { get; private set; } = null; 
        public Mesh octreeMesh { get; private set;} = null;

        public Octree generateOctree(Vector3 position, float size, uint minDepth, uint maxDepth, LayerMask obstacleMask, bool convexMeshes, GameObject octreeCleanAgent)
        {
            octree = new Octree(position, size, minDepth, maxDepth, obstacleMask, convexMeshes, octreeCleanAgent);
            UpdateOctreeMesh();
            return octree; 
        }

        //Reference for combining meshes [From the manual]
        //https://docs.unity3d.com/ScriptReference/Mesh.CombineMeshes.html
        private void UpdateOctreeMesh()
        {
            octreeMesh = new Mesh();
            octreeMesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
            MeshFilter[] meshes = new MeshFilter[octree.graphNodes.Count];
            CombineInstance[] combine = new CombineInstance[meshes.Length];
            int i = 0;
            foreach (OctreeNode node in octree.graphNodes)
            {
                GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                cube.transform.localScale = Vector3.one * node.size;
                cube.transform.position = transform.InverseTransformPoint(node.position);
                MeshFilter cubeMesh = cube.GetComponent<MeshFilter>();
                combine[i].mesh = cubeMesh.sharedMesh;
                combine[i].transform = cubeMesh.transform.localToWorldMatrix;
                DestroyImmediate(cube);
                i++;
            }
            octreeMesh.CombineMeshes(combine);
            octreeMesh.Optimize();
        }
            
    }
}
