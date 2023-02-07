using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
public class ObjectPlacer : MonoBehaviour
{
    [SerializeField] private PlanetChunkManager chunkManager;
    [SerializeField] private PlanetGenerator generator;
    [SerializeField] private GameObject landPrefab;
    [SerializeField] private GameObject waterPrefab;
    [SerializeField, Range(0,0.1f)] private float errorMargin;

    private Vector2 mousePos;
    private void Update() {
        mousePos = Mouse.current.position.ReadValue();
        if(Mouse.current.leftButton.wasPressedThisFrame){
            Ray r = Camera.main.ScreenPointToRay(mousePos);
            Physics.Raycast(r, out RaycastHit info);

            tryPlacePrefab(info.point, info.normal);
        }

    }

    private void tryPlacePrefab(Vector3 position, Vector3 up){
        if(Vector3.Distance(position, generator.transform.position) < generator.shapeGenerator.PlanetRadius + errorMargin) {
            GameObject go =  GameObject.Instantiate(waterPrefab, position, Quaternion.identity);
            go.transform.parent = this.transform;
            go.transform.up = up;

            chunkManager.allChunks.Add(new PlanetChunkManager.Chunk(go.GetComponent<Collider>(), go.GetComponent<MeshRenderer>()));
        } else if (Vector3.Distance(position, generator.transform.position) >= generator.shapeGenerator.PlanetRadius + errorMargin) {
            GameObject go =  GameObject.Instantiate(landPrefab, position, Quaternion.identity);
            go.transform.parent = this.transform;
            go.transform.up = up;

            chunkManager.allChunks.Add(new PlanetChunkManager.Chunk(go.GetComponent<Collider>(), go.GetComponent<MeshRenderer>()));
        } else {
            Debug.LogError("Not a valid placement");
        }
    }
    
}
