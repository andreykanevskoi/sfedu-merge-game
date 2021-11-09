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
    private float maxZoom = 5f;
    private float minZoom = 1f;
    private float zoomSpeedPC = 40f;
    private float zoomSpeedPhone = 0.01f;

    // Ограничения перемещения камеры
    public float leftLimit = -4f;
    public float rightLimit = 4f;
    public float bottomLimit = -5f;
    public float upperLimit = 3f;

    // Переменная для управления пальцем
    private bool holding = false;

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
        // В момент нажатия на СКМ задается стартовая точка, от которой идет перемещение камеры
        if (Input.GetMouseButtonDown(2))
        {
            startPos = cam.ScreenToWorldPoint(Input.mousePosition);
        }

        // Если СКМ удерживается, перемещаем камеру и вычисляем "целевую" позицию для плавного перемещения
        else if (Input.GetMouseButton(2))
        {
            float posX = cam.ScreenToWorldPoint(Input.mousePosition).x - startPos.x;
            float posY = cam.ScreenToWorldPoint(Input.mousePosition).y - startPos.y;

            targetPosX = Mathf.Clamp(transform.position.x - posX, leftLimit, rightLimit);
            targetPosY = Mathf.Clamp(transform.position.y - posY, bottomLimit, upperLimit);
        }

        // Управление пальцем
        Touch touch;
        if (Input.touchCount == 1)
        {
            touch = Input.GetTouch(0);

            if (holding)
            {
                float posX = cam.ScreenToWorldPoint(touch.position).x - startPos.x;
                float posY = cam.ScreenToWorldPoint(touch.position).y - startPos.y;

                targetPosX = Mathf.Clamp(transform.position.x - posX, leftLimit, rightLimit);
                targetPosY = Mathf.Clamp(transform.position.y - posY, bottomLimit, upperLimit);
            }
            else
            {
                startPos = cam.ScreenToWorldPoint(touch.position);
                holding = true;
            }
        }
        else
        {
            holding = false;
        }

        // Плавная доводка камеры до "целевой" позиции
        transform.position = new Vector3
            (
            Mathf.Lerp(transform.position.x, targetPosX, speed * Time.deltaTime), 
            Mathf.Lerp(transform.position.y, targetPosY, speed * Time.deltaTime), 
            transform.position.z
            );
    }

    // Приближение/отдаление камеры на колесико мыши
    private void Zoom()
    {
        // Если крутим колесико на себя (отдаляем)
        if (Input.mouseScrollDelta.y < 0f)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + zoomSpeedPC * Time.deltaTime, minZoom, maxZoom);
        }
        // Если крутим колесико от себя (приближаем)
        else if (Input.mouseScrollDelta.y > 0f)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - zoomSpeedPC * Time.deltaTime, minZoom, maxZoom);
        }

        // Зум двумя пальцами на смартфоне
        if (Input.touchCount == 2)
        {
            Touch touchZero = Input.GetTouch(0);
            Touch touchOne = Input.GetTouch(1);

            Vector2 touchZeroPrevPos = touchZero.position - touchZero.deltaPosition;
            Vector2 touchOnePrevPos = touchOne.position - touchOne.deltaPosition;

            float prevMagnitude = (touchZeroPrevPos - touchOnePrevPos).magnitude;
            float currentMagnitude = (touchZero.position - touchOne.position).magnitude;

            float difference = currentMagnitude - prevMagnitude;

            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - difference * zoomSpeedPhone, minZoom, maxZoom);
        }
    }

    // Функция, выводящая линии ограничения камеры (не в видны в игре)
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector2(leftLimit, upperLimit), new Vector2(rightLimit, upperLimit));
        Gizmos.DrawLine(new Vector2(leftLimit, bottomLimit), new Vector2(rightLimit, bottomLimit));
        Gizmos.DrawLine(new Vector2(leftLimit, upperLimit), new Vector2(leftLimit, bottomLimit));
        Gizmos.DrawLine(new Vector2(rightLimit, upperLimit), new Vector2(rightLimit, bottomLimit));
    }
}
