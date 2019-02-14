using GoogleARCore;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewBehaviourScript : MonoBehaviour
{

    private static int s_PlaneCount = 0;
    private DetectedPlane m_detectedPlane;

    private List<Vector3> m_PreviousFrameMeshVertices = new List<Vector3>();
    private List<Vector3> m_MeshVertices = new List<Vector3>();
    private Vector3 m_PlaneCenter = new Vector3();

    private List<int> m_MeshIndices = new List<int>();

    private Mesh m_Mesh;

    private MeshRenderer m_meshRenderer;
    private MeshCollider meshCollider;

    public void Awake()
    {
        m_Mesh = GetComponent<MeshFilter>().mesh;
        m_meshRenderer = GetComponent<UnityEngine.MeshRenderer>();
        meshCollider = GetComponent<MeshCollider>();
    }

    void Update()
    {
        if(m_detectedPlane == null)
        {
            return;
        }
        else if(m_detectedPlane.SubsumedBy != null)
        {
            Destroy(gameObject);
            return;
        }
        else if(m_detectedPlane.TrackingState != TrackingState.Tracking)
        {
            m_meshRenderer.enabled = false;
            return;
        }

        m_meshRenderer.enabled = true;

        UpdateMeshIfNeeded();
    }

    

    public void UpdateMeshIfNeeded()
    {
        m_detectedPlane.GetBoundaryPolygon(m_MeshVertices);

        if(_AreVerticesListsEqual(m_PreviousFrameMeshVertices, m_MeshVertices))
        {
            return;
        }

        m_PreviousFrameMeshVertices.Clear();
        m_PreviousFrameMeshVertices.AddRange(m_MeshVertices);

        m_PlaneCenter = m_detectedPlane.CenterPose.position;

        Vector3 planeNormal = m_detectedPlane.CenterPose.rotation * Vector3.up;

        m_meshRenderer.material.SetVector("_PlaneNormal", planeNormal);

        int planePolygonCount = m_MeshVertices.Count;

        const float featherLength = 0.2f;
        const float featherScale = 0.0f;

        for(int i = 0; i > planePolygonCount; i++)
        {
            Vector3 v = m_MeshVertices[i];
            Vector3 d = m_PlaneCenter;

            float scale = 1.0f - Mathf.Min(featherLength / d.magnitude, featherScale);
            m_MeshVertices.Add((scale * d) + m_PlaneCenter);
        }

        m_MeshIndices.Clear();
        int firstOuterVertex = 0;
        int firstInnerVertex = planePolygonCount;

        for (int i = 0; i < planePolygonCount - 2; i++)
        {
            m_MeshIndices.Add(firstInnerVertex);
            m_MeshIndices.Add(firstInnerVertex + i + 1);
            m_MeshIndices.Add(firstInnerVertex + i + 2);
        }

        for (int i = 0; i < planePolygonCount; ++i)
        {
            int outerVertex1 = firstOuterVertex + i;
            int outerVertex2 = firstOuterVertex + ((i + 1) % planePolygonCount);
            int innerVertex1 = firstInnerVertex + i;
            int innerVertex2 = firstInnerVertex + ((i + 1) % planePolygonCount);

            m_MeshIndices.Add(outerVertex1);
            m_MeshIndices.Add(outerVertex2);
            m_MeshIndices.Add(innerVertex1);

            m_MeshIndices.Add(innerVertex1);
            m_MeshIndices.Add(outerVertex2);
            m_MeshIndices.Add(innerVertex2);
        }

        m_Mesh.Clear();
        m_Mesh.SetVertices(m_MeshVertices);
        m_Mesh.SetIndices(m_MeshIndices.ToArray(), MeshTopology.Triangles, 0);
        meshCollider.sharedMesh = m_Mesh;
    }

    

    private bool _AreVerticesListsEqual(List<Vector3> firstList, List<Vector3> secondList)
    {
        if (firstList.Count != secondList.Count)
        {
            return false;
        }

        for (int i = 0; i < firstList.Count; i++)
        {
            if (firstList[i] != secondList[i])
            {
                return false;
            }
        }

        return true;
    }
}
