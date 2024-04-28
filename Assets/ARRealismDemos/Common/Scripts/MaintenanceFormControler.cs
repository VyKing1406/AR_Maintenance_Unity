using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine.UI;
using System.IO;
using System.Text;
using UnityEngine.SceneManagement;
using TMPro;
using Newtonsoft.Json;
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
    private TextMeshProUGUI maintenanceInstructionText;
    private TextMeshProUGUI videUrlText;
    private TextMeshProUGUI formtypeText;
    private SensorDevice oldSensorDevice;
    private Dropdown sensorDeviceDropdown;
    private ObjectTransform objectTransform;

    private async void Start() {
        currentState = State.Nomal;
        sensorDeviceDropdown = transform.GetComponent<Dropdown>();
        setDropDownValue(DataControler.sensorDevices);
        
        sensorDeviceDropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    public void setUpForm() {
        currentState = State.Nomal;
        
        maintenanceInstructionText.text = objectTransform.maintenanceInstruction;
        videUrlText.text = objectTransform.videoUrl;

        setInitialValueDropdown();
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

    public void SetFormType(FormType formType) {
        this.formType = formType;
    }

    private void CloseKeyboard() {
        this.keyboard.active = false;
        this.keyboard = null;
    }

    private void setDropDownValue(List<SensorDevice> sensorDevices) {
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

    public void setInstructionState() {
        KeyboardOpen(this.maintenanceInstructionText.text);
        this.currentState = State.Instruction;
    }

    public void setVideoUrlState() {
        KeyboardOpen(this.videUrlText.text);
        this.currentState = State.VideoUrl;
    }

    public void setInitialValueDropdown() {
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

    public void SetObjectTransfrom(ObjectTransform objectTransform) {
        this.objectTransform = objectTransform;
    }


    public void CancelButtonOnclick() {
        DataControler.currentSensorDevice = this.oldSensorDevice;
        transform.gameObject.SetActive(false);
    }

    public void SubmitButtonOnclick() {
        string url = "";
        string jsonData = JsonConvert.SerializeObject(this.objectTransform);
        this.objectTransform.maintenanceInstruction = this.maintenanceInstructionText.text;
        this.objectTransform.videoUrl = this.videUrlText.text;
        this.objectTransform.sensorDevice = DataControler.currentSensorDevice;
        if(formType == FormType.Create) {
            url = "";
            HttpStatusCode statusCode = APICallerHelper.PostData(url, jsonData);
            if (statusCode == HttpStatusCode.Created) {
                DataControler.currentIndex = DataControler.objectTransforms.Count;
                DataControler.objectTransforms.Add(this.objectTransform);
            }
        } else if(formType == FormType.Update) {
            url = "";
            HttpStatusCode statusCode = APICallerHelper.PostData(url, jsonData);
            if (statusCode == HttpStatusCode.OK) {
                DataControler.objectTransforms[DataControler.currentIndex] = this.objectTransform;
            }
        }
        this.objectTransform = null;
        transform.gameObject.SetActive(false);
    }
}