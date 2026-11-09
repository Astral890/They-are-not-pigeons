using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class PositionData
{
    public float x;
    public float y;
    public float z;

    public Vector3 ToVector3() => new Vector3(x, y, z);
}

[Serializable]
public class PositionListWrapper
{
    public List<PositionData> positions;
}

public class SpawAudience : MonoBehaviour
{
    [SerializeField] private GameObject audiencePrefab;
    private Coroutine spawnAudienceCoroutine;
    [SerializeField] private float spawnInterval = 1f;

    private string positionsJson = @"{
        ""positions"": [
            { ""x"": -10.6, ""y"": 1.2, ""z"": -0.93 },
            { ""x"": -11.69, ""y"": 1.0, ""z"": 1.88 },
            { ""x"": -10.63, ""y"": 1.0, ""z"": -4.02 },
            { ""x"": -10.22, ""y"": 1.0, ""z"": 3.99 },
            { ""x"": -9.33, ""y"": 1.2, ""z"": 0.45 }
        ]
    }";
    private List<PositionData> spawnPositions = new List<PositionData>();

    private void Awake()
    {
        LoadPositionsFromJson();
    }

    public void StartSpawningAudience()
    {
        if (spawnAudienceCoroutine != null)
        {
            StopCoroutine(spawnAudienceCoroutine);
        }

        spawnAudienceCoroutine = StartCoroutine(SpawnAudience());
    }

    private void LoadPositionsFromJson()
    {
        if (string.IsNullOrEmpty(positionsJson)) return;

        PositionListWrapper data = JsonUtility.FromJson<PositionListWrapper>(positionsJson);
        if (data != null && data.positions != null)
        {
            spawnPositions = data.positions;
        }
    }

    private IEnumerator SpawnAudience()
    {
        int currentIndex = 0;

        while (currentIndex < 5)
        {
            Vector3 spawnPosition = transform.position;
                // Toma la posición secuencialmente del JSON
            spawnPosition = spawnPositions[currentIndex].ToVector3();
            currentIndex = currentIndex + 1;

            Instantiate(audiencePrefab, spawnPosition, Quaternion.identity);

            yield return new WaitForSeconds(spawnInterval);
        }
        spawnAudienceCoroutine = null; // Reset the coroutine reference when done
        yield break;
    }
}
