using UnityEngine;
using System.Collections;

public class MarioControl : MonoBehaviour {
	
	BarrelSpawner barrelSpawner;
	SoundEffectPlayer soundEffectPlayer;

	bool facingRight = true;
	bool shouldJump = false;
	bool isJumping = false;
	bool isDead = false;
	bool hasWon = false;
	
	float maxSpeed = 0.5f;
	float jumpForce = 45f;

	private bool grounded = false;
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
	
	
	void Update() {

		if (isDead) return;

		grounded = Physics2D.Raycast(bottomTransform.position, new Vector2(0, -1), 0.1f, 1 << LayerMask.NameToLayer("Ground"));  

		// If the jump button is pressed and the player is grounded then the player should jump
		if(Input.GetButtonDown("Jump") && grounded && !isJumping)
			shouldJump = true;
		else if (Input.GetAxis("Vertical") != 0 && climbingLadder == null) {
			climbingLadder = FindLadderInDirection((int)Mathf.Sign(Input.GetAxis("Vertical")), 0.02f); 
			if (climbingLadder != null) {
				rigidbody2D.isKinematic = true;
			}
		}
	}
	
	
	void FixedUpdate () {

		if (isDead) return;

		if (isJumping && grounded && rigidbody2D.velocity.y <= 0) {
			isJumping = false;
		}

		float h = Input.GetAxis("Horizontal");

		if (climbingLadder != null) {
			float v = Input.GetAxis("Vertical");

			float dir = v == 0 ? 0 : Mathf.Sign(v);

			Collider2D ladder = FindLadderInDirection((int)dir, maxSpeed * 0.5f);
			if (ladder != climbingLadder) {
				if (grounded) {
					climbingLadder = null;
					rigidbody2D.isKinematic = false;
				}
				else {
					rigidbody2D.velocity = new Vector2(0, 0);
				}
			}
			else {
				rigidbody2D.velocity = new Vector2(0, dir * maxSpeed * 0.5f);
			}

			// Play walking sound
			if (Mathf.Abs(rigidbody2D.velocity.y) > 0.01) {
				soundEffectPlayer.PlayWalkEffect(true);
			}
			else soundEffectPlayer.PlayWalkEffect(false);
		}
		else {

			float dir = h == 0 ? 0 : Mathf.Sign(h);
			rigidbody2D.velocity = new Vector2(dir * maxSpeed, rigidbody2D.velocity.y);

			if(h > 0 && !facingRight)
				Flip();
			else if(h < 0 && facingRight)
				Flip();
			
			if(shouldJump) {

				soundEffectPlayer.PlayJumpEffect();

				// Add a vertical force to the player.
				rigidbody2D.AddForce(new Vector2(0f, jumpForce));
				
				// Make sure the player can't jump again until the jump conditions from Update are satisfied.
				shouldJump = false;
				isJumping = true;
			}

			// Play walking sound
			if (!isJumping && Mathf.Abs (rigidbody2D.velocity.x) > 0.01f) {
				soundEffectPlayer.PlayWalkEffect(true);
			}
			else {
				soundEffectPlayer.PlayWalkEffect(false);
			}
		}

		// Update the animation state
		if (anim != null) anim.SetFloat("Speed", Mathf.Abs(h));
		if (anim != null) anim.SetBool("Jumping", isJumping);
		if (anim != null) anim.SetBool("Climbing", climbingLadder != null);
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

		if (collision.collider.gameObject.name == "Barrel" && !isDead) {

			isDead = true;

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
