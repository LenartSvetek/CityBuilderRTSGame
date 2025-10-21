using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralMesh : MonoBehaviour
{
    void Start()
    {
        Mesh mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;

        // Define vertices
        Vector3[] vertices = new Vector3[]
        {
            new Vector3(0, 0, 0), // Bottom Left
            new Vector3(1, 0, 0), // Bottom Right
            new Vector3(0, 1, 0), // Top Left
            new Vector3(1, 1, 0)  // Top Right
        };

        // Define triangles (two triangles make a quad)
        int[] triangles = new int[]
        {
            0, 2, 1, // First triangle
            2, 3, 1  // Second triangle
        };

        // Optional: Define UVs for texturing
        Vector2[] uvs = new Vector2[]
        {
            new Vector2(0, 0),
            new Vector2(1, 0),
            new Vector2(0, 1),
            new Vector2(1, 1)
        };

        // Assign to mesh
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.uv = uvs;

        // Recalculate normals for lighting
        mesh.RecalculateNormals();
    }
}
