using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    private string player;
    
    public void HideSword()
    {
        player = "/" + this.name;
        GameObject sword = GameObject.Find
                (player + "/Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm/B_R_Forearm/B_R_Hand/2Hand-Sword");
        sword.GetComponent<MeshRenderer>().enabled = false;
    }

    public void ShowSword()
    {
        player = "/" + this.name;
        GameObject sword = GameObject.Find
                (player + "/Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm/B_R_Forearm/B_R_Hand/2Hand-Sword");
        sword.GetComponent<MeshRenderer>().enabled = true;
    }
}
