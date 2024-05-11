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
    public string createdDay { get; set; }
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
    public string videoUrl {get; set;}
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
    [SerializeField] public static List<ObjectTransform> objectTransforms;
    [SerializeField] public static List<SensorDevice> sensorDevices;
    [SerializeField]  public static SensorDevice currentSensorDevice;
    [SerializeField] public static int currentIndex = 0;
    private static Boolean isFetched = false;
    public static string BASE_URL = "http://192.168.1.2:8080/api";
    public static string stationName = "air_0001";
    public static int stationId = 1;
    public static Boolean isFormCancel = false;
    [SerializeField] public static event System.Action DataReady;
    [SerializeField] public static event System.Action SensorDeviceUpdate;

    private void Start()
    {
        DataControler.fetchData();
    }

    private void Update() {

    }

    public static async void fetchData() {
        string data = await APICallerHelper.GetData(BASE_URL + "/object/transform/1");
        DataControler.objectTransforms = JsonConvert.DeserializeObject<List<ObjectTransform>>(data);
        data = await APICallerHelper.GetData(BASE_URL + "/sensor-device/1");
        DataControler.sensorDevices = JsonConvert.DeserializeObject<List<SensorDevice>>(data);
        UpdateCurrentSensorDevice(DataControler.objectTransforms[DataControler.currentIndex].sensorDevice);
        DataControler.isFetched = true;
        DataReady?.Invoke();
    }

    public static bool IsDataReady()
    {
        return isFetched;
    }

    public static void UpdateCurrentIndex(int newIndex) {
        DataControler.currentIndex = newIndex;
    }

    public static void UpdateCurrentSensorDevice(SensorDevice sensorDevice) {
        DataControler.currentSensorDevice = sensorDevice;
        SensorDeviceUpdate?.Invoke();
    }

    public static async void UpdateObjectList() {
        string data = await APICallerHelper.GetData(BASE_URL + "/object/transform/1");
        DataControler.objectTransforms = JsonConvert.DeserializeObject<List<ObjectTransform>>(data);
    }

}


