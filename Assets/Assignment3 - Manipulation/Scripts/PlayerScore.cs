using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;


public class PlayerScore : NetworkBehaviour
{
    public NetworkVariable<int> Score = new NetworkVariable<int>();
    [ServerRpc]
    public void ResetScoreServerRpc()
    {
        Score.Value = 0;
    }
    [ServerRpc]
    public void AddScoreServerRpc(int pointsToAdd)
    {
        if (IsServer)
        {
            Score.Value += pointsToAdd;
        }
    }
}
