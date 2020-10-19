using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class LoadSceneTemp : MonoBehaviour
{
    
    Resolution[] res;
    public Dropdown dropdown;
    // Start is called before the first frame update
    void Start()
    {
        res = Screen.resolutions;
        dropdown.ClearOptions();


        List<string> resolutionStrings = new List<string>();

        int currentResIndex = 0;
        for(int i = 0; i < res.Length; i++)
        {
            string option = res[i].width + " x " + res[i].height;
            resolutionStrings.Add(option);

            if(res[i].height == Screen.height && res[i].width == Screen.width)
            {
                currentResIndex = i;
            }
        }
        dropdown.AddOptions(resolutionStrings);

        dropdown.value = currentResIndex;
        dropdown.RefreshShownValue();






    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene("Track2");
        }

        Debug.Log(QualitySettings.vSyncCount);

    }

    public void setResolution(int resIndex)
    {
        Resolution selectedResolution = res[resIndex];
        Screen.SetResolution(selectedResolution.width, selectedResolution.height, Screen.fullScreen);
    }

    public void setVSync(int index)
    {
        QualitySettings.vSyncCount = index;

    }
}
