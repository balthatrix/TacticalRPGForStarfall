using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class TestParticlesSprites : MonoBehaviour {

	// Use this for initialization
	void Start () {
//		SpriteParticleEmitter emitter =  GetComponent<SpriteParticleEmitter> ();
		GetComponent<Rigidbody2D>().velocity = new Vector2(2f, 0f);
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
