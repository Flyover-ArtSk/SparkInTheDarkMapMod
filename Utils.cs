using System.IO;
using UnityEngine;
using UnityEngine.UI;

namespace NyahahahahaMap;

public static class Utils
{
    public static Sprite LoadSpriteFromFile(string fileName)
    {
        string filePath = Path.Combine(Path.GetDirectoryName(Application.dataPath) ?? string.Empty, "Mods", "NyahahahahaMap", "assets", fileName);

        if (File.Exists(filePath))
        {
            byte[] fileData = File.ReadAllBytes(filePath);
            Texture2D texture = new Texture2D(2, 2);
            
            if (texture.LoadImage(fileData))
            {
                return Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), new Vector2(0.5f, 0.5f));
            }
        }
        return null;
    }

    public static Font GetGameFont()
    {
        var defaultFont = Resources.GetBuiltinResource(typeof(Font), "LegacyRuntime.ttf") as Font;
        var obj = GameObject.Find("UserInterface/GameMenu/PauseText");
        if (obj == null)
        {
            return defaultFont;
        }
        var text = obj.GetComponent<Text>();
        if (text == null)
        {
            return defaultFont;
        }
        return text.font;
    }
    
    public static void ShowCursor()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public static Color HexToColor(string hex)
    {
        if (ColorUtility.TryParseHtmlString(hex, out var color))
        {
            return color;
        }

        return new Color(1, 1, 1, 1);
    }

    public static void HideCursor()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
}