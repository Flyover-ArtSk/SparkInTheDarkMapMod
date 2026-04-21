using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace NyahahahahaMap;

[Serializable]
public struct PlacedMarkerData
{
    public float x;
    public float y;
    public int spriteIndex;
}

public class SaveManager : MonoBehaviour
{
    private static SaveManager _instance;

    public static SaveManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindFirstObjectByType<SaveManager>();
                if (_instance == null)
                {
                    GameObject obj = new GameObject("SaveManager");
                    _instance = obj.AddComponent<SaveManager>();
                    DontDestroyOnLoad(obj);
                }
            }

            return _instance;
        }
    }
    
    public List<PlacedMarkerData> placedMarkers = new();

    string GetPlacedMarkersFilePath()
    {
        return Path.Combine(Path.GetDirectoryName(Application.dataPath), "Mods", "NyahahahahaMap", $"markers_{SceneManager.GetActiveScene().name}.data");
    }

    public void LoadPlacedMarkers()
    {
        placedMarkers = BinarySaver.LoadList<PlacedMarkerData>(GetPlacedMarkersFilePath());
    }

    public void SavePlacedMarkers()
    {
        BinarySaver.SaveList(placedMarkers, GetPlacedMarkersFilePath());
    }
}