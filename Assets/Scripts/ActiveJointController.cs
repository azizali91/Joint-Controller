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
    public float input => Input.mousePositionDelta.x;

    public float absoluteInput => Mathf.Abs(input);


    private void Start()
    {
        ControlTailJoint();
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

        
        if (isControllInput && input != 0 && !hasHandledJoint)
        {
            Debug.Log(input);
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
