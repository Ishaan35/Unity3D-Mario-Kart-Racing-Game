using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class ScoreCount : MonoBehaviour
{
    public int COINCOUNT;
    public Text Coin_Text;
    public Text Coin_Text2;

    public Color maxCoinColor;
    public Color regCoinColor;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(COINCOUNT < 10)
        {
            Coin_Text.text = "0" + COINCOUNT;
            Coin_Text2.text = "0" + COINCOUNT;
            Coin_Text2.color = regCoinColor;

        }
        if (COINCOUNT == 10)
        {
            Coin_Text.text = COINCOUNT + "";
            Coin_Text2.text = COINCOUNT + "";
            Coin_Text2.color = maxCoinColor;
        }

        if (COINCOUNT > 10)
        {
            COINCOUNT = 10;
        }
    }
}
