﻿using UnityEngine;
using UnityEngine.UI;
using System;
using System.IO;
using System.Collections.Generic;
using System.Xml;
using System.Xml.Serialization;

[XmlRoot("Settings")]
public class Settings
{
    //static BinaryFormatter formatter = new BinaryFormatter();
    static XmlSerializer formatter = new XmlSerializer(typeof(Settings));

    [XmlElement("Quality")]
    public int quality = 0;
    [XmlElement("Resolution")]
    public int resolution = 0;
    [XmlElement("Fullscreen")]
    public bool fullScreen = true;
    [XmlElement("Language")]
    public SystemLanguage lang = SystemLanguage.English;
    /*[XmlArray("Controls")]
    [XmlArrayItem("Key")]
    public IDictionary<string, KeyCode> controls = new Dictionary<string, KeyCode>();*/

    public static Settings Load (string dirName, string fileName = "settings.xml")
    {
        Settings _s;
        using (FileStream file = File.Open(Path.Combine(dirName, fileName), FileMode.OpenOrCreate))
        {
            try
            {
                _s = formatter.Deserialize(file) as Settings;
            }
            catch (Exception ex)
            {
                _s = null;
                if (ex is XmlException)
                {
                    Debug.LogError("Invalid XML");
                }
                else if (ex is FormatException)
                {
                    Debug.LogError("Error while Deserializing");
                }
                else
                {
                    throw;
                }
            }
        }
        if (_s == null)
        {
            File.Delete(Path.Combine(dirName, fileName));
            _s = new Settings();
        }
        return _s;
    }

    public void Save (string dirName, string fileName = "settings.xml")
    {
        using (XmlWriter file = XmlWriter.Create(Path.Combine(dirName, fileName), 
            new XmlWriterSettings() { Indent = true, Encoding = System.Text.Encoding.UTF8 }))
        {
            formatter.Serialize(file, this);
        }
        Debug.Log(this);
    }

    public override string ToString ()
    {
        return string.Format("Quality = {0}, Resolution = {1}, Fullscreen = {2}, Language = {3}",
            quality, resolution, fullScreen, lang);
    }
}


public class SettingsManager : MonoBehaviour
{
    public Settings settings;
    public Settings settings_backup;

    public Toggle fullScreenToggle;
    public Dropdown resolutionDropdown;
    public Button saveButton, cancelButton;

    IDictionary<int, string> resolutions;
        
	void Awake ()
    {
        print(Application.persistentDataPath);
        settings = Settings.Load(Application.persistentDataPath);
        print(settings);
        settings_backup = settings;
        saveButton.onClick.AddListener(delegate
        {
            settings.Save(Application.persistentDataPath);
            print(settings);
        });
        fullScreenToggle.onValueChanged.AddListener(delegate
        {
            settings.fullScreen = fullScreenToggle.isOn;
            setFullscreen(settings.fullScreen);
            resolutionDropdown.interactable = !settings.fullScreen;
            print(settings);
        });
        resolutionDropdown.onValueChanged.AddListener(delegate
        {
            settings.resolution = resolutionDropdown.value;
            setResolution(resolutions[settings.resolution]);
            print(settings);
        });

        resolutions = Dropdown2dict(resolutionDropdown);
        fullScreenToggle.isOn = settings.fullScreen;
        resolutionDropdown.value = settings.resolution;
        setFullscreen(settings.fullScreen);
        if(!settings.fullScreen)
            setResolution(resolutions[settings.resolution]);
    }

    IDictionary<int, string> Dropdown2dict (Dropdown dropdown)
    {
        List<Dropdown.OptionData> ro = dropdown.options;
        IDictionary<int, string> result = new Dictionary<int, string>(ro.Count);
        for (int i = 0; i < ro.Count; i++)
        {
            result.Add(i, ro[i].text);
        }
        return result;
    }

    void setResolution (string txt)
    {
        print(txt);
        if (txt.Contains("×"))
        {
            string[] resolutionArray = txt.Split(new string[] { " × " }, StringSplitOptions.RemoveEmptyEntries);
            print(resolutionArray);
            Screen.SetResolution(Convert.ToInt32(resolutionArray[0]), Convert.ToInt32(resolutionArray[0]), false);
        }
        else
        {
            Screen.SetResolution(Screen.width, Screen.height, true);
        }
    }

    void setFullscreen (bool enable)
    {
        if (enable)
        {
            Screen.fullScreen = true;
        }
        else
        {
            Screen.fullScreen = false;
        }
    }
}
