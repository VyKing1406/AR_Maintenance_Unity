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
using Newtonsoft.Json;
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
    public GameObject currentObject;
    private string serverURL = "http://localhost:8080/api/object/transform";
    private TouchScreenKeyboard keyboard;

    private async void Start()
    {
        connectLine.positionCount = 2;
        this.orientedReticleCreate.SetActive(false);
        DisplayObjectAtIndex();
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
        }
        if(isEdit == 1) {
            maintenanceInstruction.text = keyboard.text;
        }
        currentObject.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
        RectTransform rf = currentObject.GetComponent<RectTransform>();

        Vector3 objectPosition = rf.position;
        Vector3 bottomLeftFront = new Vector3(objectPosition.x - rf.rect.size.x * rf.lossyScale.x/2f, objectPosition.y - rf.rect.size.y * rf.lossyScale.y/2f, objectPosition.z);
        connectLine.SetPosition(0, bottomLeftFront);
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
        ObjectData objectData = new ObjectData(DataControler.objectTransforms[DataControler.currentIndex]);

        currentObject.transform.position = rootObject.transform.TransformPoint(objectData.position) + new Vector3(1f, 1f, 1f);
        orientedReticle.transform.position = rootObject.transform.TransformPoint(objectData.position);

        RectTransform rf = currentObject.GetComponent<RectTransform>();

        Vector3 objectPosition = rf.position;
        Vector3 bottomLeftFront = new Vector3(objectPosition.x - rf.rect.size.x * rf.lossyScale.x/2f, objectPosition.y - rf.rect.size.y * rf.lossyScale.y/2f, objectPosition.z);

        connectLine.SetPosition(0, bottomLeftFront);
        connectLine.SetPosition(1, orientedReticle.transform.position);
        // currentObject.transform.rotation = rootObject.transform.rotation * objectData.rotation;
        currentObject.transform.localScale = objectData.scale;

        TextMeshProUGUI textMeshPro = maintenanceInstruction.GetComponentInChildren<TextMeshProUGUI>();
        if (textMeshPro != null)
        {
            textMeshPro.text = DataControler.objectTransforms[DataControler.currentIndex].maintenanceInstruction;
        }

        textMeshPro = maintenanceIndex.GetComponentInChildren<TextMeshProUGUI>();
        if (textMeshPro != null)
        {
            textMeshPro.text = DataControler.objectTransforms[DataControler.currentIndex].index.ToString();
        }
    }


    public void handleDoneButtonClick()
    {
        StartCoroutine(TransitionToNextObject());
    }

    private IEnumerator<object> TransitionToNextObject()
    {
        // Chờ đợi thời gian chuyển cảnh
        yield return new WaitForSeconds(transitionTime);

        // Tăng chỉ số hiện tại lên 1 và kiểm tra nếu vượt quá giới hạn của mảng
        DataControler.currentIndex++;
        if (DataControler.currentIndex >= DataControler.objectTransforms.Count)
        {
            // Reset chỉ số về 0 nếu đã đến cuối danh sách
            DataControler.currentIndex = 0;
        }

        // Hiển thị đối tượng tiếp theo
        DisplayObjectAtIndex();
    }


    public void createNewMaintenanceMessage()
    {
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
            textMeshPro.text = DataControler.objectTransforms.Count.ToString();
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
        
        
        ObjectTransform newObject = new ObjectTransform();

        // newObject.stationId = 1;
        newObject.positionX = objectRelativePosition.x; 
        newObject.positionY = objectRelativePosition.y; 
        newObject.positionZ = objectRelativePosition.z; 
        newObject.rotationX = objectRelativeRotation.x;  
        newObject.rotationY = objectRelativeRotation.y;  
        newObject.rotationZ = objectRelativeRotation.z;  
        newObject.rotationW = objectRelativeRotation.w;  
        newObject.scaleX = 0.08f;
        newObject.scaleY = 0.03f;
        newObject.scaleZ = 1f;
        // newObject.maintenanceInstruction  = 
        // newObject.sensorDevice.id = 


        SendDataToServer(newObject);

        DataControler.objectTransforms.Add(newObject);
    }


    private void SendDataToServer(ObjectTransform newObject)
    {
        HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://localhost:8080/api/object/transform");
        request.Method = "POST";
        request.ContentType = "application/json";
        string jsonData = JsonConvert.SerializeObject(newObject);
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

    private void OnDestroy()
    {
        // SaveDataToFile
    }
}