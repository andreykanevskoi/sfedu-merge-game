using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PaintCanvas : MonoBehaviour{
    [SerializeField] private int _textureSize = 128;
    [SerializeField] private TextureWrapMode _textureWrapMode;
    [SerializeField] private FilterMode _filterMode;
    [SerializeField] private Texture2D _texture;
    Texture2D _tmpT;

    [Range(15, 100)]
    [SerializeField] private int _brushSize = 30;

    [SerializeField, Range(0f, 100f)] private float _requaredPercent = 1f;

    private float _counter;
    private float _fullDirtyImg;
    private int _oldX, _oldY;

    private RectTransform _rectTransform;
    private WaitForFixedUpdate _waitForFixedUpdate = new WaitForFixedUpdate();
    
    private Vector2 _mousePosition;

    public Action OnFirstTouch;
    public Action OnMinigameEnd;

    private bool _isStarted = false;
    private bool _isFinished = false;
    private bool _isFirstTouch = true;

    private void Awake() {
        _rectTransform = GetComponent<RectTransform>();

        Prepare();
    }

    void OnValidate() {
        Prepare();
    }

    private void Prepare() {
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

    public void Begin() {
        _isStarted = true;
    }

    private bool InsideImage(Vector2 position) {
        return RectTransformUtility.RectangleContainsScreenPoint(_rectTransform, position);
    }

    public void OnTouch(InputAction.CallbackContext context) {
        if (!_isStarted || _isFinished) return;
        if (_isFirstTouch) {
            _isFirstTouch = false;
            OnFirstTouch?.Invoke();
        }
        StartCoroutine(StartDrawing(context.action));
    }

    public void SetMousePosition(InputAction.CallbackContext context) {
        if (!_isStarted) return;
        _mousePosition = context.ReadValue<Vector2>();
    }

    private IEnumerator StartDrawing(InputAction action) {
        while (action.ReadValue<float>() != 0) {
            if (InsideImage(_mousePosition)) {
                Vector2 localPoint = new Vector2();
                if (RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, _mousePosition, null, out localPoint)) {
                    SoundManager.PlayLoopSound(SoundManager.Sound.clean);
                    Draw(localPoint);
                }
            }
            yield return _waitForFixedUpdate;
        }
        SoundManager.StopLoopSound();

        CalculatePercent();
    }

    private void Draw(Vector2 localPoint) {
        Vector2 uv = new Vector2(
            (localPoint.x + _rectTransform.rect.width * 0.5f) / _rectTransform.rect.width,
            (localPoint.y + _rectTransform.rect.height * 0.5f) / _rectTransform.rect.height
        );

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

    private void CalculatePercent() {
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
        float precent = (dirtyImg / _fullDirtyImg) * 100f;

        Debug.Log(precent);

        if (precent <= _requaredPercent) {
            Complete();
        }
    }

    private void Complete() {
        _isFinished = true;
        OnMinigameEnd?.Invoke();
    }

    public void OnRMB(InputAction.CallbackContext context) {
        Reset();
    }

    public void Reset() {
        Graphics.CopyTexture(_tmpT, _texture);
    }

    // восстанавливает текстуру при завершении
    void OnApplicationQuit() {
        Reset();
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
