using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Camerafollow : MonoBehaviour
{
    public Transform player;
    public float offset; // we need to change this to a lesser value whenever there is antigravity AND the race is completed

    public Vector3 boost_pos = new Vector3(0, 1.24f, -6.5f);
    public Vector3 orig_pos;
    public Vector3 bulletPos;
    public float antiGravityPosY;

    private Player playerscript;
    private ItemManager playerscriptItem;

    [HideInInspector]
    public float antiGravityTimeAgo = 0;
    [HideInInspector]
    public bool rotateCamAntiGravity = false;
    [HideInInspector]
    public float rotateAmountAntigravityX = 0;
    public float rotateAmountAntigravityZ = 0;

    [HideInInspector]
    public float glideAngleZ;
    public float glideAngleX = 0;

    public float raceEndFOV = 0;
    float none = 0;


    // Start is called before the first frame update
    void Start()
    {
        playerscript = player.GetComponent<Player>();
        playerscriptItem = player.GetComponent<ItemManager>();
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (!playerscript.GetComponent<OutOfBounds>().PlayerBeingMoved)
        {
            antiGravityTimeAgo += Time.deltaTime;

            //anti gravity vs regular position
            if (playerscript.antiGravity || antiGravityTimeAgo < 3 || playerscript.GLIDER_FLY)
            {
                Ray upRay = new Ray(player.position, player.up);

                Vector3 upDist;
                if (!RACE_MANAGER.RACE_COMPLETED)
                {
                    upDist = upRay.GetPoint(offset);
                }
                else
                {
                    upDist = upRay.GetPoint(offset-0.8f);
                }
                transform.position = upDist;
            }
            else
            {

                {
                    transform.position = player.position + new Vector3(0, offset, 0);
                }
            }

            if (!playerscript.GLIDER_FLY && !playerscript.trickBoostPending && RACE_MANAGER.RACE_STARTED)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, 3f * Time.deltaTime);
            }
            else
            {

                    float angle = transform.localEulerAngles.x;
                    angle = (angle > 180) ? angle - 360 : angle;

                if (playerscript.GLIDER_FLY)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(glideAngleX, player.eulerAngles.y, glideAngleZ), 1 * Time.deltaTime);
                }
                else if(playerscript.trickBoostPending && !playerscript.antiGravity)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, player.eulerAngles.y, 0), 3 * Time.deltaTime);
                }



            }

            if (playerscript.JUMP_PANEL)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, player.rotation, 0.4f * Time.deltaTime);
            }

            //rotation antigravity
            if (playerscript.antiGravity)
            {
                if (rotateCamAntiGravity)
                {
                    transform.GetChild(0).localRotation = Quaternion.Slerp(transform.GetChild(0).localRotation, Quaternion.Euler(rotateAmountAntigravityX, transform.GetChild(0).localEulerAngles.y, rotateAmountAntigravityZ), 3 * Time.deltaTime);
                }
                else
                {
                    transform.GetChild(0).localRotation = Quaternion.Slerp(transform.GetChild(0).localRotation, Quaternion.Euler(2, transform.GetChild(0).localEulerAngles.y, 0), 3 * Time.deltaTime);
                }
            }
            else
            {
                transform.GetChild(0).localRotation = Quaternion.Slerp(transform.GetChild(0).localRotation, Quaternion.Euler(2, transform.GetChild(0).localEulerAngles.y, 0), 3 * Time.deltaTime);
            }


            if ((playerscript.Boost || playerscript.trickBoostPending) && !playerscriptItem.isBullet && !RACE_MANAGER.RACE_COMPLETED)
            {
                if (!rotateCamAntiGravity)
                    transform.GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(0).localPosition, boost_pos, 4f * Time.deltaTime);
                else
                {
                    transform.GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(0).localPosition, new Vector3(boost_pos.x, antiGravityPosY, boost_pos.z), 4f * Time.deltaTime);
                }
            }
            if (!playerscript.Boost && !playerscriptItem.isBullet)
            {
                if (!rotateCamAntiGravity)
                    transform.GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(0).localPosition, orig_pos, 4f * Time.deltaTime);
                else
                {
                    transform.GetChild(0).localPosition = Vector3.Lerp(transform.GetChild(0).localPosition, new Vector3(orig_pos.x, antiGravityPosY, orig_pos.z), 4f * Time.deltaTime);
                }
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

        if (RACE_MANAGER.RACE_COMPLETED)
        {
            transform.localScale = new Vector3(1, 1, 1);
            if(raceEndFOV > 1)
            {
                transform.GetChild(0).GetChild(0).GetComponent<Camera>().fieldOfView = raceEndFOV;
            }
        }


    }
}
