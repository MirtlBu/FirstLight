using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class SpaceDustSpawner : MonoBehaviour
{
    [Header("Dust Prefabs")]
    public GameObject[] prefabs;
    public int objectCount = 100;

    [Header("Spawn Volume (meters)")]
    public Vector3 volumeSize = new Vector3(100f, 100f, 100f);
    public Vector3 volumeCenter = Vector3.zero;
    public float minimumSpacing = 0.5f;

    [Header("Repeatable Layout")]
    public int seed = 12345;

    const string GeneratedRootName = "GeneratedDust";

    [ContextMenu("Generate Dust")]
    public void GenerateDust()
    {
        if (prefabs == null || prefabs.Length == 0)
        {
            Debug.LogWarning("SpaceDustSpawner needs at least one prefab.", this);
            return;
        }

        Transform generatedRoot = GetOrCreateGeneratedRoot();
        ClearGeneratedDust(generatedRoot);

        Random.State previousState = Random.state;
        Random.InitState(seed);
        List<Vector3> positions = new List<Vector3>(objectCount);
        int attempts = 0;
        int maxAttempts = Mathf.Max(objectCount * 100, 1000);

        while (positions.Count < objectCount && attempts++ < maxAttempts)
        {
            Vector3 position = volumeCenter + new Vector3(
                Random.Range(-volumeSize.x * 0.5f, volumeSize.x * 0.5f),
                Random.Range(-volumeSize.y * 0.5f, volumeSize.y * 0.5f),
                Random.Range(-volumeSize.z * 0.5f, volumeSize.z * 0.5f));

            bool tooClose = false;
            for (int i = 0; i < positions.Count; i++)
            {
                if (Vector3.Distance(position, positions[i]) < minimumSpacing)
                {
                    tooClose = true;
                    break;
                }
            }

            if (tooClose) continue;
            positions.Add(position);

            GameObject prefab = prefabs[Random.Range(0, prefabs.Length)];
            GameObject instance = CreateInstance(prefab, generatedRoot);
            instance.transform.localPosition = position;
            instance.transform.localRotation = Random.rotation;
            instance.transform.localScale *= Random.Range(0.7f, 1.3f);
        }

        Random.state = previousState;
        Debug.Log($"Generated {positions.Count} dust objects in {volumeSize}m volume.", this);
    }

    [ContextMenu("Clear Generated Dust")]
    public void ClearGeneratedDust()
    {
        ClearGeneratedDust(GetGeneratedRoot());
    }

    Transform GetOrCreateGeneratedRoot()
    {
        Transform generatedRoot = GetGeneratedRoot();
        if (generatedRoot != null) return generatedRoot;

        GameObject root = new GameObject(GeneratedRootName);
        root.transform.SetParent(transform, false);
        return root.transform;
    }

    Transform GetGeneratedRoot()
    {
        Transform generatedRoot = transform.Find(GeneratedRootName);
        return generatedRoot;
    }

    void ClearGeneratedDust(Transform generatedRoot)
    {
        if (generatedRoot == null) return;

        while (generatedRoot.childCount > 0)
        {
            GameObject child = generatedRoot.GetChild(0).gameObject;
#if UNITY_EDITOR
            if (!Application.isPlaying)
                Undo.DestroyObjectImmediate(child);
            else
                Destroy(child);
#else
            Destroy(child);
#endif
        }
    }

    GameObject CreateInstance(GameObject prefab, Transform parent)
    {
#if UNITY_EDITOR
        if (!Application.isPlaying)
        {
            GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
            if (instance == null)
                instance = Instantiate(prefab);

            instance.transform.SetParent(parent, false);
            Undo.RegisterCreatedObjectUndo(instance, "Generate space dust");
            return instance;
        }
#endif
        return Instantiate(prefab, parent);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0.2f, 0.8f, 1f, 0.35f);
        Matrix4x4 previousMatrix = Gizmos.matrix;
        Gizmos.matrix = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(volumeCenter, volumeSize);
        Gizmos.matrix = previousMatrix;
    }
}
