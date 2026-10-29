
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraController : MonoBehaviour
{
    [Header("Room")]
    [SerializeField] private BoxCollider currentFloor;
    [SerializeField] private Transform player;

    [Header("Camera Settings")]
    [SerializeField, Range(30f, 90f)]
    private float cameraAngle = 75f;

    [SerializeField] private bool autoHeight = true;
    [SerializeField] private float cameraHeight = 30f;

    [SerializeField, Range(1f, 1.5f)]
    private float framingMargin = 1.1f;

    private Camera cam;
    private float currentHeight;
    private Vector2 smallRoomSize;
    private bool hasSmallRoomReference;
    private BoxCollider lastSmallFloor;
    private Vector3 cameraTarget;

    [Header("Room Transition")]
    [SerializeField, Min(0.01f)]
    private float transitionSmoothTime = 0.4f;

    private Vector3 desiredPosition;
    private Vector3 transitionVelocity;
    private bool isTransitioning;
    private bool isInitialized;

    private void Awake()
    {
        cam = GetComponent<Camera>();
        cam.orthographic = false;
    }

    private void Start()
    {
        if (currentFloor != null)
            EnterRoom(currentFloor);

        isInitialized = true;
    }

    private void LateUpdate()
    {
        if (currentFloor == null)
            return;

        // Urmarirea Player-ului in incaperile mari.
        if (currentFloor.CompareTag("RoomB") &&
            player != null &&
            hasSmallRoomReference)
        {
            Bounds bounds = currentFloor.bounds;

            float halfWidth = smallRoomSize.x / 2f;
            float halfLength = smallRoomSize.y / 2f;

            Vector3 target = cameraTarget;

            // Urmarire pe X.
            if (player.position.x > target.x + halfWidth)
            {
                target.x = player.position.x - halfWidth;
            }
            else if (player.position.x < target.x - halfWidth)
            {
                target.x = player.position.x + halfWidth;
            }

            // Urmarire pe Z.
            if (player.position.z > target.z + halfLength)
            {
                target.z = player.position.z - halfLength;
            }
            else if (player.position.z < target.z - halfLength)
            {
                target.z = player.position.z + halfLength;
            }

            // Limitele podelei mari.
            target.x = ClampToRoom(
                target.x,
                bounds.min.x,
                bounds.max.x,
                halfWidth
            );

            target.z = ClampToRoom(
                target.z,
                bounds.min.z,
                bounds.max.z,
                halfLength
            );

            target.y = bounds.center.y;

            if ((target - cameraTarget).sqrMagnitude > 0.000001f)
            {
                PositionCamera(target, currentHeight);
            }
        }

        // SmoothDamp doar in timpul tranzitiei.
        if (isTransitioning)
        {
            transform.position = Vector3.SmoothDamp(
                transform.position,
                desiredPosition,
                ref transitionVelocity,
                transitionSmoothTime
            );

            // Verificam daca am ajuns suficient de aproape.
            if (Vector3.Distance(
                transform.position,
                desiredPosition
            ) < 0.01f)
            {
                transform.position = desiredPosition;
                transitionVelocity = Vector3.zero;
                isTransitioning = false;
            }
        }
    }
    private void OnValidate()
    {
        cam = GetComponent<Camera>();

        if (cam == null || currentFloor == null)
            return;

        if (currentFloor.CompareTag("RoomS"))
        {
            SetSmallRoom(currentFloor);
        }
        else if (currentFloor.CompareTag("RoomB"))
        {
            if (lastSmallFloor != null)
                SetSmallRoom(lastSmallFloor);

            SetBigRoom(currentFloor);
        }
    }

    public void EnterRoom(BoxCollider floor)
    {
        if (floor == null)
            return;

        if (floor == currentFloor &&
            Application.isPlaying &&
            currentHeight > 0f)
            return;

        bool roomChanged = currentFloor != floor;

        currentFloor = floor;

        if (Application.isPlaying && isInitialized && roomChanged)
        {
            isTransitioning = true;
            transitionVelocity = Vector3.zero;
        }

        if (floor.CompareTag("RoomS"))
            SetSmallRoom(floor);
        else if (floor.CompareTag("RoomB"))
            SetBigRoom(floor);
    }

    private float ClampToRoom(
        float position,
        float min,
        float max,
        float margin)
    {
        if (max - min < margin * 2f)
            return (min + max) / 2f;

        return Mathf.Clamp(
            position,
            min + margin,
            max - margin
        );
    }

    private void SetSmallRoom(BoxCollider floor)
    {
        lastSmallFloor = floor;
        Bounds bounds = floor.bounds;
        Vector3 center = bounds.center;

        smallRoomSize = new Vector2(
            bounds.size.x,
            bounds.size.z
        );

        hasSmallRoomReference = true;

        float angle = cameraAngle * Mathf.Deg2Rad;
        float height = cameraHeight;

        if (autoHeight)
        {
            float halfWidth = bounds.extents.x * framingMargin;
            float halfLength = bounds.extents.z * framingMargin;

            float verticalFOV = cam.fieldOfView * Mathf.Deg2Rad;
            float horizontalFOV = 2f * Mathf.Atan(
                Mathf.Tan(verticalFOV / 2f) * cam.aspect
            );

            float distanceX =
                halfWidth / Mathf.Tan(horizontalFOV / 2f);

            float distanceZ =
                halfLength / Mathf.Tan(verticalFOV / 2f);

            float distance = Mathf.Max(distanceX, distanceZ);
            height = distance * Mathf.Sin(angle);

            float sinAngle = Mathf.Sin(angle);
            float cosAngle = Mathf.Cos(angle);
            float tanV = Mathf.Tan(verticalFOV / 2f);
            float tanH = Mathf.Tan(horizontalFOV / 2f);

            float requiredHeight = 0f;

            foreach (float x in new float[] { -halfWidth, halfWidth })
            {
                foreach (float z in new float[] { -halfLength, halfLength })
                {
                    float depthOffset = z * cosAngle;
                    float verticalOffset = z * sinAngle;

                    float heightForVertical =
                        (Mathf.Abs(verticalOffset) / tanV
                        + depthOffset) * sinAngle;

                    float heightForHorizontal =
                        (Mathf.Abs(x) / tanH
                        + depthOffset) * sinAngle;

                    requiredHeight = Mathf.Max(
                        requiredHeight,
                        heightForVertical,
                        heightForHorizontal
                    );
                }
            }

            height = Mathf.Max(height, requiredHeight);
        }

        currentHeight = height;
        PositionCamera(center, height);
    }

    private void SetBigRoom(BoxCollider floor)
    {
        Bounds bounds = floor.bounds;

        float height = currentHeight > 0f
            ? currentHeight
            : cameraHeight;

        Vector3 target = bounds.center;

        if (player == null || !hasSmallRoomReference)
        {
            PositionCamera(target, height);
            return;
        }

        Vector3 playerPosition = player.position;

        float halfWidth = smallRoomSize.x / 2f;
        float halfLength = smallRoomSize.y / 2f;

        // Distantele Player-ului fata de cei patru pereti.
        float distanceWest = Mathf.Abs(
            playerPosition.x - bounds.min.x
        );

        float distanceEast = Mathf.Abs(
            bounds.max.x - playerPosition.x
        );

        float distanceSouth = Mathf.Abs(
            playerPosition.z - bounds.min.z
        );

        float distanceNorth = Mathf.Abs(
            bounds.max.z - playerPosition.z
        );

        // Determinam peretele prin care a intrat Player-ul.
        float minDistance = Mathf.Min(
            distanceWest,
            distanceEast,
            distanceSouth,
            distanceNorth
        );

        if (minDistance == distanceSouth)
        {
            // Intrare din sud.
            target.z = bounds.min.z + halfLength;

            target.x = ClampToRoom(
                playerPosition.x,
                bounds.min.x,
                bounds.max.x,
                halfWidth
            );
        }
        else if (minDistance == distanceNorth)
        {
            // Intrare din nord.
            target.z = bounds.max.z - halfLength;

            target.x = ClampToRoom(
                playerPosition.x,
                bounds.min.x,
                bounds.max.x,
                halfWidth
            );
        }
        else if (minDistance == distanceWest)
        {
            // Intrare din vest.
            target.x = bounds.min.x + halfWidth;

            target.z = ClampToRoom(
                playerPosition.z,
                bounds.min.z,
                bounds.max.z,
                halfLength
            );
        }
        else
        {
            // Intrare din est.
            target.x = bounds.max.x - halfWidth;

            target.z = ClampToRoom(
                playerPosition.z,
                bounds.min.z,
                bounds.max.z,
                halfLength
            );
        }

        target.y = bounds.center.y;

        PositionCamera(target, height);
    }
    private void PositionCamera(Vector3 target, float height)
    {
        cameraTarget = target;

        float angle = cameraAngle * Mathf.Deg2Rad;
        float horizontalOffset = height / Mathf.Tan(angle);

        desiredPosition = target + new Vector3(
            0f,
            height,
            -horizontalOffset
        );

        transform.rotation = Quaternion.Euler(
            cameraAngle,
            0f,
            0f
        );

        // Inainte de prima tranzitie, pozitionam instantaneu.
        if (!Application.isPlaying || !isInitialized)
        {
            transform.position = desiredPosition;
        }
        // In timpul urmaririi normale, nu folosim SmoothDamp.
        else if (!isTransitioning)
        {
            transform.position = desiredPosition;
        }
    }
}