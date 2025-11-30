using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CatClickInteraction : MonoBehaviour
{
    [Header("Сцена з котом")]
    public string catSceneName = "CatScene";

    void Update()
    {
        if (GlobalPauseMenu.IsGamePaused())
            return;

        if (Mouse.current.leftButton.wasPressedThisFrame)
        {
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            RaycastHit2D hit = Physics2D.Raycast(mousePos, Vector2.zero);

            if (hit.collider != null && hit.collider.gameObject == gameObject)
            {
                // натиснули на кота = завантажити сцену кота
                SceneManager.LoadScene(catSceneName);
            }
        }
    }
}
