using Godot;
using System;
using System.Net;

public partial class FpScontroller : CharacterBody3D
{
	[Export] public float WalkSpeed = 20f;
	[Export] public float SprintSpeed = 25f;
	[Export] public float JumpVelocity = 4.5f;
	[Export] float lookSensitivity = 0.006f;
	[Export] bool AutoBhop = true;
	[Export] float HeadBobAmount = 0.08f;
	[Export] float HeadBobFrequency = 1.2f;
	[Export] float groundAcceleration = 14.0f;
	[Export] float groundDecelleration = 10.0f;
	[Export] float groundFrictionOG = 6f;
	float groundFriction;
	//[Export] PackedScene BulletScene;
	//[Export] AnimationTree AnimationTree;
	[Export] Node3D WorldModel;
	[Export] Node3D Camera;

	const float CrouchTranslate = 0.7f;
	const float CrouchJump = CrouchTranslate * 0.9f;
	bool isCrouched = false;
	float originalHeight;
	[Export] float maxCrouchWalkSpeed = 10f;

	float stamina = 100f;
	[Export] float maxstamina = 100f;
	[Export] float staminaregen = 10f;
	//[Export] float weight = 15f; //TEMP
	[Export] float staminadrain = 0.01f;

	const float maxStepHeight = 0.5f;
	bool snappedToStairsLastFrame = false;
	bool outOfStamina = false;
	ulong lastFrameWasOnFloor = ulong.MinValue;

	Vector3 WishedDirection = Vector3.Zero;
	float HeadBobTime = 0;
	private Vector3 CameraBaseOffset;

	[Export] float airCap = 0.85f;
	[Export] float airAccel = 800f;
	[Export] float airMoveSpeed = 500f;

	//PackedScene BulletScene = GD.Load<PackedScene>("res://Bullet.tscn");

	public bool hasPackage = false;
	bool packageAssigned = false;

	public bool isStunned = false;
	public float stunTimer = 0f;
	float stunDuration = 2f;

	[Export] float grappleRange = 30f;
	[Export] float pullForce = 30f;
	[Export] float swingForce = 20f;
	[Export] float maxGrappleSpeed = 35f;
	[Export] float retractTreshold = 2.5f;

	public enum GrappleState { Idle, Pulling, Swinging }
	public GrappleState Grapple = GrappleState.Idle;
	private Vector3 _grapplePoint;
	private RayCast3D _grappleRay;
	MeshInstance3D _grappleRope;
	ImmediateMesh _grappleMesh;

	Vector3 lastPosition;

    Node gameData;
	//tie kas zino
	public float GetMoveSpeed(double delta)
	{
		if ((isCrouched))
		{
			return WalkSpeed * 0.6f;
		}
		if (stamina > maxstamina * 0.2) outOfStamina = false;
		if (!outOfStamina && stamina > 0)
		{
			if (Input.IsActionPressed("sprint"))
			{
				stamina -= staminadrain * (float)delta;
				return SprintSpeed;
			}
			else
			{
				stamina += staminaregen * (float)delta;
				return WalkSpeed;
			}
		}
		else
		{
			outOfStamina = true;
			stamina += staminaregen * (float)delta;
			return WalkSpeed;
		}
	}
	public override void _Ready()
	{
		lastPosition = GlobalPosition;
		gameData = GetNode<Node>("/root/GameData");
		//      PackageController = GetTree().GetFirstNodeInGroup("PMC") as PackageMasterControler;

		//if(PackageController == null)
		//{
		//	GD.Print("PMC null");
		//}

		foreach (var child in WorldModel.FindChildren("*", "VisualInstance3D"))
		{
			if (child is VisualInstance3D visual)
			{
				visual.SetLayerMaskValue(1, false);
				visual.SetLayerMaskValue(2, true);
			}
		}
		CameraBaseOffset = Camera.Transform.Origin;
		var collider = GetNode<CollisionShape3D>("CollisionShape3D");
		if (collider.Shape is CapsuleShape3D capsule)
		{
			originalHeight = capsule.Height;
		}
		groundFriction = groundFrictionOG;

		_grappleRay = new RayCast3D();
		_grappleRay.TargetPosition = new Vector3(0, 0, -grappleRange);
		_grappleRay.CollisionMask = 1;
		Camera.CallDeferred("add_child", _grappleRay);

		_grappleMesh = new ImmediateMesh();
		_grappleRope = new MeshInstance3D();
		_grappleRope.Mesh = _grappleMesh;
		var mat = new StandardMaterial3D();
		mat.ShadingMode = BaseMaterial3D.ShadingModeEnum.Unshaded;
		mat.AlbedoColor = Colors.White;
		mat.VertexColorUseAsAlbedo = true;
		_grappleRope.CastShadow = GeometryInstance3D.ShadowCastingSetting.Off;
		_grappleRope.MaterialOverride = mat;
		GetTree().Root.CallDeferred("add_child", _grappleRope);
		//staminadrain *= weight;
	}
	public override void _UnhandledInput(InputEvent @event)
	{
		var cam = Camera;
		//      if (@event is InputEventMouseButton)
		//{
		//	Input.MouseMode = Input.MouseModeEnum.Captured;
		//}
		//else if (@event.IsActionPressed("ui_cancel")){
		//	Input.MouseMode = Input.MouseModeEnum.Visible;
		//}

		if (Input.MouseMode == Input.MouseModeEnum.Captured)
		{
			if (@event is InputEventMouseMotion mouseEvent)
			{
				RotateY(-mouseEvent.Relative.X * lookSensitivity / 100);
				cam.RotateX(-mouseEvent.Relative.Y * lookSensitivity / 100);
				Vector3 rot = cam.Rotation;
				rot.X = Mathf.Clamp(rot.X, Mathf.DegToRad(-90f), Mathf.DegToRad(90f));
				cam.Rotation = rot;

			}
		}
		//if (@event is InputEventMouseButton)
		//{
		//	if (Input.IsActionJustPressed("fire"))
		//	{
		//		Fire();
		//	}
		//}
	}
	public void _HeadBobEffect(double delta)
	{
		var cam = Camera;

		HeadBobTime += (float)delta * Velocity.Length();
		Vector3 v = new Vector3(
			Mathf.Cos(HeadBobTime * HeadBobFrequency) * 0.5f * HeadBobAmount,
			Mathf.Sin(HeadBobTime * HeadBobFrequency) * HeadBobAmount,
			0);
		cam.Transform = cam.Transform with { Origin = CameraBaseOffset + v };
	}
	public bool IsSurfaceTooSteep(Vector3 normal)
	{
		return normal.AngleTo(Vector3.Up) > FloorMaxAngle;
	}
	public bool _RunBodyTestMotion(Transform3D from, Vector3 motion, PhysicsTestMotionResult3D result = null)
	{
		result ??= new PhysicsTestMotionResult3D();
		var parameters = new PhysicsTestMotionParameters3D();
		parameters.From = from;
		parameters.Motion = motion;
		return PhysicsServer3D.BodyTestMotion(GetRid(), parameters, result);
	}
	public void _SnapDownToStairsCheck()
	{
		var ray = GetNode<RayCast3D>("StairsBelowRaycast");
		bool snaped = false;
		bool floorBelow = ray.IsColliding() && !IsSurfaceTooSteep(ray.GetCollisionNormal());
		bool wasOnFloorLastFrame = Engine.GetPhysicsFrames() - lastFrameWasOnFloor == 1;
		if (!IsOnFloor() && Velocity.Y <= 0 && floorBelow && (wasOnFloorLastFrame || snappedToStairsLastFrame))
		{
			var bodyTestResult = new PhysicsTestMotionResult3D();
			if (_RunBodyTestMotion(GlobalTransform, new Vector3(0, -maxStepHeight, 0), bodyTestResult))
			{
				Vector3 pos = Position;
				pos.Y += bodyTestResult.GetTravel().Y;
				GlobalPosition = pos;
				ApplyFloorSnap();
				snaped = true;
			}
		}
		snappedToStairsLastFrame = snaped;
	}

	private bool _SnapUpStairsCheck(double delta)
	{
		if (!IsOnFloor() && !snappedToStairsLastFrame)
			return false;
		Vector3 horizontalVelocity = new Vector3(Velocity.X, 0, Velocity.Z);
		if (Velocity.Y > 0 || horizontalVelocity.Length() == 0)
			return false;
		Vector3 expectedMoveMotion = horizontalVelocity * (float)delta;
		Transform3D stepPosWithClearance = GlobalTransform.Translated(expectedMoveMotion + new Vector3(0, maxStepHeight * 2, 0));
		var downCheckResult = new PhysicsTestMotionResult3D();
		bool downHit = _RunBodyTestMotion(stepPosWithClearance, new Vector3(0, -maxStepHeight * 2, 0), downCheckResult);
		if (!downHit) return false;
		var collider = downCheckResult.GetCollider();
		if (!(collider.IsClass("StaticBody3D") || collider.IsClass("CSGShape3D")))
			return false;
		float stepHeight = (stepPosWithClearance.Origin.Y + downCheckResult.GetTravel().Y) - GlobalPosition.Y;
		if (stepHeight > maxStepHeight || stepHeight <= 0.01f || (downCheckResult.GetCollisionPoint().Y - GlobalPosition.Y) > maxStepHeight)
			return false;
		var ray = GetNode<RayCast3D>("%StairsAheadRaycast");
		ray.GlobalPosition = downCheckResult.GetCollisionPoint() + new Vector3(0, maxStepHeight, 0) + expectedMoveMotion.Normalized() * 0.1f;
		ray.ForceRaycastUpdate();

		if (ray.IsColliding() && !IsSurfaceTooSteep(ray.GetCollisionNormal()))
		{
			// Move player on top of step
			GlobalPosition = stepPosWithClearance.Origin + downCheckResult.GetTravel();
			ApplyFloorSnap();
			snappedToStairsLastFrame = true;
			return true;
		}

		return false;
	}
	public void _HandleAirPhysics(double delta)
	{

		Vector3 v = Velocity;
		v.Y -= (float)ProjectSettings.GetSetting("physics/3d/default_gravity") * (float)delta;
		Velocity = v;

		var curSpeedInWishDir = Velocity.Dot(WishedDirection);
		var cappedSpeed = Mathf.Min((airMoveSpeed * WishedDirection).Length(), airCap);
		var addSpeedTillCap = cappedSpeed - curSpeedInWishDir;
		if (addSpeedTillCap > 0)
		{
			float accelSpeed = airAccel * airMoveSpeed * (float)delta;
			accelSpeed = Mathf.Min(accelSpeed, addSpeedTillCap);
			v = Velocity;
			v.X += accelSpeed * WishedDirection.X;
			//v.Y += accelSpeed * WishedDirection.Y;
			v.Z += accelSpeed * WishedDirection.Z;
			Velocity = v;
		}
		if (IsOnWall())
		{
			if (IsSurfaceTooSteep(GetWallNormal()))
			{
				MotionMode = CharacterBody3D.MotionModeEnum.Floating;
			}
			else
			{
				MotionMode = CharacterBody3D.MotionModeEnum.Grounded;
			}
			clipVelocity(GetWallNormal(), 1);
		}
	}
	public void _HandleGroundPhysics(double delta)
	{
		var currentSpeedIWdir = Velocity.Dot(WishedDirection);
		var addSpeedTillCap = GetMoveSpeed(delta) - currentSpeedIWdir;
		stamina = Math.Clamp(stamina, 0, maxstamina);
		if (addSpeedTillCap > 0)
		{
			float accelerationSpeed = groundAcceleration * (float)delta * GetMoveSpeed(delta);
			accelerationSpeed = Mathf.Min(accelerationSpeed, addSpeedTillCap);
			Vector3 v = Velocity;
			v += WishedDirection * accelerationSpeed;
			Velocity = v;
		}

		var control = Mathf.Max(Velocity.Length(), groundDecelleration);
		var drop = control * groundFriction * delta;
		var newSpeed = Mathf.Max(Velocity.Length() - drop, 0.0);
		if (Velocity.Length() > 0)
		{
			newSpeed /= Velocity.Length();
		}
		Vector3 ve = Velocity;
		ve *= (float)newSpeed;
		Velocity = ve;

	}
	public override void _PhysicsProcess(double delta)
	{
		if (IsOnFloor()) lastFrameWasOnFloor = Engine.GetPhysicsFrames();
		var inputDirection = Input.GetVector("left", "right", "up", "down").Normalized();

		WishedDirection = GlobalTransform.Basis * new Vector3(inputDirection.X, 0f, inputDirection.Y);
		WishedDirection.Y = 0;
		WishedDirection = WishedDirection.Normalized();
		if (isStunned && stunTimer < stunDuration)
		{
			stunTimer += (float)delta;
			WishedDirection = Vector3.Zero;
		}
		else
		{
			isStunned = false;
			stunTimer = 0f;
		}
		if (!isStunned)
		{
			HandleCrouching();
			_HandleGrapple(delta);
		}
		if (IsOnFloor() || snappedToStairsLastFrame)
		{
			//GD.Print(Velocity.Length());
			if (isCrouched && Velocity.Length() > 0f) //&& Velocity.Length() > maxCrouchWalkSpeed)
			{
				//WishedDirection = Vector3.Zero;
				Vector3 rightAxis = GlobalTransform.Basis.X;
				Vector3 floorNormal = GetFloorNormal();
				Vector3 v = Velocity;
				v += rightAxis * WishedDirection.X * WalkSpeed * 0.3f * (float)delta;
				if (floorNormal != Vector3.Zero && floorNormal != Vector3.Up)
				{
					float gravity = (float)ProjectSettings.GetSetting("physics/3d/default_gravity");
					Vector3 gravVec = new Vector3(0, -gravity, 0);
					Vector3 slopeGrav = gravVec - floorNormal * gravVec.Dot(floorNormal);

					v += slopeGrav * (float)delta;
					Velocity = v;
				}
			}
			else if (isCrouched && WishedDirection.X != 0f && WishedDirection.Z != 0f)
			{
				_HeadBobEffect(delta);
			}
			_HandleGroundPhysics(delta);
            if (IsOnFloor() && Velocity.Y <= 0)
            {
                Velocity = Velocity.Slide(GetFloorNormal());
            }
            if (!isStunned && Input.IsActionJustPressed("jump") || AutoBhop == true && Input.IsActionPressed("jump") && !isStunned)
			{
				Vector3 v = Velocity;
				v.Y += JumpVelocity;
				Velocity = v;
			}

		}
		else if (Grapple != GrappleState.Idle)
		{
			Vector3 v = Velocity;
			v.Y -= (float)ProjectSettings.GetSetting("physics/3d/default_gravity") * (float)delta;
			Velocity = v;
		}
		else _HandleAirPhysics(delta);
		if (!_SnapUpStairsCheck(delta))
		{
			MoveAndSlide();
			_SnapDownToStairsCheck();
		}
		//GD.Print("Speed: " + Velocity.Length());
		if (gameData == null)
		{
			GD.PrintErr("FpScontroller: gameData not assigned!");
		}
		else
		{
			Node3D activeSource = gameData.Get("activePackageSource").As<Node3D>();
			Node3D activeDest = gameData.Get("activePackageDestination").As<Node3D>();

			if (activeSource != null && hasPackage == false &&
				GlobalPosition.DistanceTo(activeSource.GlobalPosition) < 3f)
			{
				hasPackage = true;
				activeSource.Set("hasPackage", false);
				gameData.Set("hasPackage", true);
			}

			if (activeDest != null && hasPackage == true &&
				GlobalPosition.DistanceTo(activeDest.GlobalPosition) < 3f)
			{
				hasPackage = false;
				gameData.Set("packageActive", false);
				gameData.Set("hasPackage", false);
                gameData.Set("deliveriesComplete", gameData.Get("deliveriesComplete").AsInt32() + 1);
            }
		}
		if (isStunned)
		{
			//GD.Print("Player is stunned for :" + (stunDuration - stunTimer));
		}
		//AnimationTree.Set("parameters/BlendSpace1D/blend_position", Velocity.Length() / SprintSpeed);
		gameData.Set("distanceCovered", gameData.Get("distanceCovered").AsSingle() + lastPosition.DistanceTo(GlobalPosition));
	}
	public void clipVelocity(Vector3 normal, float overbounce)
	{
		var backoff = Velocity.Dot(normal) * overbounce;
		if (backoff > 0)
		{
			return;
		}
		Vector3 change = normal * backoff;
		Velocity -= change;
		var adjust = Velocity.Dot(normal);
		if (adjust < 0)
		{
			Velocity -= normal * adjust;
		}
	}
	public bool isSurfaceTooSteep(Vector3 normal)
	{
		var maxSlopeAngDot = Vector3.Up.Rotated(new Vector3(1, 0, 0), FloorMaxAngle).Dot(Vector3.Up);
		if (normal.Dot(Vector3.Up) < maxSlopeAngDot)
		{
			return true;
		}
		return false;
	}
	public void HandleCrouching()
	{
		if (Input.IsActionPressed("crouch"))
		{
			isCrouched = true;
			//GD.Print("Crouch");
		}
		else if (isCrouched && !TestMove(GlobalTransform, new Vector3(0, CrouchTranslate, 0)))
		{
			isCrouched = false;
		}
		Vector3 i = Vector3.Zero;
		var head = GetNode<Node3D>("%Head");


		var collider = GetNode<CollisionShape3D>("%CollisionShape3D");
		if (collider.Shape is CapsuleShape3D capsule)
		{
			if (isCrouched)
			{
				i.Y = -CrouchTranslate;
				capsule.Height = originalHeight - CrouchTranslate;
				//if (Velocity.Length() < maxCrouchWalkSpeed && WishedDirection.X != 0f && WishedDirection.Z != 0f)
				//{
				//	groundFriction = groundFrictionOG;
				//	//GD.Print("Stopping Slide");
				//}
				//else groundFriction = groundFrictionOG / 15;
				groundFriction = groundFrictionOG / 15;
			}
			else
			{
				i.Y = 0;
				capsule.Height = originalHeight;
				groundFriction = groundFrictionOG;
			}
			head.Position = i;
			Vector3 j = Vector3.Zero;
			j.Y = capsule.Height / 2;
			collider.Position = j;
		}

	}
	public void _HandleGrapple(double delta)
	{
        bool grappleHoldMode = gameData.Get("grappleHoldMode").AsBool();

        bool grapplePressed = Input.IsActionJustPressed("grapple");
        bool grappleHeld = Input.IsActionPressed("grapple");
        bool grappleReleased = Input.IsActionJustReleased("grapple");

        bool shouldStartGrapple = grapplePressed; 
        bool shouldStopGrapple;

        if (grappleHoldMode)
        {
            shouldStartGrapple = grapplePressed;
            shouldStopGrapple = grappleReleased;
        }
        else
        {
            shouldStartGrapple = grapplePressed && Grapple == GrappleState.Idle;
            shouldStopGrapple = grapplePressed && Grapple != GrappleState.Idle;
        }

        if (shouldStartGrapple && Grapple == GrappleState.Idle)
        {
            if (_grappleRay == null || !_grappleRay.IsInsideTree())
            {
                _grappleRay = new RayCast3D();
                _grappleRay.TargetPosition = new Vector3(0, 0, -grappleRange);
                _grappleRay.CollisionMask = 1;
                Camera.AddChild(_grappleRay);
            }
            _grappleRay.ForceRaycastUpdate();
            if (_grappleRay.IsColliding())
            {
                _grapplePoint = _grappleRay.GetCollisionPoint();
                Grapple = GrappleState.Pulling;
            }
        }
        else if (shouldStopGrapple)
        {
            Grapple = GrappleState.Idle;
            return;
        }

        if (Grapple == GrappleState.Pulling)
		{
			Vector3 toPoint = _grapplePoint - GlobalPosition;
			Vector3 v = Velocity;
			v += toPoint.Normalized() * pullForce * (float)delta;
			//if (v.Length() > maxGrappleSpeed)
			//	v = v.Normalized() * maxGrappleSpeed;
			Velocity = v;

			if (toPoint.Length() < retractTreshold)
				Grapple = GrappleState.Idle;
			else if (toPoint.Length() < grappleRange * 0.6f)
				Grapple = GrappleState.Swinging;
		}
		else if (Grapple == GrappleState.Swinging)
		{
			Vector3 ropeDir = (_grapplePoint - GlobalPosition).Normalized();
			Vector3 v = Velocity;
			float awaySpeed = v.Dot(-ropeDir);
			if (awaySpeed > 0)
				v += ropeDir * awaySpeed; // cancel pull-away component
			v += ropeDir * swingForce * (float)delta;
			Velocity = v;

			if ((_grapplePoint - GlobalPosition).Length() < retractTreshold)
				Grapple = GrappleState.Idle;
		}
		_grappleMesh.ClearSurfaces();
		if (Grapple != GrappleState.Idle)
		{
			var cam = GetViewport().GetCamera3D();
			Vector3 ropeStart = cam.GlobalPosition + cam.GlobalTransform.Basis.X * 0.2f - cam.GlobalTransform.Basis.Y * 0.1f;
			_grappleMesh.SurfaceBegin(Mesh.PrimitiveType.Lines);
			_grappleMesh.SurfaceSetColor(Colors.White);
			_grappleMesh.SurfaceAddVertex(ropeStart);
			_grappleMesh.SurfaceSetColor(Colors.White);
			_grappleMesh.SurfaceAddVertex(_grapplePoint);
			_grappleMesh.SurfaceEnd();
		}
	}
	public override void _Process(double delta)
	{
		if (gameData.Get("gameOver").AsBool())
		{
			if (_grappleMesh != null)
			{
				_grappleMesh.ClearSurfaces();
			}
		}

	}
    public override void _ExitTree()
    {
        if (_grappleRope != null && IsInstanceValid(_grappleRope))
        {
            _grappleRope.QueueFree();
            _grappleRope = null;
        }
    }
}
