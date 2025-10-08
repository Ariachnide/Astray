using System.Collections.Generic;

public interface IGoToStoryItem {
    public bool CheckHasEvent();
    public void TriggerEvent();
    public void EventSetup(List<StoryMilestoneRaw> rawMilestones);
    public StoryMilestoneRawList GetMilestones();
}
