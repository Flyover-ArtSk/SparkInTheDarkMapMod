using UnityEngine;

namespace NyahahahahaMap;

public class MiniMapFollow : MonoBehaviour
{
    public RectTransform minimapRect;
    public RectTransform playerMarkerRect;
    public float rotationOffset;

    private readonly float _speed = 30f;
    private Camera _playerCamera;

    void Start()
    {
        _playerCamera = Camera.main;
    }

    void LateUpdate()
    {
        Quaternion targetRot = Quaternion.Euler(0, 0, _playerCamera.transform.eulerAngles.y + rotationOffset);
        Vector3 markerLocalPos = playerMarkerRect.localPosition;
        Vector3 targetLocalPos = -(targetRot * markerLocalPos);

        var t = 1f - Mathf.Exp(-_speed * Time.deltaTime);

        minimapRect.localPosition = Vector3.Lerp(minimapRect.localPosition, targetLocalPos, t);
        minimapRect.localRotation = Quaternion.Slerp(minimapRect.localRotation, targetRot, t);
    }
}