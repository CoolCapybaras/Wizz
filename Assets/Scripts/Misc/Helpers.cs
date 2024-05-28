using SFB;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class Helpers
{
    public static Texture2D GetTextureFromPC()
    {
        var extensions = new[]
        {
            new ExtensionFilter("Image Files", "png", "jpg", "jpeg")
        };
        var file = StandaloneFileBrowser.OpenFilePanel("Open Image", "", extensions, false);
        var tex = new Texture2D(2, 2);
        if (string.IsNullOrEmpty(file[0]))
            return null;

        tex.LoadImage(File.ReadAllBytes(file[0]));
        return tex;
    }

    public static void GetTextureFromAndroid(Action<Texture2D> action)
    {
        NativeGallery.Permission permission = NativeGallery.GetImageFromGallery((path) =>
        {
            Debug.Log("Image path: " + path);
            if (path != null)
            {
                // Create Texture from selected image
                Texture2D texture = NativeGallery.LoadImageAtPath(path, -1, false);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
                action.Invoke(texture);
            }
        });
    }

    public static void GetTexture(Action<Texture2D> action)
    {
#if UNITY_STANDALONE_WIN || UNITY_EDITOR
        var texture = GetTextureFromPC();
        action.Invoke(texture);
#elif UNITY_ANDROID
        Helpers.GetTextureFromAndroid((avatar) =>
        {
            action.Invoke(avatar);
        });
#endif
    }
}
