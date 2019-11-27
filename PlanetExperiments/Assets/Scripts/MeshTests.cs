using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct Triangle
{
    public int pointIndexA;
    public int pointIndexB;
    public int pointIndexC;
}

public class MeshTests : MonoBehaviour
{
    Mesh m_mesh;

    Vector3 axisA, axisB;
    Vector3 localUp;

    [Range(2, 64)]
    public int res = 2;

    [Range(0, 5)]
    public int MaxDepth = 2;
    // Start is called before the first frame update
    void Start()
    {
    }

    private void OnValidate()
    {
        if (this.gameObject.GetComponent<MeshRenderer>() == null)
        {
            this.gameObject.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
        }
        if (this.gameObject.GetComponent<MeshFilter>() == null)
        {
            m_mesh = this.gameObject.AddComponent<MeshFilter>().sharedMesh = new Mesh();
        }
        if (this.gameObject.GetComponent<MeshFilter>().sharedMesh == null)
        {
            this.gameObject.GetComponent<MeshFilter>().sharedMesh = new Mesh();
        }

        localUp = this.transform.up;
        axisA = new Vector3(localUp.y, localUp.z, localUp.x); //Up
        axisB = Vector3.Cross(localUp, axisA); //Perpendicular

        CreateBaseMesh(res);
        CreateFractal();
    }

    void CreateBaseMesh(int _res)
    {
        Vector3[] vertices = new Vector3[_res * _res];
        int[] triangles = new int[(_res - 1) * (_res - 1) * 6];

        int iTriIndex = 0;
        for (int y = 0; y < _res; y++)
        {
            for (int x = 0; x < _res; x++)
            {
                int i = x + (y * _res); //Get point on linear array
                Vector2 percent = new Vector2(x, y) / (_res - 1); //Get percentage of row completion
                Vector3 pointOnUnitCube = localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB; //Get final point
                vertices[i] = pointOnUnitCube;

                if ((x != _res - 1) && (y != _res - 1))
                {
                    triangles[iTriIndex] = i;
                    triangles[iTriIndex + 1] = i + _res + 1;
                    triangles[iTriIndex + 2] = i + _res;

                    triangles[iTriIndex + 3] = i;
                    triangles[iTriIndex + 4] = i + 1;
                    triangles[iTriIndex + 5] = i + _res + 1;
                    iTriIndex += 6;

                }
            }
        }

        m_mesh.Clear();
        m_mesh.vertices = vertices;
        m_mesh.triangles = triangles;
        m_mesh.RecalculateNormals();
    }

    void CreateFractal()
    {
        List<Vector3> vertices = new List<Vector3>();
        List<Triangle> triangles = new List<Triangle>();
        List<int> trianglesint = new List<int>();

        for (int i = 0; i < m_mesh.vertices.Length; i++)
        {
            vertices.Add(m_mesh.vertices[i]);
        }

        for (int i = 0; i < m_mesh.triangles.Length; i += 3)
        {
            Triangle newTriangle;
            newTriangle.pointIndexA = m_mesh.triangles[i];
            newTriangle.pointIndexB = m_mesh.triangles[i + 1];
            newTriangle.pointIndexC = m_mesh.triangles[i + 2];
            //triangles.Add(newTriangle);

            FractalMesh(newTriangle, ref vertices, ref triangles, MaxDepth);
        }

        Vector3[] FINALvertices = new Vector3[vertices.Count];
        int[] FINALtriangles = new int[triangles.Count * 3];

        for (int i = 0; i < vertices.Count; i++)
        {
            FINALvertices[i] = vertices[i];
        }

        int v = 0;
        for (int i = 0; i < triangles.Count; i++)
        {
            FINALtriangles[v] = triangles[i].pointIndexA;
            FINALtriangles[v + 1] = triangles[i].pointIndexB;
            FINALtriangles[v + 2] = triangles[i].pointIndexC;
            v += 3;
        }

        m_mesh.Clear();
        m_mesh.vertices = FINALvertices;
        m_mesh.triangles = FINALtriangles;
        m_mesh.RecalculateNormals();
    }

    void FractalMesh(Triangle _start, ref List<Vector3> _vertices, ref List<Triangle> _triangles, int _MaxDepth,  int _depth = 0)
    {
        int depth = _depth;

        Triangle upTriangle;
        Triangle innerTriangle;
        Triangle leftTriangle;
        Triangle rightTriangle;

        Vector3 ab;
        ab = _vertices[_start.pointIndexA] + 
            (Vector3.Normalize(_vertices[_start.pointIndexB] - _vertices[_start.pointIndexA]) * 
            (Vector3.Distance(_vertices[_start.pointIndexB], _vertices[_start.pointIndexA]) * 0.5f));
        _vertices.Add(ab);
        innerTriangle.pointIndexA = _vertices.Count-1;

        Vector3 bc;
        bc = _vertices[_start.pointIndexB] +
            (Vector3.Normalize(_vertices[_start.pointIndexC] - _vertices[_start.pointIndexB]) *
            (Vector3.Distance(_vertices[_start.pointIndexC], _vertices[_start.pointIndexB]) * 0.5f));
        _vertices.Add(bc);
        innerTriangle.pointIndexB = _vertices.Count-1;

        Vector3 ca;
        ca = _vertices[_start.pointIndexC] +
            (Vector3.Normalize(_vertices[_start.pointIndexA] - _vertices[_start.pointIndexC]) *
            (Vector3.Distance(_vertices[_start.pointIndexA], _vertices[_start.pointIndexC]) * 0.5f));
        _vertices.Add(ca);
        innerTriangle.pointIndexC = _vertices.Count-1;


        upTriangle.pointIndexA = innerTriangle.pointIndexA;// innerTriangle.pointIndexA;
        upTriangle.pointIndexB = _start.pointIndexB;// innerTriangle.pointIndexB;
        upTriangle.pointIndexC = innerTriangle.pointIndexB; // _start.pointIndexC;

        leftTriangle.pointIndexA = _start.pointIndexA; //
        leftTriangle.pointIndexB = innerTriangle.pointIndexA;//
        leftTriangle.pointIndexC = innerTriangle.pointIndexC;//

        rightTriangle.pointIndexB = innerTriangle.pointIndexB;
        rightTriangle.pointIndexA = innerTriangle.pointIndexC;
        rightTriangle.pointIndexC = _start.pointIndexC;

        if (_depth != _MaxDepth)
        {
            depth++;

            FractalMesh(innerTriangle, ref _vertices, ref _triangles, _MaxDepth, depth);
            FractalMesh(leftTriangle, ref _vertices, ref _triangles, _MaxDepth, depth);
            FractalMesh(rightTriangle, ref _vertices, ref _triangles, _MaxDepth, depth);
            FractalMesh(upTriangle, ref _vertices, ref _triangles, _MaxDepth, depth);
        }
        else
        {
            _triangles.Add(innerTriangle);
            _triangles.Add(upTriangle);
            _triangles.Add(leftTriangle);
            _triangles.Add(rightTriangle);
        }
    }
}
