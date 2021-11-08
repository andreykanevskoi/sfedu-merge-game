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
    private float zoomSpeed = 40f;

    // Ограничения перемещения камеры
    public float leftLimit = -4f;
    public float rightLimit = 4f;
    public float bottomLimit = -5f;
    public float upperLimit = 3f;

    private void Start()
    {
        cam = GetComponent<Camera>();        
        targetPosX = transform.position.x;
        targetPosY = transform.position.y;
        //OnDrawGizmos();
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
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize + zoomSpeed * Time.deltaTime, minZoom, maxZoom);
        }
        // Если крутим колесико от себя (приближаем)
        if (Input.mouseScrollDelta.y > 0f)
        {
            cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - zoomSpeed * Time.deltaTime, minZoom, maxZoom);
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
