using System.Collections.Generic;
using System;

[Serializable]
public class GameData
{
    public List<PlayerData> PlayerDatas = new List<PlayerData>();
    public string GameName;
    public string CurrentLevelName;
    public List<LevelData> LevelDatas = new List<LevelData>();
}