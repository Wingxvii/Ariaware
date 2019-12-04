using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public struct TextureHook
{
    public EntityContainer chosenPrefab;
    public Material[] chosenTexture;

    public Material getRandomTexture()
    {
        if (chosenTexture.Length == 0)
            return null;

        return chosenTexture[UnityEngine.Random.Range(0, chosenTexture.Length)];
    }
}
