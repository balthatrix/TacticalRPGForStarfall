using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Util {
	public class Singleton<T> : MonoBehaviour where T : Component {
		public static T Instance {
			get;
			private set;
		}

		public bool dontDestroyOnLoad = false;
		public virtual void Awake() {
			if (Instance == null) {
				Instance = this as T;
				if (dontDestroyOnLoad)
					DontDestroyOnLoad (Instance);
			} else {
				Destroy (this);
			}
		}       


	}
}
