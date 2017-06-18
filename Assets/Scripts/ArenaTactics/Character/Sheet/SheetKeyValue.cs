using System;
using System.Collections.Generic;
using System.Linq;
using AT.Serialization;
	
namespace AT.Character
{

	[System.Serializable]
	public class SheetKeyValue
	{
		/// <summary>
		/// The value.  This should be serializable.
		/// </summary>
		public object value;
		public string key;
		public SheetKeyValue(string key, object value) {
			this.key = key;
			this.value = value;
		}
	}

	[System.Serializable]
	public class SheetKeyValueStoreWrapper : Wrapper {
		public SheetKeyValue[] keyvals;

		public SheetKeyValueStoreWrapper() {
		}

		public override SerializedObject GetInstance() {
			SheetKeyValueStore skvs = new SheetKeyValueStore ();
			skvs.keyvals = keyvals.ToList ();
			return skvs;
		}
	}

	public class SheetKeyValueStore : SerializedObject {
		public List<SheetKeyValue> keyvals;
//		List<SheetKeyValue<object>> objectKeyVals;
		public SheetKeyValueStore() {
			keyvals = new List<SheetKeyValue> ();
		}

		public Wrapper GetSerializableWrapper() {
			SheetKeyValueStoreWrapper kvw = new SheetKeyValueStoreWrapper ();
			kvw.keyvals = keyvals.ToArray ();
			return kvw;
		}

		public SheetKeyValue GetKeyValue(string key) {
			foreach (SheetKeyValue kv in keyvals) {
				if (kv.key == key) {
					return kv;

				}
			}
			return null;
		}

        public string GetDumpKeyVals() {
            string ret = "";
            foreach (SheetKeyValue kv in keyvals)
            {
                ret += kv.key + ": " + kv.value.ToString() + "| ";
            }
            return ret;
        }


		public T GetValue<T>(string key) {
			foreach (SheetKeyValue kv in keyvals) {
				if (kv.key == key) {
					return (T) kv.value;

				}
			}
			return default(T);
		}


		public bool HasKey(string key) {
			return (GetKeyValue (key) != null);
		}
	
		public void AddKeyValue(SheetKeyValue kv) {
			if (HasKey(kv.key)) {
				UnityEngine.Debug.LogError ("Can't overrite key values!");
                UnityEngine.Debug.LogError("Dump: " + GetDumpKeyVals());
				return;
			}


			this.keyvals.Add (kv);
			UnityEngine.Debug.Log ("kv size: " + kv.key);
			UnityEngine.Debug.Log (keyvals.Count);
			UnityEngine.Debug.Log(HasKey(kv.key));
			if (!HasKey(kv.key)) {
				UnityEngine.Debug.LogError ("Just added Kvalues not there!!");
				return;
			}
		}

        public void ChangeKeyVal(string key, object val) {
            SheetKeyValue kv = GetKeyValue(key);
            if(kv != null)
            {
                kv.value = val;
            }
        }

        public T GetMetaValue<T>(string key)
        {
            SheetKeyValue kv = GetKeyValue(key);
            if (kv == null)
            {
//                UnityEngine.Debug.LogWarning("key value is null!");
                return default(T);

            }

            return (T)kv.value;
        }


        public void SetMetaValue(string key, object value)
        {
            SheetKeyValue kv = GetKeyValue(key);
            if (kv == null)
            {
                AddKeyValue(new SheetKeyValue(key, value));
            }
            else {
                kv.value = value;
            }


            
        }

    }
}

