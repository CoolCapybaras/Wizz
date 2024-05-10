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
                Texture2D texture = NativeGallery.LoadImageAtPath(path);
                if (texture == null)
                {
                    Debug.Log("Couldn't load texture from " + path);
                    return;
                }
                action.Invoke(texture);
            }
        });
    }
}
