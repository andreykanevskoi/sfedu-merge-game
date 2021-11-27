using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class LevelCompleteWindow : MonoBehaviour {
    [SerializeField] private GameObject _mergeStatisticPanel;
    [SerializeField] private GameObject _digStatisticPanel;
    [SerializeField] private Button _toCampButton;

    [SerializeField] private GameObject _statisticElementPrefab;

    private MergeStatisticCollector _mergeStatistic;
    private TileStatisticCollector _tileStatistic;

    public void Init(MergeStatisticCollector mergeStatistic, TileStatisticCollector tileStatistic, Action action) {
        _mergeStatistic = mergeStatistic;
        _tileStatistic = tileStatistic;

        _toCampButton.onClick.AddListener(() => {
            action?.Invoke();
        });

        StartCoroutine(FillStatistic());
    }

    private IEnumerator FillStatistic() {
        // Ширина элемента
        float imageWidth = _statisticElementPrefab.GetComponent<RectTransform>().rect.width;

        int totalElements = 0;
        foreach (var count in _mergeStatistic.mergeCounter.Values) {
            totalElements += count;
        }

        if (totalElements > 0) {
            float megrePanelwidth = _mergeStatisticPanel.GetComponent<RectTransform>().rect.width;
            float totalWidth = totalElements * imageWidth;
            if (totalWidth > megrePanelwidth) {
                float spacing = totalWidth / megrePanelwidth;
                _mergeStatisticPanel.GetComponent<HorizontalLayoutGroup>().spacing = -spacing;
            }

            float sleepTime = 1 / totalElements;
            foreach (var element in _mergeStatistic.mergeCounter.Keys) {
                int count = _mergeStatistic.mergeCounter[element];
                for (int i = 0; i < count; i++) {
                    Image image = Instantiate(_statisticElementPrefab, _mergeStatisticPanel.transform).GetComponent<Image>();
                    image.sprite = element.GetComponent<SpriteRenderer>().sprite;
                    yield return new WaitForSeconds(sleepTime);
                }
            }
        }

        totalElements = 0;
        foreach (var count in _tileStatistic.tileeCounter.Values) {
            totalElements += count;
        }

        if (totalElements > 0) {
            float tilePanelwidth = _mergeStatisticPanel.GetComponent<RectTransform>().rect.width;
            float totalWidth = totalElements * imageWidth;
            if (totalWidth > tilePanelwidth) {
                float spacing = (totalWidth - tilePanelwidth) / (totalElements - 2);
                _digStatisticPanel.GetComponent<HorizontalLayoutGroup>().spacing = -spacing;
                Debug.Log(spacing);
            }

            float sleepTime = 1 / totalElements;
            foreach (var element in _tileStatistic.tileeCounter.Keys) {
                int count = _tileStatistic.tileeCounter[element];
                for (int i = 0; i < count; i++) {
                    Image image = Instantiate(_statisticElementPrefab, _digStatisticPanel.transform).GetComponent<Image>();
                    image.sprite = element.sprite;
                    yield return new WaitForSeconds(sleepTime);
                }
            }
        }
    }
}
