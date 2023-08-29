using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResultsUI : MonoBehaviour
{
    public Sprite[] playerIcons;
    public Material playerGradient;

    public Color white;
    public Color black;
    public Color grey;

    private int playerResult;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
  
    }

    public void createResults(List<LapCounter> players)
    {
        for(int i = 0; i < players.Count; i++)
        {
            if(players[i].gameObject.name == "Mario")
            {
                transform.GetChild(i).GetChild(2).gameObject.GetComponent<Text>().text = "Mario";
                transform.GetChild(i).GetChild(1).gameObject.GetComponent<Image>().sprite = playerIcons[0];

                playerResult = i;

                
            }
            else if (players[i].gameObject.name == "Luigi")
            {
                transform.GetChild(i).GetChild(2).gameObject.GetComponent<Text>().text = "Luigi";
                transform.GetChild(i).GetChild(1).gameObject.GetComponent<Image>().sprite = playerIcons[1];
            }
            else if (players[i].gameObject.name == "Toad")
            {
                transform.GetChild(i).GetChild(2).gameObject.GetComponent<Text>().text = "Toad";
                transform.GetChild(i).GetChild(1).gameObject.GetComponent<Image>().sprite = playerIcons[2];
            }
            else if (players[i].gameObject.name == "Shyguy")
            {
                transform.GetChild(i).GetChild(2).gameObject.GetComponent<Text>().text = "Shy Guy";
                transform.GetChild(i).GetChild(1).gameObject.GetComponent<Image>().sprite = playerIcons[3];
            }
            else if(players[i].gameObject.name == "Waluigi")
            {
                transform.GetChild(i).GetChild(2).gameObject.GetComponent<Text>().text = "Waluigi";
                transform.GetChild(i).GetChild(1).gameObject.GetComponent<Image>().sprite = playerIcons[4];
            }
            else if (players[i].gameObject.name == "Lemmy")
            {
                transform.GetChild(i).GetChild(2).gameObject.GetComponent<Text>().text = "Lemmy";
                transform.GetChild(i).GetChild(1).gameObject.GetComponent<Image>().sprite = playerIcons[5];
            }
            else if (players[i].gameObject.name == "Baby Mario")
            {
                transform.GetChild(i).GetChild(2).gameObject.GetComponent<Text>().text = "Baby Mario";
                transform.GetChild(i).GetChild(1).gameObject.GetComponent<Image>().sprite = playerIcons[6];
            }
            else if (players[i].gameObject.name == "Peach")
            {
                transform.GetChild(i).GetChild(2).gameObject.GetComponent<Text>().text = "Peach";
                transform.GetChild(i).GetChild(1).gameObject.GetComponent<Image>().sprite = playerIcons[7];
            }


        }
        GetComponent<Animator>().SetBool("RaceEnd", true);
        StartCoroutine(yellowPlayerResult());
    }

    public IEnumerator yellowPlayerResult()
    {
        yield return new WaitForSeconds(1);

        Color color = new Color(1, 1, 1, 1);
        color.a = 1;
        transform.GetChild(playerResult).GetComponent<Image>().color = color;
        transform.GetChild(playerResult).GetComponent<UIGradient>().enabled = true;
    }
}
