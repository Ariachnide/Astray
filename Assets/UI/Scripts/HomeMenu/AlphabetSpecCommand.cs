using UnityEngine;

public enum AlphabetSpecCommandType {
    none,
    end,
    maj,
    back
}

public class AlphabetSpecCommand : MonoBehaviour {
    public AlphabetSpecCommandType type;
}
