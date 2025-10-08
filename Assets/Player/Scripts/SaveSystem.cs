using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class SaveSystem {
    public static Int16 currentSlot = 0;

    public static PlayerData GetDefaultFile() {
        return new PlayerData() {
            playerName = "Default",
            saveTime = DateTime.Now.ToString("yyyy/MM/dd HH:mm"),
            playTime = TimeSpan.Zero.ToString(@"hh\:mm\:ss"),
            mapName = "",
            savedElements = new List<string>() { "black_dress" },
            equippedElements = new List<string>() { "black_dress" },
            regElm = new List<string>(),
            maxHP = 4,
            hitPoints = 4,
            maxMana = 0,
            currentMana = 0,
            maxOrb = 100,
            orbs = 0,
            maxBomb = 0,
            bombs = 0,
            spawnPointName = null,
            spawnPointLocationName = null,
            storyStatus = StoryState.Prologue.ToString()
        };
    }

    public static void SavePlayer(PlayerData data, Int16 slot) {
        string path = Application.persistentDataPath + $"/save_{slot}.json";
        File.WriteAllText(path, JsonUtility.ToJson(data));
    }

    public static bool CheckIfAnyExistingSaves() {
        List<string> paths = new List<string>() {
            Application.persistentDataPath + "/save_1.json",
            Application.persistentDataPath + "/save_2.json",
            Application.persistentDataPath + "/save_3.json"
        };
        foreach (string path in paths) {
            if (File.Exists(path)) return true;
        }
        return false;
    }

    public static bool CheckIfFileExists(Int16 v) {
        return File.Exists(Application.persistentDataPath + $"/save_{v}.json");
    }

    public static List<PlayerData> CheckExistingSaves() {
        List<PlayerData> availableSaves = new List<PlayerData>();
        List<string> paths = new List<string>() {
            Application.persistentDataPath + "/save_1.json",
            Application.persistentDataPath + "/save_2.json",
            Application.persistentDataPath + "/save_3.json"
        };
        foreach (string path in paths) {
            if (File.Exists(path)) {
                string loadPlayerData = File.ReadAllText(path);
                availableSaves.Add(JsonUtility.FromJson<PlayerData>(loadPlayerData));
            } else {
                availableSaves.Add(null);
            }
        }
        return availableSaves;
    }

    public static void DeleteSave(Int16 slot) {
        if (File.Exists(Application.persistentDataPath + $"/save_{slot}.json"))
            File.Delete(Application.persistentDataPath + $"/save_{slot}.json");
    }

    public static PlayerData LoadSave(Int16 slot) {
        return JsonUtility.FromJson<PlayerData>(File.ReadAllText(Application.persistentDataPath + $"/save_{slot}.json"));
    }
}
