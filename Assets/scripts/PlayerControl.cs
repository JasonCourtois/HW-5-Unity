//--------------------------------
using UnityEngine;
using System.Collections;

//--------------------------------
public class PlayerControl : MonoBehaviour
{
	// How come this isn't appearing in inspector?
	[SerializeField] private GameObject MenuPrefab;
	private static GameObject StaticMenuPrefab;
	//--------------------------------
	public enum FACEDIRECTION {FACELEFT = -1, FACERIGHT = 1};
	public FACEDIRECTION Facing = FACEDIRECTION.FACERIGHT;
	public LayerMask GroundLayer;
	private Rigidbody2D ThisBody = null;
	private static Transform ThisTransform = null;
	private Animator ThisAnimator = null;
	public CircleCollider2D FeetCollider = null;
	public bool isGrounded = false;
	public string HorzAxis = "Horizontal";
	public string JumpButton = "Jump";
	public float MaxSpeed = 50f;
	public float JumpPower = 600;
	public float JumpTimeOut = 1f;
	private bool CanJump = true;
	
	private readonly int MovingBoolHash = Animator.StringToHash("Moving");
	public bool CanControl = true;
	public static PlayerControl PlayerInstance = null;
	public static Vector3 PlayerPosition => new Vector3(ThisTransform.position.x, ThisTransform.position.y + playerHeadOffset, ThisTransform.position.z);
	private static float playerHeadOffset = 0.4f;
	
	//--------------------------------
	public static float Health
	{
		get
		{
			return _Health;
		}

		set
		{
			_Health = value;

			//If we are dead, then end game
			if(_Health <= 0)
			{
				Die();
			}
		}
	}

	[SerializeField]
	private static float _Health = 100f;
	//--------------------------------
	// Use this for initialization
	void Awake ()
	{
		//Get transform and rigid body
		ThisBody = GetComponent<Rigidbody2D>();
		ThisTransform = GetComponent<Transform>();
		ThisAnimator = GetComponent<Animator>();
		
		//Set static instance
		PlayerInstance = this;
		StaticMenuPrefab = MenuPrefab;
	}
	//--------------------------------
	void Start()
	{
		//Level begins. Set starting position
		ThisTransform.position = SceneChanger.LastTarget;
	}
	//--------------------------------
	//Returns bool - is player on ground?
	private bool GetGrounded()
	{
		//Check ground
		Vector2 CircleCenter = new Vector2(ThisTransform.position.x, ThisTransform.position.y) + FeetCollider.offset;
		Collider2D[] HitColliders = Physics2D.OverlapCircleAll(CircleCenter, FeetCollider.radius, GroundLayer);
		if(HitColliders.Length > 0) return true;
		return false;
	}
	//--------------------------------
	//Flips character direction
	private void FlipDirection()
	{
		Facing = (FACEDIRECTION) ((int)Facing * -1f);
		Vector3 LocalScale = ThisTransform.localScale;
		LocalScale.x *= -1f;
		ThisTransform.localScale = LocalScale;
	}
	//--------------------------------
	//Engage jump
	private void Jump()
	{
		//If we are grounded, then jump
		if(!isGrounded || !CanJump)return;

		//Jump
		ThisBody.AddForce(Vector2.up * JumpPower);
		CanJump = false;
		Invoke ("ActivateJump", JumpTimeOut);
	}
	//--------------------------------
	//Activates can jump variable after jump timeout
	//Prevents double-jumps
	private void ActivateJump()
	{
		CanJump = true;
	}
	//--------------------------------
	// Update is called once per frame
	void FixedUpdate ()
	{
		//If we cannot control character, then exit
		if(!CanControl || Health <= 0f)
		{
			
			return;
		}

		//Update grounded status
		isGrounded = GetGrounded();
		float Horz = Input.GetAxis(HorzAxis);
		ThisAnimator.SetBool(MovingBoolHash, Horz != 0);
		ThisBody.AddForce(Vector2.right * Horz * MaxSpeed);

		if(Input.GetButton(JumpButton))
			Jump();

		//Clamp velocity
		ThisBody.linearVelocity = new Vector2(Mathf.Clamp(ThisBody.linearVelocity.x, -MaxSpeed, MaxSpeed), 
		                                Mathf.Clamp(ThisBody.linearVelocity.y, -Mathf.Infinity, JumpPower));
	
		//Flip direction if required
		if((Horz < 0f && Facing != FACEDIRECTION.FACELEFT) || (Horz > 0f && Facing != FACEDIRECTION.FACERIGHT))
			FlipDirection();

	}
	//--------------------------------
	void OnDestroy()
	{
		PlayerInstance = null;
	}
	//--------------------------------
	//Function to kill player
	static void Die()
	{
		Instantiate(StaticMenuPrefab);
		Destroy(PlayerControl.PlayerInstance.gameObject);
	}
	//--------------------------------
	//Resets player back to defaults
	public static void Reset()
	{
		Health = 100f;
		//Set to default position
		SceneChanger.LastTarget = new Vector3(1.55f,-1.63f,0f);
	}
	//--------------------------------
}
//--------------------------------