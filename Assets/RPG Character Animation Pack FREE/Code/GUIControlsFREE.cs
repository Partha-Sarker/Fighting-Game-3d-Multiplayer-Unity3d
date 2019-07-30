using UnityEngine;
using System.Collections;

namespace RPGCharacterAnims{

	public class GUIControlsFREE : MonoBehaviour{
		RPGCharacterControllerFREE rpgCharacterController;
		RPGCharacterMovementControllerFREE rpgCharacterMovementController;
		public bool useNavAgent;

		void Awake(){
			rpgCharacterController = GetComponent<RPGCharacterControllerFREE>();
			rpgCharacterMovementController = GetComponent<RPGCharacterMovementControllerFREE>();
		}

		void OnGUI(){
			//General.
			if(!rpgCharacterController.isDead){
				//Actions.
				if(rpgCharacterController.canAction){
					if(rpgCharacterMovementController.MaintainingGround()){
						//Use NavMesh.
						useNavAgent = GUI.Toggle(new Rect(500, 15, 100, 30), useNavAgent, "Use NavAgent");
						if(useNavAgent && rpgCharacterMovementController.navMeshAgent != null){
							rpgCharacterMovementController.useMeshNav = true;
							rpgCharacterMovementController.navMeshAgent.enabled = true;
						}
						else{
							rpgCharacterMovementController.useMeshNav = false;
							rpgCharacterMovementController.navMeshAgent.enabled = false;
						}
						//Rolling.
						if(GUI.Button(new Rect(25, 15, 100, 30), "Roll Forward")){
							StartCoroutine(rpgCharacterMovementController._Roll(1));
						}
						if(GUI.Button(new Rect(130, 15, 100, 30), "Roll Backward")){
							StartCoroutine(rpgCharacterMovementController._Roll(3));
						}
						if(GUI.Button(new Rect(25, 45, 100, 30), "Roll Left")){
							StartCoroutine(rpgCharacterMovementController._Roll(4));
						}
						if(GUI.Button(new Rect(130, 45, 100, 30), "Roll Right")){
							StartCoroutine(rpgCharacterMovementController._Roll(2));
						}
						//Turning.
						if(GUI.Button(new Rect(340, 15, 100, 30), "Turn Left")){
							StartCoroutine(rpgCharacterController._Turning(1));
						}
						if(GUI.Button(new Rect(340, 45, 100, 30), "Turn Right")){
							StartCoroutine(rpgCharacterController._Turning(2));
						}
						//ATTACK LEFT.
						if(GUI.Button(new Rect(25, 85, 100, 30), "Attack L")){
							rpgCharacterController.Attack(1);
						}
						//ATTACK RIGHT.
						if(GUI.Button(new Rect(130, 85, 100, 30), "Attack R")){
							rpgCharacterController.Attack(2);
						}
						//Kicking.
						if(GUI.Button(new Rect(25, 115, 100, 30), "Left Kick")){
							rpgCharacterController.AttackKick(1);
						}
						if(GUI.Button(new Rect(130, 115, 100, 30), "Right Kick")){
							rpgCharacterController.AttackKick(3);
						}
						if(GUI.Button(new Rect(30, 240, 100, 30), "Get Hit")){
							rpgCharacterController.GetHit();
						}
					}
					//Jump / Double Jump.
					if((rpgCharacterMovementController.canJump || rpgCharacterMovementController.canDoubleJump) && rpgCharacterController.canAction){
						if(rpgCharacterMovementController.MaintainingGround()){
							if(GUI.Button(new Rect(25, 175, 100, 30), "Jump")){
								if(rpgCharacterMovementController.canJump){
									rpgCharacterMovementController.currentState = RPGCharacterStateFREE.Jump;
									rpgCharacterMovementController.rpgCharacterState = RPGCharacterStateFREE.Jump;
								}
							}
						}
						if(rpgCharacterMovementController.canDoubleJump){
							if(GUI.Button(new Rect(25, 175, 100, 30), "Jump Flip")){
								rpgCharacterMovementController.currentState = RPGCharacterStateFREE.DoubleJump;
								rpgCharacterMovementController.rpgCharacterState = RPGCharacterStateFREE.DoubleJump;
							}
						}
					}
					//Death.
					if(rpgCharacterMovementController.MaintainingGround() && rpgCharacterController.canAction){
						if(GUI.Button(new Rect(30, 270, 100, 30), "Death")){
							rpgCharacterController.Death();
						}
					}
				}
			}
			//Dead
			else{
				//Death.
				if(GUI.Button(new Rect(30, 270, 100, 30), "Revive")){
					rpgCharacterController.Revive();
				}
			}
		}
	}
}