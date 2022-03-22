/*
UniGif
Copyright (c) 2015 WestHillApps (Hironari Nishioka)
This software is released under the MIT License.
http://opensource.org/licenses/mit-license.php
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static partial class UniGif
{
    /// <summary>
    /// Get GIF texture list Coroutine
    /// </summary>
    /// <param name="bytes">GIF file byte data</param>
    /// <param name="filterMode">Textures filter mode</param>
    /// <param name="wrapMode">Textures wrap mode</param>
    /// <returns>IEnumerator</returns>
    public static List<GifTexture> GetTextureList(
        byte[] bytes,
        FilterMode filterMode = FilterMode.Bilinear,
        TextureWrapMode wrapMode = TextureWrapMode.Clamp)
    {
        // Set GIF data
        var gifData = new GifData();
        if (!SetGifData(bytes, ref gifData))
        {
            Debug.LogError("GIF file data set error.");
            return null;
        }

        // Decode to textures from GIF data
        List<GifTexture> gifTexList = DecodeTexture(gifData, filterMode, wrapMode);

        if (gifTexList == null || gifTexList.Count <= 0)
        {
            Debug.LogError("GIF texture decode error.");
            return null;
        }

        return gifTexList;
    }
}