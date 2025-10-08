using UnityEngine;
using UnityEngine.InputSystem;

public class HomeMenuControls : MonoBehaviour {
    private HomeMenuUI hmUI;

    private void Awake() {
        hmUI = GetComponent<HomeMenuUI>();
    }

    public void OnSelect(InputAction.CallbackContext context) {
        if (context.started)
            hmUI.HandleAction();
    }

    public void OnMovement(InputAction.CallbackContext context) {
        if (context.started)
            hmUI.HandleMenuSelector(context.ReadValue<Vector2>().normalized);
    }
}
