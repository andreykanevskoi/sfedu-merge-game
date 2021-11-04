using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour
{
    // Отвечает за скорость перемещения камеры
    public float speed;

    private Vector2 startPos;
    private Camera cam;

    // Текущее положение камеры
    private float targetPosX;
    private float targetPosY;

    // Переменные для зума
    private float zoom = 5f;
    private float maxZoom = 5f;
    private float minZoom = 1f;

    // Область перемещения камеры
    private float minX = -2f;
    private float maxX = 2f;
    private float minY = -2f;
    private float maxY = 2f;

    private void Start()
    {
        cam = GetComponent<Camera>();        
        targetPosX = transform.position.x;
        targetPosY = transform.position.y;
    }

    private void Update()
    {
        Move();
        Zoom();
    }

    // Перемещение камеры при нажатой СКМ
    private void Move()
    {
        if (Input.GetMouseButtonDown(2))
        {
            startPos = cam.ScreenToWorldPoint(Input.mousePosition);
        }
        else if (Input.GetMouseButton(2))
        {
            float posX = cam.ScreenToWorldPoint(Input.mousePosition).x - startPos.x;
            float posY = cam.ScreenToWorldPoint(Input.mousePosition).y - startPos.y;
            targetPosX = Mathf.Clamp(transform.position.x - posX, minX, maxX);
            targetPosY = Mathf.Clamp(transform.position.y - posY, minY, maxY);
        }
        transform.position = new Vector3(Mathf.Lerp(transform.position.x, targetPosX, speed * Time.deltaTime), Mathf.Lerp(transform.position.y, targetPosY, speed * Time.deltaTime));
    }

    // Приближение/отдаление камеры на колесико мыши
    private void Zoom()
    {
        float zoomChangeAmount = 4f;
        if (Input.GetKey(KeyCode.Mouse0))
        {
            zoom = Mathf.Clamp(zoom - zoomChangeAmount * Time.deltaTime, minZoom, maxZoom);
        }
        if (Input.GetKey(KeyCode.Mouse1))
        {
            zoom = Mathf.Clamp(zoom + zoomChangeAmount * Time.deltaTime, minZoom, maxZoom);
        }
        cam.orthographicSize = zoom;
    }
}
