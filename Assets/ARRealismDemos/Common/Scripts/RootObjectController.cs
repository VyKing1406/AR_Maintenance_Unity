// using System.Collections.Generic;
// using UnityEngine;
// using System.IO;
// using System.Net;
// using System.Net.Http;
// using System.Text;
// using System.Threading.Tasks;
// using TMPro;
// using UnityEditor;
// public class ListObject
// {
//     public List<ObjectTransform> listData;

//     public ListObject()
//     {
//         listData = new List<ObjectTransform>();
//     }
// }
// [System.Serializable]
// public class JsonableListWrapper<T>
// {
//     public List<T> data;
//     public JsonableListWrapper(List<T> data) => this.data = data;
// }
// public class RootObjectController : MonoBehaviour
// {
//     [SerializeField] public GameObject objectPrefab;
//     [SerializeField] public GameObject rootObject;
//     protected List<ObjectTransform> objectList;

//     private List<GameObject> spawnedObjects = new List<GameObject>();
//     private string serverURL = "http://192.168.1.11:8080/api/object/transform";
//     private async void Start()
//     {
//         string apiUrl = "http://192.168.1.11:8080/api/object/transform/1";
//         Task<string> response = APICallerHelper.GetData(apiUrl);
//         string data = await response;
//         data = "{\"data\":" + data + "}";
//         Debug.Log(data);
//         InstantObject(data);
//     }
    

//     public void InstantObject(string response)
//     {
//         objectList = JsonParser.ParseJson(response);
//         // Tạo các đối tượng từ dữ liệu trong rootObject

//         foreach (ObjectTransform objectTransform in objectList)
//         {
//             // Tạo đối tượng 3D mới từ prefab
//             GameObject spawnedObject = Instantiate(objectPrefab);
//             ObjectData objectData = new ObjectData(objectTransform);

//             // Đặt vị trí, quay và tỷ lệ của đối tượng 3D
//             spawnedObject.transform.position = new Vector3(0, 0, 0);
//             // rootObject.transform.TransformPoint(objectData.position);
//             spawnedObject.transform.rotation = rootObject.transform.rotation * objectData.rotation;
//             spawnedObject.transform.localScale = objectData.scale;
//             TextMeshProUGUI textMeshPro = spawnedObject.GetComponentInChildren<TextMeshProUGUI>();
//             if (textMeshPro != null)
//             {
//                 textMeshPro.text = objectTransform.maintenanceInstruction;
//             }
            
//         }
//     }
//     private void Update()
//     {
//         if (Input.touchCount > 0 && MaintenanceObjectControl.NUM_OF_OBJECT_INITIAL > 0)
//         {
//             MaintenanceObjectControl.NUM_OF_OBJECT_INITIAL = 0;
//             OnPointerEnter();
//         }
//     }
    


//     public void OnPointerEnter()
//     {
//         Vector3 touchPosition = Input.mousePosition;
//         Vector2Int depthXY = DepthSource.ScreenToDepthXY(
//             (int)touchPosition.x, (int)touchPosition.y);
//         float realDepth = DepthSource.GetDepthFromXY(depthXY.x, depthXY.y, DepthSource.DepthArray);
//         if(0f > realDepth && realDepth > 6f) {
//             realDepth = 1f;
//         }
//         touchPosition.z = realDepth;
//         Vector3 worldPosition = DepthSource.ARCamera.ScreenToWorldPoint(touchPosition);
//         Quaternion rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);



//         GameObject spawnedObject = Instantiate(objectPrefab, worldPosition, rotation);
//         spawnedObjects.Add(spawnedObject);



//         Matrix4x4 qrTransform = rootObject.transform.localToWorldMatrix;
//         Matrix4x4 objectTransform = spawnedObject.transform.localToWorldMatrix;
//         Matrix4x4 objectRelativeTransform = qrTransform.inverse * objectTransform;
//         Vector3 objectRelativePosition = objectRelativeTransform.GetColumn(3);
//         Quaternion objectRelativeRotation = Quaternion.LookRotation(objectRelativeTransform.GetColumn(2), objectRelativeTransform.GetColumn(1));
//         Vector3 objectRelativeScale = new Vector3(objectRelativeTransform.GetColumn(0).magnitude, objectRelativeTransform.GetColumn(1).magnitude, objectRelativeTransform.GetColumn(2).magnitude);
//         ObjectData objectData = new ObjectData();
//         objectData.position = objectRelativePosition;
//         objectData.rotation = objectRelativeRotation;
//         objectData.scale = objectRelativeScale;
//         SendDataToServer(objectData);
//     }


//     private void SendDataToServer(ObjectData objectData)
//     {
//         string jsonData = ConvertObjectToJson(objectData);
//         HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serverURL);
//         request.Method = "POST";
//         request.ContentType = "application/json";

//         var postData = Encoding.ASCII.GetBytes(jsonData);
//         request.ContentLength = postData.Length;
//         using (var stream = request.GetRequestStream())
//         {
//             stream.Write(postData, 0, postData.Length);
//         }

//         using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
//         {
//             // Xử lý phản hồi từ server (nếu cần)
//         }

//     }



//     private void OnDestroy()
//     {
//         // SaveDataToFile
//     }


//     static string ConvertObjectToJson(ObjectData objectData)
//     {
//         string positionJson = ConvertVector3PositionToJson(objectData.position);
//         string rotationJson = ConvertQuaternionToJson(objectData.rotation);
//         string scaleJson = ConvertVector3ScaleToJson(objectData.scale);

//         return $"{{{positionJson},{rotationJson},{scaleJson}}}";
//     }

//     static string ConvertVector3PositionToJson(Vector3 vector3)
//     {
//         return $"\"positionX\":{vector3.x},\"positionY\":{vector3.y},\"positionZ\":{vector3.z}";
//     }

//     static string ConvertQuaternionToJson(Quaternion quaternion)
//     {
//         return $"\"rotationX\":{quaternion.x},\"rotationY\":{quaternion.y},\"rotationZ\":{quaternion.z},\"rotationW\":{quaternion.w}";
//     }


//     static string ConvertVector3ScaleToJson(Vector3 vector3)
//     {
//         return $"\"scaleX\":{vector3.x},\"scaleY\":{vector3.y},\"scaleZ\":{vector3.z}";
//     }



// }