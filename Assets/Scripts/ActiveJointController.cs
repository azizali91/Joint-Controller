using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class ActiveJointController : MonoBehaviour
{
    [Header("Active Control Joints")]
    public ActiveJoint tailJoint;
    public ActiveJoint headJoint;
    [SerializeField]
    private float oppositeJointMass = 20;
    [SerializeField]
    private float targetJointMass = 1;
 
    [Space(20)]
    public List<ConfigurableJoint> joints;
    
    //Properties
    public ActiveJoint[] controlJoints => new[] { tailJoint, headJoint };

    [SerializeField]
    private ActiveJoint _handledJoint;
    public ActiveJoint handledJoint
    {
        get
        {
            return _handledJoint;
        }
        set
        {
            if (value != _handledJoint)
            {
                int start,end, shift;
                if (value == tailJoint)
                {
                    start = 1;
                    end = 1;
                    shift = -1;
                    joints[0].connectedBody = null;
                    SetJointState(tailJoint.Joint, false);
                    SetJointState(headJoint.Joint, true);
                }
                else
                {
                    start = 0;
                    end = 2;
                    shift = 1;
                    joints[joints.Count-1].connectedBody = null;
                    SetJointState(tailJoint.Joint, true);
                    SetJointState(headJoint.Joint, false);
                }
                for (int i = start; i <= jointRigs.Length - end; i++)
                {
                    joints[i].connectedBody = jointRigs[i + shift];
                }

                for (int i = 0; i < controlJoints.Length; i++)
                    controlJoints[i].isHandled = controlJoints[i] == value;
                _handledJoint = value;
            }
        }
        
    }
    [ShowNativeProperty]
    public ActiveJoint leftJoint =>
        tailJoint.transform.position.z < headJoint.transform.position.z ? tailJoint : headJoint;
    [ShowNativeProperty]
    public ActiveJoint rightJoint => GetOppositeJoint(leftJoint);
    public bool hasHandledJoint => handledJoint != null;
    public bool isControllInput => Input.GetMouseButton(0);
    [ShowNativeProperty]
    public float input => Input.mousePositionDelta.x;
    public float absoluteInput => Mathf.Abs(input);
    [ShowNativeProperty]
    public float lastInput { get; protected set; }
    public float lastAbsoluteInput => Mathf.Abs(lastInput);
    
    
    //Private variables
    [SerializeField]
    private Rigidbody[] jointRigs;


    private void Start()
    {
        jointRigs = joints.Select(j => j.GetComponent<Rigidbody>()).ToArray();
        ControlTailJoint();
    }

    private void SetJointState(ConfigurableJoint joint, bool isLimited)
    {
        if (isLimited)
        {
            joint.yMotion = ConfigurableJointMotion.Limited;
            joint.zMotion = ConfigurableJointMotion.Limited;
            joint.angularXMotion = ConfigurableJointMotion.Limited;
        }
        else
        {
            joint.yMotion = ConfigurableJointMotion.Free;
            joint.zMotion = ConfigurableJointMotion.Free;
            joint.angularXMotion = ConfigurableJointMotion.Free;
        }
  
    }
    public void OnAciveJointCollisionChanged(ActiveJoint joint)
    {
        //Case joint fall to the ground
        if(handledJoint  == joint && joint.isCollided)
            ReleaseJoint();
        
        //Case opposite joint left from the ground
        if (GetOppositeJoint(joint))
        {
            
        }
    }
    
    public ActiveJoint GetOppositeJoint(ActiveJoint joint)
    {
        return joint == tailJoint ? headJoint : tailJoint;
    }

    private void ReleaseJoint() => handledJoint = null;
    
    private void Update()
    {

        if(hasHandledJoint && !isControllInput)
            ReleaseJoint();

        
        if (isControllInput && input != 0 && !hasHandledJoint && leftJoint.isCollided && rightJoint.isCollided)
        {
            Debug.Log(input);
            lastInput = input;
            handledJoint = input > 0 ? leftJoint : rightJoint;
        }
        

    }


    #region  Inspector Buttons + Info
    [Button]
    private void FillAllJoints()
    {
        var activeJoints = GetComponentsInChildren<ActiveJoint>();
        joints = GetComponentsInChildren<ConfigurableJoint>().ToList();
        tailJoint = activeJoints[0];
        headJoint = activeJoints[1];
        tailJoint.SetController(this);
        headJoint.SetController(this);
    }
    
    [Button]
    private void ControlTailJoint()
    {
        handledJoint = tailJoint;
    }
    
    [Button]
    private void ReleaseControl()
    {
        handledJoint = null;
    }
    
    #endregion
    
 
    
    
   

}
