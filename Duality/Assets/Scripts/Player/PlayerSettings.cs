using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSettings", menuName = "PlayerSettings")]
public class PlayerSettings : ScriptableObject
{
    [Range(0, 100)]
    public int volume;
    [Range(0, 5)]
    public float unscopedSensitivity;
    [Range(0, 5)]
    public float scopedSensitivity;
    [Range(60, 120)]
    public int FOV;

    private void Awake()
    {
        GetPrefs();
    }

    public void GetPrefs()
    {
        volume = PlayerPrefs.GetInt("Volume", 100);
        unscopedSensitivity = PlayerPrefs.GetFloat("UnscopedSens", 3);
        scopedSensitivity = PlayerPrefs.GetFloat("ScopedSens", 0.56f);
        FOV = PlayerPrefs.GetInt("FOV", 60);
    }

    public void UpdatePrefs()
    {
        PlayerPrefs.SetFloat("Volume", volume);
        PlayerPrefs.SetFloat("UnscopedSens", unscopedSensitivity);
        PlayerPrefs.SetFloat("ScopedSens", scopedSensitivity);
        PlayerPrefs.SetInt("FOV", FOV);
    }
}
