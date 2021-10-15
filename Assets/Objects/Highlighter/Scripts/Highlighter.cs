using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Highlighter : MonoBehaviour
{
    [SerializeField, Range(.1f, 5f)]
    private float amplitude = 0.5f;
    
    [SerializeField, Range(.1f, 5f)]
    private float frequency = 1f;

    public void Hide() {
        gameObject.SetActive(false);
    }

    public void Show() {
        gameObject.SetActive(true);
    }

    public void SetPosition(Vector3 position) {
        transform.localPosition = position;
    }

    private void Awake() {
        Hide();
    }

    private void Update() {
        Vector3 tempPos = transform.position;
        tempPos.y += Mathf.Abs(Mathf.Sin(Time.fixedTime * Mathf.PI * frequency) * amplitude);

        transform.position = tempPos;
    }
}
