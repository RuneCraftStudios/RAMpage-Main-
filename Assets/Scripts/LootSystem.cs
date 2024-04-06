using UnityEngine;
using System.Collections.Generic;

public class LootSystem : MonoBehaviour
{
    [System.Serializable]
    public class LootItem
    {
        public GameObject item;
        public float dropChancePercentage;
    }

    public List<LootItem> lootItems;
    public float upwardForce = 5f; // Upward force applied to the dropped items
    public float horizontalForce = 2f; // Horizontal force applied to the dropped items
    [SerializeField] private Vector3 offset; // Serialized private offset variable

    public void DropItems(GameObject referenceObject, Vector3 offset)
    {
        Vector3 spawnPosition = referenceObject.transform.position + offset;

        foreach (LootItem lootItem in lootItems)
        {
            // Generate a random number between 0 and 100
            float randomNumber = Random.Range(0f, 100f);

            // Check if the random number is less than the drop chance percentage
            if (randomNumber <= lootItem.dropChancePercentage)
            {
                // Instantiate the item at the specified position
                GameObject droppedItem = Instantiate(lootItem.item, spawnPosition, Quaternion.identity);

                // Add upward and horizontal forces to the dropped item
                Rigidbody rb = droppedItem.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    rb.AddForce(Vector3.up * upwardForce, ForceMode.Impulse);
                    rb.AddForce(Vector3.right * Random.Range(-horizontalForce, horizontalForce), ForceMode.Impulse);
                }
            }
        }
    }
}
