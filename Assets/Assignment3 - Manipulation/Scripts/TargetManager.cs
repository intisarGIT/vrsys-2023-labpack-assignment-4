using System.Collections;
using Unity.Netcode;
using UnityEngine;

public class TargetManager : NetworkBehaviour
{
    public GameObject targetPrefab; // Assign the target prefab in the inspector.
    public float minSpawnRadius = 10f; // The minimum radius from the center where enemies can spawn.
    public float maxSpawnRadius = 20f; // The maximum radius from the center where enemies can spawn.
    private const int totalTargets = 10;
    private int currentTargetCount = 0;

    [ServerRpc(RequireOwnership = false)]
    public void StartGameServerRpc()
    {
        if (currentTargetCount < totalTargets)
        {
            for (int i = currentTargetCount; i < totalTargets; i++)
            {
                SpawnTarget();
            }
        }
    }

    private void SpawnTarget()
    {
        Vector3 randomPosition = CalculateRandomPositionWithinRing();
        GameObject newTarget = Instantiate(targetPrefab, randomPosition, Quaternion.identity);
        newTarget.GetComponent<NetworkObject>().Spawn();

        // Increase current target count
        currentTargetCount++;

        // Start coroutine to destroy target after 5 seconds
        StartCoroutine(DestroyTargetAfterDelay(newTarget, 20f));
    }

    private IEnumerator DestroyTargetAfterDelay(GameObject target, float delay)
    {
        yield return new WaitForSeconds(delay);

        if (target != null && target.GetComponent<NetworkObject>().IsSpawned)
        {
            target.GetComponent<NetworkObject>().Despawn();
            Destroy(target);
            currentTargetCount--;

            // Spawn a new target to replace the destroyed one
            SpawnTarget();
        }
    }
    public void OnDestroyTarget(GameObject target)
    {
        if (target != null && target.GetComponent<NetworkObject>().IsSpawned)
        {
            target.GetComponent<NetworkObject>().Despawn();
            Destroy(target);
            currentTargetCount--;

            // Check if we need to spawn a new target
            if (currentTargetCount < totalTargets)
            {
                SpawnTarget();
            }
        }
    }

    private Vector3 CalculateRandomPositionWithinRing()
    {
        Vector3 randomDirection;
        float randomDistance;

        do
        {
            // Generate a random direction and distance within the max radius
            randomDirection = Random.insideUnitSphere.normalized;
            randomDistance = Random.Range(minSpawnRadius, maxSpawnRadius);

            // Apply the distance to the direction
            randomDirection *= randomDistance;

            // Set the y-coordinate to 0 or another specific value based on your game's needs
            randomDirection.y = 0;

        } while (randomDistance < minSpawnRadius); // Ensure the point is outside the min radius

        return randomDirection;
    }
}