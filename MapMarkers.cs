using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace NyahahahahaMap;

public class MapMarkers : MonoBehaviour, IPointerClickHandler, IPointerMoveHandler
{
    public float positionMultiplier = 1;
    
    private RectTransform _imageRect;
    private Vector2 _lastLocalPoint;
    private List<Sprite> _sprites;
    private int _lastSpriteIndex;

    void Start()
    {
        _imageRect = GetComponent<RectTransform>();
        
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;

        _sprites = [
            Utils.LoadSpriteFromFile("x.png"),
            Utils.LoadSpriteFromFile("chest.png"),
            Utils.LoadSpriteFromFile("skull.png"),
            Utils.LoadSpriteFromFile("home.png"),
            Utils.LoadSpriteFromFile("question.png"),
            Utils.LoadSpriteFromFile("square.png"),
        ];

        StartCoroutine(LoadMarkers());
    }

    IEnumerator LoadMarkers()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        SaveManager.Instance.LoadPlacedMarkers();
        foreach (var marker in SaveManager.Instance.placedMarkers)
        {
            PlaceMarker(new Vector2(marker.x, marker.y), marker.spriteIndex, false);
        }
    }

    private void Update()
    {
        if (Keyboard.current.zKey.wasReleasedThisFrame)
        {
            PlaceMarker(_lastLocalPoint, _lastSpriteIndex);
        }

        if (Keyboard.current.xKey.wasReleasedThisFrame)
        {
            PlaceMarker(_lastLocalPoint, 1);
        }

        if (Keyboard.current.cKey.wasReleasedThisFrame)
        {
            PlaceMarker(_lastLocalPoint, 2);
        }

        if (Keyboard.current.vKey.wasReleasedThisFrame)
        {
            PlaceMarker(_lastLocalPoint, 3);
        }

        if (Keyboard.current.bKey.wasReleasedThisFrame)
        {
            PlaceMarker(_lastLocalPoint, 4);
        }

        if (Keyboard.current.nKey.wasReleasedThisFrame)
        {
            PlaceMarker(_lastLocalPoint, 5);
        }
    }

    void PlaceMarker(Vector2 position, int spriteIndex = 0, bool save = true)
    {
        if (position == Vector2.zero)
        {
            return;
        }
        
        var sprite = _sprites[spriteIndex];
        if (sprite == null)
        {
            sprite = _sprites[0];
        }
        
        var markerObj = new GameObject("Marker");
        markerObj.transform.SetParent(transform);
        var rectTransform = markerObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(10, 10);
        var markerRect = markerObj.GetComponent<RectTransform>();
        markerRect.anchoredPosition = position * positionMultiplier;
        var image = markerObj.AddComponent<Image>();
        image.sprite = sprite;
        var mapMarker = markerObj.AddComponent<MapMarker>();
        mapMarker.position = position;
        
        _lastSpriteIndex = spriteIndex;
        
        if (save)
        {
            SaveManager.Instance.placedMarkers.Add(new PlacedMarkerData{ spriteIndex = spriteIndex,  x = position.x,  y = position.y });
            SaveManager.Instance.SavePlacedMarkers();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        bool hit = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _imageRect,
            eventData.position,
            eventData.pressEventCamera,
            out var localPoint
        );

        if (hit)
        {
            PlaceMarker(localPoint);
        }
    }

    public void OnPointerMove(PointerEventData eventData)
    {
        bool hit = RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _imageRect,
            eventData.position,
            eventData.pressEventCamera,
            out var localPoint
        );

        if (hit)
        {
            _lastLocalPoint = localPoint;
        }
        else
        {
            _lastLocalPoint = Vector2.zero;
        }
    }
}