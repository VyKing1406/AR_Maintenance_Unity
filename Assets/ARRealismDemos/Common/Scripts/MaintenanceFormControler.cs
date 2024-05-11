using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;
using TMPro;
using Newtonsoft.Json;
using System;
using UnityEditor;
using System.Net;

public enum State
{
    Instruction,
    VideoUrl,
    SensorDevice,
    Nomal,
}

public enum FormType
{
    Create,
    Update
}

public class MaintenanceFormControler : MonoBehaviour
{
    [SerializeField] public FormType formType;
    private State currentState;
    private TouchScreenKeyboard keyboard;
    [SerializeField] public TMP_Dropdown sensorDeviceDropdown;
    [SerializeField] public TextMeshProUGUI maintenanceInstructionText;
    [SerializeField] public TextMeshProUGUI videUrlText;
    [SerializeField] public TextMeshProUGUI formTypeText;
    [SerializeField] public TextMeshProUGUI indexText;
    [SerializeField] public TextMeshProUGUI stationText;
    private SensorDevice oldSensorDevice;
    public GameObject formObject;
    public MaintenanceInstructionController maintenanceInstructionController;
    private ObjectTransform objectTransform;

    private async void Start() {
        DataControler.DataReady += OnDataReady;
    }

    private void OnDataReady()
    {
        // Xử lý khi dữ liệu đã sẵn sàng
        if (DataControler.IsDataReady())
        {
            SetActiveForm(false);
            currentState = State.Nomal;
            SetDropDownValue(DataControler.sensorDevices);
            sensorDeviceDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
            SetUpForm();
        }
    }

    public void SetUpForm() {
        currentState = State.Nomal;
        
        maintenanceInstructionText.text = this.objectTransform.maintenanceInstruction;
        videUrlText.text = this.objectTransform.videoUrl;
        indexText.text = DataControler.currentIndex.ToString();
        stationText.text = DataControler.stationName;

        SetInitialValueDropdown();
    }
    
    public void SetActiveForm(Boolean active) {
        formObject.SetActive(active);
    }

    

    private void Update() {
        switch (currentState)
        {
            case State.Instruction:
                maintenanceInstructionText.text = keyboard.text;
                break;

            case State.VideoUrl:
                videUrlText.text = keyboard.text;
                break;

            case State.SensorDevice:
                break;

            case State.Nomal:
                CloseKeyboard();
                break;

            default:
                
                break;
        }

    }


    public void KeyboardOpen(string initValue) {
        keyboard = TouchScreenKeyboard.Open(initValue, TouchScreenKeyboardType.Default); 
    }

    private void CloseKeyboard() {
        this.keyboard.active = false;
        this.keyboard = null;
    }

    private void SetDropDownValue(List<SensorDevice> sensorDevices) {
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();

        foreach (SensorDevice sensorDevice in sensorDevices) {
            TMP_Dropdown.OptionData option = new TMP_Dropdown.OptionData(sensorDevice.sensorname);
            options.Add(option);
        }

        this.sensorDeviceDropdown.AddOptions(options);
    }

    private void OnDropdownValueChanged(int index) {
        DataControler.UpdateCurrentSensorDevice(DataControler.sensorDevices[index]);
    }

    public void InstructionFieldOnClick() {
        KeyboardOpen(this.maintenanceInstructionText.text);
        this.currentState = State.Instruction;
    }

    public void VideoUrlFieldOnClick() {
        KeyboardOpen(this.videUrlText.text);
        this.currentState = State.VideoUrl;
    }

    public void SetInitialValueDropdown() {
        if(formType == FormType.Create) {
            int index = 1;

            // Đặt giá trị ban đầu cho dropdown
            this.sensorDeviceDropdown.value = index;
            this.oldSensorDevice = DataControler.objectTransforms[DataControler.currentIndex].sensorDevice;
            DataControler.UpdateCurrentSensorDevice(DataControler.sensorDevices[1]);
        }
        else {
            bool isDeviceExist = DataControler.sensorDevices.Contains(objectTransform.sensorDevice);

            if (isDeviceExist)
            {
                // Lấy chỉ mục của sensorDevice trong danh sách listSensor
                int index = DataControler.sensorDevices.IndexOf(objectTransform.sensorDevice);

                // Đặt giá trị ban đầu cho dropdown
                this.sensorDeviceDropdown.value = index;
                this.oldSensorDevice = objectTransform.sensorDevice;
            }
        }
    }

    public void SetFormType(FormType formType) {
        this.formType = formType;
        if(formType == FormType.Create) {
            this.formTypeText.text = "Create form";
        } else if(formType == FormType.Update) {
            this.formTypeText.text = "Update form";
        }
    }

    public void SetObjectTransfrom(ObjectTransform objectTransform) {
        this.objectTransform = objectTransform;
    }


    public void CancelButtonOnclick() {
        this.maintenanceInstructionController.SetActiveCurrentObject(true);
        formObject.SetActive(false);
        if(DataControler.currentIndex >= DataControler.objectTransforms.Count - 1) {
            DataControler.currentIndex = DataControler.objectTransforms.Count - 1;
        }
        this.maintenanceInstructionController.DisplayObjectAtIndex();
        DataControler.UpdateCurrentSensorDevice(this.oldSensorDevice);
    }

    public void SubmitButtonOnclick() {
        formObject.SetActive(false);

        this.objectTransform.maintenanceInstruction = this.maintenanceInstructionText.text;
        this.objectTransform.videoUrl = this.videUrlText.text;
        this.objectTransform.sensorDevice = DataControler.currentSensorDevice;
        this.objectTransform.stationId = DataControler.stationId;
        string jsonData = JsonConvert.SerializeObject(this.objectTransform);
        if(this.formType == FormType.Create) {
            DataControler.currentIndex = DataControler.objectTransforms.Count;
            DataControler.objectTransforms.Add(this.objectTransform);
            this.objectTransform = null;
            this.maintenanceInstructionController.DisplayObjectAtIndex();
            this.maintenanceInstructionController.SetActiveCurrentObject(true);
            string url = DataControler.BASE_URL + "/object/transform";
            HttpStatusCode statusCode = APICallerHelper.PostData(url, jsonData);
            if (statusCode == HttpStatusCode.Created) {
                
            }
        } else if(this.formType == FormType.Update) {
            UpdateObject();
            this.objectTransform = null;
            this.maintenanceInstructionController.DisplayObjectAtIndex();
            this.maintenanceInstructionController.SetActiveCurrentObject(true);
            string url = DataControler.BASE_URL + "/object/transform";
            HttpStatusCode statusCode = APICallerHelper.PatchData(url, jsonData);
            if (statusCode == HttpStatusCode.OK) {
                
            }
        }
        
    }

    public void UpdateObject() {
        DataControler.objectTransforms[DataControler.currentIndex].positionX = this.objectTransform.positionX;
        DataControler.objectTransforms[DataControler.currentIndex].positionY = this.objectTransform.positionY;
        DataControler.objectTransforms[DataControler.currentIndex].positionZ = this.objectTransform.positionZ;
        DataControler.objectTransforms[DataControler.currentIndex].rotationX = this.objectTransform.rotationX;
        DataControler.objectTransforms[DataControler.currentIndex].rotationY = this.objectTransform.rotationY;
        DataControler.objectTransforms[DataControler.currentIndex].rotationZ = this.objectTransform.rotationZ;
        DataControler.objectTransforms[DataControler.currentIndex].rotationW = this.objectTransform.rotationW;
        DataControler.objectTransforms[DataControler.currentIndex].scaleX = this.objectTransform.scaleX;
        DataControler.objectTransforms[DataControler.currentIndex].scaleY = this.objectTransform.scaleY;
        DataControler.objectTransforms[DataControler.currentIndex].scaleZ = this.objectTransform.scaleZ;
        DataControler.objectTransforms[DataControler.currentIndex].maintenanceInstruction = this.maintenanceInstructionText.text;
        DataControler.objectTransforms[DataControler.currentIndex].videoUrl = this.videUrlText.text;
    }
}