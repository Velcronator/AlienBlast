using System.Collections.Generic;
using System;

[Serializable]
public class LevelData
{
    public string LevelName;
    public List<CoinData> CoinDatas = new List<CoinData>();
    public List<LaserSwitchData> LaserSwitchDatas = new List<LaserSwitchData>();
}