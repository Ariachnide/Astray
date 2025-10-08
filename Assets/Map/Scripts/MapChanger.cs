using System.Collections;
using UnityEngine;

public class MapChanger : MonoBehaviour {
    public bool shouldRegister;
    [SerializeField]
    private GameObject
        nextMap,
        globalMap,
        backgroundLayer,
        previousMPGroup;
    [SerializeField]
    private SpawnPoint nextSpawnPoint;
    [SerializeField]
    private Area nextArea;

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player") && collider.GetComponent<PlayerMain>().canInteractWithMapElements)
            StartCoroutine(HandleChange(collider.gameObject));
    }

    private IEnumerator HandleChange(GameObject player) {
        GlobalMapHandler gmh = globalMap.GetComponent<GlobalMapHandler>();
        DisplayUIElement displayUIElm = backgroundLayer.GetComponent<DisplayUIElement>();

        displayUIElm.HideScene(Color.black, 0.5f);
        yield return new WaitForSecondsRealtime(0.5f);

        player.GetComponent<PlayerAlteration>().PurgeMinorAlterations();

        previousMPGroup.GetComponent<MarkPointGroupManager>().CallExpulsions();

        gmh.SetActiveMap(nextMap, nextArea);

        nextMap.GetComponent<MapController>().HandleSpawn(nextSpawnPoint, shouldRegister);
        displayUIElm.DisplayScene(0.5f);
    }
}
