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

    private StatisticCollector _mergeStatistic;
    private StatisticCollector _tileStatistic;

    [SerializeField, Range(1f, 10f)] private float _statisticAnimationTime = 5f;

    delegate Sprite GetSprite<T>(T element);

    public void Init(StatisticCollector mergeStatistic, StatisticCollector tileStatistic, Action action) {
        _mergeStatistic = mergeStatistic;
        _tileStatistic = tileStatistic;

        _toCampButton.onClick.AddListener(() => {
            action?.Invoke();
        });

        StartCoroutine(Apeare());
    }

    private IEnumerator Apeare() {
        yield return new WaitForSeconds(0.15f);
        StartCoroutine(FillStatistic());
    }

    private IEnumerator FillStatistic() {
        // Ширина элемента
        float imageWidth = _statisticElementPrefab.GetComponent<RectTransform>().rect.width;

        yield return StartCoroutine(FillPannel(_mergeStatisticPanel, _mergeStatistic, imageWidth));
        yield return StartCoroutine(FillPannel(_digStatisticPanel, _tileStatistic, imageWidth));
    }

    private IEnumerator FillPannel(GameObject panel, StatisticCollector collector, float elementWidth) {
        int totalElements = collector.CountElement();
        
        if (totalElements < 0) yield break;

        float panelWidth = panel.GetComponent<RectTransform>().rect.width;
        float spacing = CalculateSpacing(panelWidth, totalElements * elementWidth, totalElements);
        panel.GetComponent<HorizontalLayoutGroup>().spacing = -spacing;

        float playTime = _statisticAnimationTime / totalElements;
        var waitTime = new WaitForSeconds(playTime);

        foreach (var pair in collector.statistic) {
            for (int i = 0; i < pair.Value; i++) {
                Image image = Instantiate(_statisticElementPrefab, panel.transform).GetComponent<Image>();
                image.sprite = pair.Key;

                yield return waitTime;
            }
        }
    }

    private float CalculateSpacing(float panelWidth, float width, int totalElements) {
        if (width > panelWidth) {
            float spacing = (width - panelWidth) / (totalElements - 2);
            return spacing;
        }
        return 0f;
    }
}
