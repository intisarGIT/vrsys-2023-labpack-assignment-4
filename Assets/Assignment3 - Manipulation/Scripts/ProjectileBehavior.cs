using UnityEngine;
using Unity.Netcode;

public class ProjectileBehavior : MonoBehaviour
{
    public float lifetime = 5f;
    private float growthRate;
    private Vector3 initialScale;
    public GameObject Owner { get; set; } // Property to store the owner of the projectile

    void Start()
    {
        initialScale = transform.localScale;
        growthRate = (1.5f * initialScale.x - initialScale.x) / lifetime; // Adjust the growth rate to achieve 50% increase over 5 seconds
        Destroy(gameObject, lifetime); // Destroy projectile after its lifetime
    }

    void Update()
    {
        // Increase size by growthRate each second
        transform.localScale += new Vector3(growthRate, growthRate, growthRate) * Time.deltaTime;
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Target") && Owner != null)
        {
            FindObjectOfType<TargetManager>().OnDestroyTarget(other.gameObject);
            PlayerScore playerScore = Owner.GetComponent<PlayerScore>();
            if (playerScore != null)
            {
                playerScore.AddScoreServerRpc(10);
            }
            Destroy(gameObject);
        }
    }
}