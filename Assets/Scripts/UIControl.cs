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

    public bool hitUI;
    public event Action<InputControl.InputModes> OnModeSwitch;

    private readonly Dictionary<string, string> _roleDescriptions =  new Dictionary<string, string> {
        {"Workaholic", "Workaholics prefer to go to work and work longer than usual Residents."},
        {"Glutton", "Gluttons love to go to the market to eat something. They move slower than usual Residents."},
        {"Religious", "Religious Residents visit the church more frequently and loose their satisfactions more slowly when they go to a church regularly."},
        {"Alcoholic", "Alcoholics love their tavern and will stop walking from time to time after they visited the tavern."}
    };

    // UI params
    public int currentTaxes;
    public string currentFoodPortion;

    // Toolbar
    private VisualElement _toolbar;
    private Button _exploreButton;
    private Button _buildButton;
    private Button _upgradeButton;
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
    private VisualElement _inspectorResidentRoleContainer;
    private Label _inspectorResidentRole;
    private Label _inspectorResidentRoleDescription;
    private VisualElement _inspectorScrollRiot;
    private Label _inspectorRiotStatus;
    private ProgressBar _inspectorRiotProgress;
    private Label _inspectorRiotResidentCount;
    private VisualElement _inspectorRiotReasons;
    private VisualElement _inspectorScrollHouse;
    private Label _inspectorHouseResidentCount;
    private VisualElement _inspectorHouseVisualResidents;
    private ProgressBar _inspectorHouseSatisfaction;
    private ProgressBar _inspectorHouseHappiness;
    private Label _inspectorHouseUpgrade;
    

    private Transform _currentlyInspected;
    public string currentlyInspectedType;
    [HideInInspector] public Resident currentlyInspectedResident;
    private Riot _currentlyInspectedRiot;
    public House currentlyInspectedHouse;

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
    private Button _presettingFive;

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
        VisualElement root = _uiDocument.rootVisualElement;
        
        // Toolbar
        _exploreButton = root.Q<Button>("exploreButton");
        _exploreButton.clicked += () => { ChangeMode(InputControl.InputModes.ExploreMode); };
        _buildButton = root.Q<Button>("buildButton");
        _buildButton.clicked += () => { ChangeMode(InputControl.InputModes.BuildingMode); };
        _upgradeButton = root.Q<Button>("upgradeButton");
        _upgradeButton.clicked += () => { ChangeMode(InputControl.InputModes.UpgradeMode); };
        _deleteButton = root.Q<Button>("deleteButton");
        _deleteButton.clicked += () => { ChangeMode(InputControl.InputModes.DeleteMode); };
        ChangeMode(InputControl.InputModes.ExploreMode);
        
        // Inspector
        _inspector = root.Q<VisualElement>("inspector");
        _inspectorName = root.Q<Label>("inspectorName");
        _inspectorResidentOverall =root.Q<VisualElement>("inspectorResidentOverall");
        _inspectorSatisfaction = root.Q<ProgressBar>("inspectorSatisfaction");
        _inspectorHappiness = root.Q<ProgressBar>("inspectorHappiness");
        _inspectorResidentNext = root.Q<VisualElement>("inspectorResidentNext");
        _inspectorResidentNextLabel = root.Q<Label>("inspectorResidentNextLabel");
        _inspectorScrollResident = root.Q<VisualElement>("inspectorScrollResident");
        _inspectorFood = root.Q<ProgressBar>("inspectorFood");
        _inspectorReligion = root.Q<ProgressBar>("inspectorReligion");
        _inspectorTavern = root.Q<ProgressBar>("inspectorTavern");
        _inspectorResidentRoleContainer = root.Q<VisualElement>("inspectorResidentRoleContainer");
        _inspectorResidentRole = root.Q<Label>("inspectorResidentRole");
        _inspectorResidentRoleDescription  = root.Q<Label>("inspectorResidentRoleDescription");
        _inspectorScrollRiot = root.Q<VisualElement>("inspectorScrollRiot");
        _inspectorRiotStatus = root.Q<Label>("inspectorRiotStatus");
        _inspectorRiotProgress = root.Q<ProgressBar>("inspectorRiotProgress");
        _inspectorRiotResidentCount = root.Q<Label>("inspectorRiotResidentCount");
        _inspectorRiotReasons = root.Q<VisualElement>("inspectorRiotReasons");
        _inspectorScrollHouse = root.Q<VisualElement>("inspectorScrollHouse");
        _inspectorHouseResidentCount = root.Q<Label>("inspectorHouseResidentCount");
        _inspectorHouseVisualResidents = root.Q<VisualElement>("inspectorHouseVisualResidents");
        _inspectorHouseSatisfaction = root.Q<ProgressBar>("inspectorHouseSatisfaction");
        _inspectorHouseHappiness = root.Q<ProgressBar>("inspectorHouseHappiness");
        _inspectorHouseUpgrade = root.Q<Label>("inspectorHouseUpgrade");
        
        HideInspector();
        
        // Statusbar
        _dayLabel = root.Q<Label>("dayLabel");
        _timeLabel = root.Q<Label>("timeLabel");
        
        // Settings
        _settingsActivator = root.Q<Button>("settingsActivator");
        _settingsActivator.clicked += ToggleSettings;
        _settings = root.Q<VisualElement>("settings");
        _settings.visible = false;
        
        _workSettingButton = root.Q<Button>("workSettingButton");
        _taxesSettingButton = root.Q<Button>("taxesSettingButton");
        _foodSettingButton = root.Q<Button>("foodSettingButton");
        _workSettingButton.clicked += () => { SelectSettingTab("work"); };
        _taxesSettingButton.clicked += () => { SelectSettingTab("taxes"); };
        _foodSettingButton.clicked += () => { SelectSettingTab("food"); };
        
        _workSettingTab = root.Q<VisualElement>("workSettingTab");
        _taxesSettingTab = root.Q<VisualElement>("taxesSettingTab");
        _foodSettingTab = root.Q<VisualElement>("foodSettingTab");
        
        _workNecessitiesSlider = root.Q<SliderInt>("workNecessitiesSlider");
        _necessitiesLabelValue = root.Q<Label>("necessitiesLabelValue");
        _workLabelValue = root.Q<Label>("workLabelValue");
        
        _taxesSlider = root.Q<SliderInt>("taxesSlider");
        _taxesLabelValue = root.Q<Label>("taxesLabelValue");
        _taxesSlider.RegisterCallback<ChangeEvent<int>>((evt) => {
            currentTaxes = evt.newValue;
            _taxesLabelValue.style.left = Length.Percent(evt.newValue);
            _taxesLabelValue.text = evt.newValue.ToString();
        });

        _foodPortionRadioButtonGroup = root.Q<RadioButtonGroup>("foodPortionRadioButtonGroup");
        _foodPortionRadioButtonGroup.choices = _foodPortions;
        int preSelectedFoodOption = 2;
        _foodPortionRadioButtonGroup.value = preSelectedFoodOption;
        currentFoodPortion = _foodPortions[preSelectedFoodOption];
        _foodPortionRadioButtonGroup.RegisterCallback<ChangeEvent<int>>((evt) => {
            currentFoodPortion = _foodPortions[evt.newValue];
        });

        // Presetting selection
        _presettingSelection = root.Q<VisualElement>("presettingSelection");
        _freePlay = root.Q<Button>("freePlay");
        _freePlay.clicked += () => { _presettingSelection.visible = false; };
        _presettingOne = root.Q<Button>("presettingOne"); 
        _presettingOne.clicked += () => { StartPresetting(1); };
        _presettingTwo = root.Q<Button>("presettingTwo");
        _presettingTwo.clicked += () => { StartPresetting(2); };
        _presettingThree = root.Q<Button>("presettingThree");
        _presettingThree.clicked += () => { StartPresetting(3); };
        _presettingFour = root.Q<Button>("presettingFour");
        _presettingFour.clicked += () => { StartPresetting(4); };
        _presettingFive = root.Q<Button>("presettingFive");
        _presettingFive.clicked += () => { StartPresetting(5); };

        RegisterUIBlock(root);
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
            case 5:
                presettingGenerator.GeneratePresettingFive();
                break;
        }
        _presettingSelection.visible = false;
        ChangeMode(InputControl.InputModes.ExploreMode);
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
                    _inspectorSatisfaction.title =  Math.Floor(currentlyInspectedResident.satisfaction) + " / 100";
                    _inspectorSatisfaction.value = currentlyInspectedResident.satisfaction;
                    _inspectorHappiness.title =  Math.Floor(currentlyInspectedResident.happiness) + " / 100";
                    _inspectorHappiness.value = currentlyInspectedResident.happiness;
                    _inspectorFood.title = Math.Floor(currentlyInspectedResident.foodSatisfaction) + " / 100";
                    _inspectorFood.value = currentlyInspectedResident.foodSatisfaction;
                    _inspectorReligion.title =  Math.Floor(currentlyInspectedResident.religionSatisfaction) + " / 100";
                    _inspectorReligion.value = currentlyInspectedResident.religionSatisfaction;
                    _inspectorTavern.title =  Math.Floor(currentlyInspectedResident.tavernSatisfaction) + " / 100";
                    _inspectorTavern.value = currentlyInspectedResident.tavernSatisfaction;
                    if (currentlyInspectedResident.currentTask == Resident.AvailableTasks.None) {
                        _inspectorResidentNext.style.visibility = Visibility.Hidden;
                    }
                    else {
                        _inspectorResidentNext.style.visibility = Visibility.Visible;
                        _inspectorResidentNextLabel.text = currentlyInspectedResident.currentTask.ToString();
                    }
                    break;
                case "House":
                    _inspectorScrollHouse.style.display = DisplayStyle.Flex;
                    _inspectorHouseResidentCount.text = currentlyInspectedHouse.residents.Count.ToString();
                    if (_inspectorHouseVisualResidents.childCount != currentlyInspectedHouse.residents.Count) {
                        _inspectorHouseVisualResidents.Clear();
                        for (int i = 0; i < currentlyInspectedHouse.residents.Count; i++) {
                            VisualElement residentVisual = new VisualElement();
                            residentVisual.AddToClassList("farmerIconInspector");
                        }
                    }
                    _inspectorHouseSatisfaction.title = Math.Floor(currentlyInspectedHouse.allResidentsSatisfaction) + " / 100";
                    _inspectorHouseSatisfaction.value = currentlyInspectedHouse.allResidentsSatisfaction;
                    _inspectorHouseHappiness.title = Math.Floor(currentlyInspectedHouse.allResidentsHappiness) + " / 100";
                    _inspectorHouseHappiness.value = currentlyInspectedHouse.allResidentsHappiness;
                    _inspectorHouseUpgrade.text = currentlyInspectedHouse.upgradeable ? "Yes" : "No";
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

    public void ChangeMode(InputControl.InputModes type) {
        OnModeSwitch?.Invoke(type);
        if (type != InputControl.InputModes.ExploreMode) {
            HideInspector();
        }
        switch (type) {
            case InputControl.InputModes.BuildingMode:
                _inputControl.currentMode = InputControl.InputModes.BuildingMode;
                buildingGhost.RefreshVisual();
                BorderVisualElement(_buildButton, 2f, 2f, 2f, 2f);
                BorderVisualElement(_exploreButton, 1f, 1f, 1f, 1f);
                BorderVisualElement(_upgradeButton, 1f, 1f, 1f, 1f);
                BorderVisualElement(_deleteButton, 1f, 1f, 1f, 1f);
                break;
            case InputControl.InputModes.DeleteMode:
                _inputControl.currentMode = InputControl.InputModes.DeleteMode;
                buildingGhost.DestroyVisual();
                BorderVisualElement(_buildButton, 1f, 1f, 1f, 1f);
                BorderVisualElement(_exploreButton, 1f, 1f, 1f, 1f);
                BorderVisualElement(_upgradeButton, 1f, 1f, 1f, 1f);
                BorderVisualElement(_deleteButton, 2f, 2f, 2f, 2f);
                break;
            case InputControl.InputModes.UpgradeMode:
                _inputControl.currentMode = InputControl.InputModes.UpgradeMode;
                BorderVisualElement(_buildButton, 1f, 1f, 1f, 1f);
                BorderVisualElement(_exploreButton, 1f, 1f, 1f, 1f);
                BorderVisualElement(_upgradeButton, 2f, 2f, 2f, 2f);
                BorderVisualElement(_deleteButton, 1f, 1f, 1f, 1f);
                break;
            default:
            case InputControl.InputModes.ExploreMode:
                _inputControl.currentMode = InputControl.InputModes.ExploreMode;
                buildingGhost.DestroyVisual();
                BorderVisualElement(_buildButton, 1f, 1f, 1f, 1f);
                BorderVisualElement(_exploreButton, 2f, 2f, 2f, 2f);
                BorderVisualElement(_upgradeButton, 1f, 1f, 1f, 1f);
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
                currentlyInspectedResident = _currentlyInspected.GetComponent<Resident>();
                currentlyInspectedResident.EnableIndicator();
                _inspectorName.text = currentlyInspectedType;
                if (currentlyInspectedResident.role != "") {
                    _inspectorResidentRoleContainer.style.display = DisplayStyle.Flex;
                    _inspectorResidentRole.text = currentlyInspectedResident.role;
                    _inspectorResidentRoleDescription.text = _roleDescriptions[currentlyInspectedResident.role];
                }
                break;
            case "House":
                House house = inspectionTransform.GetComponent<House>();
                if (house) {
                    _inspector.visible = true;
                    currentlyInspectedHouse = house;
                    currentlyInspectedHouse.EnableIndicator();
                    _inspectorName.text = currentlyInspectedType;
                }
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
        _inspectorScrollHouse.style.display = DisplayStyle.None;
        _inspectorName.text = "-";
        if (_currentlyInspected) {
            switch (currentlyInspectedType) {
                case "Resident":
                    _inspectorResidentRoleContainer.style.display = DisplayStyle.None;
                    currentlyInspectedResident.DisableIndicator();
                    currentlyInspectedResident = null;
                    break;
                case "House":
                    currentlyInspectedHouse.DisableIndicator();
                    currentlyInspectedHouse = null;
                    break;
                case "Riot":
                    _currentlyInspectedRiot = null;
                    break;
            }
            _currentlyInspected = null;
            currentlyInspectedType = null;
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
