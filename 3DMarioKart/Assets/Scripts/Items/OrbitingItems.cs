using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OrbitingItems : MonoBehaviour
{
    string whoHasItems;
    // Start is called before the first frame update
    void Start()
    {
        whoHasItems = transform.parent.parent.parent.parent.gameObject.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.gameObject.tag == "Opponent" && other.gameObject.name != whoHasItems)
        {
            OpponentItemManager itemManage = other.gameObject.GetComponent<OpponentItemManager>();
            //the person who has the items
            GameObject itemOwner = GameObject.Find(whoHasItems);
            if(itemOwner.tag == "Player")
            {
                if (gameObject.tag == "Shell")
                {
                    itemManage.hitByShell();
                }
                else
                {
                    itemManage.hitByBanana();
                }
                itemOwner.GetComponent<ItemManager>().tripleItemCount--;
                    int item_index = itemOwner.GetComponent<ItemManager>().item_index;
                    gameObject.SetActive(false); 

                    if(itemOwner.GetComponent<ItemManager>().tripleItemCount < 1)
                    {
                        itemOwner.GetComponent<ItemManager>().item_gameobjects[item_index].SetActive(false);
                        itemOwner.GetComponent<ItemManager>().item_gameobjects[item_index].transform.GetChild(0).gameObject.SetActive(true);
                        itemOwner.GetComponent<ItemManager>().item_gameobjects[item_index].transform.GetChild(1).gameObject.SetActive(true);
                        itemOwner.GetComponent<ItemManager>().item_gameobjects[item_index].transform.GetChild(2).gameObject.SetActive(true);
                        itemOwner.GetComponent<ItemManager>().used_Item_Done();
                    }
                
            }
            else
            {

            }
        }
    }


}
