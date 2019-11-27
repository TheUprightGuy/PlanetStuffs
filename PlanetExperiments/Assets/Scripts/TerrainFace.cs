using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainFace 
{
    Mesh mesh;
    int res;
    Vector3 localUp;

    ShapeGen shapeGen;

    Vector3 axisA, axisB;

    public TerrainFace(ShapeGen _shapeGen, Mesh mesh, int res, Vector3 localUp)
    {
        this.mesh = mesh;
        this.res = res;
        this.localUp = localUp;
        this.shapeGen = _shapeGen;

        axisA = new Vector3(localUp.y, localUp.z, localUp.x); //Up
        axisB = Vector3.Cross(localUp, axisA); //Perpendicular

    }

    public void ConstructMesh()
    {
        Vector3[] vertices = new Vector3[res * res];
        int[] triangles = new int[(res - 1) * (res - 1) * 6];

        int iTriIndex = 0;
        for (int y = 0; y < res; y++)
        {
            for (int x = 0; x < res; x++)
            {
                int i = x + (y * res); //Get point on linear array
                Vector2 percent = new Vector2(x, y) / (res - 1); //Get percentage of row completion
                Vector3 pointOnUnitCube = localUp + (percent.x - 0.5f) * 2 * axisA + (percent.y - 0.5f) * 2 * axisB; //Get final point
                Vector3 pointOnUnitSphere = pointOnUnitCube.normalized;


                vertices[i] = shapeGen.CalcPointOnPlanet(pointOnUnitSphere);

                if ((x!= res - 1) && (y != res - 1))
                {
                    triangles[iTriIndex] = i;
                    triangles[iTriIndex+1] = i + res + 1;
                    triangles[iTriIndex+2] = i + res;

                    triangles[iTriIndex+3] = i;
                    triangles[iTriIndex+4] = i + 1;
                    triangles[iTriIndex+5] = i + res + 1;
                    iTriIndex += 6;
                    
                }
            }
        }
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

    }
}
