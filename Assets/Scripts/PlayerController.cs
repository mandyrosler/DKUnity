using UnityEngine;
using System.Collections;

public enum State {
	Idle,
	Walking,
	Jumping,
	Climbing,
	Dead
};

public class PlayerController : MonoBehaviour {

	State currentState = State.Idle;

	BarrelSpawner barrelSpawner;
	SoundEffectPlayer soundEffectPlayer;
	
	bool facingRight = true;
	bool shouldJump = false;
	bool hasWon = false;
	
	float maxSpeed = 0.5f;
	float jumpForce = 45f;
	float timeSinceJumpStarted;
	
	private Animator anim;
	private Collider2D climbingLadder;
	private Transform bottomTransform;
	private SpriteRenderer gameOverSprite;
	
	void Awake() {
		
		anim = GetComponent<Animator>();
		GameObject bottomPos = new GameObject();
		bottomPos.transform.parent = transform;
		bottomPos.transform.localPosition = new Vector3(0, -0.07980061f, 0);
		bottomPos.name = "BottomPos";
		bottomTransform = bottomPos.transform;
		
		if (GameObject.Find("DonkeyKong") != null) barrelSpawner = GameObject.Find("DonkeyKong").GetComponent<BarrelSpawner>();
		soundEffectPlayer = gameObject.AddComponent<SoundEffectPlayer>();
		
		if (GameObject.Find ("GameOver") != null) gameOverSprite = GameObject.Find ("GameOver").GetComponent<SpriteRenderer>();
	}

	void Update () {

		float horizontalInput = Input.GetAxis("Horizontal");
		float verticalInput = Input.GetAxis("Vertical");
		bool pressedJumpButton = Input.GetButtonDown("Jump");

		bool isOnGround = Physics2D.Raycast(bottomTransform.position, new Vector2(0, -1), 0.05f, 1 << LayerMask.NameToLayer("Ground")); 

		//-------- Idle/Walking --------//
		if (currentState == State.Idle || currentState == State.Walking) {

			float direction = horizontalInput == 0 ? 0 : Mathf.Sign(horizontalInput);
			rigidbody2D.velocity = new Vector2(direction * maxSpeed, rigidbody2D.velocity.y);
			
			if(horizontalInput > 0 && !facingRight)
				Flip();
			else if(horizontalInput < 0 && facingRight)
				Flip();

			// Play walking sound
			if (direction != 0) {
				soundEffectPlayer.PlayWalkEffect(true);
			}
			else soundEffectPlayer.PlayWalkEffect(false);

			// Check if we should switch state
			if(pressedJumpButton) {
				SwitchToState(State.Jumping);
			}
			else if (verticalInput != 0) {

				climbingLadder = FindLadderInDirection((int)Mathf.Sign(verticalInput), 0.05f);
				if (climbingLadder != null) {
					SwitchToState(State.Climbing);
				}
			}
			else if (direction == 0) {
				SwitchToState(State.Idle);
			}
			else if (direction != 0) {
				SwitchToState(State.Walking);
			}
		}

		//-------- Jumping --------//
		else if (currentState == State.Jumping) {

			if (isOnGround && !shouldJump && timeSinceJumpStarted > 0.1f) {
				SwitchToState(State.Idle);
			}
		}

		//-------- Climbing --------//
		else if (currentState == State.Climbing) {

			float direction = verticalInput == 0 ? 0 : Mathf.Sign(verticalInput);
			
			Collider2D ladder = FindLadderInDirection((int)direction, maxSpeed * 0.5f);
			if (ladder == climbingLadder) {
				rigidbody2D.velocity = new Vector2(0, direction * maxSpeed * 0.5f);
			}
			else {
				rigidbody2D.velocity = Vector2.zero;
			}

			// Play walking sound
			if (Mathf.Abs(rigidbody2D.velocity.y) > 0.0f) {
				soundEffectPlayer.PlayWalkEffect(true);
			}
			else soundEffectPlayer.PlayWalkEffect(false);

			// Check if we should switch state
			if (Mathf.Abs (horizontalInput) > 0.0f && isOnGround && rigidbody2D.velocity.y == 0) {
				SwitchToState(State.Walking);
			}
		}

		//-------- Dead --------//
		else if (currentState == State.Dead) {
			// Do nothing...
		}

		// Update the animation state
		if (anim != null) {
			anim.SetBool("Idle", currentState == State.Idle);
			anim.SetBool("Walking", currentState == State.Walking);
			anim.SetBool("Jumping", currentState == State.Jumping);

			anim.SetBool("Climbing", currentState == State.Climbing);
		}

	}

	void SwitchToState(State nextState) {

		if (currentState == nextState) return;

		if (nextState == State.Idle || nextState == State.Walking) {

			climbingLadder = null;
			rigidbody2D.isKinematic = false;
		}
		else if (nextState == State.Jumping) {

			shouldJump = true; 
			soundEffectPlayer.PlayWalkEffect(false);
			soundEffectPlayer.PlayJumpEffect();
		}
		else if (nextState == State.Climbing) {

			rigidbody2D.isKinematic = true;
		}
		else if (nextState == State.Dead) {

			// Play the death animation
			if (anim != null) anim.SetBool("Death", true);
			
			// Set Mario's velocity to 0, and increase his mass so that the barrels can't push him around :)
			rigidbody2D.velocity = Vector2.zero;
			rigidbody2D.mass = 1000;
			
			// Stop the background music & walking sound, and play the death sound effect instead
			soundEffectPlayer.PlayWalkEffect(false);
			soundEffectPlayer.StopBackgroundMusic();
			soundEffectPlayer.PlayDieEffect();
			
			// Don't spawn any more barrels
			if (barrelSpawner != null) barrelSpawner.Stop();
			
			// Show the Game Over sprite
			if (gameOverSprite != null) gameOverSprite.enabled = true;
		}

		currentState = nextState;
	}
	
	
	void FixedUpdate () {

		if (currentState == State.Jumping) {

			if (shouldJump) {

				// Add a vertical force to the player.
				rigidbody2D.AddForce(new Vector2(0f, jumpForce));
		
				// Make sure the player can't jump again until the jump conditions from Update are satisfied.
				shouldJump = false;

				timeSinceJumpStarted = 0.0f;
			}
			else {
				timeSinceJumpStarted += Time.fixedDeltaTime;
			}
		}
	}
	
	
	void Flip () {
		
		facingRight = !facingRight;
		
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}
	
	Collider2D FindLadderInDirection(int dir, float distance) {
		
		RaycastHit2D hit = Physics2D.Raycast(bottomTransform.position, new Vector2(0, dir), distance, 1 << LayerMask.NameToLayer("Ladder"));  
		return hit.collider;
	}
	
	void OnCollisionEnter2D(Collision2D collision) {
		
		if (collision.collider.gameObject.name.Contains ("Barrel")) {
			SwitchToState(State.Dead);
		}
	}
	
	void OnTriggerEnter2D(Collider2D otherCollider) {
		
		if (otherCollider.gameObject.name == "WinArea" && !hasWon) {
			Debug.Log("You win!");
			
			hasWon = true;
			
			// Stop background music and play the level completion sound effect instead
			soundEffectPlayer.StopBackgroundMusic();
			soundEffectPlayer.PlayWinEffect();
			
			// Don't spawn any more barrels
			if (barrelSpawner != null) barrelSpawner.Stop();
		}
	}
}
