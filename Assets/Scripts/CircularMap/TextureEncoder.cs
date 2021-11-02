using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class TextureEncoder 
{
    public static void WriteTextureData(Texture2D textureOne, Texture2D textureTwo)
    {
        byte[] bytesOne = textureOne.EncodeToPNG();
        byte[] bytesTwo = textureTwo.EncodeToPNG();
        var dirPath = Application.dataPath + "/../Assets/Generated/";
        if(!Directory.Exists(dirPath)) {
            Directory.CreateDirectory(dirPath);
        }
        File.WriteAllBytes(dirPath + "_mainTexOne" + ".png", bytesOne);
        File.WriteAllBytes(dirPath + "_mainTexTwo" + ".png", bytesTwo);
        UnityEditor.AssetDatabase.Refresh();
    }
}
