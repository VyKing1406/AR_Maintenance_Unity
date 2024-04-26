using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Newtonsoft.Json;
using System.Net.Http;
using UnityEngine.Networking;
using System.Net;
using System.Text;


[Serializable]
public class Comment
{
    public int id { get; set; }
    public object objectTransformId { get; set; }
    public string content { get; set; }
    public DateTime createdDay { get; set; }
    public string userId { get; set; }
}
[Serializable]
public class ObjectTransform
{
    public int id { get; set; }
    public int index { get; set; }
    public object stationId { get; set; }
    public float positionX { get; set; }
    public float positionY { get; set; }
    public float positionZ { get; set; }
    public float rotationX { get; set; }
    public float rotationY { get; set; }
    public float rotationZ { get; set; }
    public float rotationW { get; set; }
    public float scaleX { get; set; }
    public float scaleY { get; set; }
    public float scaleZ { get; set; }
    public string maintenanceInstruction { get; set; }
    public List<Comment> comments { get; set; }
    public SensorDevice sensorDevice { get; set; }
}
[Serializable]
public class SensorDevice
{
    public int id { get; set; }
    public object sensorId { get; set; }
    public string sensorname { get; set; }
    public string sensorUnit { get; set; }
    public object stationId { get; set; }
    public string sensorImageUrl { get; set; }
}

[System.Serializable]
public class ObjectData
{
    public Vector3 position {get; set;}
    public Quaternion rotation {get; set;}
    public Vector3 scale {get; set;}

    public ObjectData(ObjectTransform objectTransform)
    {
        position = new Vector3(objectTransform.positionX, objectTransform.positionY, objectTransform.positionZ);
        rotation = new Quaternion(objectTransform.rotationX, objectTransform.rotationY, objectTransform.rotationZ, objectTransform.rotationW);
        scale = new Vector3(objectTransform.scaleX, objectTransform.scaleY, objectTransform.scaleZ);
    }

    public ObjectData()
    {
        position = new Vector3();
        rotation = new Quaternion();
        scale = new Vector3();
    }
}

public class DataControler : MonoBehaviour
{
    public static List<ObjectTransform> objectTransforms;
    public static int currentIndex = 0;



    private async void Start()
    {
        string apiUrl = "http://localhost:8080/api/object/transform/1";
        string data = await APICallerHelper.GetData(apiUrl);
        objectTransforms = JsonConvert.DeserializeObject<List<ObjectTransform>>(data);
    }



}


