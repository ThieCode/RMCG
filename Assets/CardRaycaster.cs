using GameEvents;
using UnityEngine;
using UnityEngine.InputSystem;

public class CardRaycaster : MonoBehaviour
{
    [Header("Input")]
    [Tooltip("Reference to the 'Click' action in your CardInputActions asset.")]
    [SerializeField] private InputActionReference clickAction;

    [Header("Camera & Layers")]
    [SerializeField] private Camera targetCamera;
    [SerializeField] private LayerMask cardsLayerMask;

    private void Awake()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        if (targetCamera == null)
            Debug.LogError("CardRaycasterInputActions: No camera assigned and no MainCamera in scene.");
    }

    private void OnEnable()
    {
        if (clickAction == null)
        {
            Debug.LogError("CardRaycasterInputActions: Click InputActionReference is not assigned.");
            return;
        }

        // Subscribe to performed event and enable the action
        clickAction.action.performed += OnClickPerformed;
        clickAction.action.Enable();
    }

    private void OnDisable()
    {
        if (clickAction != null)
        {
            clickAction.action.performed -= OnClickPerformed;
            clickAction.action.Disable();
        }
    }

    private void OnClickPerformed(InputAction.CallbackContext ctx)
    {
        if (targetCamera == null)
            return;

        // Try to get a pointer position (works for mouse & touch)
        Vector2 screenPos = GetCurrentPointerPosition();
        if (screenPos == Vector2.negativeInfinity)
            return;

        Ray ray = targetCamera.ScreenPointToRay(screenPos);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, cardsLayerMask))
        {
            Debug.Log($"[CardRaycaster] Hit card: {hit.collider.gameObject.name}", hit.collider.gameObject);
            GameEventBus.Raise(new CardClickedEvent(hit.collider.GetComponent<Card>()));
        }
    }

    private Vector2 GetCurrentPointerPosition()
    {
        // If there's a mouse, prefer that
        if (Mouse.current != null)
            return Mouse.current.position.ReadValue();

        // If there is a touchscreen, use first active touch
        if (Touchscreen.current != null)
        {
            var ts = Touchscreen.current;
            if (ts.primaryTouch.press.isPressed)
                return ts.primaryTouch.position.ReadValue();
        }

        // Fallback: if there's a generic Pointer (pen etc.)
        if (Pointer.current != null)
            return Pointer.current.position.ReadValue();

        return Vector2.negativeInfinity;
    }
}
