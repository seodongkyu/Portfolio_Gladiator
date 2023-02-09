using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingWindow : MonoBehaviour
{
    public Slider _progressBar;
    [SerializeField] Image _progressImg;

    public void SetLoadingProgress(float value)
    {
        _progressBar.value = value;
        _progressImg.fillAmount = value;
    }
    public void Close()
    {
        SceneController._instance._isLoading = false;
        Destroy(gameObject);
    }
}
