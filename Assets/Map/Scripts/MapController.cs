using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour {
    public List<SpawnPoint> spawnPoints;
    [SerializeField]
    private GameObject
        elements,
        actors;
    private GameObject
        activeElementsGroup = null,
        activeActorsGroup = null;
    [field: SerializeField]
    public List<GameObject> registeredElements { get; private set; }

    public void HandleSpawn(SpawnPoint sp, bool shouldRegister) {
        PlayerMain pm = transform
            .parent
            .GetComponent<GlobalMapHandler>()
            .GetPlayer()
            .GetComponent<PlayerMain>();
        pm.state = PlayerState.busy;
        pm.canInteractWithMapElements = false;
        if (shouldRegister) pm.currentSpawnPoint = sp;

        if (
            transform
                .parent
                .GetComponent<GlobalMapHandler>()
                .GetStorySwitch()
                .GetComponent<StorySwitch>()
                .CheckIfEventOccurs()
        ) {
            transform
                .parent
                .GetComponent<GlobalMapHandler>()
                .GetStorySwitch()
                .GetComponent<StorySwitch>()
                .ExecuteEvent();
            return;
        }

        switch (sp.behavior) {
            case BehaviorAtSpawning.still:
                pm.HandleSpawningPosition(sp);
                break;
            case BehaviorAtSpawning.shortWalk or BehaviorAtSpawning.longWalk:
                pm.HandleSpawnWalk(sp);
                break;
            case BehaviorAtSpawning.special:
                GetComponent<ISpecBehavior>().DispatchBehavior(sp);
                break;
        }
    }

    private void OnDisable() {
        activeElementsGroup.SetActive(false);
        activeElementsGroup = null;
        activeActorsGroup.SetActive(false);
        activeActorsGroup = null;
    }

    public void SetActiveGroups(Area area) {
        if (activeActorsGroup != null) activeActorsGroup.SetActive(false);
        if (activeElementsGroup != null) activeElementsGroup.SetActive(false);
        activeElementsGroup = elements.transform.Find(area.name).gameObject;
        activeElementsGroup.SetActive(true);
        activeActorsGroup = actors.transform.Find(area.name).gameObject;
        activeActorsGroup.SetActive(true);
    }

    public SpawnPoint GetSpawnPointByName(string spName) {
        foreach (SpawnPoint sp in spawnPoints)
            if (sp.name == spName) return sp;
        Debug.LogError($"NO SPAWNPOINT WITH NAME {spName} FOUND ON MAP {gameObject.name}");
        return null;
    }
}
