using System.Collections.Generic;
using UnityEngine;

public class MeshDrawer {
        
    private Triangulator triangulator;
    private List<int> triangles;

    public MeshDrawer() {
        this.triangulator = new Triangulator();
        this.triangles = new List<int>();
    }
    
    public Mesh GetMesh(List<Vector3> vertices, bool plane = true) {
        Mesh mesh = new Mesh();
        
        triangles.Clear();
        
        Vector2[] vertices2D = new Vector2[vertices.Count];

        if (plane) {
            for (int i = 0; i < vertices.Count; i++) {
                vertices2D[i] = new Vector2(vertices[i].x, vertices[i].z);
            }
        } else {
            for (int i = 0; i < vertices.Count; i++) {
                vertices2D[i] = new Vector2(vertices[i].x, vertices[i].y);
            }
        }

        triangles = new List<int>(triangulator.Triangulate(vertices2D));

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        
        Vector2[] uv = new Vector2[vertices.Count];
        
        for (int i = 0; i < vertices.Count; i++) {
            uv[i] = new Vector2((vertices[i].x - mesh.bounds.min.x) / (mesh.bounds.max.x - mesh.bounds.min.x), (vertices[i].y - mesh.bounds.min.y) / (mesh.bounds.max.y - mesh.bounds.min.y));
        }

        mesh.uv = uv;

        return mesh;
    }
}