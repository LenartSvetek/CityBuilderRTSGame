using Unity.AI.Navigation;
using UnityEngine;

public class NavMeshBaker : MonoBehaviour
{
    public NavMeshSurface surface;

    public void Build()
    {
        surface.BuildNavMesh();
    }
}
