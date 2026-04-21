using System.Collections.Generic;
using System.Diagnostics;
using UI;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace NyahahahahaMap;

public class MapManager: MonoBehaviour
{
    private struct WorldBounds(Vector2 min, Vector2 max)
    {
        public readonly Vector2 Min = min;
        public readonly Vector2 Max = max;
    }
    
    private static MapManager _instance;
    public static MapManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<MapManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("MapManager");
                    _instance = obj.AddComponent<MapManager>();
                    DontDestroyOnLoad(obj);
                }
            }
            return _instance;
        }
    }
    
    private GameObject _player;
    private string _currentSceneName;
    private WorldBounds _currentWorldBounds;
    private Dictionary<string, Sprite> _markerTypes;
    private bool _inited;
    private bool _show;
    private Canvas _mapCanvas;
    private Canvas _miniMapCanvas;
    private readonly Dictionary<string, WorldBounds> _worldBounds = new()
    {
        { "Level1", new WorldBounds(new Vector2(68, 31), new Vector2(-268, -311)) },
        { "Level2", new WorldBounds(new Vector2(-13, -19), new Vector2(287, 280)) },
        { "Level3", new WorldBounds(new Vector2(436, -13), new Vector2(136, -313)) },
    };

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            SceneManager.sceneLoaded -= OnSceneLoaded;
            SceneManager.sceneLoaded += OnSceneLoaded;
            DontDestroyOnLoad(gameObject);
        }
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if(!string.Equals(scene.path, SceneManager.GetActiveScene().path))
        {
            return;
        }

        _inited = false;
    }
    
    public void HandleMap()
    {
        if ((Keyboard.current != null && Keyboard.current.mKey.wasReleasedThisFrame) || (Gamepad.current != null && Gamepad.current.rightStickButton.wasPressedThisFrame))
        {
            if (_show)
            {
                HideMap();
            }
            else
            {
                ShowMap();
            }
        }
    }

    public void ShowMap()
    {
        _player = GameObject.Find("Player");
        if (_player == null)
        {
            return;
        }

        _currentSceneName = SceneManager.GetActiveScene().name;

        if (!_worldBounds.TryGetValue(_currentSceneName, out var bounds))
        {
            return;
        }
        _currentWorldBounds = bounds;

        _miniMapCanvas.enabled = false;
        Utils.ShowCursor();
        Game.SetGamePause(true);

        _show = true;

        if (_inited)
        {
            _mapCanvas.enabled = true;
            return;
        }

        _markerTypes = new Dictionary<string, Sprite>()
        {
            { "Z", Utils.LoadSpriteFromFile("x.png") },
            { "X", Utils.LoadSpriteFromFile("chest.png") },
            { "C", Utils.LoadSpriteFromFile("skull.png") },
            { "V", Utils.LoadSpriteFromFile("home.png") },
            { "B", Utils.LoadSpriteFromFile("question.png") },
            { "N", Utils.LoadSpriteFromFile("square.png") },
        };
        
        var mapCanvasObj = new GameObject("MapCanvas");
        SceneManager.MoveGameObjectToScene(mapCanvasObj, SceneManager.GetActiveScene());
        var canvas = mapCanvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        _mapCanvas = canvas;
        var canvasScaler = mapCanvasObj.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
        canvasScaler.scaleFactor = 1f;
        canvasScaler.referencePixelsPerUnit = 100f;
        canvasScaler.referenceResolution = new Vector2(800, 600);
        mapCanvasObj.AddComponent<GraphicRaycaster>();
        
        var mapImage = CreateMap(mapCanvasObj.transform);
        CreateMapLegend(mapImage.transform);
        CreateMapPlayer(mapImage.transform);
        
        _inited = true;
    }
    
    public void HideMap(bool hideCursor = true, bool unpauseGame = true)
    {
        if (_mapCanvas == null)
        {
            return;
        }
        
        _miniMapCanvas.enabled = true;
        _mapCanvas.enabled = false;
        _show = false;
        
        if (hideCursor)
        {
            Utils.HideCursor();
        }

        if (unpauseGame)
        {
            Game.SetGamePause(false);
        }
    }

    Image CreateMap(Transform parent = null)
    {
        var mapBg1 = new GameObject("Bg1");
        mapBg1.transform.SetParent(parent);
        var mapBg1Rect = mapBg1.AddComponent<RectTransform>();
        mapBg1Rect.pivot = new Vector2(0.5f, 0.5f);
        mapBg1Rect.anchorMin = new Vector2(0f, 0f);
        mapBg1Rect.anchorMax = new Vector2(1f, 1f);
        mapBg1Rect.offsetMin = Vector2.zero;
        mapBg1Rect.offsetMax = Vector2.zero;
        var mapBg1Image = mapBg1.AddComponent<Image>();
        mapBg1Image.color = Utils.HexToColor("#000000D2");
        
        var mapBg2 = new GameObject("Bg2");
        mapBg2.transform.SetParent(parent); 
        var mapBg2Rect = mapBg2.AddComponent<RectTransform>();
        mapBg2Rect.sizeDelta = new Vector2(600, 600);
        mapBg2Rect.anchoredPosition = new Vector2(0, 0);
        mapBg2Rect.pivot = new Vector2(0.5f, 0.5f);
        var mapBg2Image = mapBg2.AddComponent<Image>();
        mapBg2Image.sprite = Utils.LoadSpriteFromFile("bg.png");
        mapBg2Image.color = Utils.HexToColor("#000000");
        
        var mapImageOj = new GameObject("Map");
        mapImageOj.transform.SetParent(parent); 
        var rectTransform = mapImageOj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(400, 400);
        rectTransform.anchoredPosition = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);
        mapImageOj.AddComponent<MapMarkers>();
        var mapImage = mapImageOj.AddComponent<Image>();
        var sprite = Utils.LoadSpriteFromFile($"Map_{_currentSceneName}.png");
        mapImage.sprite = sprite;
        mapImage.color = Utils.HexToColor("#C4DEFF");
        
        var topDivider = new GameObject("TopDivider");
        topDivider.transform.SetParent(mapImageOj.transform); 
        var topDividerRect = topDivider.AddComponent<RectTransform>();
        topDividerRect.sizeDelta = new Vector2(0, 3);
        topDividerRect.anchorMin = new Vector2(0f, 1f);
        topDividerRect.anchorMax = new Vector2(1f, 1f);
        topDividerRect.anchoredPosition = new Vector2(0, 0);
        topDividerRect.pivot = new Vector2(0.5f, 0.5f);
        var topDividerImage = topDivider.AddComponent<Image>();
        topDividerImage.sprite = Utils.LoadSpriteFromFile("line.png");
        
        var bottomDivider = new GameObject("bottomDivider");
        bottomDivider.transform.SetParent(mapImageOj.transform); 
        var bottomDividerRect = bottomDivider.AddComponent<RectTransform>();
        bottomDividerRect.sizeDelta = new Vector2(0, 3);
        bottomDividerRect.anchorMin = new Vector2(0f, 0f);
        bottomDividerRect.anchorMax = new Vector2(1f, 0f);
        bottomDividerRect.anchoredPosition = new Vector2(0, 0);
        bottomDividerRect.pivot = new Vector2(0.5f, 0.5f);
        var bottomDividerImage = bottomDivider.AddComponent<Image>();
        bottomDividerImage.sprite = Utils.LoadSpriteFromFile("line.png");
        
        var itemTextObj = new GameObject("Title");
        itemTextObj.transform.SetParent(mapImageOj.transform); 
        var itemTextRect = itemTextObj.AddComponent<RectTransform>();
        itemTextRect.sizeDelta = new Vector2(160, 50);
        itemTextRect.anchoredPosition = new Vector2(0, 15);
        itemTextRect.anchorMin = new Vector2(0.5f, 1f);
        itemTextRect.anchorMax = new Vector2(0.5f, 1f);
        itemTextRect.pivot = new Vector2(0.5f, 0.5f);
        var itemText = itemTextObj.AddComponent<Text>();
        itemText.text = LocalizationManager.Instance.CurrentLanguage == SystemLanguage.Russian ? "* КАРТА *" : "* MAP *";
        itemText.font = Utils.GetGameFont();
        itemText.fontSize = 14;
        itemText.color = Utils.HexToColor("#AA883F");
        itemText.fontStyle = FontStyle.Normal;
        itemText.alignment = TextAnchor.MiddleCenter;

        return mapImage;
    }

    Image CreateMapLegend(Transform parent = null)
    {
        if (parent == null)
        {
            return null;
        }
        
        var obj = new GameObject("Legend");
        obj.transform.SetParent(parent); 
        var rectTransform = obj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(0, 20);
        rectTransform.anchorMin = new Vector2(0f, 0f);
        rectTransform.anchorMax = new Vector2(1f, 0f);
        rectTransform.anchoredPosition = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(1f, 0f);

        var grid = obj.AddComponent<GridLayoutGroup>();
        grid.cellSize = new Vector2(20, 20);
        grid.spacing = new Vector2(20, 0);
        grid.startCorner = GridLayoutGroup.Corner.LowerLeft;
        grid.startAxis = GridLayoutGroup.Axis.Horizontal;
        grid.childAlignment = TextAnchor.LowerCenter;
        grid.constraint = GridLayoutGroup.Constraint.Flexible;
        
        foreach(KeyValuePair<string, Sprite> entry in _markerTypes)
        {
            CreateLegendItem(entry.Key, entry.Value, obj.transform);
        }

        return null;
    }

    void CreateLegendItem(string text, Sprite sprite, Transform parent)
    {
        if (parent == null)
        {
            return;
        }

        var itemObj = new GameObject("Item");
        itemObj.transform.SetParent(parent); 
        var rectTransform = itemObj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(20, 20);
        rectTransform.anchoredPosition = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        var itemImageObj = new GameObject("Image");
        itemImageObj.transform.SetParent(itemObj.transform); 
        var itemImageRect = itemImageObj.AddComponent<RectTransform>();
        itemImageRect.sizeDelta = new Vector2(10, 10);
        itemImageRect.anchoredPosition = new Vector2(0, 0);
        itemImageRect.pivot = new Vector2(0.5f, 0.5f);
        var mapImage = itemImageObj.AddComponent<Image>();
        mapImage.sprite = sprite;
        
        var itemTextObj = new GameObject("Text");
        itemTextObj.transform.SetParent(itemObj.transform); 
        var itemTextRect = itemTextObj.AddComponent<RectTransform>();
        itemTextRect.sizeDelta = new Vector2(25, 25);
        itemTextRect.anchoredPosition = new Vector2(15, 1);
        itemTextRect.pivot = new Vector2(0.5f, 0.5f);
        var itemText = itemTextObj.AddComponent<Text>();
        itemText.text = "[" + text + "]";
        itemText.font = Utils.GetGameFont();
        itemText.fontSize = 8;
        itemText.fontStyle = FontStyle.Normal;
        itemText.alignment = TextAnchor.MiddleCenter;
    }
    
    Image CreateMapPlayer(Transform parent = null)
    {
        if (parent == null)
        {
            return null;
        }
        
        var playerImageOj = new GameObject("MapPlayer");
        playerImageOj.transform.SetParent(parent.transform); 
        var rectTransform = playerImageOj.AddComponent<RectTransform>();
        rectTransform.sizeDelta = new Vector2(6, 6);
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(0, 0);
        rectTransform.anchoredPosition = new Vector2(0, 0);
        rectTransform.pivot = new Vector2(0.5f, 0.5f);

        var playerImage = playerImageOj.AddComponent<Image>();
        playerImage.sprite = Utils.LoadSpriteFromFile("player_arrow.png");
        playerImage.color = Utils.HexToColor("#AA883F");
        
        var mapPlayer = playerImageOj.AddComponent<MapPlayer>();
        
        if (_player != null)
        {
            mapPlayer.player = _player.transform;
            mapPlayer.mapRect = parent.GetComponent<RectTransform>();
            mapPlayer.worldBoundsMin = _currentWorldBounds.Min;
            mapPlayer.worldBoundsMax = _currentWorldBounds.Max;
            mapPlayer.rotationOffset = _currentSceneName == "Level2" ? 0 : 180;
        }

        return playerImage;
    }
    
    public void CreateMiniMap()
    {
        _player = GameObject.Find("Player");
        if (_player == null)
        {
            return;
        }
        
        _currentSceneName = SceneManager.GetActiveScene().name;

        if (!_worldBounds.TryGetValue(_currentSceneName, out var bounds))
        {
            return;
        }
        _currentWorldBounds = bounds;
        
        Utils.HideCursor();
        
        var mapCanvasObj = new GameObject("MiniMapCanvas");
        SceneManager.MoveGameObjectToScene(mapCanvasObj, SceneManager.GetActiveScene());
        var canvas = mapCanvasObj.AddComponent<Canvas>();
        _miniMapCanvas = canvas;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var canvasScaler = mapCanvasObj.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
        canvasScaler.scaleFactor = 1f;
        canvasScaler.referencePixelsPerUnit = 100f;
        canvasScaler.referenceResolution = new Vector2(800, 600);
        mapCanvasObj.AddComponent<GraphicRaycaster>();
        
        var miniMapMask = new GameObject("MiniMapMask");
        miniMapMask.transform.SetParent(mapCanvasObj.transform);
        var miniMapMaskRect = miniMapMask.AddComponent<RectTransform>();
        miniMapMaskRect.sizeDelta = new Vector2(150, 150);
        miniMapMaskRect.pivot = new Vector2(0.5f, 0.5f);
        miniMapMaskRect.anchorMin = new Vector2(0f, 1f);
        miniMapMaskRect.anchorMax = new Vector2(0f, 1f);
        miniMapMaskRect.anchoredPosition = new Vector2(75, -75);
        var miniMapMaskRectMask = miniMapMask.AddComponent<RectMask2D>();
        miniMapMaskRectMask.softness = new Vector2Int(50, 50);
        
        var miniMapImageOj = new GameObject("MiniMap");
        miniMapImageOj.transform.SetParent(miniMapMaskRectMask.transform); 
        var miniMapImageRect = miniMapImageOj.AddComponent<RectTransform>();
        miniMapImageRect.sizeDelta = new Vector2(800, 800);
        miniMapImageRect.anchoredPosition = new Vector2(0, 0);
        miniMapImageRect.pivot = new Vector2(0.5f, 0.5f);
        var mapMarkers = miniMapImageOj.AddComponent<MapMarkers>();
        mapMarkers.positionMultiplier = 2;
        var miniMapImage = miniMapImageOj.AddComponent<Image>();
        var sprite = Utils.LoadSpriteFromFile($"Map_{_currentSceneName}.png");
        miniMapImage.sprite = sprite;
        miniMapImage.color = Utils.HexToColor("#C4DEFF");
        
        var miniMapPlayer = CreateMapPlayer(miniMapImageOj.transform);
        
        var miniMapFollow = miniMapImageOj.AddComponent<MiniMapFollow>();
        miniMapFollow.minimapRect = miniMapImageRect;
        miniMapFollow.playerMarkerRect = miniMapPlayer.rectTransform;
        miniMapFollow.rotationOffset = _currentSceneName == "Level2" ? 0 : 180;
    }
    
    public void CreateTransparentMap()
    {
        _player = GameObject.Find("Player");
        if (_player == null)
        {
            return;
        }
        
        _currentSceneName = SceneManager.GetActiveScene().name;
    
        if (!_worldBounds.TryGetValue(_currentSceneName, out var bounds))
        {
            return;
        }
        _currentWorldBounds = bounds;
        
        var mapCanvasObj = new GameObject("TransparentMapCanvas");
        SceneManager.MoveGameObjectToScene(mapCanvasObj, SceneManager.GetActiveScene());
        var canvas = mapCanvasObj.AddComponent<Canvas>();
        _miniMapCanvas = canvas;
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        var canvasScaler = mapCanvasObj.AddComponent<CanvasScaler>();
        canvasScaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        canvasScaler.screenMatchMode = CanvasScaler.ScreenMatchMode.MatchWidthOrHeight;
        canvasScaler.matchWidthOrHeight = 0.5f;
        canvasScaler.scaleFactor = 1f;
        canvasScaler.referencePixelsPerUnit = 100f;
        canvasScaler.referenceResolution = new Vector2(800, 600);
        mapCanvasObj.AddComponent<GraphicRaycaster>();
        var canvasGroup = mapCanvasObj.AddComponent<CanvasGroup>();
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0.3f;
        
        var miniMapMask = new GameObject("TransparentMapMask");
        miniMapMask.transform.SetParent(mapCanvasObj.transform);
        var miniMapMaskRect = miniMapMask.AddComponent<RectTransform>();
        miniMapMaskRect.sizeDelta = new Vector2(300, 300);
        miniMapMaskRect.anchoredPosition = new Vector2(0, 0);
        miniMapMaskRect.pivot = new Vector2(0.5f, 0.5f);
        var miniMapMaskRectMask = miniMapMask.AddComponent<RectMask2D>();
        miniMapMaskRectMask.softness = new Vector2Int(50, 50);
        
        var miniMapImageOj = new GameObject("TransparentMap");
        miniMapImageOj.transform.SetParent(miniMapMask.transform); 
        var miniMapImageRect = miniMapImageOj.AddComponent<RectTransform>();
        miniMapImageRect.sizeDelta = new Vector2(400, 400);
        miniMapImageRect.anchoredPosition = new Vector2(0, 0);
        miniMapImageRect.pivot = new Vector2(0.5f, 0.5f);
        miniMapImageOj.AddComponent<MapMarkers>();
        var miniMapImage = miniMapImageOj.AddComponent<Image>();
        var sprite = Utils.LoadSpriteFromFile($"Map_{_currentSceneName}_T.png");
        miniMapImage.sprite = sprite;
        miniMapImage.color = Utils.HexToColor("#C4DEFF");
        miniMapImageOj.AddComponent<MapMarkers>();
        
        var miniMapPlayer = CreateMapPlayer(miniMapImageOj.transform);
        
        var miniMapFollow = miniMapImageOj.AddComponent<MiniMapFollow>();
        miniMapFollow.minimapRect = miniMapImageRect;
        miniMapFollow.playerMarkerRect = miniMapPlayer.rectTransform;
    }
}