using System;
using System.Collections.Generic;

[Serializable]
public class PlayerData {
    public string
        playerName,
        saveTime,
        playTime,
        mapName,
        spawnPointName,
        spawnPointLocationName,
        storyStatus;
    public List<string>
        savedElements,
        equippedElements,
        regElm;
    public Int16
        spawnPointIndex,
        maxHP,
        hitPoints,
        maxMana,
        currentMana,
        maxOrb,
        orbs,
        maxBomb,
        bombs;
    public List<StoryMilestoneRawList> storyMilestones;
}
