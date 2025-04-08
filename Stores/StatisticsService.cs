using XO.Models;

namespace XO.Stores;

// StatisticsService.cs
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public static class StatisticsService
{
    private const string StatsFilePath = "playerStats.json";
    private static Dictionary<string, PlayerStats> _playerStats;

    static StatisticsService()
    {
        LoadStats();
    }

    public static void RecordGameResult(string winnerName, string loserName, bool isDraw = false)
    {
        if (isDraw)
        {
            GetOrCreateStats(winnerName).Draws++;
            GetOrCreateStats(loserName).Draws++;
        }
        else
        {
            GetOrCreateStats(winnerName).Wins++;
            GetOrCreateStats(loserName).Losses++;
        }
        
        SaveStats();
    }

    public static Dictionary<string, PlayerStats> GetAllStats()
    {
        return _playerStats;
    }

    private static PlayerStats GetOrCreateStats(string playerName)
    {
        if (_playerStats.ContainsKey(playerName))
        {
            return _playerStats[playerName];
        }

        var newStats = new PlayerStats
        {
            PlayerName = playerName,
            Wins = 0,
            Losses = 0,
            Draws = 0
        };
        
        _playerStats[playerName] = newStats;
        return newStats;
    }

    private static void LoadStats()
    {
        if (File.Exists(StatsFilePath))
        {
            var json = File.ReadAllText(StatsFilePath);
            _playerStats = JsonConvert.DeserializeObject<Dictionary<string, PlayerStats>>(json);
        }
        else
        {
            _playerStats = new Dictionary<string, PlayerStats>();
        }
    }

    private static void SaveStats()
    {
        var json = JsonConvert.SerializeObject(_playerStats, Formatting.Indented);
        File.WriteAllText(StatsFilePath, json);
    }
}