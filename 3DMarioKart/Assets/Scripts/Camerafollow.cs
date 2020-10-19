using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camerafollow : MonoBehaviour
{
    public Transform player;
    public Vector3 offset;

    public Vector3 boost_pos = new Vector3(0, 1.24f, -6.5f);
    public Vector3 orig_pos;
    public Vector3 bulletPos;

    private Player playerscript;
    private ItemManager playerscriptItem;

    // Start is called before the first frame update
    void Start()
    {
        playerscript = player.GetComponent<Player>();
        playerscriptItem = player.GetComponent<ItemManager>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.position = player.position + offset;

        if (!playerscript.GLIDER_FLY)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, 3f * Time.deltaTime);
        }
        else
        {
            float angle = transform.localEulerAngles.x;
            angle = (angle > 180) ? angle - 360 : angle;

            transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.Euler(0, player.eulerAngles.y, 0), 3 * Time.deltaTime);

            
        }

        if (playerscript.JUMP_PANEL)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, 0.4f* Time.deltaTime);
        }






        if (playerscript.Boost && !playerscriptItem.isBullet && !RACE_MANAGER.RACE_COMPLETED)
        {
            transform.GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(0).localPosition, boost_pos, 4f * Time.deltaTime);
        }
        if(!playerscript.Boost && !playerscriptItem.isBullet)
        {
            transform.GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(0).localPosition, orig_pos, 3 * Time.deltaTime);
        }
        if (RACE_MANAGER.RACE_COMPLETED)
        {
            transform.GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(0).localPosition, orig_pos, 3 * Time.deltaTime);
        }
        if (playerscriptItem.isBullet && !RACE_MANAGER.RACE_COMPLETED)
        {
            transform.GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(0).localPosition, bulletPos, 6 * Time.deltaTime);
        }


    }
}
