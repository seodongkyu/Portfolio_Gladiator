using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using DefineUtility;


public class SceneController : TSingleton<SceneController>
{
    public bool _isLoading = false;
    private void Awake()
    {
        base.Init();
    }

    public void LoadScene(eSceneType type)
    {
        StartCoroutine(LoadingScene(type));
    }

    IEnumerator LoadingScene(eSceneType type)
    {
        _isLoading = true;
        GameObject go = Instantiate(ResourcePoolManager._instance._loadingWindow,transform);
        LoadingWindow loadingWnd = go.GetComponent<LoadingWindow>();
        AsyncOperation aOper = SceneManager.LoadSceneAsync(type.ToString());
        aOper.allowSceneActivation = false;
        float temp = 0;
        while (!aOper.isDone)
        {
            yield return null;
            temp += Time.deltaTime;
            Debug.Log(aOper.progress);
            if(aOper.progress >= 0.9f)
            {
                loadingWnd._progressBar.value = Mathf.Lerp(loadingWnd._progressBar.value, 1f, temp);
                if (loadingWnd._progressBar.value >= 1.0f)
                    aOper.allowSceneActivation = true;
            }
            else
            {
                loadingWnd._progressBar.value = Mathf.Lerp(aOper.progress, 1f, temp);
                if (loadingWnd._progressBar.value >= aOper.progress)
                    temp = 0f;
            }
        }
        yield return new WaitForSeconds(0.5f);
        loadingWnd.Close();
    }
}
