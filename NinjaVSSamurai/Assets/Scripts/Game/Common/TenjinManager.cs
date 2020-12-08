using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TenjinManager : SingletonMonoBehaviour<TenjinManager> {
	BaseTenjin TenjinInstance { get; set; }


	void Start () {
		TenjinInstance = Tenjin.getInstance("D4DVXQQPQWKAGCGR91UTMTOERXPGWYBO");
		TenjinInstance.Connect();
	}

	public void SendEvent (string eventName) {
		TenjinInstance.SendEvent(eventName);
	}

}
