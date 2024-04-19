using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Net;
using System.Net.Http;
using UnityEngine.Networking;
using System.Text;
using UnityEngine.SceneManagement;
using TMPro;
using System.Threading.Tasks;
using UnityEditor;

public class MaintenanceInstructionController : MonoBehaviour
{
    [SerializeField] public GameObject rootObject;
    [SerializeField] public GameObject objectPrefab;
    public GameObject orientedReticle;
    public GameObject orientedReticleCreate;
    public GameObject editButton;
    public GameObject createButton;
    public GameObject saveButton;
    public GameObject doneButton;
    public GameObject dropButton;
    public LineRenderer connectLine;
    public TextMeshProUGUI maintenanceInstruction;
    public TextMeshProUGUI maintenanceIndex;
    private TextMeshProUGUI oldMaintenanceInstruction;
    private int isCreate = 0;
    private int isEdit = 0;
    private int isSave = 0;
    private float transitionTime = 0.5f; // Thời gian chuyển cảnh
    private int currentIndex = 0;
    public GameObject currentObject;
    private List<ObjectTransform> objectTransforms;
    private string serverURL = "http://192.168.1.12:8080/api/object/transform";
    private TouchScreenKeyboard keyboard;

    private async void Start()
    {
        string apiUrl = "http://192.168.1.12:8080/api/object/transform/1";
        // Task<string> response = 
        string data = await APICallerHelper.GetData(apiUrl);
        data = "{\"data\":" + data + "}";
        objectTransforms = JsonParser.ParseJson(data);
        connectLine.positionCount = 2;
        this.orientedReticleCreate.SetActive(false);
        DisplayObjectAtIndex();
    }

    public IEnumerator<object> FetchData(string url) {
        UnityWebRequest webRed = UnityWebRequest.Get(url);
        yield return webRed.SendWebRequest();
        if(webRed.result == UnityWebRequest.Result.ConnectionError || webRed.result == UnityWebRequest.Result.Success) {
            Debug.Log(webRed.error);
        }
        else {
            string data = "{\"data\":" + webRed.downloadHandler.text + "}";
            objectTransforms = JsonParser.ParseJson(data);
        }
    }

    private void Update()
    {
        if (Input.touchCount > 0)
        {
            GameObject gameObjectSelected = RaycastSelection.StartSelect();
            
            if(gameObjectSelected == createButton && this.isCreate == 0 && this.isEdit == 0) {
                this.isCreate = 1;
                currentObject.SetActive(false);
                orientedReticleCreate.SetActive(true);
                dropButton.SetActive(true);
            }

            if(gameObjectSelected == editButton && isEdit == 0) {
                isEdit = 1;
                keyboard = TouchScreenKeyboard.Open("", TouchScreenKeyboardType.Default); 
            }

            if(gameObjectSelected == saveButton && isEdit == 1) {
                isEdit = 0;
                saveMaintenanceMessage();
                DisplayObjectAtIndex();
                currentObject.SetActive(true);
            }


            if(gameObjectSelected == doneButton) {
                handleDoneButtonClick();
            }
            if (Input.touchCount == 2) {
                Rescale();
            }
            
        }
        if(isEdit == 1) {
            maintenanceInstruction.text = keyboard.text;
        }
        currentObject.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
    }

    public void handleDropOrient() {
        createNewMaintenanceMessage();
        this.dropButton.SetActive(false);
        this.orientedReticleCreate.SetActive(false);
        this.isCreate = 0;
    }

    public void DisplayObjectAtIndex()
    {
        if(currentObject == null) {
            currentObject = new GameObject();
        }
        ObjectData objectData = new ObjectData(objectTransforms[this.currentIndex]);

        // Đặt vị trí, quay và tỷ lệ của đối tượng 3D
        currentObject.transform.position = rootObject.transform.TransformPoint(objectData.position) + new Vector3(1f, 1f, 1f);
        orientedReticle.transform.position = rootObject.transform.TransformPoint(objectData.position);
        

        MeshRenderer objectMeshRenderer = currentObject.GetComponent<MeshRenderer>();
        Bounds objectBounds = objectMeshRenderer.bounds;
        Vector3 objectSize = objectBounds.size;

        // Vector3 objectPosition = currentObject.transform.position;

        // Vector3 bottomLeftFront = objectPosition - currentObject.transform.right * (objectSize.x / 2f) - currentObject.transform.up * (objectSize.y / 2f) - currentObject.transform.forward * (objectSize.z / 2f);

        RectTransform rf = currentObject.GetComponent<RectTransform>();

        Vector3 objectPosition = rf.position;
        objectPosition.x - rf.rect.size.x * rf.lossyScale.x/2f
        objectPosition.y - rf.rect.size.y * rf.lossyScale.y/2f
        //Vector3 bottomLeftFront = objectPosition - currentObject.transform.right * (objectSize.x / 2f) - currentObject.transform.up * (objectSize.y / 2f) - currentObject.transform.forward * (objectSize.z / 2f);
        Vector3 bottomLeftFront = (objectPosition.x - rf.rect.size.x * rf.lossyScale.x/2f,objectPosition.y - rf.rect.size.y * rf.lossyScale.y/2f,objectPosition.z);
        connectLine.SetPosition(0, bottomLeftFront);
        connectLine.SetPosition(1, orientedReticle.transform.position);
        // currentObject.transform.rotation = rootObject.transform.rotation * objectData.rotation;
        currentObject.transform.localScale = objectData.scale;

        TextMeshProUGUI textMeshPro = maintenanceInstruction.GetComponentInChildren<TextMeshProUGUI>();
        if (textMeshPro != null)
        {
            textMeshPro.text = objectTransforms[this.currentIndex].maintenanceInstruction;
        }

        textMeshPro = maintenanceIndex.GetComponentInChildren<TextMeshProUGUI>();
        if (textMeshPro != null)
        {
            textMeshPro.text = objectTransforms[this.currentIndex].index.ToString();
        }
    }


    public void handleDoneButtonClick()
    {
        Debug.Log("handleDoneButton");
        StartCoroutine(TransitionToNextObject());
    }

    private IEnumerator<object> TransitionToNextObject()
    {
        // Chờ đợi thời gian chuyển cảnh
        yield return new WaitForSeconds(transitionTime);

        // Tăng chỉ số hiện tại lên 1 và kiểm tra nếu vượt quá giới hạn của mảng
        this.currentIndex++;
        if (this.currentIndex >= this.objectTransforms.Count)
        {
            // Reset chỉ số về 0 nếu đã đến cuối danh sách
            this.currentIndex = 0;
        }

        // Hiển thị đối tượng tiếp theo
        DisplayObjectAtIndex();
    }


    public void createNewMaintenanceMessage()
    {
        // Vector3 createPosition = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0f);
        // Vector2Int depthXY = DepthSource.ScreenToDepthXY(
        //     (int)(Screen.width * 0.5f), (int)(Screen.height * 0.5f));
        // float realDepth = DepthSource.GetDepthFromXY(depthXY.x, depthXY.y, DepthSource.DepthArray);
        // if(0f > realDepth && realDepth > 4f) {
        //     realDepth = 1f;
        // }
        // createPosition.z = realDepth;
        // Vector3 worldPosition = DepthSource.ARCamera.ScreenToWorldPoint(createPosition);

        orientedReticle.transform.position = orientedReticleCreate.transform.position;

        Quaternion rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
        currentObject.transform.position = orientedReticleCreate.transform.position + new Vector3(1f, 1f, 1f);
        currentObject.transform.rotation = rotation;
        
        
        connectLine.SetPosition(0, orientedReticleCreate.transform.position);
        connectLine.SetPosition(1, currentObject.transform.position);



        TextMeshProUGUI textMeshPro = maintenanceInstruction.GetComponentInChildren<TextMeshProUGUI>();
        if (textMeshPro != null)
        {
            textMeshPro.text = "This is the message instruction";
        }

        textMeshPro = maintenanceIndex.GetComponentInChildren<TextMeshProUGUI>();
        if (textMeshPro != null)
        {
            textMeshPro.text = objectTransforms.Count.ToString();
        }
        currentObject.SetActive(true);
    }

    public void saveMaintenanceMessage() {

        Matrix4x4 qrTransform = rootObject.transform.localToWorldMatrix;
        Matrix4x4 objectTransform = currentObject.transform.localToWorldMatrix;
        Matrix4x4 objectRelativeTransform = qrTransform.inverse * objectTransform;
        Vector3 objectRelativePosition = objectRelativeTransform.GetColumn(3);
        Quaternion objectRelativeRotation = Quaternion.LookRotation(objectRelativeTransform.GetColumn(2), objectRelativeTransform.GetColumn(1));
        Vector3 objectRelativeScale = new Vector3(objectRelativeTransform.GetColumn(0).magnitude, objectRelativeTransform.GetColumn(1).magnitude, objectRelativeTransform.GetColumn(2).magnitude);
        ObjectData objectData = new ObjectData();
        objectData.position = objectRelativePosition;
        objectData.rotation = objectRelativeRotation;
        objectData.scale = new Vector3(0.09f, 0.03f, 1f);
        SendDataToServer(objectData);
    }


    private void SendDataToServer(ObjectData objectData)
    {
        string jsonData = ConvertObjectToJson(objectData);
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create(serverURL);
        request.Method = "POST";
        request.ContentType = "application/json";

        var postData = Encoding.ASCII.GetBytes(jsonData);
        request.ContentLength = postData.Length;
        using (var stream = request.GetRequestStream())
        {
            stream.Write(postData, 0, postData.Length);
        }

        using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
        {
            // Xử lý phản hồi từ server (nếu cần)
        }

    }
    private float initialDistance;
    private Vector3 initialScale;
    private float currentScale;
    public float maxObjectScale = 3f;
    public float minObjectScale = 1f;
    void Rescale()
    {
        if (Input.touchCount > 0)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            if (touchZero.phase == TouchPhase.Ended || touchZero.phase == TouchPhase.Canceled ||
                touchOne.phase == TouchPhase.Ended || touchOne.phase == TouchPhase.Canceled)
                return;

            if (touchZero.phase == TouchPhase.Began || touchOne.phase == TouchPhase.Began)
            {
                initialDistance = Vector2.Distance(touchZero.position, touchOne.position);
                initialScale = currentObject.transform.localScale;
            } else {
                float currentDistance = Vector2.Distance(touchZero.position, touchOne.position);

                if (Mathf.Approximately(initialDistance, 0))
                    return;

                float factor = currentDistance / initialDistance;
                float newScale = initialScale.x * factor;
                newScale = Mathf.Clamp(newScale, minObjectScale, maxObjectScale);
                currentObject.transform.localScale = new Vector3(newScale, newScale, newScale);
            }
            currentScale = currentObject.transform.localScale.x;
        }
    }




    private void OnDestroy()
    {
        // SaveDataToFile
    }


    private string ConvertObjectToJson(ObjectData objectData)
    {
        string positionJson = ConvertVector3PositionToJson(objectData.position);
        string rotationJson = ConvertQuaternionToJson(objectData.rotation);
        string scaleJson = ConvertVector3ScaleToJson(objectData.scale);
        string message = maintenanceInstruction.text;

        return $"{{{positionJson},{rotationJson},{scaleJson},\"maintenanceInstruction\": \"{message}\"}}";
    }

    private string ConvertVector3PositionToJson(Vector3 vector3)
    {
        return $"\"positionX\":{vector3.x},\"positionY\":{vector3.y},\"positionZ\":{vector3.z}";
    }

    private string ConvertQuaternionToJson(Quaternion quaternion)
    {
        return $"\"rotationX\":{quaternion.x},\"rotationY\":{quaternion.y},\"rotationZ\":{quaternion.z},\"rotationW\":{quaternion.w}";
    }


    private string ConvertVector3ScaleToJson(Vector3 vector3)
    {
        return $"\"scaleX\":{vector3.x},\"scaleY\":{vector3.y},\"scaleZ\":{vector3.z}";
    }
}