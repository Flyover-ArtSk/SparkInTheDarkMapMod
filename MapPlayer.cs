using UnityEngine;
using UnityEngine.Serialization;

namespace NyahahahahaMap;

public class MapPlayer : MonoBehaviour
{
    public Transform player;
    public RectTransform mapRect;
    public Vector2 worldBoundsMin;
    public Vector2 worldBoundsMax;
    public float rotationOffset;

    private RectTransform _iconRect;

    void Start()
    {
        _iconRect = GetComponent<RectTransform>();

        if (player == null)
            player = GameObject.FindGameObjectWithTag("Player")?.transform;

        if (mapRect == null)
            mapRect = transform.parent?.GetComponent<RectTransform>();
    }

    void Update()
    {
        UpdateIconPosition();
    }

    void UpdateIconPosition()
    {
        if (player == null || mapRect == null)
        {
            return;
        }
        
        Vector3 playerPos = player.position;
        Vector2 playerPos2D = new Vector2(playerPos.x, playerPos.z);

        float normX = Mathf.InverseLerp(worldBoundsMin.x, worldBoundsMax.x, playerPos2D.x);
        float normY = Mathf.InverseLerp(worldBoundsMin.y, worldBoundsMax.y, playerPos2D.y);

        normX = Mathf.Clamp01(normX);
        normY = Mathf.Clamp01(normY);

        Vector2 mapSize = mapRect.rect.size;
        Vector2 anchoredPos = new Vector2(normX * mapSize.x, normY * mapSize.y);

        _iconRect.anchoredPosition = anchoredPos;

        var yaw = player.eulerAngles.y;
        var minimapRotation = mapRect.localEulerAngles.z;
        _iconRect.localRotation = Quaternion.Euler(0, 0, -yaw + rotationOffset);
    }
}