﻿using BepuPhysics.Constraints;
using Stride.BepuPhysics.Extensions;
using Stride.BepuPhysics.Processors;
using Stride.Core;
using Stride.Core.Mathematics;
using Stride.Engine;
using Stride.Engine.Design;

namespace Stride.BepuPhysics.Components.Constraints
{
    [DataContract]
    [DefaultEntityComponentProcessor(typeof(ConstraintProcessor), ExecutionMode = ExecutionMode.Runtime)]
    [ComponentCategory("Bepu - Constraint")]
    public sealed class AngularAxisGearMotorConstraintComponent : TwoBodyConstraintComponent<AngularAxisGearMotor>
    {
        public AngularAxisGearMotorConstraintComponent() => BepuConstraint = new() { Settings = new MotorSettings(1000, 10) };

        public Vector3 LocalAxisA
        {
            get
            {
                return BepuConstraint.LocalAxisA.ToStrideVector();
            }
            set
            {
                BepuConstraint.LocalAxisA = value.ToNumericVector();
                ConstraintData?.TryUpdateDescription();
            }
        }

        public float VelocityScale
        {
            get
            {
                return BepuConstraint.VelocityScale;
            }
            set
            {
                BepuConstraint.VelocityScale = value;
                ConstraintData?.TryUpdateDescription();
            }
        }

        public float MotorDamping
        {
            get
            {
                return BepuConstraint.Settings.Damping;
            }
            set
            {
                BepuConstraint.Settings.Damping = value;
                ConstraintData?.TryUpdateDescription();
            }
        }

        public float MotorMaximumForce
        {
            get
            {
                return BepuConstraint.Settings.MaximumForce;
            }
            set
            {
                BepuConstraint.Settings.MaximumForce = value;
                ConstraintData?.TryUpdateDescription();
            }
        }
    }
}
