using System.Dynamic;
using Sandbox;
using Sandbox.Citizen;
public sealed class Pplayer : Component
{

	[Property]
	public GameObject Camera{get; set;}

	[Property]
	public CharacterController Controller{get; set;}

	[Property]
	public CitizenAnimationHelper Anims{get; set;}

	[Property]
	public float WalkSpeed{get; set;} = 100f;

	[Property]
	public float RunSpeed{get; set;} = 200f;

	[Property]
	public float JumpHeight{get;set;} = 300f;

	[Property]
	public Vector3 EyePos{get;set;}

	public Angles EyeAngles{get;set;}
	Transform init_camera_transform;



    protected override void DrawGizmos()
    {
       Gizmo.Draw.LineSphere(EyePos, 10f);
    }

    protected override void OnUpdate()
	{
		EyeAngles += Input.AnalogLook;
		EyeAngles = EyeAngles.WithPitch(MathX.Clamp(EyeAngles.pitch, -79f, 79f));
		Transform.Rotation = Rotation.FromYaw(EyeAngles.yaw);

		if (Camera != null){
			Camera.Transform.Local = init_camera_transform.RotateAround(EyePos, EyeAngles.WithYaw(0f));
		}
	}

	protected override void OnFixedUpdate() {
		base.OnFixedUpdate();

		if (Controller == null) return;


		var wish_speed = Input.Down("Run") ? RunSpeed : WalkSpeed; // is running?
		var wish_velocity = Input.AnalogMove.Normal * wish_speed * Transform.Rotation;
		Controller.Accelerate(wish_velocity);

		if (Controller.IsOnGround){
			Controller.Acceleration = 10f;
			Controller.ApplyFriction(5f);

			if (Input.Pressed("Jump")){
				Controller.Punch(Vector3.Up * JumpHeight);

				if (Anims != null){
					Anims.TriggerJump();
				}
			}
		}else{
			Controller.Acceleration = 2f;
			Controller.Velocity += Scene.PhysicsWorld.Gravity * Time.Delta;
		}
		Controller.Move();
		if (Anims != null){
			Anims.IsGrounded = Controller.IsOnGround;
			Anims.WithVelocity(Controller.Velocity);
		}
	}
    protected override void OnStart()
    {
        base.OnStart();
		if (Camera != null){
			init_camera_transform = Camera.Transform.Local;
     }
	}
}
