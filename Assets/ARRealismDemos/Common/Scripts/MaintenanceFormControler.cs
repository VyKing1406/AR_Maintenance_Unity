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
    private State currentState;
    private FormType formType;
    private TouchScreenKeyboard keyboard;
    [SerializeField] public TextMeshProUGUI maintenanceInstructionText;
    [SerializeField] public TextMeshProUGUI videUrlText;
    [SerializeField] public Dropdown sensorDeviceDropdown;
    private TextMeshProUGUI formtypeText;
    private SensorDevice oldSensorDevice;
    private ObjectTransform objectTransform;

    private async void Start() {
        DataControler.DataReady += OnDataReady;
    }

    private void OnDataReady()
    {
        // Xử lý khi dữ liệu đã sẵn sàng
        if (DataControler.IsDataReady())
        {
            currentState = State.Nomal;
            SetDropDownValue(DataControler.sensorDevices);
            sensorDeviceDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
            SetUpForm();
        }
    }
    
    public void SetActiveForm(Boolean active) {
        gameObject.SetActive(active);
    }

    public void SetUpForm() {
        currentState = State.Nomal;
        
        maintenanceInstructionText.text = objectTransform.maintenanceInstruction;
        videUrlText.text = objectTransform.videoUrl;


        SetInitialValueDropdown();
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
        List<Dropdown.OptionData> dropdownOptions = new List<Dropdown.OptionData>();

        foreach (SensorDevice device in sensorDevices) {
            Dropdown.OptionData option = new Dropdown.OptionData(device.sensorname);
            dropdownOptions.Add(option);
        }

        this.sensorDeviceDropdown.options = dropdownOptions;
    }

    private void OnDropdownValueChanged(int index) {
        DataControler.currentSensorDevice = DataControler.sensorDevices[index];
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
        if(objectTransform.sensorDevice == null) {
            int index = 0;

            // Đặt giá trị ban đầu cho dropdown
            this.sensorDeviceDropdown.value = index;
            this.oldSensorDevice = DataControler.sensorDevices[0];
            DataControler.currentSensorDevice = DataControler.sensorDevices[0];
        }
        else {
            bool isDeviceExist = DataControler.sensorDevices.Contains(objectTransform.sensorDevice);

            if (isDeviceExist)
            {
                // Lấy chỉ mục của sensorDevice trong danh sách listSensor
                int index = DataControler.sensorDevices.IndexOf(objectTransform.sensorDevice);

                // Đặt giá trị ban đầu cho dropdown
                this.sensorDeviceDropdown.value = index;
                this.oldSensorDevice = DataControler.currentSensorDevice;
            }
        }
    }

    public void SetFormType(FormType formType) {
        this.formType = formType;
    }

    public void SetObjectTransfrom(ObjectTransform objectTransform) {
        this.objectTransform = objectTransform;
    }


    public void CancelButtonOnclick() {
        DataControler.currentSensorDevice = this.oldSensorDevice;
        transform.gameObject.SetActive(false);
    }

    public void SubmitButtonOnclick() {
        this.objectTransform.maintenanceInstruction = this.maintenanceInstructionText.text;
        this.objectTransform.videoUrl = this.videUrlText.text;
        this.objectTransform.sensorDevice = DataControler.currentSensorDevice;
        string jsonData = JsonConvert.SerializeObject(this.objectTransform);
        if(this.formType == FormType.Create) {
            string url = "http://192.168.1.6:8080/api/object/transform";
            HttpStatusCode statusCode = APICallerHelper.PostData(url, jsonData);
            if (statusCode == HttpStatusCode.Created) {
                DataControler.currentIndex = DataControler.objectTransforms.Count;
                DataControler.objectTransforms.Add(this.objectTransform);
            }
        } else if(this.formType == FormType.Update) {
            string url = "http://192.168.1.6:8080/api/object/transform";
            HttpStatusCode statusCode = APICallerHelper.PostData(url, jsonData);
            if (statusCode == HttpStatusCode.OK) {
                DataControler.objectTransforms[DataControler.currentIndex] = this.objectTransform;
            }
        }
        this.objectTransform = null;
        transform.gameObject.SetActive(false);
    }
}