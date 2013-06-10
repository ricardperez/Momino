using UnityEngine;
using System.Collections;

[RequireComponent (typeof (MeshFilter))]
public class Circle : MonoBehaviour {
	
	private int nTriangles;
	private float radius;
	
	public void SetRadius(float radius)
	{
		this.radius = radius;
		this.nTriangles = (int)(40*Mathf.Sqrt(radius));
		this.Recalculate();
	}

    void Start() {
		MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
		if (meshFilter == null)
		{
			meshFilter = this.gameObject.AddComponent<MeshFilter>();
		}
		
		MeshRenderer meshRenderer = this.gameObject.GetComponent<MeshRenderer>();
		if (meshRenderer == null)
		{
			meshRenderer = this.gameObject.AddComponent<MeshRenderer>();
		}
		meshRenderer.material.shader = Shader.Find("Diffuse");
		meshRenderer.material.color = Color.red;
		
    }
	
	private void Recalculate()
	{
		MeshFilter meshFilter = this.gameObject.GetComponent<MeshFilter>();
		Mesh mesh = new Mesh();
        meshFilter.mesh = mesh;
		
		float angleStep = (2*Mathf.PI / nTriangles);
		
		Vector3 []vertices = new Vector3[nTriangles+3];
		int []triangles = new int[3*(nTriangles+1)];
		Vector3 []normals = new Vector3[nTriangles+3];
		Vector2 []uv = new Vector2[nTriangles+3];
		
		Vector3 normal = new Vector3(0.0f, 1.0f, 0.0f);
		
		float xCoordinate = radius;
		float yCoordinate = 0.0f;
		float zCoordinate = 0.0f;
		float currAngle = 0.0f;
		
		vertices[0] = new Vector3(0.0f, yCoordinate, 0.0f);
		normals[0] = normal;
		uv[0] = new Vector2(0.5f, 0.5f);
		
		vertices[1] = new Vector3(xCoordinate, yCoordinate, zCoordinate);
		normals[1] = normal;
		uv[1] = new Vector2(1.0f, 0.5f);
		
		for (int triangleIndex = 0; triangleIndex <= nTriangles; ++triangleIndex)
		{
			xCoordinate = radius*Mathf.Sin(currAngle);
			zCoordinate = radius*Mathf.Cos(currAngle);
			
			int vertexIndex = (triangleIndex+2);
			Vector3 nextVertex = new Vector3(xCoordinate, yCoordinate, zCoordinate);
			vertices[vertexIndex] = nextVertex;
			normals[vertexIndex] = normal;
			uv[vertexIndex] = new Vector2(xCoordinate/radius, zCoordinate/radius);
			
			triangles[3*triangleIndex] = 0;
			triangles[3*triangleIndex+1] = vertexIndex-1;
			triangles[3*triangleIndex+2] = vertexIndex;
			
			currAngle += angleStep;
		}
		
		mesh.vertices = vertices;
		mesh.triangles = triangles;
		mesh.normals = normals;
		mesh.uv = uv;
	}
}
