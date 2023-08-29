using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDistributionManager : MonoBehaviour
{

    /*Player: Bullet: 0, Banana: 1, Greenshell: 2, Coin: 3, Redshell: 4, TripBananas: 5, mushroom: 6, 
        tripGreenShell: 7, TripMushroom: 8, TripRedSHell: 9, Golden mushroom: 10, bobomb: 11, blueshell: 12, star: 13
    */





    /*Opponent: Greenshell: 0, Redhshell: 1, Banana: 2, Blueshell: 3, Bobomb: 4, Bullet: 5, coin: 6, 
        tripGreenShell: 7, TripRedSHell: 8, TripMushrooms: 9, TripBananas: 10, Golden Mushroom: 11, Mushroom: 12, star: 13
    */

    int position;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        position = GetComponent<LapCounter>().Position;
    }

    public int getItemNumber()
    {
        if(gameObject.tag == "Player")
        {
            if (position == 1)
            {
                int range = Random.Range(0, 6);

                if(range < 2) { return 3; }//coin
                else if(range < 4) { return 1; } //banana
                else { return 2; } //green shell
            }
            if(position == 2)
            {
                int range = Random.Range(0, 5);

                if (range < 1) { return 1; }//banana
                else if (range < 3) { return 2; }//green shell
                else { return 4; }//red shell
            }
            if (position == 3)
            {
                int range = Random.Range(0, 6);

                if (range < 2) { return 6; }//mushroom
                else if (range < 5) { return 4; }//red shell
                else { return 11; } //bobomb
            }
            if(position == 4)
            {
                int range = Random.Range(0, 6);

                if (range < 1) { return 4; }//red shell
                else if(range < 2) { return 11; } //bobomb
                else if (range < 3) { return 6; }//mushroom
                else if (range < 4) { return 7; }//trip green
                else { return 5; } //triple banana
            }
            if(position == 5)
            {
                int range = Random.Range(0, 6);

                if (range < 1) { return 7; } //triple banana
                else if (range < 2) { return 6; } //mushroom
                else if (range < 4) { return 9; } //triple red shell
                else { return 8; } //triple mushroom
            }
            if(position == 6)
            {
                int range = Random.Range(0, 7);

                if (range < 2) { return 8; } //triple mushroom
                else if (range < 5) { return 9; } //triple red shell
                else{ return 12; }//blue shell
            }
            if(position == 7)
            {
                int range = Random.Range(0, 8);

                if (range < 2) { return 12; } //blue shell
                else if (range < 5) { return 10; } //golden mushroom
                else { return 13; } //star
            }
            else //8th place
            {
                int range = Random.Range(0, 8);

                if (range < 2) { return 13; } //star
                else if (range < 5) { return 10; } //golden mushroom
                else { return 0; } //bullet
            }
        }
        
        else //opponent
        {
            if(position == 1)
            {
                int range = Random.Range(0, 5);

                if(range < 2) { return 6; }//coin
                else if(range < 4) { return 2; } //banana
                else { return 0; } //green shell
            }
            if(position == 2)
            {
                int range = Random.Range(0, 5);

                if (range < 1) { return 2; }//banana
                else if (range < 3) { return 0; }//green shell
                else { return 1; }//red shell
            }
            if (position == 3)
            {
                int range = Random.Range(0, 5);

                if (range < 2) { return 12; }//mushroom
                else if (range < 4) { return 1; }//red shell
                else { return 4; } //bobomb
            }
            if(position == 4)
            {
                int range = Random.Range(0, 6);

                if (range < 1) { return 1; }//red shell
                else if(range < 2) { return 4; } //bobomb
                else if (range < 3) { return 12; }//mushroom
                else if (range < 4) { return 7; }//trip green
                else { return 10; } //triple banana
            }
            if(position == 5)
            {
                int range = Random.Range(0, 6);

                if (range < 1) { return 7; } //triple banana
                else if (range < 2) { return 12; } //mushroom
                else if (range < 4) { return 8; } //triple red shell
                else { return 9; } //triple mushroom
            }
            if(position == 6)
            {
                int range = Random.Range(0, 7);

                if (range < 3) { return 9; } //triple mushroom
                else if (range < 5) { return 8; } //triple red shell
                else{
                    if (GameObject.Find("RaceManager").GetComponent<RACE_MANAGER>().currentBlueShellCount == 0)
                    {
                        GameObject.Find("RaceManager").GetComponent<RACE_MANAGER>().currentBlueShellCount = 1;
                        StartCoroutine(resetBlueShell());
                        return 3;
                    }
                    else
                        return 8;
                }//blue shell or triple red
            }
            if(position == 7)
            {
                int range = Random.Range(0, 8);

                if (range < 3) {
                    if (GameObject.Find("RaceManager").GetComponent<RACE_MANAGER>().currentBlueShellCount == 0)
                    {
                        GameObject.Find("RaceManager").GetComponent<RACE_MANAGER>().currentBlueShellCount = 1;
                        StartCoroutine(resetBlueShell());
                        return 3;
                    }
                    else
                        return 13;
                } //blue shell or star
                else if (range < 5) { return 11; } //golden mushroom
                else { return 13; } //star
            }
            else //8th place
            {
                int range = Random.Range(0, 8);

                if (range < 2) { return 13; } //star
                else if (range < 5) { return 11; } //golden mushroom
                else { return 5; } //bullet
            }
        }
    }

    IEnumerator resetBlueShell()
    {
        yield return new WaitForSeconds(10);
        GameObject.Find("RaceManager").GetComponent<RACE_MANAGER>().currentBlueShellCount = 0;
    }
}
