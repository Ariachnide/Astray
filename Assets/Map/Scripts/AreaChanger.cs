using UnityEngine;

public enum CamSlideDirection {
    horizontal,
    vertical
}

public class AreaChanger : MonoBehaviour {
    private float playerShift = 1.5f;
    public CamSlideDirection slideDirection;
    [SerializeField] private GameObject
        globalMap,
        rightOrUpMPGroup,
        leftOrDownMPGroup;
    [SerializeField] private Area
        rightOrUpArea,
        leftOrDownArea;
    [SerializeField] private SpawnPoint
        rightOrUpSpawnPoint,
        leftOrDownSpawnPoint;

    private void OnTriggerEnter2D(Collider2D collider) {
        if (collider.CompareTag("Player")) {

            collider.GetComponent<PlayerAlteration>().PurgeMinorAlterations();

            if (slideDirection == CamSlideDirection.vertical) {
                if (collider.transform.position.y < transform.position.y) {
                    collider.transform.position += new Vector3(0, playerShift, 0);
                    if (rightOrUpSpawnPoint != null)
                        collider.GetComponent<PlayerMain>().currentSpawnPoint = rightOrUpSpawnPoint;

                    Camera.main.GetComponent<CamPlayerMovement>().UpdateArea(rightOrUpArea);

                    leftOrDownMPGroup.GetComponent<MarkPointGroupManager>().CallExpulsions();
                    globalMap.GetComponent<GlobalMapHandler>().UpdateActiveGroup(rightOrUpArea);

                } else if (collider.transform.position.y > transform.position.y) {
                    collider.transform.position -= new Vector3(0, playerShift, 0);
                    if (leftOrDownSpawnPoint != null)
                        collider.GetComponent<PlayerMain>().currentSpawnPoint = leftOrDownSpawnPoint;

                    Camera.main.GetComponent<CamPlayerMovement>().UpdateArea(leftOrDownArea);

                    rightOrUpMPGroup.GetComponent<MarkPointGroupManager>().CallExpulsions();
                    globalMap.GetComponent<GlobalMapHandler>().UpdateActiveGroup(leftOrDownArea);

                } else {
                    Debug.LogError("UNABLE TO RESOLVE CAMERA POSITION");
                }
            } else if (slideDirection == CamSlideDirection.horizontal) {
                if (collider.transform.position.x < transform.position.x) {
                    collider.transform.position += new Vector3(playerShift, 0, 0);
                    if (rightOrUpSpawnPoint != null)
                        collider.GetComponent<PlayerMain>().currentSpawnPoint = rightOrUpSpawnPoint;

                    Camera.main.GetComponent<CamPlayerMovement>().UpdateArea(rightOrUpArea);

                    leftOrDownMPGroup.GetComponent<MarkPointGroupManager>().CallExpulsions();
                    globalMap.GetComponent<GlobalMapHandler>().UpdateActiveGroup(rightOrUpArea);

                } else if (collider.transform.position.x > transform.position.x) {
                    collider.transform.position -= new Vector3(playerShift, 0, 0);
                    if (leftOrDownSpawnPoint != null)
                        collider.GetComponent<PlayerMain>().currentSpawnPoint = leftOrDownSpawnPoint;

                    Camera.main.GetComponent<CamPlayerMovement>().UpdateArea(leftOrDownArea);

                    rightOrUpMPGroup.GetComponent<MarkPointGroupManager>().CallExpulsions();
                    globalMap.GetComponent<GlobalMapHandler>().UpdateActiveGroup(leftOrDownArea);

                } else {
                    Debug.LogError("UNABLE TO RESOLVE CAMERA POSITION");
                }
            }
        }
    }
}
