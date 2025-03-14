using UnityEngine;

public class InputManager : SaiSingleton<InputManager>
{
    [SerializeField] protected Vector2 screenBounds;

    protected override void Awake()
    {
        base.Awake();
        CalculateScreenBounds();
    }

    private void CalculateScreenBounds()
    {
        Camera cam = Camera.main;
        Vector3 topRight = cam.ViewportToWorldPoint(new Vector3(1, 1, cam.nearClipPlane));
        this.screenBounds = (Vector2)topRight;
    }

    public Vector3 GetMouseWorldPosition()
    {
        Vector3 mouseScreenPosition = Input.mousePosition;
        mouseScreenPosition.z = Camera.main.transform.position.y;
        Vector3 worldPosition = Camera.main.ScreenToWorldPoint(mouseScreenPosition);
        return worldPosition;
    }

    private Vector3 ClampPositionToScreen(Vector3 position)
    {
        position.x = Mathf.Clamp(position.x, -screenBounds.x, screenBounds.x);
        position.y = Mathf.Clamp(position.y, -screenBounds.y, screenBounds.y);
        return position;
    }
}
