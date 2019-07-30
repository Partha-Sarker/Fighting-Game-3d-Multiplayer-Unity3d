using UnityEngine;
using System.Collections;

namespace RPGCharacterAnims{

	public enum RPGCharacterStateFREE{
		Idle = 0,
		Move = 1,
		Jump = 2,
		DoubleJump = 3,
		Fall = 4,
		Block = 6,
		Roll = 8
	}

	public class RPGCharacterMovementControllerFREE : SuperStateMachine{
		
		//Components.
		private SuperCharacterController superCharacterController;
		private RPGCharacterControllerFREE rpgCharacterController;
		[HideInInspector] public UnityEngine.AI.NavMeshAgent navMeshAgent;
		private RPGCharacterInputControllerFREE rpgCharacterInputController;
		private Rigidbody rb;
		private Animator animator;
		CapsuleCollider capCollider;
		public RPGCharacterStateFREE rpgCharacterState;

		[HideInInspector] public bool useMeshNav = false;
		[HideInInspector] public Vector3 lookDirection { get; private set; }
		[HideInInspector] public bool isKnockback;
		public float knockbackMultiplier = 1f;

		//Jumping.
		[HideInInspector] public bool canJump;
		[HideInInspector] public bool canDoubleJump = false;
		bool doublejumped = false;
		public float gravity = 25.0f;
		public float jumpAcceleration = 5.0f;
		public float jumpHeight = 3.0f;
		public float doubleJumpHeight = 4f;

		//Movement.
		[HideInInspector] public Vector3 currentVelocity;
		[HideInInspector] public bool isMoving = false;
		[HideInInspector] public bool canMove = true;
		public float movementAcceleration = 90.0f;
		public float walkSpeed = 4f;
		public float runSpeed = 6f;
		float rotationSpeed = 40f;
		public float groundFriction = 50f;

		//Rolling.
		[HideInInspector] public bool isRolling = false;
		public float rollSpeed = 8;
		public float rollduration = 0.35f;
		private int rollNumber;

		//Air control.
		public float inAirSpeed = 6f;

		void Awake(){
			superCharacterController = GetComponent<SuperCharacterController>();
			rpgCharacterController = GetComponent<RPGCharacterControllerFREE>();
			rpgCharacterInputController = GetComponent<RPGCharacterInputControllerFREE>();
			navMeshAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();
			animator = GetComponentInChildren<Animator>();
			capCollider = GetComponent<CapsuleCollider>();
			rb = GetComponent<Rigidbody>();
			if(rb != null){
				//Set restraints on startup if using Rigidbody.
				rb.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationZ;
			}
			//Set currentState to idle on startup.
			currentState = RPGCharacterStateFREE.Idle;
			rpgCharacterState = RPGCharacterStateFREE.Idle;
		}

		/*
		Update is normally run once on every frame update. We won't be using it in this case, since the SuperCharacterController component sends a callback Update called SuperUpdate. SuperUpdate is recieved by the SuperStateMachine, and then fires further callbacks depending on the state

		void Update(){
		}

		*/

		//Put any code in here you want to run BEFORE the state's update function. This is run regardless of what state you're in.
		protected override void EarlyGlobalSuperUpdate(){
		}

		//Put any code in here you want to run AFTER the state's update function.  This is run regardless of what state you're in.
		protected override void LateGlobalSuperUpdate(){
			//Move the player by our velocity every frame.
			transform.position += currentVelocity * superCharacterController.deltaTime;
			//If using Navmesh nagivation, update values.
			if(navMeshAgent != null){
				if(useMeshNav){
					if(navMeshAgent.velocity.sqrMagnitude > 0){
						animator.SetBool("Moving", true);
						animator.SetFloat("Velocity Z", navMeshAgent.velocity.magnitude);
					}
					else{
						animator.SetFloat("Velocity Z", 0);
					}
				}
			}
			//If alive and is moving, set animator.
			if(!useMeshNav && !rpgCharacterController.isDead && canMove){
				if(currentVelocity.magnitude > 0 && rpgCharacterInputController.HasMoveInput()){
					isMoving = true;
					animator.SetBool("Moving", true);
					animator.SetFloat("Velocity Z", currentVelocity.magnitude);
				}
				else{
					isMoving = false;
					animator.SetBool("Moving", false);
				}
			}
			//Strafing.
			if(!rpgCharacterController.isStrafing){
				if(rpgCharacterInputController.HasMoveInput() && canMove){
					RotateTowardsMovementDir();
				}
			}
			else{
				Strafing(rpgCharacterController.target.transform.position);
			}
		}

		private bool AcquiringGround(){
			return superCharacterController.currentGround.IsGrounded(false, 0.01f);
		}

		public bool MaintainingGround(){
			return superCharacterController.currentGround.IsGrounded(true, 0.5f);
		}

		public void RotateGravity(Vector3 up){
			lookDirection = Quaternion.FromToRotation(transform.up, up) * lookDirection;
		}

		/// <summary>
		/// Constructs a vector representing our movement local to our lookDirection, which is controlled by the camera.
		/// </summary>
		private Vector3 LocalMovement(){
			return rpgCharacterInputController.moveInput;
		}

		// Calculate the initial velocity of a jump based off gravity and desired maximum height attained
		private float CalculateJumpSpeed(float jumpHeight, float gravity){
			return Mathf.Sqrt(2 * jumpHeight * gravity);
		}

		//Below are the state functions. Each one is called based on the name of the state, so when currentState = Idle, we call Idle_EnterState. If currentState = Jump, we call Jump_SuperUpdate()
		void Idle_EnterState(){
			superCharacterController.EnableSlopeLimit();
			superCharacterController.EnableClamping();
			canJump = true;
			doublejumped = false;
			canDoubleJump = false;
			animator.SetInteger("Jumping", 0);
		}

		//Run every frame we are in the idle state.
		void Idle_SuperUpdate(){
			//If Jump.
			if(rpgCharacterInputController.allowedInput && rpgCharacterInputController.inputJump){
				currentState = RPGCharacterStateFREE.Jump;
				rpgCharacterState = RPGCharacterStateFREE.Jump;
				return;
			}
			if(!MaintainingGround()){
				currentState = RPGCharacterStateFREE.Fall;
				rpgCharacterState = RPGCharacterStateFREE.Fall;
				return;
			}
			if(rpgCharacterInputController.HasMoveInput() && canMove){
				currentState = RPGCharacterStateFREE.Move;
				rpgCharacterState = RPGCharacterStateFREE.Move;
				return;
			}
			//Apply friction to slow to a halt.
			currentVelocity = Vector3.MoveTowards(currentVelocity, Vector3.zero, groundFriction * superCharacterController.deltaTime);
		}

		void Idle_ExitState(){
			//Run once when exit the idle state.
		}

		void Move_SuperUpdate(){
			//If Jump.
			if(rpgCharacterInputController.allowedInput && rpgCharacterInputController.inputJump){
				currentState = RPGCharacterStateFREE.Jump;
				rpgCharacterState = RPGCharacterStateFREE.Jump;
				return;
			}
			if(!MaintainingGround()){
				currentState = RPGCharacterStateFREE.Fall;
				rpgCharacterState = RPGCharacterStateFREE.Fall;
				return;
			}
			//Set speed determined by movement type.
			if(rpgCharacterInputController.HasMoveInput() && canMove){
				//Keep strafing animations from playing.
				animator.SetFloat("Velocity X", 0F);
				//Strafing or Walking.
				if(rpgCharacterController.isStrafing){
					currentVelocity = Vector3.MoveTowards(currentVelocity, LocalMovement() * walkSpeed, movementAcceleration * superCharacterController.deltaTime);
					if(rpgCharacterController.weapon != Weapon.RELAX){
						Strafing(rpgCharacterController.target.transform.position);
					}
					return;
				}
				//Run.
				currentVelocity = Vector3.MoveTowards(currentVelocity, LocalMovement() * runSpeed, movementAcceleration * superCharacterController.deltaTime);
			}
			else{
				currentState = RPGCharacterStateFREE.Idle;
				rpgCharacterState = RPGCharacterStateFREE.Idle;
				return;
			}
		}

		void Jump_EnterState(){
			superCharacterController.DisableClamping();
			superCharacterController.DisableSlopeLimit();
			currentVelocity += superCharacterController.up * CalculateJumpSpeed(jumpHeight, gravity);
			//Set weaponstate to Unarmed if Relaxed.
			if(rpgCharacterController.weapon == Weapon.RELAX){
				rpgCharacterController.weapon = Weapon.UNARMED;
				animator.SetInteger("Weapon", 0);
			}
			canJump = false;
			animator.SetInteger("Jumping", 1);
			animator.SetTrigger("JumpTrigger");
		}

		void Jump_SuperUpdate(){
			Vector3 planarMoveDirection = Math3d.ProjectVectorOnPlane(superCharacterController.up, currentVelocity);
			Vector3 verticalMoveDirection = currentVelocity - planarMoveDirection;
			if(Vector3.Angle(verticalMoveDirection, superCharacterController.up) > 90 && AcquiringGround()){
				currentVelocity = planarMoveDirection;
				currentState = RPGCharacterStateFREE.Idle;
				rpgCharacterState = RPGCharacterStateFREE.Idle;
				return;            
			}
			planarMoveDirection = Vector3.MoveTowards(planarMoveDirection, LocalMovement() * inAirSpeed, jumpAcceleration * superCharacterController.deltaTime);
			verticalMoveDirection -= superCharacterController.up * gravity * superCharacterController.deltaTime;
			currentVelocity = planarMoveDirection + verticalMoveDirection;
			//Can double jump if starting to fall.
			if(currentVelocity.y < 0){
				DoubleJump();
			}
		}

		void DoubleJump_EnterState(){
			currentVelocity += superCharacterController.up * CalculateJumpSpeed(doubleJumpHeight, gravity);
			canDoubleJump = false;
			doublejumped = true;
			animator.SetInteger("Jumping", 3);
			animator.SetTrigger("JumpTrigger");
		}

		void DoubleJump_SuperUpdate(){
			Jump_SuperUpdate();
		}

		void DoubleJump(){
			if(!doublejumped){
				canDoubleJump = true;
			}
			if(rpgCharacterInputController.inputJump && canDoubleJump && !doublejumped){
				currentState = RPGCharacterStateFREE.DoubleJump;
				rpgCharacterState = RPGCharacterStateFREE.DoubleJump;
			}
		}

		void Fall_EnterState(){
			if(!doublejumped){
				canDoubleJump = true;
			}
			superCharacterController.DisableClamping();
			superCharacterController.DisableSlopeLimit();
			canJump = false;
			animator.SetInteger("Jumping", 2);
			animator.SetTrigger("JumpTrigger");
		}

		void Fall_SuperUpdate(){
			if(AcquiringGround()){
				currentVelocity = Math3d.ProjectVectorOnPlane(superCharacterController.up, currentVelocity);
				currentState = RPGCharacterStateFREE.Idle;
				rpgCharacterState = RPGCharacterStateFREE.Idle;
				return;
			}
			DoubleJump();
			currentVelocity -= superCharacterController.up * gravity * superCharacterController.deltaTime;
		}

		void Roll_SuperUpdate(){
			if(rollNumber == 1){
				currentVelocity = Vector3.MoveTowards(currentVelocity, transform.forward * rollSpeed, groundFriction * superCharacterController.deltaTime);
			}
			if(rollNumber == 2){
				currentVelocity = Vector3.MoveTowards(currentVelocity, transform.right * rollSpeed, groundFriction * superCharacterController.deltaTime);
			}
			if(rollNumber == 3){
				currentVelocity = Vector3.MoveTowards(currentVelocity, -transform.forward * rollSpeed, groundFriction * superCharacterController.deltaTime);
			}
			if(rollNumber == 4){
				currentVelocity = Vector3.MoveTowards(currentVelocity, -transform.right * rollSpeed, groundFriction * superCharacterController.deltaTime);
			}
		}

		public void DirectionalRoll(){
			//Check which way the dash is pressed relative to the character facing.
			float angle = Vector3.Angle(rpgCharacterInputController.moveInput, -transform.forward);
			float sign = Mathf.Sign(Vector3.Dot(transform.up, Vector3.Cross(rpgCharacterInputController.aimInput, transform.forward)));
			//Angle in [-179,180].
			float signed_angle = angle * sign;
			//Angle in 0-360.
			float angle360 = (signed_angle + 180) % 360;
			//Deternime the animation to play based on the angle.
			if(angle360 > 315 || angle360 < 45){
				StartCoroutine(_Roll(1));
			}
			if(angle360 > 45 && angle360 < 135){
				StartCoroutine(_Roll(2));
			}
			if(angle360 > 135 && angle360 < 225){
				StartCoroutine(_Roll(3));
			}
			if(angle360 > 225 && angle360 < 315){
				StartCoroutine(_Roll(4));
			}
		}

		/// <summary>
		/// Character Roll.
		/// </summary>
		/// <param name="1">Forward.</param>
		/// <param name="2">Right.</param>
		/// <param name="3">Backward.</param>
		/// <param name="4">Left.</param>
		public IEnumerator _Roll(int roll){
			rollNumber = roll;
			currentState = RPGCharacterStateFREE.Roll;
			rpgCharacterState = RPGCharacterStateFREE.Roll;
			if(rpgCharacterController.weapon == Weapon.RELAX){
				rpgCharacterController.weapon = Weapon.UNARMED;
				animator.SetInteger("Weapon", 0);
			}
			animator.SetInteger("Action", rollNumber);
			animator.SetTrigger("RollTrigger");
			isRolling = true;
			rpgCharacterController.canAction = false;
			yield return new WaitForSeconds(rollduration);
			isRolling = false;
			rpgCharacterController.canAction = true;
			currentState = RPGCharacterStateFREE.Idle;
			rpgCharacterState = RPGCharacterStateFREE.Idle;
		}

		public void SwitchCollisionOff(){
			canMove = false;
			superCharacterController.enabled = false;
			animator.applyRootMotion = true;
			if(rb != null){
				rb.isKinematic = false;
			}
		}

		public void SwitchCollisionOn(){
			canMove = true;
			superCharacterController.enabled = true;
			animator.applyRootMotion = false;
			if(rb != null){
				rb.isKinematic = true;
			}
		}

		void RotateTowardsMovementDir(){
			if(rpgCharacterInputController.moveInput != Vector3.zero){
				transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(rpgCharacterInputController.moveInput), Time.deltaTime * rotationSpeed);
			}
		}

		void RotateTowardsTarget(Vector3 targetPosition){
			Quaternion targetRotation = Quaternion.LookRotation(targetPosition - new Vector3(transform.position.x, 0, transform.position.z));
			transform.eulerAngles = Vector3.up * Mathf.MoveTowardsAngle(transform.eulerAngles.y, targetRotation.eulerAngles.y, (rotationSpeed * Time.deltaTime) * rotationSpeed);
		}

		void Aiming(){
//			Debug.Log("Aiming");
			for(int i = 0; i < Input.GetJoystickNames().Length; i++){
				//If right joystick is moved, use that for facing.
				if(Mathf.Abs(rpgCharacterInputController.inputAimHorizontal) > 0.8 || Mathf.Abs(rpgCharacterInputController.inputAimVertical) < -0.8){
					Vector3 joyDirection = new Vector3(rpgCharacterInputController.inputAimHorizontal, 0, -rpgCharacterInputController.inputAimVertical);
					joyDirection = joyDirection.normalized;
					Quaternion joyRotation = Quaternion.LookRotation(joyDirection);
					transform.rotation = joyRotation;
				}
			}
			//No joysticks, use mouse aim.
			if(Input.GetJoystickNames().Length == 0){
				Plane characterPlane = new Plane(Vector3.up, transform.position);
				Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
				Vector3 mousePosition = new Vector3(0, 0, 0);
				float hitdist = 0.0f;
				if(characterPlane.Raycast(ray, out hitdist)){
					mousePosition = ray.GetPoint(hitdist);
				}
				mousePosition = new Vector3(mousePosition.x, transform.position.y, mousePosition.z);
				RotateTowardsTarget(mousePosition);
			}
			//Update animator with local movement values.
			animator.SetFloat("Velocity X", transform.InverseTransformDirection(currentVelocity).x);
			animator.SetFloat("Velocity Z", transform.InverseTransformDirection(currentVelocity).z);
		}

		//Update animator with local movement values.
		void Strafing(Vector3 targetPosition){
//			Debug.Log("Strafing");
			animator.SetFloat("Velocity X", transform.InverseTransformDirection(currentVelocity).x);
			animator.SetFloat("Velocity Z", transform.InverseTransformDirection(currentVelocity).z);
			RotateTowardsTarget(targetPosition);
		}

		public IEnumerator _Knockback(Vector3 knockDirection, int knockBackAmount, int variableAmount){
			isKnockback = true;
			StartCoroutine(_KnockbackForce(knockDirection, knockBackAmount, variableAmount));
			yield return new WaitForSeconds(.1f);
			isKnockback = false;
		}

		IEnumerator _KnockbackForce(Vector3 knockDirection, int knockBackAmount, int variableAmount){
			while(isKnockback){
				rb.AddForce(knockDirection * ((knockBackAmount + Random.Range(-variableAmount, variableAmount)) * (knockbackMultiplier * 10)), ForceMode.Impulse);
				yield return null;
			}
		}

		//Keep character from moving.
		public void LockMovement(){
			canMove = false;
			animator.SetBool("Moving", false);
			animator.applyRootMotion = true;
			currentVelocity = new Vector3(0, 0, 0);
		}

		public void UnlockMovement(){
			canMove = true;
			animator.applyRootMotion = false;
		}
	}
}