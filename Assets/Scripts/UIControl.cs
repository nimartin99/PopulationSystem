using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using Button = UnityEngine.UIElements.Button;

public class UIControl : MonoBehaviour
{
    public static UIControl Instance { get; private set; }
    
    private InputControl _inputControl;
    [SerializeField] private BuildingGhost buildingGhost;
    [SerializeField] private PresettingGenerator presettingGenerator;
    
    private UIDocument _uiDocument;
    private TimeController _timeController;
    
    // UI params
    public int currentTaxes;
    public string currentFoodPortion;

    // Toolbar
    private Button _exploreButton;
    private Button _buildButton;
    private Button _deleteButton;
    
    // Inspector
    private VisualElement _inspector;
    private Label _inspectorName;
    private ProgressBar _inspectorSatisfaction;
    private ProgressBar _inspectorHappiness;
    private ProgressBar _inspectorFood;
    private ProgressBar _inspectorReligion;
    private ProgressBar _inspectorTavern;
    private Transform _currentlyInspected;
    private string _currentlyInspectedType;
    private Resident _currentlyInspectedResident;

    // Statusbar
    private Label _dayLabel;
    private Label _timeLabel;
    
    // Settings
    private Button _settingsActivator;
    private VisualElement _settings;
    private Button _workSettingButton;
    private Button _taxesSettingButton;
    private Button _foodSettingButton;
    private VisualElement _workSettingTab;
    private VisualElement _taxesSettingTab;
    private VisualElement _foodSettingTab;
    private SliderInt _workNecessitiesSlider;
    private Label _necessitiesLabelValue;
    private Label _workLabelValue;
    private SliderInt _taxesSlider;
    private Label _taxesLabelValue;
    private readonly List<string> _foodPortions = new List<string> { "No Food", "1/2 Portion", "Full Portion" };
    private RadioButtonGroup _foodPortionRadioButtonGroup;

    // Presetting
    private VisualElement _presettingSelection;
    private Button _freePlay;
    private Button _presettingOne;
    private Button _presettingTwo;
    private Button _presettingThree;

    private void Awake() {
        if (Instance != null && Instance != this) { 
            Destroy(this); 
        } 
        else { 
            Instance = this; 
        } 
    }

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
        ButtonClicked("explore");
        
        // Inspector
        _inspector = _uiDocument.rootVisualElement.Q<VisualElement>("inspector");
        _inspectorName = _uiDocument.rootVisualElement.Q<Label>("inspectorName");
        _inspectorSatisfaction = _uiDocument.rootVisualElement.Q<ProgressBar>("inspectorSatisfaction");
        _inspectorHappiness = _uiDocument.rootVisualElement.Q<ProgressBar>("inspectorHappiness");
        _inspectorFood = _uiDocument.rootVisualElement.Q<ProgressBar>("inspectorFood");
        _inspectorReligion = _uiDocument.rootVisualElement.Q<ProgressBar>("inspectorReligion");
        _inspectorTavern = _uiDocument.rootVisualElement.Q<ProgressBar>("inspectorTavern");
        _inspector.visible = false;
        
        // Statusbar
        _dayLabel = _uiDocument.rootVisualElement.Q<Label>("dayLabel");
        _timeLabel = _uiDocument.rootVisualElement.Q<Label>("timeLabel");
        
        // Settings
        _settingsActivator = _uiDocument.rootVisualElement.Q<Button>("settingsActivator");
        _settingsActivator.clicked += () => { ToggleSettings(); };
        _settings = _uiDocument.rootVisualElement.Q<VisualElement>("settings");
        _settings.visible = false;
        
        _workSettingButton = _uiDocument.rootVisualElement.Q<Button>("workSettingButton");
        _taxesSettingButton = _uiDocument.rootVisualElement.Q<Button>("taxesSettingButton");
        _foodSettingButton = _uiDocument.rootVisualElement.Q<Button>("foodSettingButton");
        _workSettingButton.clicked += () => { SelectSettingTab("work"); };
        _taxesSettingButton.clicked += () => { SelectSettingTab("taxes"); };
        _foodSettingButton.clicked += () => { SelectSettingTab("food"); };
        
        _workSettingTab = _uiDocument.rootVisualElement.Q<VisualElement>("workSettingTab");
        _taxesSettingTab = _uiDocument.rootVisualElement.Q<VisualElement>("taxesSettingTab");
        _foodSettingTab = _uiDocument.rootVisualElement.Q<VisualElement>("foodSettingTab");
        
        _workNecessitiesSlider = _uiDocument.rootVisualElement.Q<SliderInt>("workNecessitiesSlider");
        _workNecessitiesSlider.highValue = _timeController.hoursInADay;
        _necessitiesLabelValue = _uiDocument.rootVisualElement.Q<Label>("necessitiesLabelValue");
        _workLabelValue = _uiDocument.rootVisualElement.Q<Label>("workLabelValue");
        
        _taxesSlider = _uiDocument.rootVisualElement.Q<SliderInt>("taxesSlider");
        _taxesLabelValue = _uiDocument.rootVisualElement.Q<Label>("taxesLabelValue");
        _taxesSlider.RegisterCallback<ChangeEvent<int>>((evt) => {
            currentTaxes = evt.newValue;
            _taxesLabelValue.style.left = Length.Percent(evt.newValue);
            _taxesLabelValue.text = evt.newValue.ToString();
        });

        _foodPortionRadioButtonGroup = _uiDocument.rootVisualElement.Q<RadioButtonGroup>("foodPortionRadioButtonGroup");
        _foodPortionRadioButtonGroup.choices = _foodPortions;
        int preSelectedFoodOption = 2;
        _foodPortionRadioButtonGroup.value = preSelectedFoodOption;
        currentFoodPortion = _foodPortions[preSelectedFoodOption];
        _foodPortionRadioButtonGroup.RegisterCallback<ChangeEvent<int>>((evt) => {
            currentFoodPortion = _foodPortions[evt.newValue];
        });

        // Presetting selection
        _presettingSelection = _uiDocument.rootVisualElement.Q<VisualElement>("presettingSelection");
        _freePlay = _uiDocument.rootVisualElement.Q<Button>("freePlay");
        _freePlay.clicked += () => { _presettingSelection.visible = false; };
        _presettingOne = _uiDocument.rootVisualElement.Q<Button>("presettingOne"); 
        _presettingOne.clicked += () => {
            presettingGenerator.GeneratePresettingOne(); 
            _presettingSelection.visible = false;
            ButtonClicked("explore");
        };
        _presettingTwo = _uiDocument.rootVisualElement.Q<Button>("presettingTwo");
        _presettingTwo.clicked += () => {
            presettingGenerator.GeneratePresettingTwo(); 
            _presettingSelection.visible = false;
            ButtonClicked("explore");
        };
        _presettingThree = _uiDocument.rootVisualElement.Q<Button>("presettingThree");
        _presettingThree.clicked += () => {
            presettingGenerator.GeneratePresettingThree(); 
            _presettingSelection.visible = false;
            ButtonClicked("explore");
        };
    }

    private void Update() {
        if (_inspector.visible) {
            switch (_currentlyInspectedType) {
                case "Resident":
                    _inspectorSatisfaction.title =  Math.Floor(_currentlyInspectedResident.satisfaction) + " / 100";
                    _inspectorSatisfaction.value = _currentlyInspectedResident.satisfaction;
                    _inspectorHappiness.title =  Math.Floor(_currentlyInspectedResident.happiness) + " / 100";
                    _inspectorHappiness.value = _currentlyInspectedResident.happiness;
                    _inspectorFood.title = Math.Floor(_currentlyInspectedResident.foodSatisfaction) + " / 100";
                    _inspectorFood.value = _currentlyInspectedResident.foodSatisfaction;
                    _inspectorReligion.title =  Math.Floor(_currentlyInspectedResident.religionSatisfaction) + " / 100";
                    _inspectorReligion.value = _currentlyInspectedResident.religionSatisfaction;
                    _inspectorTavern.title =  Math.Floor(_currentlyInspectedResident.tavernSatisfaction) + " / 100";
                    _inspectorTavern.value = _currentlyInspectedResident.tavernSatisfaction;
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
                BorderVisualElement(_buildButton, 2f, 2f, 2f, 2f);
                BorderVisualElement(_exploreButton, 1f, 1f, 1f, 1f);
                BorderVisualElement(_deleteButton, 1f, 1f, 1f, 1f);
                break;
            case "delete":
                _inputControl.currentMode = InputControl.InputModes.DeleteMode;
                buildingGhost.DestroyVisual();
                BorderVisualElement(_buildButton, 1f, 1f, 1f, 1f);
                BorderVisualElement(_exploreButton, 1f, 1f, 1f, 1f);
                BorderVisualElement(_deleteButton, 2f, 2f, 2f, 2f);
                break;
            default:
            case "explore":
                _inputControl.currentMode = InputControl.InputModes.ExploreMode;
                buildingGhost.DestroyVisual();
                BorderVisualElement(_buildButton, 1f, 1f, 1f, 1f);
                BorderVisualElement(_exploreButton, 2f, 2f, 2f, 2f);
                BorderVisualElement(_deleteButton, 1f, 1f, 1f, 1f);
                break;
        }
    }

    public void SetToInspector(Transform inspectionTransform, string type) {
        if (_currentlyInspectedResident) {
            _currentlyInspectedResident.DisableIndicator();
        }
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

    private void BorderVisualElement(VisualElement visualElement, StyleFloat left, StyleFloat right, StyleFloat top,
        StyleFloat bottom) {
        visualElement.style.borderLeftWidth = left;
        visualElement.style.borderRightWidth = right;
        visualElement.style.borderTopWidth = top;
        visualElement.style.borderBottomWidth = bottom;
    }

    private void ToggleSettings() {
        _settings.visible = !_settings.visible;
        if (_settings.visible) {
            _settingsActivator.style.borderBottomRightRadius = 0;
            _settingsActivator.style.borderBottomLeftRadius = 0;
            SelectSettingTab("work");
        }
        else {
            _settingsActivator.style.borderBottomRightRadius = 3;
            _settingsActivator.style.borderBottomLeftRadius = 3;
            _workSettingTab.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            _taxesSettingTab.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
            _foodSettingTab.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
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

    private void SelectSettingTab(String tab) {
        switch (tab)
        {
            case "work":
                _workSettingTab.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                _taxesSettingTab.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                _foodSettingTab.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                BorderVisualElement(_workSettingButton.parent, 1f, 1f, 1f, 0f);
                BorderVisualElement(_taxesSettingButton.parent, 0f, 0f, 1f, 1f);
                BorderVisualElement(_foodSettingButton.parent, 1f, 1f, 1f, 1f);
                break;
            case "taxes":
                _workSettingTab.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                _taxesSettingTab.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                _foodSettingTab.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                BorderVisualElement(_workSettingButton.parent, 1f, 1f, 1f, 1f);
                BorderVisualElement(_taxesSettingButton.parent, 0f, 0f, 1f, 0f);
                BorderVisualElement(_foodSettingButton.parent, 1f, 1f, 1f, 1f);
                break;
            case "food":
                _workSettingTab.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                _taxesSettingTab.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.None);
                _foodSettingTab.style.display = new StyleEnum<DisplayStyle>(DisplayStyle.Flex);
                BorderVisualElement(_workSettingButton.parent, 1f, 1f, 1f, 1f);
                BorderVisualElement(_taxesSettingButton.parent, 0f, 0f, 1f, 1f);
                BorderVisualElement(_foodSettingButton.parent, 1f, 1f, 1f, 0f);
                break;
        }
    }
}
