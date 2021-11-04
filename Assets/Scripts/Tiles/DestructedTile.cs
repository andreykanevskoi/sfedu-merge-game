using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructedTile : MonoBehaviour
{
    [SerializeField, Range(0.1f, 10f)] private float _fadeSpeed = 1f;

    private SpriteRenderer _spriteRenderer;
    public Sprite Sprite { 
        get => _spriteRenderer.sprite; 
        set => _spriteRenderer.sprite = value;
    }

    private float _fade = 1f;

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void Update() {
        _fade -= Time.deltaTime * _fadeSpeed;
        if (_fade <= 0f) {
            _fade = 0f;
        }
        _spriteRenderer.material.SetFloat("_Fade", _fade);

        if (_fade <= 0f) {
            Destroy(gameObject);
        }
    }
}
