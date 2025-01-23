using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using NaughtyAttributes;
using TMPro.EditorUtilities;
using UnityEngine;

[RequireComponent(typeof(ConfigurableJoint))]
public class ActiveJoint : MonoBehaviour
{
    
    //Paramaters
    [SerializeField,Required] private ActiveJointController controller;
    public float targetSpring;
    public Vector2 targetAngleRange = new Vector2(20,170);
    [Header("Speed/ Duration")]
    public float rotateSpeed = 1f;
    [Space(3)]
    public TransitionTimeUnit handleUnit;
    [SerializeField] private float handle=1f;
    [Space(3)]
    public TransitionTimeUnit releaseUnit;
    [SerializeField ] private float release = 0.2f;
    
    
    //Properties
    public float HandleFactor => handleUnit == TransitionTimeUnit.Duration ? targetSpring / handle : handle;
    public float ReleaseFactor => releaseUnit == TransitionTimeUnit.Duration ? targetSpring / release : release;

    [SerializeField,ReadOnly]
    private bool _isHandled;
    public bool isHandled
    {
        get
        {
            return _isHandled;
        }
        set
        {
            if (value != _isHandled)
            {
                _isHandled = value;
                OnHandlingJointChanged();
            }
        }
    }
    [ShowNativeProperty]
    public bool isCollided { get; protected set; }
    [ShowNativeProperty]
    public float currentAngle { get; set; }

    //test
    [Range(0, -175f)] public float testAngle = 0f;

    public float eulerAngleTest;
    //Private variables
    private ConfigurableJoint joint;

    public ConfigurableJoint Joint => joint;
    private void Start()
    {
        joint = GetComponent<ConfigurableJoint>();
    }

    private void Update()
    {
 
        var angularSpringX = joint.angularXDrive;
        float currentSpring, springSpeed;
        if (isHandled)
        {
            var currentTargetAngle = controller.input > 0 ? targetAngleRange.y : targetAngleRange.x;
            currentSpring = targetSpring;
            currentAngle =  Mathf.MoveTowards(currentAngle, currentTargetAngle,
                rotateSpeed * controller.absoluteInput* Time.deltaTime);
          
            springSpeed = HandleFactor;
        }
        else
        {
            currentSpring = 0;
            springSpeed = ReleaseFactor;
        }
        
        angularSpringX.positionSpring = Mathf.MoveTowards(angularSpringX.positionSpring, currentSpring,
            springSpeed * Time.deltaTime);
        joint.targetRotation = Quaternion.Euler(currentAngle * joint.axis);
        joint.angularXDrive = angularSpringX;
    }


    private float GetCurrentAngle()
    {
        return Vector3.Angle(transform.forward, Vector3.back) - 180f;
    }
    //Methods
    private void OnHandlingJointChanged()
    {
        if (isHandled)
        {
            currentAngle = GetCurrentAngle();
            Debug.Log(transform.localEulerAngles + "  " + joint.axis + "  " + currentAngle);
        }
    }
    
    
 

    public void SetController(ActiveJointController controller) => this.controller = controller;
    private void OnCollisionEnter(Collision other) => OnCollisionChanged(true);
    private void OnCollisionExit(Collision other) => OnCollisionChanged(false);
    private void OnCollisionChanged(bool isCollided)
    {
        this.isCollided = isCollided;
        controller.OnAciveJointCollisionChanged(this);
    }

}