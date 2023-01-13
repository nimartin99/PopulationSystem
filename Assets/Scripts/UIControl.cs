using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class UIControl : MonoBehaviour
{
    private InputControl _inputControl;
    [SerializeField] private BuildingGhost buildingGhost;
    
    private UIDocument _uiDocument;
    private TimeController _timeController;
    
    // Toolbar
    private Button _exploreButton;
    private Button _buildButton;
    private Button _deleteButton;
    
    // Inspector
    private VisualElement _inspector;
    private Label _inspectorName;
    private ProgressBar _inspectorFood;
    private ProgressBar _inspectorReligion;
    private Transform _currentlyInspected;
    private string _currentlyInspectedType;
    private Resident _currentlyInspectedResident;

    // Statusbar
    private Label _dayLabel;
    private Label _timeLabel;
    
    // Settings
    private Button _settingsActivator;
    private VisualElement _settings;
    private SliderInt _workNecessitiesSlider;
    private Label _necessitiesLabelValue;
    private Label _workLabelValue;

    void Start() {
        _timeController = TimeController.Instance;
        _inputControl = GetComponent<InputControl>();
        
        _uiDocument = GetComponent<UIDocument>();
        // Toolbar
        _exploreButton = _uiDocument.rootVisualElement.Q<Button>("exploreButton");
        _exploreButton.clicked += () => { ButtonClicked("explore"); };
        _buildButton = _uiDocument.rootVisualElement.Q<Button>("buildButton");
        _buildButton.clicked += () => { ButtonClicked("build"); };
        _deleteButton = _uiDocument.rootVisualElement.Q<Button>("deleteButton");
        _deleteButton.clicked += () => { ButtonClicked("delete"); };
        StyleButton(_exploreButton, _buildButton, _deleteButton);
        
        // Inspector
        _inspector = _uiDocument.rootVisualElement.Q<VisualElement>("inspector");
        _inspectorName = _uiDocument.rootVisualElement.Q<Label>("inspectorName");
        _inspectorFood = _uiDocument.rootVisualElement.Q<ProgressBar>("inspectorFood");
        _inspectorReligion = _uiDocument.rootVisualElement.Q<ProgressBar>("inspectorReligion");
        _inspector.visible = false;
        
        // Statusbar
        _dayLabel = _uiDocument.rootVisualElement.Q<Label>("dayLabel");
        _timeLabel = _uiDocument.rootVisualElement.Q<Label>("timeLabel");
        
        // Settings
        _settingsActivator = _uiDocument.rootVisualElement.Q<Button>("settingsActivator");
        _settingsActivator.clicked += () => { ToggleSettings(); };
        _settings = _uiDocument.rootVisualElement.Q<VisualElement>("settings");
        _settings.visible = false;
        _workNecessitiesSlider = _uiDocument.rootVisualElement.Q<SliderInt>("workNecessitiesSlider");
        _workNecessitiesSlider.highValue = _timeController.hoursInADay;
        _necessitiesLabelValue = _uiDocument.rootVisualElement.Q<Label>("necessitiesLabelValue");
        _workLabelValue = _uiDocument.rootVisualElement.Q<Label>("workLabelValue");
            
    }

    private void Update() {
        if (_inspector.visible) {
            switch (_currentlyInspectedType) {
                case "Resident":
                    _inspectorFood.title = Math.Floor(_currentlyInspectedResident.foodSatisfaction) + " / 100";
                    _inspectorFood.value = _currentlyInspectedResident.foodSatisfaction;
                    _inspectorReligion.title =  Math.Floor(_currentlyInspectedResident.religionSatisfaction) + " / 100";
                    _inspectorReligion.value = _currentlyInspectedResident.religionSatisfaction;
                    break;
                case "House":
                    break;
            }
        }


        if (_workNecessitiesSlider.visible) {
            int workTime = _workNecessitiesSlider.value;
            _timeController.workHours = workTime;
            _workLabelValue.text = workTime.ToString();
            _necessitiesLabelValue.text = (_timeController.hoursInADay - workTime).ToString();
        }

        DisplayClockToStatusbar();
    }

    private void ButtonClicked(string type) {
        switch (type) {
            case "build":
                _inputControl.currentMode = InputControl.InputModes.BuildingMode;
                buildingGhost.RefreshVisual();
                StyleButton(_buildButton, _exploreButton, _deleteButton);
                break;
            case "delete":
                _inputControl.currentMode = InputControl.InputModes.DeleteMode;
                buildingGhost.DestroyVisual();
                StyleButton(_deleteButton, _exploreButton, _buildButton);
                break;
            default:
            case "explore":
                _inputControl.currentMode = InputControl.InputModes.ExploreMode;
                buildingGhost.DestroyVisual();
                StyleButton(_exploreButton, _buildButton, _deleteButton);
                break;
        }
    }

    public void SetToInspector(Transform inspectionTransform, string type) {
        _inspector.visible = true;
        _currentlyInspected = inspectionTransform;
        _currentlyInspectedType = type;
        switch (_currentlyInspectedType) {
            case "Resident":
                _currentlyInspectedResident = _currentlyInspected.GetComponent<Resident>();
                _currentlyInspectedResident.EnableIndicator();
                _inspectorName.text = _currentlyInspectedType;
                break;
            case "House":
                break;
        }
    }

    public void HideInspector() {
        _inspector.visible = false;
        switch (_currentlyInspectedType) {
            case "Resident":
                _currentlyInspected.GetComponent<Resident>().DisableIndicator();
                break;
        }
    }

    private void StyleButton(Button buttonToStyle, Button buttonToUnstyle1, Button buttonToUnstyle2) {
        buttonToStyle.style.borderBottomWidth = 2f;
        buttonToStyle.style.borderLeftWidth = 2f;
        buttonToStyle.style.borderTopWidth = 2f;
        buttonToStyle.style.borderRightWidth = 2f;
        
        buttonToUnstyle1.style.borderBottomWidth = 1f;
        buttonToUnstyle1.style.borderLeftWidth = 1f;
        buttonToUnstyle1.style.borderTopWidth = 1f;
        buttonToUnstyle1.style.borderRightWidth = 1f;
        
        buttonToUnstyle2.style.borderBottomWidth = 1f;
        buttonToUnstyle2.style.borderLeftWidth = 1f;
        buttonToUnstyle2.style.borderTopWidth = 1f;
        buttonToUnstyle2.style.borderRightWidth = 1f;
    }

    private void ToggleSettings() {
        _settings.visible = !_settings.visible;
        if (_settings.visible) {
            _settingsActivator.style.borderBottomRightRadius = 0;
            _settingsActivator.style.borderBottomLeftRadius = 0;
        }
        else {
            _settingsActivator.style.borderBottomRightRadius = 3;
            _settingsActivator.style.borderBottomLeftRadius = 3;
        }
    }

    private void DisplayClockToStatusbar() {
        double hours = _timeController.hours;
        double minutes = _timeController.minutes;
        String hoursAndMinutes = "";
        if (hours < 10) {
            hoursAndMinutes = "0" + hours;
        }
        else {
            hoursAndMinutes = hours.ToString();
        }
        hoursAndMinutes += ":";
        if (minutes < 10) {
            hoursAndMinutes += "0" + minutes;
        }
        else {
            hoursAndMinutes += minutes.ToString();
        }

        _dayLabel.text = "Day " + _timeController.daysSinceStart + " -";
        _timeLabel.text = hoursAndMinutes;
    }
}
