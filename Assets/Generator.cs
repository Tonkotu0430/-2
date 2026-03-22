using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Generator : MonoBehaviour
{
    [System.Serializable]
    public class Terrainsettings
    {
        public int width = 100;
        public int length = 100;

        [Range(0, 50)] public int maxHeight = 10;
        [Range(0, 1)] public float noiseScale = 0.1f;


    }

    [System.Serializable]
    public class BlockSettings
    {
        public GameObject grassBlock;
        public GameObject dirtBlock;
        public GameObject waterBlock;

        public int waterLevel = 3;
    }

    [Header("地形生成")]
    [SerializeField] private Terrainsettings terrainSettings;

    [Header("ブロック設定")]
    [SerializeField] private BlockSettings blockSettings;

    [Header("シード設定")]
    [SerializeField] private bool useRandomSeed = true;
    [SerializeField] private int seed;

    // Start is called before the first frame update
    void Start()
    {
        GenerateStage();
    }

    private void GenerateStage()
    {
        if (terrainSettings.width > 200 || terrainSettings.length > 200)
        {
            Debug.LogError("地形が大きすぎます！200以下にしてください");
            return;

        }

        if (useRandomSeed)
        {
            seed = Random.Range(0, 99999);
        }
        Random.InitState(seed);

        GameObject stageParent = new GameObject("GenerateStage");

        for (int x = 0; x < terrainSettings.width; x++)
        {

            for (int z = 0; z < terrainSettings.length; z++)
            {

                float xCoord = x * terrainSettings.noiseScale;
                float zCoord = z * terrainSettings.noiseScale;

                float Height = Mathf.PerlinNoise(xCoord + seed, zCoord + seed);

                int terrainHeight = Mathf.FloorToInt(Height * terrainSettings.maxHeight);

                GenerateColumn(x, z, terrainHeight, stageParent.transform);
            }
        }
    }



    private void GenerateColumn(int x, int z, int height, Transform parent)
    {

        for (int y = 0; y < blockSettings.waterLevel; y++)
        {
            Vector3 pos = new Vector3(x, y, z);

            if (y < height)
            {
                if (y < height)
                {
                    InstantiateBlock(blockSettings.grassBlock, pos, parent);
                }
                else
                {
                    InstantiateBlock(blockSettings.dirtBlock, pos, parent);
                }
            }
            else
            {
                InstantiateBlock(blockSettings.waterBlock, pos, parent);
            }
        }

        for (int y = blockSettings.waterLevel; y < height; y++) 
        {
            Vector3 pos = new Vector3(x, y, z);

            if (y == height - 1)
            {
                InstantiateBlock(blockSettings.grassBlock, pos, parent);
            }
            else
            {
                InstantiateBlock(blockSettings.dirtBlock, pos, parent);
            }
        }
    }

    private void InstantiateBlock(GameObject blockPrefab, Vector3 position, Transform parent)
    {
        GameObject block = Instantiate(blockPrefab, position, Quaternion.identity, parent);

        block.name = "Block_" + position.x + "Block2_" + position.y + "Block3_" + position.z;

    }
}
