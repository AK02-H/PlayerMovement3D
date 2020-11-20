///Simple 3D Character Controller
///By Amin 'A.K' Khan
///
///A simple 3D character controller 
///Includes jumping (with a max jump counter for multiple air jumps), seperate speeds for different movement directions and camera rotation, and clamps for vertical rotation.
///Speed values and jump buttons can be set from the inspector
///Requires: a camera object to be a child of the player object; an empty transform child to act as the position for the ground check; ground objects to be marked with a 'ground' layer, that must also be set in the inspector for this script
///Press ESC to unlock the cursor during playmode

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerControl : MonoBehaviour
{
    //Components
    Camera _cam;
    Rigidbody _myRb;
    [SerializeField] Transform _footPos;    //Drag in empty transform attatched to the bottom of the player

    //Values
    [SerializeField] private float _speed = 450f, _strafeSpeed = 200f, _rotationSpeedX = 600f, _rotationSpeedY = 600f, _jumpForce = 5f;
    public int _maxJumps = 2;
    [SerializeField] private float yClampNeg = -45, yClampPos = 45;
    [SerializeField] private LayerMask _layerGround;
    [SerializeField] private KeyCode _jumpButton;

    //Variables
    float _forwardMove, _sideMove, _rotDeltY, _rotDeltX, _rotX, _rotY;
    bool _grounded;
    [SerializeField] float _footRadius = 0.8f;
    private int _jumpCounter = 0;

    private void Start()
    {
        _myRb = GetComponent<Rigidbody>();
        _cam = GetComponentInChildren<Camera>();
        Cursor.lockState = CursorLockMode.Locked;
    }
    
    private void Update()
    {

        //Get movement axis
        _forwardMove = Input.GetAxis("Vertical");
        _sideMove = Input.GetAxis("Horizontal");
        
        //Get mouse axis
        _rotDeltX = Input.GetAxis("Mouse X");
        _rotDeltY = Input.GetAxis("Mouse Y");
        
        //Set rotation values
        _rotX += (_rotDeltX * _rotationSpeedX * Time.deltaTime);
        _rotY += (_rotDeltY * _rotationSpeedY * Time.deltaTime);
        _rotY = Mathf.Clamp(_rotY, yClampNeg, yClampPos);
        
        //Apply camera rotation
        transform.localRotation = Quaternion.Euler(new Vector3(0f, _rotX, 0f));
        _cam.transform.localRotation = Quaternion.Euler(new Vector3(-_rotY, 0f, 0f));

        //Jump
        if (Input.GetKeyDown(_jumpButton))
        {
            if (_jumpCounter < _maxJumps)
            {
                _myRb.velocity += new Vector3(0, _jumpForce, 0);
                _jumpCounter++;
            }
            else
            {
                if (Physics.CheckSphere(_footPos.position, _footRadius, _layerGround))
                {
                    _jumpCounter = 0;
                }
            }
        }
    }

    private void FixedUpdate()
    {
        //Apply Movement
        Vector3 movement = (transform.forward * _forwardMove * _speed * Time.fixedDeltaTime) + (transform.right * _sideMove * _strafeSpeed * Time.fixedDeltaTime);
        _myRb.velocity = new Vector3(movement.x, _myRb.velocity.y, movement.z);
    }
    
    
    private void OnGUI()
    {
        //Lock Cursor
        if (GUI.Button(new Rect(0, 0, 100, 50), "Lock Cursor"))
        {
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    private void OnDrawGizmosSelected()
    {
        //Draw ground checker
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(_footPos.position, _footRadius);
    }

}
