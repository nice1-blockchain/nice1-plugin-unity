using UnityEngine;

public class UIPanel : MonoBehaviour
{
    private CanvasGroup _panelCanvas;

    private bool _isShowing = false;

    private void Awake()
    {
        _panelCanvas = GetComponent<CanvasGroup>();
    }

    public void ShowPanel()
    {
        if (!_isShowing)
        {
            _isShowing = true;

            _panelCanvas.blocksRaycasts = true;

            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f);

        }
    }

    public void HidePanel()
    {
        if (_isShowing)
        {
            _isShowing = false;

            _panelCanvas.blocksRaycasts = false;

            transform.localScale = new Vector3(0.0f, 0.0f, 1.0f);

        }
    }
}
