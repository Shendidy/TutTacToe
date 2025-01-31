﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public static class GameDataManager
{
    private static string _path { get; set;  }

    public static void SaveGameData(GameData gameData)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        SetPath();
        FileStream file;

        if (File.Exists(_path)) file = File.OpenWrite(_path);
        else file = File.Create(_path);

        formatter.Serialize(file, gameData);
        file.Close();
    }

    public static GameData LoadGameData()
    {
        SetPath();
        FileStream file;

        if (File.Exists(_path))
        {
            file = File.OpenRead(_path);
            BinaryFormatter formatter = new BinaryFormatter();
            GameData gameData = (GameData)formatter.Deserialize(file);
            file.Close();

            var storedDate = gameData._date.ToString(Constants._RefillKeysString);
            var currentDate = DateTime.UtcNow.ToString(Constants._RefillKeysString);

            if (storedDate != currentDate && gameData._keys < Constants._MaximumDailyKeysRefill)
            {
                SaveGameData(new GameData(Constants._MaximumDailyKeysRefill, DateTime.UtcNow));
                gameData = LoadGameData();
            }

            return gameData;
        }
        else
        {
            GameData gameData = new GameData(Constants._MaximumDailyKeysRefill, DateTime.UtcNow);
            SaveGameData(gameData);
            LoadGameData();
        }

        return null;
    }

    private static void SetPath()
    {
        _path = Application.persistentDataPath + "/gd.ps";
    }
}
