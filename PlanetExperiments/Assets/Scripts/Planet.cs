using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Planet : MonoBehaviour
{
    [Range(2, 256)]
    public int res = 128;

    public bool AutoUpdate = true;

    [HideInInspector]
    public bool shapeSettingsFoldOut;
    [HideInInspector]
    public bool colorSettingFoldOut;

    [SerializeField, HideInInspector]
    MeshFilter[] meshFilters;
    TerrainFace[] terrainFaces;

    public ShapeSettings shapeSettings;
    public ColorSettings colorSettings;

    ShapeGen shapeGen;

    void Init()
    {
        shapeGen = new ShapeGen(shapeSettings);

        if (meshFilters == null || meshFilters.Length == 0)
        {
            meshFilters = new MeshFilter[6];
        }

        terrainFaces = new TerrainFace[6];

        Vector3[] dirs = { Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward, Vector3.back };
        for (int i = 0; i < 6; i++)
        {
            if (meshFilters[i] == null)
            {
                GameObject newMesh = new GameObject("Mesh");
                newMesh.transform.parent = transform;

                newMesh.AddComponent<MeshRenderer>().sharedMaterial = new Material(Shader.Find("Standard"));
                meshFilters[i] = newMesh.AddComponent<MeshFilter>();
                meshFilters[i].sharedMesh = new Mesh();
            }
            terrainFaces[i] = new TerrainFace(shapeGen, meshFilters[i].sharedMesh, res, dirs[i]);
        }
    }

    public void GenPlanet()
    {
        Init();
        GenMesh();
        GenColor();
    }
    public void OnShapeSettingsUpdates()
    {
        if (AutoUpdate)
        {
            Init();
            GenMesh();
        }
        
    }

    public void OnColorSettingsUpdated()
    {
        if (AutoUpdate)
        {
            Init();
            GenColor();
        }
        
    }

    void GenMesh()
    {
        foreach(TerrainFace face in terrainFaces)
        {
            face.ConstructMesh();
        }
    }

    void GenColor()
    {
        foreach(MeshFilter m in meshFilters)
        {
            m.GetComponent<MeshRenderer>().sharedMaterial.color = colorSettings.planetColor;
        }
    }
}
