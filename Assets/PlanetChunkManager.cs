using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class PlanetChunkManager : MonoBehaviour
{
    [SerializeField] private Transform viewTransform;
    [SerializeField, Range(0, 10)] private float viewRadius;
    [SerializeField] private LayerMask chunkLayer;

    [System.Serializable] public class Chunk{
        public Chunk(Collider _collider, MeshRenderer _renderer){
            collider = _collider;
            renderer = _renderer;
        }
        public Collider collider;
        public MeshRenderer renderer;
    }

    public List<Chunk> allChunks;
    public List<Chunk> activeChunks;

    private void Awake() {
        foreach (Chunk chunk in allChunks){
            chunk.renderer.enabled = false;
        }
    }
    
    private void FixedUpdate() {
        foreach (Chunk chunk in activeChunks){
            chunk.renderer.enabled = false;
        }
        activeChunks = new();
        
        
        //determien chunks in range
        List<Collider> hits = Physics.OverlapSphere(viewTransform.position, viewRadius, chunkLayer).ToList(); //change to non alloc
        

        foreach (Collider c in hits){
            foreach (Chunk chunk in allChunks){
                if(chunk.collider == c)
                    activeChunks.Add(chunk);
                
            }
        }

        foreach (Chunk chunk in activeChunks){
            chunk.renderer.enabled = true;
        }
    }

}
