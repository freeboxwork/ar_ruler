using System.Collections.Generic;
using UnityEngine;

namespace NDRO.Ruler
{
    public class NDRO_PolygonMeshCreator : MonoBehaviour
    {
        public Color color;

        void Start()
        {

        }

        public void InitMeshCreater(List<NDRO_RulerPoints> rulerPoints)
        {
            List<Transform> points = new List<Transform>();
            for (int i = 0; i < rulerPoints.Count; i++)
            {
                points.Add(rulerPoints[i].pointA);
            }
            CreateMesh(points);
        }

        public void CreateMesh(List<Transform> points)
        {
            GameObject meshObject = CreatePolygonMesh(points);
            AdjustMeshPivot(meshObject);
        }

        GameObject CreatePolygonMesh(List<Transform> points)
        {
            // 새 게임 오브젝트 생성
            GameObject meshObject = new GameObject("PolygonMeshObject");

            // MeshFilter와 MeshRenderer 컴포넌트 추가
            MeshFilter meshFilter = meshObject.AddComponent<MeshFilter>();
            MeshRenderer meshRenderer = meshObject.AddComponent<MeshRenderer>();

            // 버텍스 설정
            Vector3[] vertices = new Vector3[points.Count];
            for (int i = 0; i < points.Count; i++)
            {
                vertices[i] = points[i].position;
            }

            // 메시 생성
            Mesh mesh = new Mesh();
            mesh.vertices = vertices;

            // 트라이앵글 설정
            List<int> triangles = new List<int>();
            for (int i = 1; i < points.Count - 1; i++)
            {
                triangles.Add(0);
                triangles.Add(i);
                triangles.Add(i + 1);
            }
            mesh.triangles = triangles.ToArray();
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();

            // 메시 필터에 메시 할당
            meshFilter.mesh = mesh;

            // 적절한 머티리얼 할당
            meshRenderer.material = new Material(Shader.Find("UI/UnlitTransparent"));
            meshRenderer.material.SetColor("_Color", color);

            return meshObject;
        }

        void AdjustMeshPivot(GameObject meshObject)
        {
            Mesh mesh = meshObject.GetComponent<MeshFilter>().mesh;
            Vector3 center = mesh.bounds.center;

            // 버텍스의 위치를 중심점으로부터의 상대적 위치로 변경
            Vector3[] adjustedVertices = new Vector3[mesh.vertices.Length];
            for (int i = 0; i < mesh.vertices.Length; i++)
            {
                adjustedVertices[i] = mesh.vertices[i] - center;
            }
            mesh.vertices = adjustedVertices;
            mesh.RecalculateBounds();

            // 게임 오브젝트의 위치를 중심점으로 이동
            meshObject.transform.position = center;
        }
    }

}

