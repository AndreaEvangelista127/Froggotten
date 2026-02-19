using UnityEngine;

public class BackGroundScaler : MonoBehaviour
{
    private void Start()
    {
        FitToCamera();
    }

    private void FitToCamera()
    {
        Camera cam = Camera.main;
        SpriteRenderer sr = GetComponent<SpriteRenderer>();

        if (sr == null || cam == null) return;

        float cameraHeight = cam.orthographicSize * 2f;
        float cameraWidth = cameraHeight * cam.aspect;

        float spriteHeight = sr.sprite.bounds.size.y;
        float spriteWidth = sr.sprite.bounds.size.x;

        float scaleX = cameraWidth / spriteWidth;
        float scaleY = cameraHeight / spriteHeight;

        float scale = Mathf.Max(scaleX, scaleY);


        transform.localScale = new Vector3(scaleX, scaleY, 1f);
    }
}
