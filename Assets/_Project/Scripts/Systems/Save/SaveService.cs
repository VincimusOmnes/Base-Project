using System;
using Marmalade.Core;
using MessagePipe;
using Newtonsoft.Json;
using UnityEngine;
using Marmalade.Shared;

namespace Marmalade.Systems
{
    /// <summary>
    /// Implements ISaveService using PlayerPrefs as the storage backend.
    /// Save data is serialized to JSON and stored under a single namespaced
    /// key. Always calls PlayerPrefs.Save() after writing to force an
    /// immediate flush to IndexedDB on WebGL, where the tab can be closed
    /// without warning.
    /// </summary>
    public class SaveService : ISaveService
    {
        private const string SaveKey = "Marmalade.Save";

        private readonly IPublisher<GameSavedMessage> _gameSavedPublisher;
        private readonly IPublisher<GameLoadedMessage> _gameLoadedPublisher;
        private readonly IPublisher<SaveDeletedMessage> _saveDeletedPublisher;

        public SaveData Data { get; private set; } = new();
        public bool HasSave => PlayerPrefs.HasKey(SaveKey);

        public SaveService(
            IPublisher<GameSavedMessage> gameSavedPublisher,
            IPublisher<GameLoadedMessage> gameLoadedPublisher,
            IPublisher<SaveDeletedMessage> saveDeletedPublisher)
        {
            _gameSavedPublisher   = gameSavedPublisher;
            _gameLoadedPublisher  = gameLoadedPublisher;
            _saveDeletedPublisher = saveDeletedPublisher;
        }

        public void Load()
        {
            if (!HasSave)
            {
                Data = new SaveData();
                Log.Info("[SaveService] No save found, initialising defaults.");
                return;
            }

            string json = PlayerPrefs.GetString(SaveKey);

            try
            {
                Data = JsonConvert.DeserializeObject<SaveData>(json);

                if (Data == null)
                {
                    Log.Warning("[SaveService] Save data deserialised as null, initialising defaults.");
                    Data = new SaveData();
                }
                Log.Info("[SaveService] Save loaded.");
                _gameLoadedPublisher.Publish(new GameLoadedMessage());
            }
            catch (Exception e)
            {
                Log.Exception(e);
                Data = new SaveData();
            }
        }

        public void Save()
        {
            Data.LastSavedUtc = DateTime.UtcNow.ToString("o");

            string json = JsonConvert.SerializeObject(Data);
            PlayerPrefs.SetString(SaveKey, json);
            PlayerPrefs.Save();

            Log.Info("[SaveService] Game saved.");
            _gameSavedPublisher.Publish(new GameSavedMessage());
        }

        public void Delete()
        {
            PlayerPrefs.DeleteKey(SaveKey);
            PlayerPrefs.Save();

            Data = new SaveData();

            Log.Info("[SaveService] Save deleted.");
            _saveDeletedPublisher.Publish(new SaveDeletedMessage());
        }
    }
}