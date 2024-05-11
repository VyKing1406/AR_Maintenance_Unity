using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using System.Net.Http;
using UnityEngine.Networking;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using UnityEngine.UI;

public class SensorDeviceInfor : MonoBehaviour
{
    public Image image;

    private async void Start()
    {
        DataControler.DataReady += OnDataReady;
        DataControler.SensorDeviceUpdate += UpdateImageSensorDevice;
    }

    private void OnDataReady()
    {
        // Xử lý khi dữ liệu đã sẵn sàng
        
        if (DataControler.IsDataReady())
        {     
            UpdateImageSensorDevice();
        }
    }


    private void UpdateImageSensorDevice() {
        StartCoroutine(LoadImageFromUrl(DataControler.currentSensorDevice.sensorImageUrl));
    }

    IEnumerator LoadImageFromUrl(string url)
    {
        UnityWebRequest request = UnityWebRequestTexture.GetTexture(url);
        yield return request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.Log("Failed to load image from URL: " + request.error);
        }
        else
        {
            Texture2D texture = ((DownloadHandlerTexture)request.downloadHandler).texture;
            Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.one * 0.5f);
            image.sprite = sprite;
        }
    }

}
