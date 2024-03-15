using System.Collections;
using UnityEngine;
using UnityEngine.XR;

public class SpellCastingController : MonoBehaviour
{
    public GameObject projectilePrefab;
    public Transform shootingPoint; // This should be the same as the hand Transform
    public float shootingForce = 1000f;
    private float gestureThreshold = 5.0f; // Adjust as needed for the gesture sensitivity
    private bool isSwingingForward = false;
    private Vector3 previousPosition;

    void Start()
    {
        if (shootingPoint == null)
        {
            Debug.LogError("Shooting Point Transform is not assigned.");
            this.enabled = false;
            return;
        }

        previousPosition = shootingPoint.position;
    }

    void Update()
    {
        Vector3 currentPosition = shootingPoint.position;
        Vector3 currentVelocity = (currentPosition - previousPosition) / Time.deltaTime;
        previousPosition = currentPosition;

        // Detecting the forward swing gesture based on velocity
        if (!isSwingingForward && currentVelocity.magnitude > gestureThreshold)
        {
            isSwingingForward = true;
        }
        // Detecting the end of the gesture
        else if (isSwingingForward && currentVelocity.magnitude <= gestureThreshold)
        {
            isSwingingForward = false;
            CastSpell(); // Cast the spell at the end of the gesture
        }
    }

    private void CastSpell()
    {
        GameObject projectile = Instantiate(projectilePrefab, shootingPoint.position + shootingPoint.forward * 1.5f, Quaternion.identity);
        ProjectileBehavior projectileBehavior = projectile.GetComponent<ProjectileBehavior>();

        // Set the owner of the projectile to the root player GameObject
        projectileBehavior.Owner = transform.root.gameObject;

        projectile.transform.forward = shootingPoint.forward;
        Rigidbody rb = projectile.GetComponent<Rigidbody>();
        rb.velocity = Vector3.zero;
        rb.AddForce(shootingPoint.forward * shootingForce, ForceMode.VelocityChange);
    }


}

