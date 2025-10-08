using System;
using System.Collections.Generic;
using UnityEngine;

public enum Part1EventType { 

}

public struct Part1StoryMilestone {
    public Part1EventType EventType;
    public bool HasBeenCompleted;
    public List<string> Details;
}

public class Part1Handler : MonoBehaviour, IGoToStoryItem {
    public bool CheckHasEvent() {
        return false;
    }

    public void TriggerEvent() {

    }

    public void EventSetup(List<StoryMilestoneRaw> rawMilestones) {
        foreach (StoryMilestoneRaw rawMilestone in rawMilestones) {
            Part1EventType type = (Part1EventType)Enum.Parse(typeof(Part1EventType), rawMilestone.EventName);

            // switch (type) {}
        }
    }

    public StoryMilestoneRawList GetMilestones() {
        List<StoryMilestoneRaw> milestones = new List<StoryMilestoneRaw>();

        return new StoryMilestoneRawList() {
            StoryName = StoryState.Part1.ToString(),
            Milestones = milestones
        };
    }
}
