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

    public void MooMooMeadows()
    {   
            SceneManager.LoadScene("MooMooMeadows");   
    }
    public void ToadHarbor()
    {
        SceneManager.LoadScene("Toad Harbor");
    }
    public void MarioCircuit()
    {
        SceneManager.LoadScene("Mario Circuit");
    }
    public void RainbowRoad()
    {
        SceneManager.LoadScene("Rainbow Road");
    }
    public void WaterPark()
    {
        SceneManager.LoadScene("Water Park");
    }
}
