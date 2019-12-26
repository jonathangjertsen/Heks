using UnityEngine;

namespace CreaturePhysics
{
    public class CreaturePhysics : IFlipX
    {
        private readonly Rigidbody2D rigidBody2d;
        private readonly Transform transform;
        readonly float axCoeffX;
        readonly float axCoeffY;
        readonly float rotCoeff;
        readonly float maxVelocityX;
        readonly float maxVelocityY;
        readonly Vector3 initialScale;

        private bool flipX;
        public bool FlipX { get => flipX; set
            {
                flipX = value;
                if (value)
                {
                    transform.localScale = new Vector3(-initialScale.x, initialScale.y, initialScale.z);
                }
                else
                {
                    transform.localScale = initialScale;
                }
            }
        }

        public CreaturePhysics(
            MonoBehaviour bh,
            float axCoeffX = 0.01f,
            float axCoeffY = 0.03f,
            float rotCoeff = 1f,
            float maxVelocityX = 10.0f,
            float maxVelocityY = 10.0f
        )
        {
            rigidBody2d = bh.GetComponent<Rigidbody2D>();
            transform = bh.transform;
            initialScale = transform.localScale;
            this.axCoeffX = axCoeffX;
            this.axCoeffY = axCoeffY;
            this.rotCoeff = rotCoeff;
            this.maxVelocityX = maxVelocityX;
            this.maxVelocityY = maxVelocityY;
            FlipX = false;
        }

        public void ApproachVelocity(bool updateX, bool updateY, float velocityXTarget, float velocityYTarget)
        {
            float axX = 0;
            float axY = 0;

            if (updateX)
            {
                axX = axCoeffX * (velocityXTarget - rigidBody2d.velocity.x);
            }

            if (updateY)
            {
                axY = axCoeffY * (velocityYTarget - rigidBody2d.velocity.y);
            }

            if (updateX || updateY)
            {
                rigidBody2d.velocity += new Vector2(axX, axY);
            }
        }

        public void ApproachVelocity(float velocityXTarget, float velocityYTarget)
        {
            ApproachVelocity(true, true, velocityXTarget, velocityYTarget);
        }

        public void ApproachAngularVelocity(float velocityXTarget, float velocityYTarget)
        {
            float targetRotation = (float)System.Math.Atan2(velocityYTarget, velocityXTarget);
            float currentRotation = rigidBody2d.rotation;
            rigidBody2d.angularVelocity += rotCoeff * (targetRotation - currentRotation);
        }

        public void ApproachAngle(float dy, float dx)
        {
            float targetRotation = (float)System.Math.Atan2(dy, dx);
            float currentRotation = rigidBody2d.rotation;
            rigidBody2d.rotation += rotCoeff * (targetRotation - currentRotation);
        }

        public void LookAt(Transform transform)
        {
            if (FlipX)
            {
                this.transform.right = transform.position - this.transform.position;
            }
            else
            {
                this.transform.right = this.transform.position - transform.position;
                
            }
        }

        public void Jump(float force)
        {
            rigidBody2d.velocity += new Vector2(0, force);
        }

        public void Recoil(float torque)
        {
            if (FlipX)
            {
                rigidBody2d.angularVelocity += torque;
            }
            else
            {
                rigidBody2d.angularVelocity -= torque;
            }
        }
    }
}
