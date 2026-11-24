using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

[Serializable]
public class SpawnTransformData
{
    public float x;
    public float y;
    public float z;

    // Ángulos de rotación en grados (Euler)
    public float rx;
    public float ry;
    public float rz;

    public Vector3 ToVector3() => new Vector3(x, y, z);
    public Quaternion ToQuaternion() => Quaternion.Euler(rx, ry, rz);
}

[Serializable]
public class SpawnTransformListWrapper
{
    public List<SpawnTransformData> positions;
}

public class SpawAudience : MonoBehaviour
{
    private List<GameObject> spawnedAudience = new List<GameObject>();
    public List<GameObject> audiencePrefabs;
    private Coroutine spawnAudienceCoroutine;
    [SerializeField] private float spawnInterval = 1f;

    // Se agregaron rx, ry, rz (en grados) a cada objeto del JSON
    private string positionsJson = @"{
        ""positions"": [
            { ""x"": -10.47,  ""y"": 0.093, ""z"": 0.1020748, ""rx"": 0, ""ry"": -165.439,  ""rz"": 0 },
            { ""x"": -12.823, ""y"": 0.112, ""z"": -1.433615,  ""rx"": 0, ""ry"": -168.501, ""rz"": 0 },
            { ""x"": -11.822, ""y"": 0.151, ""z"": -2.185783, ""rx"": 0, ""ry"": -158.974,  ""rz"": 0 },
            { ""x"": -10.24596, ""y"": 0.127, ""z"": -2.410222,  ""rx"": 0, ""ry"": -143.535, ""rz"": 0 },
            { ""x"": -7.540265,  ""y"": 0.149, ""z"": -1.492139,  ""rx"": 0, ""ry"": -136.661,  ""rz"": 0 }
        ]
    }";
    private List<SpawnTransformData> spawnTransforms = new List<SpawnTransformData>();

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

        SpawnTransformListWrapper data = JsonUtility.FromJson<SpawnTransformListWrapper>(positionsJson);
        if (data != null && data.positions != null)
        {
            spawnTransforms = data.positions;
        }
    }

    private IEnumerator SpawnAudience()
    {
        int currentIndex = 0;

        while (currentIndex < spawnTransforms.Count && currentIndex < audiencePrefabs.Count)
        {
            Vector3 spawnPosition = spawnTransforms[currentIndex].ToVector3();
            Quaternion spawnRotation = spawnTransforms[currentIndex].ToQuaternion();

            GameObject instance = Instantiate(audiencePrefabs[currentIndex], spawnPosition, spawnRotation);
            spawnedAudience.Add(instance);

            currentIndex++;

            yield return new WaitForSeconds(spawnInterval);
        }
        spawnAudienceCoroutine = null;
    }

    public void ClearAudiendie()
    {
        if (spawnAudienceCoroutine != null)
        {
            StopCoroutine(spawnAudienceCoroutine);
            spawnAudienceCoroutine = null;
        }

        foreach (GameObject obj in spawnedAudience)
        {
            if (obj != null)
            {
                Destroy(obj);
            }
        }

        spawnedAudience.Clear();
    }
}