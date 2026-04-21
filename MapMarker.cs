using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

namespace NyahahahahaMap;

public class MapMarker : MonoBehaviour, IPointerClickHandler
{
    public Vector2 position = Vector2.zero;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        SaveManager.Instance.placedMarkers.RemoveAll(i => Mathf.Approximately(i.x, position.x) && Mathf.Approximately(i.y, position.y));
        SaveManager.Instance.SavePlacedMarkers();
        DestroyImmediate(gameObject);
    }
}