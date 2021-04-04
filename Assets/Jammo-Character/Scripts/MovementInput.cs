
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//This script requires you to have setup your animator with 3 parameters, "InputMagnitude", "InputX", "InputZ"
//With a blend tree to control the inputmagnitude and allow blending between animations.
[RequireComponent(typeof(CharacterController))]
public class MovementInput : MonoBehaviour {

    public float Velocity;
    [Space]

	public Vector3 desiredMoveDirection;
	public float desiredRotationSpeed = 0.1f;
	public Animator anim;
	public float Speed;
	public float allowPlayerRotation = 0.1f;
	public Camera cam;
	public CharacterController controller;

	public Rigidbody rigidbodyVel;
	public Transform transformVel;

	[Header("Animation Smoothing")]
    [Range(0, 1f)]
    public float HorizontalAnimSmoothTime = 0.2f;
    [Range(0, 1f)]
    public float VerticalAnimTime = 0.2f;
    [Range(0,1f)]
    public float StartAnimTime = 0.3f;
    [Range(0, 1f)]
    public float StopAnimTime = 0.15f;

	// Use this for initialization
	void Start () {
		anim = this.GetComponent<Animator> ();
		cam = Camera.main;
		controller = this.GetComponent<CharacterController> ();
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = new Vector3(transformVel.position.x, transformVel.position.y - 0.45f, transformVel.position.z);
		Speed = rigidbodyVel.velocity.magnitude;

		//Physically move player

		if (Speed > allowPlayerRotation)
		{
			anim.SetFloat("Blend", Speed, StartAnimTime, Time.deltaTime);
		}
		else if (Speed < allowPlayerRotation)
		{
			anim.SetFloat("Blend", Speed, StopAnimTime, Time.deltaTime);
		}
		RotateToCamera(cam.transform);
	}

    public void LookAt(Vector3 pos)
    {
        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(pos), desiredRotationSpeed);
    }

    public void RotateToCamera(Transform t)
    {

        var camera = Camera.main;
        var forward = cam.transform.forward;
        var right = cam.transform.right;

        desiredMoveDirection = forward;

		transform.rotation = Quaternion.Slerp(t.rotation, Quaternion.LookRotation(desiredMoveDirection), desiredRotationSpeed);
    }
}
