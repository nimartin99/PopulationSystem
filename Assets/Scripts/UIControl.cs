using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
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

    public bool hitUI;
    
    // UI params
    public int currentTaxes;
    public string currentFoodPortion;

    // Toolbar
    private VisualElement _toolbar;
    private Button _exploreButton;
    private Button _buildButton;
    private Button _deleteButton;
    
    // Inspector
    private VisualElement _inspector;
    private Label _inspectorName;
    private VisualElement _inspectorResidentOverall;
    private ProgressBar _inspectorSatisfaction;
    private ProgressBar _inspectorHappiness;
    private VisualElement _inspectorResidentNext;
    private Label _inspectorResidentNextLabel;
    private VisualElement _inspectorScrollResident;
    private ProgressBar _inspectorFood;
    private ProgressBar _inspectorReligion;
    private ProgressBar _inspectorTavern;
    private VisualElement _inspectorScrollRiot;
    private Label _inspectorRiotStatus;
    private ProgressBar _inspectorRiotProgress;
    private Label _inspectorRiotResidentCount;
    private VisualElement _inspectorRiotReasons;

    private Transform _currentlyInspected;
    public string currentlyInspectedType;
    private Resident _currentlyInspectedResident;
    private Riot _currentlyInspectedRiot;

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
    private Button _presettingFour;

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
        _exploreButton.clicked += () => { ChangeMode("explore"); };
        _buildButton = _uiDocument.rootVisualElement.Q<Button>("buildButton");
        _buildButton.clicked += () => { ChangeMode("build"); };
        _deleteButton = _uiDocument.rootVisualElement.Q<Button>("deleteButton");
        _deleteButton.clicked += () => { ChangeMode("delete"); };
        ChangeMode("explore");
        
        // Inspector
        _inspector = _uiDocument.rootVisualElement.Q<VisualElement>("inspector");
        _inspectorName = _uiDocument.rootVisualElement.Q<Label>("inspectorName");
        _inspectorResidentOverall =_uiDocument.rootVisualElement.Q<VisualElement>("inspectorResidentOverall");
        _inspectorSatisfaction = _uiDocument.rootVisualElement.Q<ProgressBar>("inspectorSatisfaction");
        _inspectorHappiness = _uiDocument.rootVisualElement.Q<ProgressBar>("inspectorHappiness");
        _inspectorResidentNext = _uiDocument.rootVisualElement.Q<VisualElement>("inspectorResidentNext");
        _inspectorResidentNextLabel = _uiDocument.rootVisualElement.Q<Label>("inspectorResidentNextLabel");
        _inspectorScrollResident = _uiDocument.rootVisualElement.Q<VisualElement>("inspectorScrollResident");
        _inspectorFood = _uiDocument.rootVisualElement.Q<ProgressBar>("inspectorFood");
        _inspectorReligion = _uiDocument.rootVisualElement.Q<ProgressBar>("inspectorReligion");
        _inspectorTavern = _uiDocument.rootVisualElement.Q<ProgressBar>("inspectorTavern");
        _inspectorScrollRiot = _uiDocument.rootVisualElement.Q<VisualElement>("inspectorScrollRiot");
        _inspectorRiotStatus = _uiDocument.rootVisualElement.Q<Label>("inspectorRiotStatus");
        _inspectorRiotProgress = _uiDocument.rootVisualElement.Q<ProgressBar>("inspectorRiotProgress");
        _inspectorRiotResidentCount = _uiDocument.rootVisualElement.Q<Label>("inspectorRiotResidentCount");
        _inspectorRiotReasons = _uiDocument.rootVisualElement.Q<VisualElement>("inspectorRiotReasons");
        HideInspector();
        
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
        _presettingOne.clicked += () => { StartPresetting(1); };
        _presettingTwo = _uiDocument.rootVisualElement.Q<Button>("presettingTwo");
        _presettingTwo.clicked += () => { StartPresetting(2); };
        _presettingThree = _uiDocument.rootVisualElement.Q<Button>("presettingThree");
        _presettingThree.clicked += () => { StartPresetting(3); };
        _presettingFour = _uiDocument.rootVisualElement.Q<Button>("presettingFour");
        _presettingFour.clicked += () => { StartPresetting(4); };

        RegisterUIBlock(_uiDocument.rootVisualElement);
    }

    private void StartPresetting(int setting) {
        hitUI = true;
        switch (setting) {
            case 1:
                presettingGenerator.GeneratePresettingOne();
                break;
            case 2:
                presettingGenerator.GeneratePresettingTwo();
                break;
            case 3:
                presettingGenerator.GeneratePresettingThree();
                break;
            case 4:
                presettingGenerator.GeneratePresettingFour();
                break;
        }
        _presettingSelection.visible = false;
        ChangeMode("explore");
    }

    private void RegisterUIBlock(VisualElement element) {
        element.RegisterCallback<MouseDownEvent>((eve) => { HitUI(); });
        foreach (VisualElement childElement in element.Children()) {
            RegisterUIBlock(childElement);
        }
    }

    private void Update() {
        if (hitUI) {
            hitUI = false;
        }
        if (_inspector.visible) {
            switch (currentlyInspectedType) {
                case "Resident":
                    _inspectorResidentOverall.style.display = DisplayStyle.Flex;
                    _inspectorScrollResident.style.display = DisplayStyle.Flex;
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
                    if (_currentlyInspectedResident.currentTask == Resident.AvailableTasks.None) {
                        _inspectorResidentNext.style.visibility = Visibility.Hidden;
                    }
                    else {
                        _inspectorResidentNext.style.visibility = Visibility.Visible;
                        _inspectorResidentNextLabel.text = _currentlyInspectedResident.currentTask.ToString();
                    }
                    break;
                case "House":
                    break;
                case "Riot":
                    _inspectorScrollRiot.style.display = DisplayStyle.Flex;
                    _inspectorRiotStatus.text = _currentlyInspectedRiot.riotCurrentStatus;
                    _inspectorRiotProgress.value = _currentlyInspectedRiot.riotProgress;
                    _inspectorRiotProgress.title = Math.Floor(_currentlyInspectedRiot.riotProgress) + " / 100";
                    _inspectorRiotProgress.style.display = _currentlyInspectedRiot.movingRiot
                        ? DisplayStyle.Flex : DisplayStyle.None;
                    _inspectorRiotResidentCount.text = _currentlyInspectedRiot.residents.Count.ToString();
                    if (_inspectorRiotReasons.childCount != _currentlyInspectedRiot.riotingReasons.Count) {
                        RefreshRiotReasonsToUI();
                    }
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

    public void ChangeMode(string type) {
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
        HideInspector();
        _currentlyInspected = inspectionTransform;
        currentlyInspectedType = type;
        switch (currentlyInspectedType) {
            case "Resident":
                _inspector.visible = true;
                _currentlyInspectedResident = _currentlyInspected.GetComponent<Resident>();
                _currentlyInspectedResident.EnableIndicator();
                _inspectorName.text = currentlyInspectedType;
                break;
            case "House":
                House house = inspectionTransform.GetComponent<House>();
                break;
            case "Riot":
                Riot riot = inspectionTransform.parent.GetComponent<Riot>();
                if (riot) {
                    _inspector.visible = true;
                    _inspectorName.text = currentlyInspectedType;
                    _currentlyInspectedRiot = riot;
                    _inspectorRiotReasons.style.display = DisplayStyle.Flex;
                    RefreshRiotReasonsToUI();
                }
                break;
        }
    }

    public void HideInspector() {
        _inspector.visible = false;
        _inspectorResidentOverall.style.display = DisplayStyle.None;
        _inspectorScrollResident.style.display = DisplayStyle.None;
        _inspectorScrollRiot.style.display = DisplayStyle.None;
        _inspectorRiotReasons.style.display = DisplayStyle.None;
        _inspectorName.text = "-";
        switch (currentlyInspectedType) {
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
        hitUI = true;
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
        hitUI = true;
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

    private void RefreshRiotReasonsToUI() {
        int count = _inspectorRiotReasons.childCount;
        for (int i=1; i < count; i++)
        {
            _inspectorRiotReasons.RemoveAt(1);
        }
        foreach (string reason in _currentlyInspectedRiot.riotingReasons) {
            string fullReason = "";
            switch (reason) {
                case "food":
                    fullReason = "The residents are hungry. Build more markets or increase food portions.";
                    break;
                case "religion":
                    fullReason = "The residents want more churches in their village.";
                    break;
                case "tavern":
                    fullReason = "The residents are way to sober! Build them more taverns.";
                    break;
            }
            Label label = new Label(fullReason);
            label.AddToClassList("riotReasonText");
            _inspectorRiotReasons.Add(label);
        }
    }

    private void HitUI() {
        hitUI = true;
    }
}
