using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]

public class RequestNewShapesButton : MonoBehaviour
{
    public int numberOfRequests = 3;
    public TextMeshProUGUI numberText;

    private int _currentNumberOfRequests;
    private Button _button;
    private bool _isLocked;

    private void Start()
    {
        _currentNumberOfRequests = numberOfRequests;
        numberText.text = _currentNumberOfRequests.ToString();
        _button = GetComponent<Button>();
        _button.onClick.AddListener(OnButtonDown);
        Unlock();
    }

    private void OnButtonDown()
    {
        if (_isLocked == false)
        {
            _currentNumberOfRequests--;
            GameEvents.RequestNewShapes();
            GameEvents.CheckIfPlayerLost();

            if (_currentNumberOfRequests <= 0)
            {
                Lock();
            }

            numberText.text = _currentNumberOfRequests.ToString();
        }
    }

    private void Lock()
    {
        _isLocked = true;
        _button.interactable = false;
        numberText.text = _currentNumberOfRequests.ToString();
    }

    private void Unlock()
    {
        _isLocked = false;
        _button.interactable = true;
    }
}
