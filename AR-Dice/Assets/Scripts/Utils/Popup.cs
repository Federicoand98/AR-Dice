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

        _textMeshPro.SetText("");

        LeanTween.alphaCanvas(_canvasGroup, 0f, 0f);
    }

    void Update() {
        
    }

    public void ShowPopup(string text) {
        _textMeshPro.SetText(text);
        LeanTween.alphaCanvas(_canvasGroup, 0.8f, 1f);
        StartCoroutine(DissolvePopup());
    }

    private IEnumerator DissolvePopup() {
        yield return new WaitForSeconds(3);
        LeanTween.alphaCanvas(_canvasGroup, 0f, 1f);
    }
}
