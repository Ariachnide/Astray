using System;

public enum StoryState {
    Prologue,
    Part1
}

public static class StoryTracker {
    public static StoryState currentStoryState { get; private set; }

    public static StoryState SetupStoryState(string rawStoryState) {
        currentStoryState = (StoryState)Enum.Parse(typeof(StoryState), rawStoryState);
        return currentStoryState;
    }

    public static void UpdateStoryState(StoryState storyState) {
        currentStoryState = storyState;
    }
}
