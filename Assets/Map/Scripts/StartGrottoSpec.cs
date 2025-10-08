using UnityEngine;

public class StartGrottoSpec : MonoBehaviour, ISpecBehavior {
    [SerializeField]
    private GameObject player;

    public void DispatchBehavior(SpawnPoint sp) {
        switch (sp.specialDetails) {
            case "House Grotto Bed":
                // player.GetComponent<PlayerHouseGrottoBed>().SetUpBehavior(sp);
                Debug.LogError("SPECIAL BEHAVIOR FROM 'HOUSE GROTTO BED' DISABLED");
                break;
            default:
                Debug.LogError($"UNKNOWN SPECIAL SPAWNPOINT BEHAVIOR: {sp.specialDetails}");
                break;
        }
    }
}
