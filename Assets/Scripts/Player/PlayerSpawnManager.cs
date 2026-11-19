using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSpawnManager : MonoBehaviour
{
    [SerializeField] private Transform spawnPoint1;
    [SerializeField] private Transform spawnPoint2;

    private int _playerCount = 0;

    public void OnPlayerJoined(PlayerInput playerInput)
    {
        Transform targetSpawn = (_playerCount % 2 == 0) ? spawnPoint1 : spawnPoint2;
        if (targetSpawn == null) return;

        CharacterController cc = playerInput.GetComponent<CharacterController>();
        if (cc != null) cc.enabled = false;

        playerInput.transform.SetPositionAndRotation(targetSpawn.position, targetSpawn.rotation);

        if (cc != null) cc.enabled = true;

        _playerCount++;
    }
}