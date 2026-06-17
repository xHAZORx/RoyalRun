using Unity.VisualScripting;
using UnityEngine;
using System.Collections.Generic;

public class LevelGenerator : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CameraController cameraController;
    [SerializeField] GameObject[] chunkPrefabs;
    [SerializeField] GameObject checkpointChunkPrefab;
    [SerializeField] Transform chunkParent;
    [SerializeField] ScoreManager scoreManager;

    [Header("Level Settings")]
    [SerializeField] int StartingChunksAmount = 12;
    [SerializeField] int checkpointChunkInterval = 8;
    [Tooltip("Do not change this value at runtime, it is used to calculate the spawn position of the chunks")]        
    [SerializeField] float chunkLength = 10f;    
    [SerializeField] float moveSpeed = 8f;
    [SerializeField] float minMoveSpeed = 2f;
    [SerializeField] float maxMoveSpeed = 20f;
    [SerializeField] float minGravityZ = -22f;
    [SerializeField] float maxGravityZ = -2f;


    List<GameObject> chunks = new List<GameObject>();
    int chunksSpawned = 0;

    void Start()
    {
        SpawnStartingChunks();
    }
    void Update()
    {
        MoveChunks();
    }

    public void ChangeChunkMoveSpeed(float speedAmount)
    {
        float newMoveSpeed = moveSpeed + speedAmount;
        newMoveSpeed = Mathf.Clamp(newMoveSpeed, minMoveSpeed, maxMoveSpeed);
        

        if (newMoveSpeed != moveSpeed)
        {
            moveSpeed = newMoveSpeed;

            float newGravityZ = Physics.gravity.z - speedAmount;
            newGravityZ = Mathf.Clamp(newGravityZ, minGravityZ, maxGravityZ);

            Physics.gravity = new Vector3(Physics.gravity.x, Physics.gravity.y, newGravityZ);

            cameraController.ChangeCameraFOV(speedAmount);
        }       
    }

    void SpawnStartingChunks()
    {
        for (int i = 0; i < StartingChunksAmount; i++)
        {
            SpawnChunk();
        }
    }

    private void SpawnChunk()
    {
        float spawnPositionZ = CalculateSpawnPositionZ();
        Vector3 chunkPosition = new Vector3(transform.position.x, transform.position.y, spawnPositionZ);
        GameObject ChunkToSpawn = ChooseChunkToSpawn();
        GameObject newChunkGO = Instantiate(ChunkToSpawn, chunkPosition, Quaternion.identity, chunkParent);
        chunks.Add(newChunkGO);
        Chunk newChunk = newChunkGO.GetComponent<Chunk>();
        newChunk.Init(this, scoreManager);
        chunksSpawned++;
    }

    private GameObject ChooseChunkToSpawn()
    {
        GameObject ChunkToSpawn;

        if (chunksSpawned % checkpointChunkInterval == 0 && chunksSpawned != 0)
        {
            ChunkToSpawn = checkpointChunkPrefab;
        }
        else
        {
            ChunkToSpawn = chunkPrefabs[Random.Range(0, chunkPrefabs.Length)];
        }

        return ChunkToSpawn;
    }

    float CalculateSpawnPositionZ()
    {
        float spawnPositionZ;
        if (chunks.Count == 0)
        {
            spawnPositionZ = transform.position.z;
        }
        else
        {        
            spawnPositionZ = chunks[chunks.Count - 1].transform.position.z + chunkLength;
        }

        return spawnPositionZ;
    }
    void MoveChunks()
    {
        for (int i = 0; i < chunks.Count; i++)
        {
            GameObject chunk = chunks[i];
            chunk.transform.Translate(-transform.forward * (moveSpeed * Time.deltaTime));
            if (chunk.transform.position.z <= Camera.main.transform.position.z - chunkLength)
            {
                chunks.Remove(chunk);
                Destroy(chunk);
                SpawnChunk();
            }
        }
    }
}
