using UnityEngine;
using System.Collections;

public class BarrelAI : MonoBehaviour {

	public float dir = 1;
	float speed = 0.75f;
	Collider2D currentFloor;
	float timeUntilFallDown = 100;

	void OnCollisionEnter2D(Collision2D collision) {

		if (collision.collider.gameObject.name == "InvisibleWallLeft") {
			dir = 1.0f;
		}
		else if (collision.collider.gameObject.name == "InvisibleWallRight") {
			dir = -1.0f;
		}
		else if (collision.collider.gameObject.name.Contains("Floor")) {
			currentFloor = collision.collider;
		}
	}

	void FixedUpdate() {

		bool grounded = Physics2D.Raycast(transform.position, new Vector2(0, -1), 0.07f, 1 << LayerMask.NameToLayer("Ground"));  

		if (currentFloor != null && grounded)
			rigidbody2D.velocity = new Vector2(speed * dir, 0.0f);

		if (timeUntilFallDown != 100) 
			timeUntilFallDown -= Time.fixedDeltaTime;

		if (timeUntilFallDown <= 0) {
			FallDown();
		}
	}

	void OnTriggerEnter2D(Collider2D otherCollider) {
		
		if (otherCollider.gameObject.name == "BarrelRemovalArea") {
			Destroy(gameObject);
		}
		else if (otherCollider.gameObject.name == "Ladder" && Random.Range(0, 10) > 7) {

			if (otherCollider.transform.position.y < transform.position.y) {
				timeUntilFallDown = 0.09f;
			}
		}
	}

	void FallDown() {

		Physics2D.IgnoreCollision(currentFloor, gameObject.collider2D);
		rigidbody2D.velocity = Vector2.zero;
		currentFloor = null;
		timeUntilFallDown = 100;
		dir *= -1;
	}
}
