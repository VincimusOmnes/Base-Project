using System;
using Marmalade.Core;
using MessagePipe;
using Newtonsoft.Json;
using UnityEngine;
using Marmalade.Shared;

namespace Marmalade.Systems
{
    /// <summary>
    /// Implements ISettingsService using PlayerPrefs as the storage backend.
    /// Settings are serialized to JSON and stored under a single namespaced
    /// key. Always calls PlayerPrefs.Save() after writing to force an
    /// immediate flush to IndexedDB on WebGL, where the tab can be closed
    /// without warning.
    /// </summary>
    public class SettingsService : ISettingsService
    {
        private const string SettingsKey = "Marmalade.Settings";

        private readonly IPublisher<SettingsChangedMessage> _settingsChangedPublisher;

        public SettingsData Data { get; private set; } = new();

        public SettingsService(IPublisher<SettingsChangedMessage> settingsChangedPublisher)
        {
            _settingsChangedPublisher = settingsChangedPublisher;
        }

        public void Load()
        {
            if (!PlayerPrefs.HasKey(SettingsKey))
            {
                Data = new SettingsData();
                Log.Info("[SettingsService] No settings found, initialising defaults.");
                return;
            }

            string json = PlayerPrefs.GetString(SettingsKey);

            try
            {
                Data = JsonConvert.DeserializeObject<SettingsData>(json) ?? new SettingsData();
                Log.Info("[SettingsService] Settings loaded.");
            }
            catch (Exception e)
            {
                Log.Exception(e);
                Data = new SettingsData();
            }
        }

        public void Save()
        {
            string json = JsonConvert.SerializeObject(Data);
            PlayerPrefs.SetString(SettingsKey, json);
            PlayerPrefs.Save();

            Log.Info("[SettingsService] Settings saved.");
            _settingsChangedPublisher.Publish(new SettingsChangedMessage());
        }

        public void ResetToDefaults()
        {
            Data = new SettingsData();
            Save();

            Log.Info("[SettingsService] Settings reset to defaults.");
        }
    }
}