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
using System;

public class MaintenanceInstructionController : MonoBehaviour
{
    [SerializeField] public GameObject rootObject;
    [SerializeField] public GameObject objectPrefab;
    public MaintenanceFormControler maintenanceFormControler;
    public GameObject orientedReticleCreate;
    public GameObject editButton;
    public GameObject form;
    public GameObject createButton;
    public GameObject saveButton;
    public GameObject doneButton;
    public GameObject dropButton;
    public LineRenderer connectLine;
    public TextMeshProUGUI maintenanceInstruction;
    public TextMeshProUGUI maintenanceIndex;
    private float transitionTime = 0.5f; // Thời gian chuyển cảnh
    public GameObject currentObject;
    private TouchScreenKeyboard keyboard;

    private async void Start()
    {
        DataControler.DataReady += OnDataReady;
    }

    private void OnDataReady()
    {
        // Xử lý khi dữ liệu đã sẵn sàng
        
        if (DataControler.IsDataReady())
        {
            connectLine.positionCount = 2;
            this.orientedReticleCreate.SetActive(false);
            DisplayObjectAtIndex();
        }
    }


    private void Update()
    {
        currentObject.transform.rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
        
    }

    public void FormSubmited() {
        DisplayObjectAtIndex();
        SetActiveCurrentObject(true);
    }

    public void FormCanceled() {
        DisplayObjectAtIndex();
        SetActiveCurrentObject(true);
    }

    public void SetActiveCurrentObject(Boolean active) {
        this.currentObject.SetActive(active);
    }

    public void CreateButtonOnlcick() {
        // currentObject.SetActive(false);
        orientedReticleCreate.SetActive(true);
        dropButton.SetActive(true); 
        DataControler.currentIndex = DataControler.objectTransforms.Count;
        this.maintenanceFormControler.SetFormType(FormType.Create);
    }


    public void EditButtonOnlcick() {
        // currentObject.SetActive(false);
        orientedReticleCreate.SetActive(true);
        dropButton.SetActive(true);
        this.maintenanceFormControler.SetFormType(FormType.Update);
    }

    public void DropButtonOnclick() {
        if(this.maintenanceFormControler.formType == FormType.Create) {
            CreateNewMaintenanceMessage();
            this.dropButton.SetActive(false);
            this.orientedReticleCreate.SetActive(false);
            this.maintenanceFormControler.SetActiveForm(true);
            this.maintenanceFormControler.SetObjectTransfrom(CreateObjectTransform("This is the message instruction", " ", null, DataControler.objectTransforms.Count, -1));
            this.maintenanceFormControler.SetUpForm();
        }
        else {
            SetPositionCurrentObject();
            this.dropButton.SetActive(false);
            this.orientedReticleCreate.SetActive(false);
            this.maintenanceFormControler.SetActiveForm(true);
            this.maintenanceFormControler.SetObjectTransfrom(CreateObjectTransform(DataControler.objectTransforms[DataControler.currentIndex].maintenanceInstruction, DataControler.objectTransforms[DataControler.currentIndex].videoUrl, DataControler.objectTransforms[DataControler.currentIndex].sensorDevice, DataControler.currentIndex, DataControler.objectTransforms[DataControler.currentIndex].id));
            this.maintenanceFormControler.SetUpForm();
        }
    }

    public void CreateNewMaintenanceMessage()
    {
        SetPositionCurrentObject();


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

    public void SetPositionCurrentObject() {
        Quaternion rotation = Quaternion.LookRotation(Camera.main.transform.forward, Camera.main.transform.up);
        currentObject.transform.position = orientedReticleCreate.transform.position + new Vector3(1f, 1f, 1f);
        currentObject.transform.rotation = rotation;
        
        RectTransform rf = currentObject.GetComponent<RectTransform>();
        Vector3 objectPosition = rf.position;
        Vector3 bottomLeftFront = new Vector3(objectPosition.x - rf.rect.size.x * rf.lossyScale.x/2f, objectPosition.y - rf.rect.size.y * rf.lossyScale.y/2f, objectPosition.z);
        
        connectLine.SetPosition(0, bottomLeftFront);
        connectLine.SetPosition(1, orientedReticleCreate.transform.position);

    }

    public ObjectTransform CreateObjectTransform(string maintenanceInstruction, string videoUrl, SensorDevice sensorDevice, int index, int id) {
        Matrix4x4 qrTransform = rootObject.transform.localToWorldMatrix;
        Matrix4x4 objectTransform = orientedReticleCreate.transform.localToWorldMatrix;
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
        newObject.scaleX = 0.01f;
        newObject.scaleY = 0.005f;
        newObject.scaleZ = 1f;
        newObject.maintenanceInstruction = maintenanceInstruction;
        newObject.videoUrl = videoUrl;
        newObject.id = id;
        newObject.index = index;
        newObject.sensorDevice = sensorDevice;
        return newObject;
    }

    



    public void DisplayObjectAtIndex()
    {
        if(currentObject == null) {
            currentObject = new GameObject();
        }
        ObjectData objectData = new ObjectData(DataControler.objectTransforms[DataControler.currentIndex]);

        currentObject.transform.position = rootObject.transform.TransformPoint(objectData.position) + new Vector3(1f, 1f, 1f);

        RectTransform rf = currentObject.GetComponent<RectTransform>();

        Vector3 objectPosition = rf.position;
        Vector3 bottomLeftFront = new Vector3(objectPosition.x - rf.rect.size.x * rf.lossyScale.x/2f, objectPosition.y - rf.rect.size.y * rf.lossyScale.y/2f, objectPosition.z);

        connectLine.SetPosition(0, bottomLeftFront);
        connectLine.SetPosition(1, rootObject.transform.TransformPoint(objectData.position));
        currentObject.transform.rotation = rootObject.transform.rotation * objectData.rotation;
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


    public void DoneButtonOnClick() {
        StartCoroutine(TransitionToNextObject());
    }

    private IEnumerator<object> TransitionToNextObject()
    {
        // Chờ đợi thời gian chuyển cảnh
        yield return new WaitForSeconds(transitionTime);

        // Tăng chỉ số hiện tại lên 1 và kiểm tra nếu vượt quá giới hạn của mảng
        DataControler.currentIndex += 1;
        if (DataControler.currentIndex >= DataControler.objectTransforms.Count)
        {
            // Reset chỉ số về 0 nếu đã đến cuối danh sách
            DataControler.currentIndex = 0;
        }
        DataControler.UpdateCurrentSensorDevice(DataControler.objectTransforms[DataControler.currentIndex].sensorDevice);

        // Hiển thị đối tượng tiếp theo
        DisplayObjectAtIndex();
    }



    private void OnDestroy()
    {
        // SaveDataToFile
    }
}