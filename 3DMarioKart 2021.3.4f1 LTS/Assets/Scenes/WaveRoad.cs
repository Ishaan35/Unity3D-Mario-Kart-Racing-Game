using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveRoad : MonoBehaviour
{
    SkinnedMeshRenderer meshRenderer;
    MeshCollider collider;

    // Start is called before the first frame update
    void Start()
    {
        meshRenderer = GetComponent<SkinnedMeshRenderer>();
        collider = GetComponent<MeshCollider>();
    }

    private float time = 0;
    // Update is called once per frame
    void Update()
    {
        UpdateCollider();
    }



    public void UpdateCollider()
    {
        Mesh colliderMesh = new Mesh();
        meshRenderer.BakeMesh(colliderMesh);
        collider.sharedMesh = null;
        collider.sharedMesh = colliderMesh;
    }
}