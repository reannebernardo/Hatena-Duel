using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SettingsUIScript : MonoBehaviour
{
    MenuAudioManagerScript MenuAudio;

    public GameObject SettingsDialog;
    public GameObject KeyChangeDialog;

    public Slider BGMusicSlider;
    public Slider SFXSlider;
    public Button MoveLeftButton;
    public Button MoveUpButton;
    public Button MoveRightButton;
    public Button MoveDownButton;
    public Button Skill1Button;
    public Button Skill2Button;
    public Button Skill3Button;
    public Button UltimateAttButton;
    public Toggle FullscreenToggle;

    private SettingsSerializer prevSettings;

    void Start()
    {
        MenuAudio = FindObjectOfType<MenuAudioManagerScript>();
    }

    public void SettingsClick()
    {
        SettingsDialog.SetActive(true);
        prevSettings = SettingsSerializer.CreateFromSettings();

        BGMusicSlider.value = prevSettings.BGMusicVolume;
        SFXSlider.value = prevSettings.SFXVolume;
        MoveLeftButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = prevSettings.LeftKey.ToString();
        MoveUpButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = prevSettings.JumpKey.ToString();
        MoveRightButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = prevSettings.RightKey.ToString();
        MoveDownButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = prevSettings.GroundKey.ToString();
        Skill1Button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = prevSettings.Skill1Key.ToString();
        Skill2Button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = prevSettings.Skill2Key.ToString();
        Skill3Button.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = prevSettings.Skill3Key.ToString();
        UltimateAttButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = prevSettings.UltimateAttKey.ToString();
        FullscreenToggle.isOn = prevSettings.Fullscreen;
    }
    public void SettingsClose()
    {
        SettingsManager.UpdateSettings(prevSettings);
        SettingsDialog.SetActive(false);
    }
    public void SettingsApply()
    {
        SettingsManager.UpdateSettings(SettingsSerializer.CreateFromSettings());

        MenuAudio.UpdateVolume();

        SettingsDialog.SetActive(false);
    }

    #region SLIDER UI SETTINGS VALUE CHANGED
    public void BGMusicVolumeValueChanged()
    {
        Settings.BGMusicVolume = BGMusicSlider.value;
    }
    public void SFXVolumeValueChanged()
    {
        Settings.SFXVolume = SFXSlider.value;
    }
    #endregion

    #region KEY CONTROL SETTINGS
    private int SettingButtonPressed;
    private GameObject ButtonClicked;
    public void KeyButtonPressed(int btn)
    {
        ButtonClicked = EventSystem.current.currentSelectedGameObject;
        SettingButtonPressed = btn;
        KeyChangeDialog.SetActive(true);
    }

    private KeyCode currentKeyDownPressed;
    void Update()
    {
        if (KeyChangeDialog.activeSelf)
        {
            foreach (KeyCode vKey in System.Enum.GetValues(typeof(KeyCode)))
            {
                // ignore mouse inputs
                if (vKey.ToString().Contains("Mouse"))
                    continue;

                if (Input.GetKeyDown(vKey))
                {
                    GameObject keyPressedLabel = KeyChangeDialog.transform.GetChild(1).gameObject;
                    currentKeyDownPressed = vKey;
                    keyPressedLabel.GetComponent<TextMeshProUGUI>().text = vKey.ToString();
                }
                else if (Input.GetKeyUp(currentKeyDownPressed))
                {
                    if (SettingsManager.IsValidKeySettingChange(currentKeyDownPressed, SettingsSerializer.CreateFromSettings()))
                    {
                        switch (SettingButtonPressed)
                        {
                            case 0:
                                Settings.LeftKey = currentKeyDownPressed;
                                break;
                            case 1:
                                Settings.JumpKey = currentKeyDownPressed;
                                break;
                            case 2:
                                Settings.RightKey = currentKeyDownPressed;
                                break;
                            case 3:
                                Settings.GroundKey = currentKeyDownPressed;
                                break;
                            case 4:
                                Settings.Skill1Key = currentKeyDownPressed;
                                break;
                            case 5:
                                Settings.Skill2Key = currentKeyDownPressed;
                                break;
                            case 6:
                                Settings.Skill3Key = currentKeyDownPressed;
                                break;
                            case 7:
                                Settings.UltimateAttKey = currentKeyDownPressed;
                                break;
                            default:
                                Debug.Log("There is no such key #" + SettingButtonPressed + 1);
                                break;
                        }
                        // set text of the key button ui clicked
                        ButtonClicked.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = currentKeyDownPressed.ToString();    
                    }

                    // reset key pressed label
                    GameObject keyPressedLabel = KeyChangeDialog.transform.GetChild(1).gameObject;
                    keyPressedLabel.GetComponent<TextMeshProUGUI>().text = "_";
                    KeyChangeDialog.SetActive(false);
                }
            }
        }
    }
    #endregion

    #region GENERAL SETTINGS
    public void FullscreenChanged()
    {
        Settings.Fullscreen = FullscreenToggle.isOn;
    }
    #endregion
}
