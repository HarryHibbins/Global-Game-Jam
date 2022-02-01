using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public Slider volumeSlider;
    public TMP_InputField volumeInput;

    public Slider unScopedSensSlider;
    public TMP_InputField unscopedSensInput;

    public Slider scopedSensSlider;
    public TMP_InputField scopedSensInput;

    public Slider fovSlider;
    public TMP_InputField fovInput;

    public PlayerSettings playerSettings;

    private void Awake()
    {
        volumeSlider.minValue = 0;
        volumeSlider.maxValue = 100;
        unScopedSensSlider.minValue = 0;
        unScopedSensSlider.maxValue = 5;
        scopedSensSlider.minValue = 0;
        scopedSensSlider.maxValue = 5;
        fovSlider.minValue = 50;
        fovSlider.maxValue = 120;

        playerSettings.GetPrefs();
        volumeSlider.value = playerSettings.volume;
        unScopedSensSlider.value = playerSettings.unscopedSensitivity;
        scopedSensSlider.value = playerSettings.scopedSensitivity;
        fovSlider.value = playerSettings.FOV;
    }

    private void OnEnable()
    {
        playerSettings.GetPrefs();
        volumeSlider.value = playerSettings.volume;
        unScopedSensSlider.value = playerSettings.unscopedSensitivity;
        scopedSensSlider.value = playerSettings.scopedSensitivity;
        fovSlider.value = playerSettings.FOV;
    }

    private void Update()
    {
        playerSettings.volume = (int)volumeSlider.value;
        playerSettings.unscopedSensitivity = unScopedSensSlider.value;
        playerSettings.scopedSensitivity = scopedSensSlider.value;
        playerSettings.FOV = (int)fovSlider.value;

    }

    private void OnDisable()
    {
        playerSettings.UpdatePrefs();
    }

    public void UpdateSlider()
    {
        volumeSlider.value = int.Parse(volumeInput.text);
        unScopedSensSlider.value = float.Parse(unscopedSensInput.text);
        scopedSensSlider.value = float.Parse(scopedSensInput.text);
        fovSlider.value = int.Parse(fovInput.text);

        volumeSlider.value = ((int)volumeInput.characterValidation);
        unScopedSensSlider.value = ((int)unscopedSensInput.characterValidation);
        scopedSensSlider.value = ((int)scopedSensInput.characterValidation);
        fovSlider.value = ((int)fovInput.characterValidation);
    }

    public void UpdateText()
    {
        volumeInput.text = volumeSlider.value.ToString();
        unscopedSensInput.text = unScopedSensSlider.value.ToString("F2");
        scopedSensInput.text = scopedSensSlider.value.ToString("F2");
        fovInput.text = fovSlider.value.ToString();
    }
}
