using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraManipulatorBehaviour : MonoBehaviour, ICameraManipulator
{
    new private Camera camera;
    private Coroutine cameraZoomCoroutine;
    private float initialCameraSize;
    private float currentCameraSize;

    public float shakeBase = 0.01f;
    public float cameraZoomOut = 1.4f;
    public float cameraZoomDuration = 0.2f;

    private void Start()
    {
        camera = GetComponent<Camera>();
        initialCameraSize = camera.orthographicSize;
        cameraZoomCoroutine = null;
    }

    public void Shake(float duration, float magnitude)
    {
        StartCoroutine(ShakeCoroutine(duration, magnitude));
    }

    public void ResetZoom()
    {
        if (cameraZoomCoroutine != null)
        {
            StopCoroutine(cameraZoomCoroutine);
        }
        cameraZoomCoroutine = StartCoroutine(ZoomCoroutine(false));
    }

    public void ZoomOut()
    {
        if (cameraZoomCoroutine != null)
        {
            StopCoroutine(cameraZoomCoroutine);
        }
        cameraZoomCoroutine = StartCoroutine(ZoomCoroutine(true));
    }

    private IEnumerator ZoomCoroutine(bool zoomOut)
    {
        float elapsed = 0.0f;

        float maxZoom = 1 + cameraZoomOut;
        float curZoom = camera.orthographicSize / initialCameraSize;

        while(elapsed < cameraZoomDuration)
        {
            if (zoomOut)
            {
                camera.orthographicSize = initialCameraSize * (curZoom + (maxZoom - curZoom) * (elapsed / cameraZoomDuration));
            }
            else
            {
                camera.orthographicSize = initialCameraSize * (curZoom - (curZoom - 1) * (elapsed / cameraZoomDuration));
            }
            elapsed += Time.deltaTime;
            yield return null;
        }
        cameraZoomCoroutine = null;
    }

    private IEnumerator ShakeCoroutine(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.localPosition;
        float elapsed = 0.0f;
        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * shakeBase * magnitude * (1 - elapsed / duration);
            float y = Random.Range(-1f, 1f) * shakeBase * magnitude * (1 - elapsed / duration);
            transform.localPosition = new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = originalPosition;
    }
}
