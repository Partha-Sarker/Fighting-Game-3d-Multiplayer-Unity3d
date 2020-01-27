using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationEvent : MonoBehaviour
{
    public FistAttack leftAttack;
    public Collider leftAttackCollider;
    public FistAttack rightAttack;
    public Collider rightAttackCollider;
    public SwordAttack swordAttack;
    public Collider swordAttackCollider;
    private AudioManager audioManager;
    [SerializeField]
    private MeshRenderer sword;

    public void Start()
    {
        audioManager = GetComponent<AudioManager>();
    }

    public void HideSword()
    {
        audioManager.PlaySFX("Sheath");
        //player = "/" + this.name;
        //print(player);
        //GameObject sword = GameObject.Find
        //        (player + "/Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm/B_R_Forearm/B_R_Hand/2Hand-Sword");
        //sword.GetComponent<MeshRenderer>().enabled = false;
        sword.enabled = false;
        if (this.name == "local player")
        {
            swordAttack.enabled = false;
            swordAttackCollider.enabled = false;
            leftAttack.enabled = rightAttack.enabled = true;
            leftAttackCollider.enabled = rightAttackCollider.enabled = true;
        }
    }

    public void ShowSword()
    {
        audioManager.PlaySFX("Unsheath");
        //player = "/" + this.name;
        //print(player);
        //GameObject sword = GameObject.Find
        //        (player + "/Motion/B_Pelvis/B_Spine/B_Spine1/B_Spine2/B_R_Clavicle/B_R_UpperArm/B_R_Forearm/B_R_Hand/2Hand-Sword");
        //sword.GetComponent<MeshRenderer>().enabled = true;
        sword.enabled = true;
        if (this.name == "local player")
        {
            swordAttack.enabled = true;
            swordAttackCollider.enabled = true;
            leftAttack.enabled = rightAttack.enabled = false;
            leftAttackCollider.enabled = rightAttackCollider.enabled = false;
        }
    }

    public void PlayFootStep()
    {
        audioManager.PlaySFX("Footstep");
    }
}
