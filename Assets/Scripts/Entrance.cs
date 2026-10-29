
using UnityEngine;

public class Entrance : MonoBehaviour
{
    [Header("Entrance Settings")]
    [SerializeField] private bool isActive = false;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private CameraController cameraController;

    [Header("Debug")]
    [SerializeField] private bool showDebug = true;

    private void OnTriggerExit(Collider other)
    {
        if (!isActive || !other.CompareTag("Player"))
            return;

        Vector3 playerPosition = other.transform.position;
        Vector3 rayOrigin = playerPosition + Vector3.up * 2f;

        if (Physics.Raycast(
            rayOrigin,
            Vector3.down,
            out RaycastHit hit,
            20f,
            groundLayer,
            QueryTriggerInteraction.Ignore))
        {
            GameObject ground = hit.collider.gameObject;

            if (!ground.CompareTag("RoomS") &&
                !ground.CompareTag("RoomB"))
                return;

            BoxCollider floor = hit.collider as BoxCollider;

            if (floor == null)
                return;

            if (showDebug)
            {
                Debug.Log(
                    $"[Entrance: {name}]\n" +
                    $"Player position: {playerPosition}\n" +
                    $"Detected floor: {ground.name}\n" +
                    $"Room type: {ground.tag}"
                );
            }

            if (cameraController != null)
                cameraController.EnterRoom(floor);
            else
                Debug.LogWarning(
                    $"[Entrance: {name}] CameraController is missing!"
                );
        }
        else if (showDebug)
        {
            Debug.LogWarning(
                $"[Entrance: {name}] No ground detected " +
                $"below player at {playerPosition}."
            );
        }
    }
}
