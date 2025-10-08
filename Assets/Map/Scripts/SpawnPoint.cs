using UnityEngine;

public enum BehaviorAtSpawning {
    still,
    shortWalk,
    longWalk,
    special,
    storySpecial
}

public enum WalkAtSpawningDirection {
    up,
    down,
    left,
    right,
    none
}

[CreateAssetMenu(fileName = "New SpawnPoint Asset", menuName = "Custom Assets/SpawnPoint")]
public class SpawnPoint : ScriptableObject {
    public Vector2 position;
    public BehaviorAtSpawning behavior;
    public WalkAtSpawningDirection walkDirection = WalkAtSpawningDirection.none;
    public string specialDetails, locationName;
    public Area area;
}
