using UnityEngine;
using UnityEngine.Rendering;
using States;
using System.Collections.Generic;
using System;

public class SettingsManagerScript : MonoBehaviour, IGameListener
{
    public GameManagerScript Game { get; private set; }

    public VolumeProfile[] VolumeProfiles;

    private Dictionary<Settings, SettingsVariant> settings;
    private Volume cameraVolume;
    private SpawnManagerScript[] spawnManagers;
    private ReflectionProbe[] reflections;

    private void Awake()
    {
        Camera.main.GetComponentOrThrow(out cameraVolume);
        if (VolumeProfiles.Length != QualitySettings.names.Length)
        {
            throw new NotSupportedException("Incorrect number of Volume Profiles");
        }

        spawnManagers = FindObjectsOfType<SpawnManagerScript>();
        foreach (var spawnManager in spawnManagers)
        {
            if (spawnManager.Settings.Length != QualitySettings.names.Length)
            {
                throw new NotSupportedException("Incorrect number of Spawner Settings");
            }
        }

        reflections = FindObjectsOfType<ReflectionProbe>();
        Extensions.AssertTrue(reflections.Length > 0);

        for (int i = 0; i < QualitySettings.names.Length; ++i)
        {
            if (QualitySettings.names[i] != ((Quality)i).ToString())
            {
                throw new NotSupportedException("Quality level: " + QualitySettings.names[i]);
            }
        }

        settings = new();
        AddSetting(Settings.Quality, 0);
        AddSetting(Settings.Sensitivity, 1.0f);
        SetCallback<int>(Settings.Quality, ChangeQuality);
    }

    private void Start()
    {
        // Set all the settings.
        foreach (var setting in settings)
        {
            // Set the callbacks.
            if (setting.Value.Is<int>())
                Game.UI.SliderChangedCallback(setting.Key, (float value) => setting.Value.Set((int)value));
            else if (setting.Value.Is<float>())
                Game.UI.SliderChangedCallback(setting.Key, (float value) => setting.Value.Set(value));

            // Invoke all of them to set them.
            setting.Value.Invoke();
        }
    }

    public void SetManager(GameManagerScript manager)
    {
        Game = manager;
    }

    public void Set<T>(Settings setting, T val)
    {
        settings[setting].Set(val);
    }

    public T Get<T>(Settings setting)
    {
        return settings[setting].Get<T>();
    }

    public void SetCallback<T>(Settings setting, Action<T> callback)
    {
        settings[setting].AddCallback(callback);
    }

    private void AddSetting<T>(Settings setting, T value)
    {
        settings.Add(setting, SettingsVariant.Create(setting.ToString(), value));
    }

    private void ChangeQuality(int quality)
    {
        QualitySettings.SetQualityLevel(quality);

        foreach (var spawnManager in spawnManagers)
        {
            spawnManager.Spawn(quality);
        }

        cameraVolume.profile = VolumeProfiles[quality];

        ReflectionProbeRefreshMode reflectionMode = (Quality)quality switch
        {
            Quality.Max => ReflectionProbeRefreshMode.EveryFrame,
            Quality.Med => ReflectionProbeRefreshMode.ViaScripting,
            Quality.Min => ReflectionProbeRefreshMode.OnAwake,
            _ => throw new NotImplementedException(),
        };

        foreach (var reflection in reflections)
        {
            reflection.refreshMode = reflectionMode;
        }
    }
}
