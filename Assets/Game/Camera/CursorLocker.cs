using UnityEngine;

public class CursorLocker : MonoBehaviour {
    void Start() {
        LockCursor();
    }

    void Update() {
        // Press Escape to unlock
        if (Input.GetKeyDown(KeyCode.Escape)) {
            UnlockCursor();
        }

        // Press Left Click to lock again
        if (Input.GetMouseButtonDown(0)) {
            LockCursor();
        }
    }

    void LockCursor() {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void UnlockCursor() {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
