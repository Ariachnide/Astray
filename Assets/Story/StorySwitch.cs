using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public struct StoryMilestoneRaw {
    public string EventName;
    public bool HasBeenCompleted;
    public List<string> Details;
}

[Serializable]
public struct StoryMilestoneRawList {
    public string StoryName;
    public List<StoryMilestoneRaw> Milestones;
}

public class StorySwitch : MonoBehaviour {
    [SerializeField]
    private GameObject
        prologueHandler,
        part1Handler;

    public bool CheckIfEventOccurs() {
        switch (StoryTracker.currentStoryState) {
            case StoryState.Prologue:
                return prologueHandler.GetComponent<IGoToStoryItem>().CheckHasEvent();
            case StoryState.Part1:
                return part1Handler.GetComponent<IGoToStoryItem>().CheckHasEvent();
            default:
                Debug.LogError($"UNHANDLED STORY STATE VALUE (AT STORYTRACKER.CURRENTSTORYSTATE: {StoryTracker.currentStoryState.ToString()}");
                return false;
        }
    }

    public void ExecuteEvent() {
        switch (StoryTracker.currentStoryState) {
            case StoryState.Prologue:
                prologueHandler.GetComponent<IGoToStoryItem>().TriggerEvent();
                break;
            case StoryState.Part1:
                part1Handler.GetComponent<IGoToStoryItem>().TriggerEvent();
                break;
            default:
                Debug.LogError($"UNHANDLED STORY STATE VALUE (AT STORYTRACKER.CURRENTSTORYSTATE: {StoryTracker.currentStoryState.ToString()}");
                break;
        }
    }

    public void SetupEvents(List<StoryMilestoneRawList> storyMilestoneRawList) {
        foreach (StoryMilestoneRawList storyMilestoneRaw in storyMilestoneRawList) {
            StoryState storyType = (StoryState)Enum.Parse(typeof(StoryState), storyMilestoneRaw.StoryName);
            switch (storyType) {
                case StoryState.Prologue:
                    prologueHandler.GetComponent<IGoToStoryItem>().EventSetup(storyMilestoneRaw.Milestones);
                    break;
                case StoryState.Part1:
                    part1Handler.GetComponent<IGoToStoryItem>().EventSetup(storyMilestoneRaw.Milestones);
                    break;
            }
        }
    }

    public List<StoryMilestoneRawList> GetAllMilestones() {
        List<StoryMilestoneRawList> storyMilestonesList = new List<StoryMilestoneRawList>();
        StoryMilestoneRawList milestones;

        milestones = prologueHandler.GetComponent<IGoToStoryItem>().GetMilestones();
        if (milestones.Milestones.Count > 0) storyMilestonesList.Add(milestones);

        milestones = part1Handler.GetComponent<IGoToStoryItem>().GetMilestones();
        if (milestones.Milestones.Count > 0) storyMilestonesList.Add(milestones);

        return storyMilestonesList;
    }
}
