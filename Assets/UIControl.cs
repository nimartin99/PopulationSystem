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
    
    // Toolbar
    private Button _exploreButton;
    private Button _buildButton;
    private Button _deleteButton;
    
    // Inspector
    private VisualElement _inspector;
    private Label _inspectorName;
    private ProgressBar _inspectorReligion;
    private Transform _currentlyInspected;
    private string _currentlyInspectedType;
    private Resident _currentlyInspectedResident;

    void Start() {
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
        _inspectorReligion = _uiDocument.rootVisualElement.Q<ProgressBar>("inspectorReligion");
        _inspector.visible = false;
    }

    private void Update() {
        if (_inspector.visible) {
            switch (_currentlyInspectedType) {
                case "Resident":
                    // Religion
                    _inspectorReligion.title = _currentlyInspectedResident.religionSatisfaction + " / 100";
                    break;
                case "House":
                    break;
            }
        }
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
}
