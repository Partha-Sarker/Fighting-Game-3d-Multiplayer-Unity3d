using UnityEngine;
using System.Collections;

namespace RPGCharacterAnims{

	[RequireComponent(typeof(RPGCharacterMovementControllerFREE))]
	[RequireComponent(typeof(RPGCharacterWeaponControllerFREE))]
	[RequireComponent(typeof(RPGCharacterInputControllerFREE))]
	public class RPGCharacterControllerFREE : MonoBehaviour{

		//Components.
		[HideInInspector]	public RPGCharacterMovementControllerFREE rpgCharacterMovementController;
		[HideInInspector]	public RPGCharacterWeaponControllerFREE rpgCharacterWeaponController;
		[HideInInspector]	public RPGCharacterInputControllerFREE rpgCharacterInputController;
		[HideInInspector] public Animator animator;
		[HideInInspector] public IKHandsFREE ikHands;
		public Weapon weapon = Weapon.UNARMED;
		public GameObject target;

		//Strafing/action.
		[HideInInspector] public bool isDead = false;
		[HideInInspector] public bool isBlocking = false;
		[HideInInspector] public bool isStrafing = false;
		[HideInInspector] public bool canAction = true;

		public float animationSpeed = 1;

		#region Initialization

		void Awake(){
			rpgCharacterMovementController = GetComponent<RPGCharacterMovementControllerFREE>();
			rpgCharacterWeaponController = GetComponent<RPGCharacterWeaponControllerFREE>();
			rpgCharacterInputController = GetComponent<RPGCharacterInputControllerFREE>();
			animator = GetComponentInChildren<Animator>();
			if(animator == null){
				Debug.LogError("ERROR: There is no animator for character.");
				Destroy(this);
			}
			if(target == null){
				Debug.LogError("ERROR: There is no target set for character.");
				Destroy(this);
			}
			ikHands = GetComponent<IKHandsFREE>();
			//Set for starting Unarmed state.
			weapon = Weapon.UNARMED;
			animator.SetInteger("Weapon", 0);
			animator.SetInteger("WeaponSwitch", -1);
		}

		void Start(){
			rpgCharacterMovementController.SwitchCollisionOn();
		}

		#endregion

		#region Updates

		void Update(){
			UpdateAnimationSpeed();
			if(rpgCharacterMovementController.MaintainingGround()){
				//Revive.
				if(rpgCharacterInputController.inputDeath){
					if(isDead){
						Revive();
					}
				}
				if(canAction){
					if(!isBlocking){
						Strafing();
						Rolling();
						//Hit.
						if(rpgCharacterInputController.inputLightHit){
							GetHit();
						}
						//Death.
						if(rpgCharacterInputController.inputDeath){
							if(!isDead){
								Death();
							}
							else{
								Revive();
							}
						}
						//Attacks.
						if(rpgCharacterInputController.inputAttackL){
							Attack(1);
						}
						if(rpgCharacterInputController.inputAttackR){
							Attack(2);
						}
						if(rpgCharacterInputController.inputLightHit){
							GetHit();
						}
						//Switch weapons.
						if(rpgCharacterInputController.inputSwitchUpDown){
							if(rpgCharacterWeaponController.isSwitchingFinished){
								if(weapon == Weapon.UNARMED){
									rpgCharacterWeaponController.SwitchWeaponTwoHand(1);
								}
								else{
									rpgCharacterWeaponController.SwitchWeaponTwoHand(0);
								}
							}
						}
						//Navmesh.
						if(Input.GetMouseButtonDown(0)){
							if(rpgCharacterMovementController.useMeshNav){
								RaycastHit hit;
								if(Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out hit, 100)){
									rpgCharacterMovementController.navMeshAgent.destination = hit.point;
								}
							}
						}
					}
				}
			}
			//Slow time toggle.
			if(Input.GetKeyDown(KeyCode.T)){
				if(Time.timeScale != 1){
					Time.timeScale = 1;
				}
				else{
					Time.timeScale = 0.005f;
				}
			}
			//Pause toggle.
			if(Input.GetKeyDown(KeyCode.P)){
				if(Time.timeScale != 1){
					Time.timeScale = 1;
				}
				else{
					Time.timeScale = 0f;
				}
			}
			//Strafe toggle.
			if(rpgCharacterInputController.inputStrafe){
				animator.SetBool("Strafing", true);
			}
			else{
				animator.SetBool("Strafing", false);
			}
		}

		void UpdateAnimationSpeed(){
			animator.SetFloat("AnimationSpeed", animationSpeed);
		}

		#endregion

		#region Aiming / Turning

		//Turning.
		public IEnumerator _Turning(int direction){
			if(direction == 1){
				Lock(true, true, true, 0, 0.55f);
				animator.SetTrigger("TurnLeftTrigger");
			}
			if(direction == 2){
				Lock(true, true, true, 0, 0.55f);
				animator.SetTrigger("TurnRightTrigger");
			}
			yield return null;
		}

		#endregion

		#region Combat

		//0 = No side
		//1 = Left
		//2 = Right
		//weaponNumber 0 = Unarmed
		//weaponNumber 1 = 2H Sword
		public void Attack(int attackSide){
			int attackNumber = 0;
			if(canAction){
				//Ground attacks.
				if(rpgCharacterMovementController.MaintainingGround()){
					//Stationary attack.
					if(!rpgCharacterMovementController.isMoving){
						if(weapon == Weapon.RELAX){
							weapon = Weapon.UNARMED;
							animator.SetInteger("Weapon", 0);
						}
						//Armed or Unarmed.
						if(weapon == Weapon.UNARMED || weapon == Weapon.ARMED || weapon == Weapon.ARMEDSHIELD){
							int maxAttacks = 3;
							//Left attacks.
							if(attackSide == 1){
								animator.SetInteger("AttackSide", 1);
								attackNumber = Random.Range(1, maxAttacks + 1);
							}
							//Right attacks.
							else if(attackSide == 2){
								animator.SetInteger("AttackSide", 2);
								attackNumber = Random.Range(4, maxAttacks + 4);
							}
							//Set the Locks.
							if(attackSide != 3){
								Lock(true, true, true, 0, 1.25f);
							}
						}
						else{
							int maxAttacks = 6;
							attackNumber = Random.Range(1, maxAttacks);
							if(weapon == Weapon.TWOHANDSWORD){
								Lock(true, true, true, 0, 0.85f);
							}
							else{
								Lock(true, true, true, 0, 0.75f);
							}
						}
					}
				}
				//Trigger the animation.
				animator.SetInteger("Action", attackNumber);
				if(attackSide == 3){
					animator.SetTrigger("AttackDualTrigger");
				}
				else{
					animator.SetTrigger("AttackTrigger");
				}
			}
		}

		public void AttackKick(int kickSide){
			if(rpgCharacterMovementController.MaintainingGround()){
				if(weapon == Weapon.RELAX){
					weapon = Weapon.UNARMED;
					animator.SetInteger("Weapon", 0);
				}
				animator.SetInteger("Action", kickSide);
				animator.SetTrigger("AttackKickTrigger");
				Lock(true, true, true, 0, 0.9f);
			}
		}

		void Strafing(){
			if(rpgCharacterInputController.inputStrafe && weapon != Weapon.RIFLE){
				if(weapon != Weapon.RELAX){
					animator.SetBool("Strafing", true);
					isStrafing = true;
				}
			}
			else{
				isStrafing = false;
				animator.SetBool("Strafing", false);
			}
		}

		void Rolling(){
			if(!rpgCharacterMovementController.isRolling){
				if(rpgCharacterInputController.inputRoll){
					rpgCharacterMovementController.DirectionalRoll();
				}
			}
		}

		public void GetHit(){
			if(weapon == Weapon.RELAX){
				weapon = Weapon.UNARMED;
				animator.SetInteger("Weapon", 0);
			}
			if(weapon != Weapon.RIFLE || weapon != Weapon.TWOHANDCROSSBOW){
				int hits = 5;
				if(isBlocking){
					hits = 2;
				}
				int hitNumber = Random.Range(1, hits + 1);
				animator.SetInteger("Action", hitNumber);
				animator.SetTrigger("GetHitTrigger");
				Lock(true, true, true, 0.1f, 0.4f);
				if(isBlocking){
					StartCoroutine(rpgCharacterMovementController._Knockback(-transform.forward, 3, 3));
					return;
				}
				//Apply directional knockback force.
				if(hitNumber <= 1){
					StartCoroutine(rpgCharacterMovementController._Knockback(-transform.forward, 8, 4));
				}
				else if(hitNumber == 2){
					StartCoroutine(rpgCharacterMovementController._Knockback(transform.forward, 8, 4));
				}
				else if(hitNumber == 3){
					StartCoroutine(rpgCharacterMovementController._Knockback(transform.right, 8, 4));
				}
				else if(hitNumber == 4){
					StartCoroutine(rpgCharacterMovementController._Knockback(-transform.right, 8, 4));
				}
			}
		}

		public void Death(){
			animator.SetTrigger("Death1Trigger");
			Lock(true, true, false, 0.1f, 0f);
			isDead = true;
		}

		public void Revive(){
			animator.SetTrigger("Revive1Trigger");
			Lock(true, true, true, 0f, 1f);
			isDead = false;
		}

		#endregion

		#region Actions

		/// <summary>
		/// Keep character from doing actions.
		/// </summary>
		void LockAction(){
			canAction = false;
		}

		/// <summary>
		/// Let character move and act again.
		/// </summary>
		void UnLock(bool movement, bool actions){
			if(movement){
				rpgCharacterMovementController.UnlockMovement();
			}
			if(actions){
				canAction = true;
			}
		}

		#endregion

		#region Misc

		//Placeholder functions for Animation events.
		public void Hit(){
		}

		public void Shoot(){
		}

		public void FootR(){
		}

		public void FootL(){
		}

		public void Land(){
		}

		IEnumerator _GetCurrentAnimationLength(){
			yield return new WaitForEndOfFrame();
			float f = (float)animator.GetCurrentAnimatorClipInfo(0).Length;
			Debug.Log(f);
		}

		/// <summary>
		/// Lock character movement and/or action, on a delay for a set time.
		/// </summary>
		/// <param name="lockMovement">If set to <c>true</c> lock movement.</param>
		/// <param name="lockAction">If set to <c>true</c> lock action.</param>
		/// <param name="timed">If set to <c>true</c> timed.</param>
		/// <param name="delayTime">Delay time.</param>
		/// <param name="lockTime">Lock time.</param>
		public void Lock(bool lockMovement, bool lockAction, bool timed, float delayTime, float lockTime){
			StopCoroutine("_Lock");
			StartCoroutine(_Lock(lockMovement, lockAction, timed, delayTime, lockTime));
		}

		//Timed -1 = infinite, 0 = no, 1 = yes.
		public IEnumerator _Lock(bool lockMovement, bool lockAction, bool timed, float delayTime, float lockTime){
			if(delayTime > 0){
				yield return new WaitForSeconds(delayTime);
			}
			if(lockMovement){
				rpgCharacterMovementController.LockMovement();
			}
			if(lockAction){
				LockAction();
			}
			if(timed){
				if(lockTime > 0){
					yield return new WaitForSeconds(lockTime);
				}
				UnLock(lockMovement, lockAction);
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
				rpgCharacterWeaponController.leftWeapon = Lweapon;
				animator.SetInteger("LeftWeapon", Lweapon);
				//Set Shield.
				if(Lweapon == 7){
					animator.SetBool("Shield", true);
				}
				else{
					animator.SetBool("Shield", false);
				}
			}
			//Set weapon side if applicable.
			if(weaponSide != -1){
				animator.SetInteger("LeftRight", weaponSide);
			}
			SetWeaponState(weapon);
		}

		public void SetWeaponState(int weaponNumber){
			if(weaponNumber == -1){
				weapon = Weapon.RELAX;
			}
			else if(weaponNumber == 0){
				weapon = Weapon.UNARMED;
			}
			else if(weaponNumber == 1){
				weapon = Weapon.TWOHANDSWORD;
			}
		}

		public void AnimatorDebug(){
			Debug.Log("ANIMATOR SETTINGS---------------------------");
			Debug.Log("Moving: " + animator.GetBool("Moving"));
			Debug.Log("Strafing: " + animator.GetBool("Strafing"));
			Debug.Log("Stunned: " + animator.GetBool("Stunned"));
			Debug.Log("Weapon: " + animator.GetInteger("Weapon"));
			Debug.Log("WeaponSwitch: " + animator.GetInteger("WeaponSwitch"));
			Debug.Log("Jumping: " + animator.GetInteger("Jumping"));
			Debug.Log("Action: " + animator.GetInteger("Action"));
			Debug.Log("Velocity X: " + animator.GetFloat("Velocity X"));
			Debug.Log("Velocity Z: " + animator.GetFloat("Velocity Z"));
		}

		#endregion

	}
}