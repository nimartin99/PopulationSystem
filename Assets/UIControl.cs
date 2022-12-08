using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class UIControl : MonoBehaviour
{
    private UIDocument _uiDocument;
    
    private Button _exploreButton;
    private Button _buildButton;
    private Button _deleteButton;

    private InputControl _inputControl;

    void Start() {
        _inputControl = GetComponent<InputControl>();
        
        _uiDocument = GetComponent<UIDocument>();
        _exploreButton = _uiDocument.rootVisualElement.Q<Button>("exploreButton");
        _exploreButton.clicked += () => { ButtonClicked("explore"); };
        _buildButton = _uiDocument.rootVisualElement.Q<Button>("buildButton");
        _buildButton.clicked += () => { ButtonClicked("build"); };
        _deleteButton = _uiDocument.rootVisualElement.Q<Button>("deleteButton");
        _deleteButton.clicked += () => { ButtonClicked("delete"); };
    }

    private void ButtonClicked(string type) {
        switch (type) {
            case "build":
                _inputControl.currentMode = InputControl.InputModes.BuildingMode;
                break;
            case "delete":
                _inputControl.currentMode = InputControl.InputModes.DeleteMode;
                break;
            default:
            case "explore":
                _inputControl.currentMode = InputControl.InputModes.ExploreMode;
                break;
        }
    }
}
