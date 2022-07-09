using UnityEngine;
using System.Collections;
/// <summary>
/// Happy Glass 
/// Credit: Satyam Parkhi
/// Email: satyamparkhi@gmail.com
/// Facebook : https://www.facebook.com/satyamparkhi
/// Instagram : https://www.instagram.com/satyamparkhi/
/// Whatsapp : +91 7050225661
/// </summary>
public class ParticleGenerator : MonoBehaviour {		
	float SPAWN_INTERVAL=0.01f;                                                                                 // How much time until the next particle spawns
	float lastSpawnTime=float.MinValue;                                                                         //The last spawn time
	public float PARTICLE_LIFETIME=3;                                                                             //How much time will each particle live
	public Vector3 particleForce;                                                                               //Is there a initial force particles should have?
	public DynamicParticle.STATES particlesState=DynamicParticle.STATES.WATER;                                  // The state of the particles spawned
	public Transform particlesParent;                                                                           // Where will the spawned particles will be parented (To avoid covering the whole inspector with them)

	void Start() {
        Invoke("TapClose",2f);
        GetComponent<AudioSource>().Play();
    }

	void Update() {
        if (lastSpawnTime + SPAWN_INTERVAL < Time.time)
        { // Is it time already for spawning a new particle?
            GameObject newLiquidParticle = (GameObject)Instantiate(Resources.Load("DynamicParticle"));          //Spawn a particle Dont remove the particle from resource folder
            newLiquidParticle.GetComponent<Rigidbody2D>().AddForce(particleForce);                              //Add our custom force
            DynamicParticle particleScript = newLiquidParticle.GetComponent<DynamicParticle>();                 // Get the particle script
            particleScript.SetLifeTime(PARTICLE_LIFETIME);                                                      //Set each particle lifetime
            particleScript.SetState(particlesState);                                                            //Set the particle State
            newLiquidParticle.transform.position = new Vector3(Random.Range(transform.position.x - .001f, transform.position.x + .001f), Random.Range(transform.position.y - .001f, transform.position.y + .001f), 0); // Relocate to the spawner position
            //newLiquidParticle.transform.position = transform.position;
            newLiquidParticle.transform.parent = particlesParent;                                               // Add the particle to the parent container			
            lastSpawnTime = Time.time;                                                                          // Register the last spawnTime			
        }
	}

    void TapClose() {
        GetComponent<AudioSource>().Stop();
        GetComponent<ParticleGenerator>().enabled = false;
    }
}
