using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Popup : MonoBehaviour {

    private CanvasGroup _canvasGroup;
    private TextMeshProUGUI _textMeshPro;
    private bool isEnabled = false;

    void Start() {
        _canvasGroup = GetComponent<CanvasGroup>();
        _textMeshPro = gameObject.transform.GetChild(0).GetComponent<TextMeshProUGUI>();

        LeanTween.alphaCanvas(_canvasGroup, 0f, 0f);
    }

    void Update() {
        
    }

    public void ShowPopup(string text) {
        Debug.Log("\n----------\nstring: " + text + "\n-----------\n");
        _textMeshPro.SetText(text);
        Debug.Log("\n----------\ntmp: " + _textMeshPro.text + "\n-----------\n");
        LeanTween.alphaCanvas(_canvasGroup, 0.5f, 1f);
        StartCoroutine(DissolvePopup());
    }

    private IEnumerator DissolvePopup() {
        yield return new WaitForSeconds(3);
        LeanTween.alphaCanvas(_canvasGroup, 0f, 1f);
        _textMeshPro.SetText("");
    }
}
