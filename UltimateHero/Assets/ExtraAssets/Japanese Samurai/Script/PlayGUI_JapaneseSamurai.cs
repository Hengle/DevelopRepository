using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PlayGUI_JapaneseSamurai : MonoBehaviour {
	public Transform[] transforms;
	
	private List<AnimationButtonGroup> animationButtonGroupList = new List<AnimationButtonGroup>();

	private Animator[] animator;
	
	private string currentGroup = "";
	private string currentState = "";

	// Use this for initialization
	void Awake () {
    	initButtonInfo();
		getAnimator();
	}
	
	private void initButtonInfo() {
		AnimationButtonGroup animationButtonGroup = new AnimationButtonGroup();
		animationButtonGroup.groupName = "GENERAL";
		animationButtonGroup.status = AnimationButtonGroup.Status.CONTENT;
		animationButtonGroup.itemName.Add("idle1");
		animationButtonGroup.itemName.Add("walk");
		animationButtonGroup.itemName.Add("run");
		animationButtonGroupList.Add(animationButtonGroup);
		
		animationButtonGroup = new AnimationButtonGroup();
		animationButtonGroup.groupName = "BATTLE";
		animationButtonGroup.status = AnimationButtonGroup.Status.TITLE;
		animationButtonGroup.itemName.Add("attack0");
		animationButtonGroup.itemName.Add("attack1");
		animationButtonGroup.itemName.Add("wound0");
		animationButtonGroup.itemName.Add("death0");
		animationButtonGroupList.Add(animationButtonGroup);
		
		currentGroup = animationButtonGroupList[0].groupName;
	}
	
	private void getAnimator() {
		animator = new Animator[transforms.Length];
		for (int i = 0; i < transforms.Length; i++) {
			animator[i] = transforms[i].GetComponent<Animator>();
		}
	}
	
	private bool title = false;
	private bool content = false;
	void OnGUI() {
		refreshGUI();
		closeAnimation();
		setAnimation();
	}
	
	private void refreshGUI() {
		GUILayout.BeginVertical("box");
		for (int i = 0; i < animationButtonGroupList.Count; i++) {
			GUILayout.BeginVertical("box");
			
			title = false;
			content = false;
			
			if(animationButtonGroupList[i].status == AnimationButtonGroup.Status.TITLE) {
				title = true;
			} else if(animationButtonGroupList[i].status == AnimationButtonGroup.Status.CONTENT) {
				content = true;
			} else if(animationButtonGroupList[i].status == AnimationButtonGroup.Status.BOTH) {
				title = true;
				content = true;
			}
			
			if(title) {
				if (GUILayout.Button(animationButtonGroupList[i].groupName)) {
					currentGroup = animationButtonGroupList[i].groupName;
					currentState = animationButtonGroupList[i].groupName;
				}
			}
			if(content) {
				GUILayout.BeginVertical("box");
				for (int j = 0; j < animationButtonGroupList[i].itemName.Count; j++) {
					if (GUILayout.Button(animationButtonGroupList[i].itemName[j])) {
						currentState = animationButtonGroupList[i].itemName[j];
					}
				}
				GUILayout.EndVertical();	
			}
			GUILayout.EndVertical();
		}
		GUILayout.EndVertical();
	}
	
	
	
	AnimatorStateInfo stateInfo;
	private void closeAnimation() {
		stateInfo  = animator[0].GetCurrentAnimatorStateInfo(0);
		
		switch (currentGroup) {
		case "GENERAL":
			for (int j = 0; j < animator.Length; j++) {
	          animator[j].SetBool("battle_idleToGeneral_idle0", true);
				
			  animator[j].SetBool("battle_death0ToBattle_idle", true);
			}
			
			if (stateInfo.IsName("Base Layer.general_idle0")) {
				for (int j = 0; j < animator.Length; j++) {
				  animator[j].SetBool("general_walkToGeneral_idle0", false);
				  animator[j].SetBool("general_runToGeneral_idle0", false);
				}
			} else {
				for (int j = 0; j < animator.Length; j++) {
		          animator[j].SetBool("general_idle0ToGeneral_idle1", false);
		          animator[j].SetBool("general_idle0ToGeneral_walk", false);
		          animator[j].SetBool("general_idle0ToGeneral_run", false);
					
			      animator[j].SetBool("general_idle0ToBattle_idle", false);
		        }
			}
			break;
		case "BATTLE":
			for (int j = 0; j < animator.Length; j++) {
	          animator[j].SetBool("general_idle0ToBattle_idle", true);
				
			  animator[j].SetBool("general_walkToGeneral_idle0", true);
			  animator[j].SetBool("general_runToGeneral_idle0", true);
			}
			
			if (stateInfo.IsName("Base Layer.battle_idle")) {
		        for (int j = 0; j < animator.Length; j++) {
					animator[j].SetBool("battle_death0ToBattle_idle", false);
				}
				
			} else {
				for (int j = 0; j < animator.Length; j++) {
		          animator[j].SetBool("battle_idleToBattle_attack0", false);
		          animator[j].SetBool("battle_idleToBattle_attack1", false);
				  animator[j].SetBool("battle_idleToBattle_wound0", false);
		          animator[j].SetBool("battle_idleToBattle_death0", false);
					
				  animator[j].SetBool("battle_idleToGeneral_idle0", false);
		        }
			}
			break;
		}
	}
	
	private void setAnimation() {
		if (currentState != "") {
			
			if (stateInfo.IsName("Base Layer.general_walk") && currentState != "walk") {
				for (int j = 0; j < animator.Length; j++) {
					animator[j].SetBool("general_walkToGeneral_idle0", true);
				}
			}
			if (stateInfo.IsName("Base Layer.general_run") && currentState != "run") {
				for (int j = 0; j < animator.Length; j++) {
					animator[j].SetBool("general_runToGeneral_idle0", true);
				}
			}
			if (stateInfo.IsName("Base Layer.battle_death0") && currentState != "death0") {
				for (int j = 0; j < animator.Length; j++) {
					animator[j].SetBool("battle_death0ToBattle_idle", true);
				}
			}
			
			switch (currentState) {
			/********** GENERAL ***********/
	        case "idle1":
				for (int j = 0; j < animator.Length; j++) {
					animator[j].SetBool("general_idle0ToGeneral_idle1", true);
				}				
				break;
	        case "walk":
				for (int j = 0; j < animator.Length; j++) {
					animator[j].SetBool("general_idle0ToGeneral_walk", true);
				}
				break;
			case "run":
				for (int j = 0; j < animator.Length; j++) {
					animator[j].SetBool("general_idle0ToGeneral_run", true);
				}
				break;
				
			/********** BATTLE ***********/
			case "attack0":
				for (int j = 0; j < animator.Length; j++) {
					animator[j].SetBool("battle_idleToBattle_attack0", true);
				}
				break;
			case "attack1":
				for (int j = 0; j < animator.Length; j++) {
					animator[j].SetBool("battle_idleToBattle_attack1", true);
				}
				break;	
			case "wound0":
				for (int j = 0; j < animator.Length; j++) {
					animator[j].SetBool("battle_idleToBattle_wound0", true);
				}
				break;	
			case "death0":
				for (int j = 0; j < animator.Length; j++) {
					animator[j].SetBool("battle_idleToBattle_death0", true);
				}
				break;
			
			/********** groupSwitch ***********/
			case "GENERAL":
				if (stateInfo.IsName("Base Layer.battle_idle")) {
					for (int j = 0; j < animator.Length; j++) {
						animator[j].SetBool("battle_idleToGeneral_idle0", true);
					}
				}
				animationButtonGroupList[0].status = AnimationButtonGroup.Status.CONTENT;
				animationButtonGroupList[1].status = AnimationButtonGroup.Status.TITLE;
				break;
			case "BATTLE":
				if (stateInfo.IsName("Base Layer.general_idle0")) {
					for (int j = 0; j < animator.Length; j++) {
						animator[j].SetBool("general_idle0ToBattle_idle", true);
					}
				}
				animationButtonGroupList[0].status = AnimationButtonGroup.Status.TITLE;
				animationButtonGroupList[1].status = AnimationButtonGroup.Status.CONTENT;
				break;
			default:
			break;
			}
			currentState = "";
		}
	}
	
	public class AnimationButtonGroup {
		public enum Status {
			TITLE,
			CONTENT,
			BOTH,
			NONE
		}
		
		public Status status = Status.NONE;
		public string groupName;
		public List<string> itemName = new List<string>();
	}
	
}
