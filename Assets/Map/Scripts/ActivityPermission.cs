using System.Collections.Generic;
using UnityEngine;

public class ActivityPermission : MonoBehaviour, IHandleActivationPermission {
    [SerializeField]
    private List<StoryState> allowedStories;

    public bool CheckPermission() {
        return allowedStories.Contains(StoryTracker.currentStoryState);
    }

    public void SetPermission(List<StoryState> permissions, bool add) {
        foreach (StoryState p in permissions) {
            if (add) {
                if (!allowedStories.Contains(p)) allowedStories.Add(p);
            } else {
                if (allowedStories.Contains(p)) {
                    int i = allowedStories.IndexOf(p);
                    allowedStories.RemoveAt(i);
                }
            }
        }
    }

    public List<StoryState> GetPermissions() {
        return allowedStories;
    }
}
