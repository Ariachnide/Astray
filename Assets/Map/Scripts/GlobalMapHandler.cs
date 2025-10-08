using UnityEngine;

public class GlobalMapHandler : MonoBehaviour {
    private GameObject activeMap;
    [SerializeField]
    private GameObject 
        player,
        storySwitch;

    private void Awake() {
        foreach (Transform child in transform)
            child.gameObject.SetActive(false);
    }

    public SpawnPoint LoadInitialSetup(string mapName, string spawnPointName) {
        activeMap = transform.Find(mapName).gameObject;
        activeMap.SetActive(true);

        SpawnPoint spawnPoint = activeMap.GetComponent<MapController>().GetSpawnPointByName(spawnPointName);

        activeMap.GetComponent<MapController>().SetActiveGroups(spawnPoint.area);
        activeMap.GetComponent<MapController>().HandleSpawn(spawnPoint, false);

        return spawnPoint;
    }

    public void SetActiveMap(GameObject go, Area nextArea) {
        activeMap.SetActive(false);
        activeMap = go;
        activeMap.SetActive(true);
        activeMap.GetComponent<MapController>().SetActiveGroups(nextArea);
    }

    public string GetActiveMapName() {
        return activeMap.name;
    }

    public void UpdateActiveGroup(Area area) {
        activeMap.GetComponent<MapController>().SetActiveGroups(area);
    }

    public GameObject GetPlayer() {
        return player;
    }

    public GameObject GetStorySwitch() {
        return storySwitch;
    }
}
