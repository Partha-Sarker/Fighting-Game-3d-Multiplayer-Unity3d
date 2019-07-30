using UnityEngine;
using System.Collections;

namespace RPGCharacterAnims{
	
	public enum Weapon{
		UNARMED = 0,
		TWOHANDSWORD = 1,
		TWOHANDSPEAR = 2,
		TWOHANDAXE = 3,
		TWOHANDBOW = 4,
		TWOHANDCROSSBOW = 5,
		STAFF = 6,
		ARMED = 7,
		RELAX = 8,
		RIFLE = 9,
		SHIELD = 11,
		ARMEDSHIELD = 12
	}
		
	public class RPGCharacterWeaponControllerFREE : MonoBehaviour{
		RPGCharacterControllerFREE rpgCharacterController;
		private Animator animator;
		
		//Weapon Parameters.
		[HideInInspector] public int leftWeapon = 0;
		[HideInInspector] public bool isSwitchingFinished = true;
		[HideInInspector] public bool isWeaponSwitching = false;
		[HideInInspector] public bool instantWeaponSwitch;
		
		//Weapon Model.
		public GameObject twoHandSword;

		void Awake(){
			rpgCharacterController = GetComponent<RPGCharacterControllerFREE>();
			//Find the Animator component.
			animator = GetComponentInChildren<Animator>();
			StartCoroutine(_HideAllWeapons(false, false));
		}
			
		//weaponNumber 0 = Unarmed
		//weaponNumber 1 = 2H Sword
		public IEnumerator _SwitchWeapon(int weaponNumber){
//			Debug.Log("Switch Weapon: " + weaponNumber);
			if(instantWeaponSwitch){
				StartCoroutine(_InstantWeaponSwitch(weaponNumber));
				yield break;
			}
			isSwitchingFinished = false;
			//Reset for animation events.
			isWeaponSwitching = true;
			//If is Unarmed/Relax.
			if(IsNoWeapon(animator.GetInteger("Weapon"))){
				//Switch to Relax.
				if(weaponNumber == -1){
					StartCoroutine(_SheathWeapon(0, -1));
				}
				//Switch to Unarmed.
				else{
					StartCoroutine(_UnSheathWeapon(weaponNumber));
				}
			}
			//Character has 2handed weapon.
			else if(Is2HandedWeapon(animator.GetInteger("Weapon"))){
				StartCoroutine(_SheathWeapon(leftWeapon, weaponNumber));
				yield return new WaitForSeconds(1.2f);
				//Switching to weapon.
				if(weaponNumber > 0){
					StartCoroutine(_UnSheathWeapon(weaponNumber));
				}
			}
			yield return null;
		}


		public IEnumerator _UnSheathWeapon(int weaponNumber){
			//Switching to 2handed weapon.
			if(Is2HandedWeapon(weaponNumber)){
				//Switching from 2handed weapon.
				if(Is2HandedWeapon(animator.GetInteger("Weapon"))){
					DoWeaponSwitch(0, weaponNumber, weaponNumber, -1, false);
					yield return new WaitForSeconds(0.75f);
					SetAnimator(weaponNumber, -2, animator.GetInteger("Weapon"), -1, -1);
				}
				else{
					DoWeaponSwitch(animator.GetInteger("Weapon"), weaponNumber, weaponNumber, -1, false);
					yield return new WaitForSeconds(0.75f);
					SetAnimator(weaponNumber, -2, weaponNumber, -1, -1);
				}
			}
			rpgCharacterController.SetWeaponState(weaponNumber);
			yield return null;
		}

		public IEnumerator _SheathWeapon(int weaponNumber, int weaponTo){
			//Switching to Unarmed or Relaxed.
			if(weaponTo < 1){
				//Have at least 1 weapon.
				if(leftWeapon != 0){
					//Sheath 2handed weapon.
					DoWeaponSwitch(weaponTo, weaponNumber, animator.GetInteger("Weapon"), -1, true);
					yield return new WaitForSeconds(0.5f);
					SetAnimator(weaponTo, -2, 0, 0, -1);
				}
			}
			//Switching to 2handed weapon.
			else if(Is2HandedWeapon(weaponTo)){
				//Switching from 1handed weapons.
				if(animator.GetInteger("Weapon") == 7){
					DoWeaponSwitch(0, weaponNumber, 7, -1, true);
					yield return new WaitForSeconds(0.5f);
				}
				//Switching from 2handed weapons.
				else{
					DoWeaponSwitch(0, weaponNumber, animator.GetInteger("Weapon"), -1, true);
					yield return new WaitForSeconds(0.5f);
					SetAnimator(weaponNumber, -2, weaponNumber, 0, -1);
				}
			}
			rpgCharacterController.SetWeaponState(weaponTo);
			yield return null;
		}

		IEnumerator _InstantWeaponSwitch(int weaponNumber){
			animator.SetInteger("Weapon", -2);
			yield return new WaitForEndOfFrame();
			animator.SetTrigger("InstantSwitchTrigger");
			animator.SetInteger("Weapon", weaponNumber);
			StartCoroutine(_HideAllWeapons(false, false));
			StartCoroutine(_WeaponVisibility(weaponNumber, true, false));
			rpgCharacterController.SetWeaponState(weaponNumber);
		}

		void DoWeaponSwitch(int weaponSwitch, int weaponVisibility, int weaponNumber, int leftRight, bool sheath){
			//Go to Null state and wait for animator.
			animator.SetInteger("Weapon", -2);
			while(animator.isActiveAndEnabled && animator.GetInteger("Weapon") != -2){
			}
			//Lock character for switch unless has moving sheath/unsheath anims.
			if(weaponSwitch < 1){
				if(Is2HandedWeapon(weaponNumber)){
					rpgCharacterController.Lock(true, true, true, 0f, 1f);
				}
			}
			//Set weaponSwitch if applicable.
			if(weaponSwitch != -2){
				animator.SetInteger("WeaponSwitch", weaponSwitch);
			}
			animator.SetInteger("Weapon", weaponNumber);
			//Set animator trigger.
			if(sheath){
				animator.SetTrigger("WeaponSheathTrigger");
				StartCoroutine(_WeaponVisibility(weaponVisibility, false, false));
				//If using IKHands, trigger IK blend.
				if(rpgCharacterController.ikHands != null){
					StartCoroutine(rpgCharacterController.ikHands._BlendIK(false, 0f, 0.2f, weaponVisibility));
				}
			}
			else{
				animator.SetTrigger("WeaponUnsheathTrigger");
				StartCoroutine(_WeaponVisibility(weaponVisibility, true, false));
				//If using IKHands, trigger IK blend.
				if(rpgCharacterController.ikHands != null){
					StartCoroutine(rpgCharacterController.ikHands._BlendIK(true, 0.5f, 1, weaponVisibility));
				}
			}
		}

		/// <summary>
		/// Controller weapon switching.
		/// </summary>
		public void SwitchWeaponTwoHand(int upDown){
			if(instantWeaponSwitch){
				StartCoroutine(_HideAllWeapons(false, false));
			}
			isSwitchingFinished = false;
			if(upDown == 0){
				StartCoroutine(_SwitchWeapon(0));
			}
			if(upDown == 1){
				StartCoroutine(_SwitchWeapon(1));
			}
		}

		//For Animation Event.
		public void WeaponSwitch(){
			if(isWeaponSwitching){
				isWeaponSwitching = false;
			}
		}

		public IEnumerator _HideAllWeapons(bool timed, bool resetToUnarmed){
			if(timed){
				while(!isWeaponSwitching && instantWeaponSwitch){
					yield return null;
				}
			}
			//Reset to Unarmed.
			if(resetToUnarmed){
				animator.SetInteger("Weapon", 0);
				rpgCharacterController.weapon = Weapon.UNARMED;
				StartCoroutine(rpgCharacterController.rpgCharacterWeaponController._WeaponVisibility(rpgCharacterController.rpgCharacterWeaponController.leftWeapon, false, true));
				animator.SetInteger("RightWeapon", 0);
				animator.SetInteger("LeftWeapon", 0);
				animator.SetInteger("LeftRight", 0);
			}
			if(twoHandSword != null){
				twoHandSword.SetActive(false);
			}
		}
	
		/// <summary>
		/// Sets the animator state.
		/// </summary>
		/// <param name="weapon">Weapon.</param>
		/// <param name="weaponSwitch">Weapon switch.</param>
		/// <param name="Lweapon">Lweapon.</param>
		/// <param name="Rweapon">Rweapon.</param>
		/// <param name="weaponSide">Weapon side.</param>
		void SetAnimator(int weapon, int weaponSwitch, int Lweapon, int Rweapon, int weaponSide){
			Debug.Log("SETANIMATOR: Weapon:" + weapon + " Weaponswitch:" + weaponSwitch + " Lweapon:" + Lweapon + " Rweapon:" + Rweapon + " Weaponside:" + weaponSide);
			//Set Weapon if applicable.
			if(weapon != -2){
				animator.SetInteger("Weapon", weapon);
			}
			//Set WeaponSwitch if applicable.
			if(weaponSwitch != -2){
				animator.SetInteger("WeaponSwitch", weaponSwitch);
			}
			//Set left weapon if applicable.
			if(Lweapon != -1){
				leftWeapon = Lweapon;
				animator.SetInteger("LeftWeapon", Lweapon);
			}
			//Set weapon side if applicable.
			if(weaponSide != -1){
				animator.SetInteger("LeftRight", weaponSide);
			}
			rpgCharacterController.SetWeaponState(weapon);
		}

		public IEnumerator _WeaponVisibility(int weaponNumber, bool visible, bool dual){
//			Debug.Log("WeaponVisiblity: " + weaponNumber + "  Visible: " + visible + "  dual: " + dual);
			while(isWeaponSwitching){
				yield return null;
			}
			if(weaponNumber == 1){
				twoHandSword.SetActive(visible);
			}
			yield return null;
		}
		
		public bool IsNoWeapon(int weaponNumber){
			if(weaponNumber < 1){
				return true;
			}
			else{
				return false;
			}
		}
		
		public bool Is2HandedWeapon(int weaponNumber){
			if((weaponNumber > 0 && weaponNumber < 7) || weaponNumber == 18 || weaponNumber == 20){
				return true;
			}
			else{
				return false;
			}
		}
	}
}