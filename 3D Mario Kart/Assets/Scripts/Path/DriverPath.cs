using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DriverPath : MonoBehaviour
{
    public Color line_color;
    private List<Transform> nodes = new List<Transform>(); //kind of like a dynamic array
     
    private void OnDrawGizmos() //draws something at transforms location
    {
        Gizmos.color = line_color;

        Transform[] pathTransforms = GetComponentsInChildren<Transform>(); //this array contains transforms of child objects, but including ourselves
        nodes = new List<Transform>();

        for(int i = 0; i < pathTransforms.Length; i++)
        {
            if(pathTransforms[i] != transform) //make sure the transform is not our own transform, but child object transform
            {
                nodes.Add(pathTransforms[i]);
            }
        }

        //draw a line between nodes. use count an not length because nodes is a list. First node should make a line with last node
        for(int i = 0; i < nodes.Count; i++)
        {
            Vector3 previousNode = Vector3.zero; //default before assigning proper values
            Vector3 currentNode = nodes[i].position;
            if(i > 0)
            {
                previousNode = nodes[i - 1].position;
            }
            else if(i == 0 && nodes.Count > 1) //if we are at first node and there is more than 1 node in the list
            {
                previousNode = nodes[nodes.Count - 1].position; //last node
            }
            Gizmos.DrawLine(previousNode, currentNode);
        }
    }
}
