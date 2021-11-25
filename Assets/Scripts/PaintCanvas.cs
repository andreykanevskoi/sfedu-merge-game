using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PaintCanvas : MonoBehaviour{
    [SerializeField] private int _textureSize = 128;
    [SerializeField] private TextureWrapMode _textureWrapMode;
    [SerializeField] private FilterMode _filterMode;
    [SerializeField] private Texture2D _texture;
    Texture2D _tmpT;

    [Range(15, 100)]
    [SerializeField] private int _brushSize = 30;

    private float _counter;
    private float _fullDirtyImg;
    private int _oldX, _oldY;

    private RectTransform _rectTransform;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();
    }

    void OnValidate() {
        if (_texture == null) {
            _texture = new Texture2D(_textureSize, _textureSize, TextureFormat.ARGB32, false);
        }
        if (_texture.width != _textureSize) {
            _texture.Resize(_textureSize, _textureSize);
        }
        _texture.wrapMode = _textureWrapMode;
        _texture.filterMode = _filterMode;
        _texture.Apply();

        _tmpT = new Texture2D(_texture.width, _texture.height, TextureFormat.ARGB32, false);
        Graphics.CopyTexture(_texture, _tmpT);

        _counter = _texture.width * _texture.height;// подсчет изначального размера грязной текстуры
        float fullImg = _counter;
        for (int x = 0; x < _texture.width; x++) {
            for (int y = 0; y < _texture.height; y++) {
                Color color = _texture.GetPixel(x, y);
                if (color.a == 0) {
                    _counter--;
                }
            }
        }
        _fullDirtyImg = _counter;
    }

    private bool InsideImage(Vector2 position) {
        return RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, position);
    }

    private void Update() {
        //_brushSize += (int)Input.mouseScrollDelta.y;
        if (Input.GetMouseButton(0))// очистка
        {
            Vector2 position = Input.mousePosition;

            if (InsideImage(position)) {
                Vector2 localPoint = new Vector2();
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, position, null, out localPoint)) {
                    Debug.Log("Inside!");
                    Debug.Log(localPoint);

                    Vector2 uv = new Vector2(
                        (localPoint.x + _rectTransform.rect.width * 0.5f) / _rectTransform.rect.width,
                        (localPoint.y + _rectTransform.rect.height * 0.5f) / _rectTransform.rect.height
                    );

                    Debug.Log(uv);

                    int newX = (int)(uv.x * _textureSize);
                    int newY = (int)(uv.y * _textureSize);

                    _texture.SetPixel(newX, newY, new Color(0, 0, 0, 0));

                    if (_oldX != newX || _oldY != newY) {
                        DrawCircle(newX, newY);
                        _oldX = newX;
                        _oldY = newY;
                    }
                    _texture.Apply();
                }
            }
        }


        if (Input.GetKeyUp(KeyCode.Mouse0))// подсчет % грязи по отпусканию ЛКМ
        {
            _counter = _texture.width * _texture.height;
            float fullImg = _counter;
            float dirtyImg;

            for (int x = 0; x < _texture.width; x++) {
                for (int y = 0; y < _texture.height; y++) {
                    Color color = _texture.GetPixel(x, y);
                    if (color.a == 0) {
                        _counter--;
                    }
                }
            }
            dirtyImg = _counter;
            float pers = (dirtyImg / _fullDirtyImg) * 100;
            Debug.Log(pers);
        }

        if (Input.GetMouseButton(1))// обновить изображение
        {
            Debug.Log("Image updated");
            Graphics.CopyTexture(_tmpT, _texture);
        }
    }

    // восстанавливает текстуру при завершении
    void OnApplicationQuit() {
        Debug.Log("Application ending");
        Graphics.CopyTexture(_tmpT, _texture);
    }

    void DrawCircle(int rayX, int rayY)// круглая кисть
    {
        for (int y = 0; y < _brushSize; y++) {
            for (int x = 0; x < _brushSize; x++) {
                float x2 = Mathf.Pow(x - _brushSize / 2, 2);
                float y2 = Mathf.Pow(y - _brushSize / 2, 2);
                float r2 = Mathf.Pow(_brushSize / 2 - 0.5f, 2);

                if (x2 + y2 < r2) {
                    int pixelX = rayX + x - _brushSize / 2;
                    int pixelY = rayY + y - _brushSize / 2;

                    if (pixelX >= 0 && pixelX < _textureSize && pixelY >= 0 && pixelY < _textureSize) {
                        _texture.SetPixel(pixelX, pixelY, new Color(0, 0, 0, 0));
                    }
                }
            }
        }
    }
}
