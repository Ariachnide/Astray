using UnityEngine;

[CreateAssetMenu(fileName = "New Area Asset", menuName = "Custom Assets/Area")]
public class Area : ScriptableObject {
    public string mapName;
    public Vector2 minPosition;
    public Vector2 maxPosition;
}
