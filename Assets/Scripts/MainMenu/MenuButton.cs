using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.Windows;

public class MenuButton : MonoBehaviour
{
    [SerializeField] private Camera playerCamera;
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private EventSystem eventSystem;

    private readonly List<RaycastResult> _results = new();

    void Update()
    {

    }


}
