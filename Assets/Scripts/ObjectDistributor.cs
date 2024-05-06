using UnityEngine;
using UnityEditor;
using UnityEngine.AI;

public class ObjectDistributor : MonoBehaviour
{
    public GameObject objectToDistributePrefab; // The prefab of the object you want to distribute
    public int numberOfObjects = 10; // Number of objects to distribute
    public float distributionRadius = 10f; // Radius within which to distribute objects

    // This function is called in the editor script
    public void DistributeObjects()
    {
        for (int i = 0; i < numberOfObjects; i++)
        {
            Vector3 randomPoint = Random.insideUnitSphere * distributionRadius + transform.position;
            RaycastHit hit;
            if (Physics.Raycast(randomPoint + Vector3.up * 100f, Vector3.down, out hit, Mathf.Infinity, NavMesh.AllAreas))
            {
#if UNITY_EDITOR
                // Editor-only code
                GameObject prefabInstance = PrefabUtility.InstantiatePrefab(objectToDistributePrefab) as GameObject;
                prefabInstance.transform.position = hit.point;
#endif

            }
        }
    }
}
