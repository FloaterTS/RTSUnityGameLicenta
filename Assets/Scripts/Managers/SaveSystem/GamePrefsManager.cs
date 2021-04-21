using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GamePrefsManager : MonoBehaviour
{
    public static GamePrefsManager instance;

    public Slider cameraXRotationSlider;
    public Slider cameraFOVSlider;
    public Toggle cameraSnapRotationToggle;
    public Toggle cameraMouseMovementToggle;

    [SerializeField]
    private float defaultCamXRotation = 60f;
    [SerializeField]
    private float defaultCamFOV = 60f;
    [SerializeField]
    private bool defaultCamSnapRotation = false;
    [SerializeField]
    private bool defaultCamMouseMovement = true;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Debug.LogError("Another game prefs manager present.");
    }

    private void Start()
    {
        LoadPreferences();
    }

    private void LoadPreferences()
    {
        LoadCameraXRotationPref();
        LoadCameraFOVPref();
        LoadCameraSnapRotationPref();
        LoadCameraMouseMovementPref();
    }

    public void SaveCameraXRotationPref(float valueXRotation)
    {
        PlayerPrefs.SetFloat("cameraXRotation", valueXRotation);
    }

    public void SaveCameraFieldOfViewPref(float valueFOV)
    {
        PlayerPrefs.SetFloat("cameraFOV", valueFOV);
    }

    public void SaveCameraSnapRotationPref(bool snapRotationActive)
    {
        PlayerPrefs.SetInt("cameraSnapRotation", snapRotationActive ? 1 : 0);
    }

    public void SaveCameraMovementByMousePref(bool movementByMouseActive)
    {
        PlayerPrefs.SetInt("cameraMouseMovement", movementByMouseActive ? 1 : 0);
    }

    private void LoadCameraXRotationPref()
    {
        float cameraXRot;
        if (PlayerPrefs.HasKey("cameraXRotation"))
            cameraXRot = PlayerPrefs.GetFloat("cameraXRotation");
        else
            cameraXRot = defaultCamXRotation;

        if (cameraXRotationSlider != null)
        {
            cameraXRotationSlider.value = cameraXRot;
            CameraController.instance.AdjustXRotation(cameraXRot, true);
        }
        else
            Debug.LogError("Camera X rotation slider reference not assigned!");
    }

    private void LoadCameraFOVPref()
    {
        float cameraFOV;
        if (PlayerPrefs.HasKey("cameraFOV"))
            cameraFOV = PlayerPrefs.GetFloat("cameraFOV");
        else
            cameraFOV = defaultCamFOV;

        if (cameraFOVSlider != null)
        {
            cameraFOVSlider.value = cameraFOV;
            CameraController.instance.AdjustFieldOfView(cameraFOV);
        }
        else
            Debug.LogError("Camera FOV slider reference not assigned!");
    }

    private void LoadCameraSnapRotationPref()
    {
        bool cameraSnapRot;
        if (PlayerPrefs.HasKey("cameraSnapRotation"))
            cameraSnapRot = (PlayerPrefs.GetInt("cameraSnapRotation") != 0);
        else
            cameraSnapRot = defaultCamSnapRotation;

        if (cameraSnapRotationToggle != null)
        {
            cameraSnapRotationToggle.isOn = cameraSnapRot;
            CameraController.instance.ToggleSnapRotation(cameraSnapRot);
        }
        else
            Debug.LogError("Camera snap rotation toggle reference not assigned!");
    }

    private void LoadCameraMouseMovementPref()
    {
        bool cameraMouseMovement;
        if (PlayerPrefs.HasKey("cameraSnapRotation"))
            cameraMouseMovement = (PlayerPrefs.GetInt("cameraMouseMovement") != 0);
        else
            cameraMouseMovement = defaultCamMouseMovement;

        if (cameraMouseMovementToggle != null)
        {
            cameraMouseMovementToggle.isOn = cameraMouseMovement;
            CameraController.instance.ToggleMovementByMouse(cameraMouseMovement);
        }
        else
            Debug.LogError("Camera mouse movement toggle reference not assigned!");
    }
}
