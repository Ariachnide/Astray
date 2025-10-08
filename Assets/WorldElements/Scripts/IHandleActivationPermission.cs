using System.Collections.Generic;

public interface IHandleActivationPermission {
    public bool CheckPermission();
    public void SetPermission(List<StoryState> permissions, bool add);
    public List<StoryState> GetPermissions();
}
