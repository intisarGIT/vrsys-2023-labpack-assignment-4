using Unity.Netcode;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameStarter : MonoBehaviour
{
    [Header("H.O.M.E.R. Components")]
    public Transform head;
    public float originHeadOffset = 0.2f;
    public Transform hand;

    [Header("H.O.M.E.R. Parameters")]
    public LineRenderer ray;
    public float rayMaxLength = 100f;
    public LayerMask layerMask;

    [Header("Input Actions")]
    public InputActionProperty grabAction;

    private void Awake()
    {
        ray.enabled = true;
        ray.positionCount = 2;
    }

    private void Start()
    {
        if (GetComponentInParent<NetworkObject>() != null && !GetComponentInParent<NetworkObject>().IsOwner)
        {
            Destroy(this);
            return;
        }
    }

    private void Update()
    {
        UpdateRay();

        if (grabAction.action.WasPressedThisFrame())
        {
            CheckForGameStart();
        }
    }

    private void UpdateRay()
    {
        Vector3 origin = hand.position;
        Vector3 direction = hand.forward;

        if (Physics.Raycast(origin, direction, out var hit, rayMaxLength, layerMask))
        {
            ray.SetPosition(0, origin);
            ray.SetPosition(1, hit.point);
            ray.startColor = hit.collider.CompareTag("GameButton") ? Color.blue : Color.green;
            ray.endColor = ray.startColor;
        }
        else
        {
            ray.SetPosition(0, origin);
            ray.SetPosition(1, origin + direction * rayMaxLength);
            ray.startColor = Color.red;
            ray.endColor = Color.red;
        }
    }

    private void CheckForGameStart()
    {
        if (Physics.Raycast(hand.position, hand.forward, out var hit, rayMaxLength, layerMask) && hit.collider.CompareTag("GameButton"))
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        if (NetworkManager.Singleton.IsServer)
        {
            var targetManager = FindObjectOfType<TargetManager>();
            if (targetManager != null)
            {
                targetManager.StartGameServerRpc();
            }
            else
            {
                Debug.LogError("TargetManager not found in the scene.");
            }
        }
        else
        {
            Debug.Log("Only the server can start the game.");
        }
    }
}
