using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{
    private Mesh mesh;
    [SerializeField] private LayerMask layerMask;
    private Vector3 origin;
    private float startingAngle;
    private float fov;

    private void Start()
    {
        mesh = new Mesh();
        GetComponent<MeshFilter>().mesh = mesh;
        origin = Vector3.zero;
        fov = 90f;
    }

    private void LateUpdate()
    {
        int rayCount = 50;
        float angle = startingAngle;
        float angleIncrease = fov / rayCount;
        float viewDistance = 30f;

        Vector3[] vertices = new Vector3[rayCount + 1 + 1];
        Vector2[] uv = new Vector2[vertices.Length];
        int[] triangles = new int[rayCount * 3];

        vertices[0] = origin;

        int vertexIndex = 1;
        int triangleIndex = 0;
        for(int i = 0; i <= rayCount; i++)
        {
            Vector3 vertex; 
            RaycastHit2D hit = Physics2D.Raycast(origin, VectorFromAngle(angle), viewDistance, layerMask);
            if(hit.collider == null)
            {
                vertex = origin + VectorFromAngle(angle) * viewDistance;
            }
            else
            {
                vertex = hit.point;
            }
            vertices[vertexIndex] = vertex;

            if(i > 0)
            {
                triangles[triangleIndex] = 0;
                triangles[triangleIndex + 1] = vertexIndex - 1;
                triangles[triangleIndex + 2] = vertexIndex;
                triangleIndex += 3;
            }

            vertexIndex++;
            angle -= angleIncrease;

        }

        mesh.vertices = vertices;
        mesh.uv = uv;
        mesh.triangles = triangles;
    }

    public Vector3 VectorFromAngle(float angle)
    {
        float angleRadians = angle * (Mathf.PI / 180f);
        return new Vector3(Mathf.Cos(angleRadians), Mathf.Sin(angleRadians));
    }

    public float AngleFromVector(Vector3 direction)
    {
        direction = direction.normalized;
        float angle = Mathf.Atan2(direction.x, direction.y) * Mathf.Rad2Deg;
        if(angle > 0)
        {
            angle += 360;
        }
        return angle;
    }

    public void SetOrigin(Vector3 pOrigin)
    {
        origin = pOrigin;
    }

    public void SetDirection(Vector3 direction)
    {
        startingAngle = AngleFromVector(direction) + fov / 2f;
    }
}
