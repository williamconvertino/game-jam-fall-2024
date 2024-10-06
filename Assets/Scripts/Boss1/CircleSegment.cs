using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(EdgeCollider2D))]
public class CircleSegment : MonoBehaviour
{
    public int segments;
    public float innerRadius;
    public float outerRadius;
    public float startAngle;
    public float endAngle;

    private EdgeCollider2D edgeCollider;

    // Start is called before the first frame update
    void Start()
    {
        edgeCollider = GetComponent<EdgeCollider2D>();
        GenerateArc();
    }

    private void GenerateArc()
    {
        List<Vector2> innerArc = new();
        List<Vector2> outerArc = new();

        List<Vector3> vertices = new();
        List<int> triangles = new();
        List<Vector2> uvs = new();

        float angleOffset = (endAngle - startAngle) / segments * Mathf.Deg2Rad;
        float currAngle = startAngle * Mathf.Deg2Rad;

        Vector2 unitPos = new Vector2(Mathf.Cos(currAngle), Mathf.Sin(currAngle));
        Vector2 l_i = innerRadius * unitPos;
        Vector2 l_o = outerRadius * unitPos;
        vertices.Add(l_i);
        vertices.Add(l_o);

        uvs.Add(new Vector2(1f, 0f));
        uvs.Add(new Vector2(1f, 1f));

        float uvOffset = 1f / segments;
        float currUVx = 1f - uvOffset;

        innerArc.Add(l_i);
        outerArc.Add(l_o);

        currAngle += angleOffset;

        for (int i = 1; i <= segments; i++)
        {
            int index = i * 2;
            // Draw a quad from triangles using points
            unitPos = new Vector2(Mathf.Cos(currAngle), Mathf.Sin(currAngle));
            Vector2 r_i = innerRadius * unitPos;
            Vector2 r_o = outerRadius * unitPos;
            innerArc.Add(r_i);
            outerArc.Add(r_o);

            vertices.Add(r_i); vertices.Add(r_o);
            triangles.AddRange(new List<int>
            {
                index - 2, index, index - 1,
                index, index + 1, index - 1
            });
            uvs.Add(new Vector2(currUVx, 0f));
            uvs.Add(new Vector2(currUVx, 1f));

            currAngle += angleOffset;
            currUVx -= uvOffset;
        }

        Mesh arcMesh = new Mesh();
        arcMesh.vertices = vertices.ToArray();
        arcMesh.triangles = triangles.ToArray();
        arcMesh.uv = uvs.ToArray();

        GetComponent<MeshFilter>().mesh = arcMesh;

        // Collider setup
        outerArc.Reverse();
        innerArc.AddRange(outerArc);
        innerArc.Add(innerArc[0]);
        edgeCollider.points = innerArc.ToArray();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
