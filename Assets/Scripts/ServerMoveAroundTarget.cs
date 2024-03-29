using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public class ServerMoveAroundTarget : NetworkBehaviour
{
    public Transform target;

    public float degreesPerSecond = 20;

    private Vector3 targetPositionXZ
    {
        get
        {
            return new Vector3(target.position.x, 0, target.position.z);
        }
    }

    private Vector3 positionXZ
    {
        get
        {
            return new Vector3(transform.position.x, 0, transform.position.z);
        }
    }

    private Vector3 directionToTarget
    {
        get
        {
            return (targetPositionXZ - positionXZ);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!IsServer)
            return;
        var newPosition = CalculatePositionUpdate();
        var newRotation = CalculateRotationUpdate(newPosition);
        transform.position = newPosition;
        transform.rotation = newRotation;
    }

    Vector3 CalculatePositionUpdate()
    {
        var y = transform.position.y;
        var newPosition = targetPositionXZ - RotationUtils.ManualYRotation(directionToTarget, degreesPerSecond * Time.deltaTime);
        newPosition.y = y;
        return newPosition;
    }

    Quaternion CalculateRotationUpdate(Vector3 newPosition)
    {
        var movementDir = newPosition - transform.position;
        return Quaternion.LookRotation(movementDir.normalized, Vector3.up);
    }
}
