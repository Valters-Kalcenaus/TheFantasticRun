using UnityEngine;
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
using UnityEngine.InputSystem;
#endif
using System.Collections;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class FirstPersonController : MonoBehaviour
    {
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 4.0f;
        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 6.0f;
        [Tooltip("Rotation speed of the character")]
        public float RotationSpeed = 1.0f;
        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;
        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.1f;
        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;
        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;
        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.5f;
        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;
        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 90.0f;
        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -90.0f;

        [Header("Grappling")]
        public bool canGrapple = false;
        private bool isGrappling;
        private bool canMove = true; // This is used to disable movement while climbing
        private Coroutine grappleCoroutine;
        public float grappleTimeout = 2f;
        public float grappleSpeed = 5.0f;

        // cinemachine
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
        private PlayerInput _playerInput;
#endif
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;


        private bool isSpeedBoosted = false;
        private float originalSpeed;

        public void IncreaseSpeed(float multiplier, float duration)
        {
            if (!isSpeedBoosted)
            {
                isSpeedBoosted = true;
                originalSpeed = MoveSpeed; 
                MoveSpeed *= multiplier;

               
                StartCoroutine(ResetSpeedAfterDelay(duration));
            }
        }

        private IEnumerator ResetSpeedAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);
            MoveSpeed = originalSpeed;
            isSpeedBoosted = false;
        }


        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }
        }

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM && STARTER_ASSETS_PACKAGES_CHECKED
            _playerInput = GetComponent<PlayerInput>();
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            JumpAndGravity();
            GroundedCheck();
            if (canMove)
            {
                Move();
            }
            GrappleCheck();
            if (isGrappling && Grounded)
            {
                CancelGrapple();
            }
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers, QueryTriggerInteraction.Ignore);
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                _cinemachineTargetPitch += _input.look.y * RotationSpeed * deltaTimeMultiplier;
                _rotationVelocity = _input.look.x * RotationSpeed * deltaTimeMultiplier;

                _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

                CinemachineCameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
                transform.Rotate(Vector3.up * _rotationVelocity);
            }
        }

        private void Move()
        {
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            if (currentHorizontalSpeed < targetSpeed - speedOffset || currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
            if (_input.move != Vector2.zero)
            {
                inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
            }

            
            if (canMove)
            {
                _controller.Move(inputDirection.normalized * (_speed * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);
                }

                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }



        private void OnTriggerEnter(Collider other)
        {
            if (other.CompareTag("grapplingArea") && !isGrappling)
            {
                canGrapple = true;
                Debug.Log("Grapple area entered");
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.CompareTag("grapplingArea"))
            {
                canGrapple = false;
            }
        }
        private Transform FindNearestGrapplePoint(Vector3 position)
        {
            // Find all grapple points in the scene
            GameObject[] grapplePoints = GameObject.FindGameObjectsWithTag("GrapplePoint");
            Transform nearestPoint = null;
            float closestDistanceSqr = Mathf.Infinity;

            //loop to the closest one
            foreach (GameObject grapplePoint in grapplePoints)
            {
                Vector3 directionToTarget = grapplePoint.transform.position - position;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (dSqrToTarget < closestDistanceSqr)
                {
                    closestDistanceSqr = dSqrToTarget;
                    nearestPoint = grapplePoint.transform;
                }
            }

            return nearestPoint;
        }

        private void GrappleCheck()
        {
            if (canGrapple && Keyboard.current.spaceKey.wasPressedThisFrame)
            {
                Debug.Log("Grapple key pressed");
                Transform nearestGrapplePoint = FindNearestGrapplePoint(transform.position);
                if (nearestGrapplePoint != null)
                {
                    Debug.Log("Nearest grapple point found: " + nearestGrapplePoint.name);
                    StartGrapple(nearestGrapplePoint.position);
                }
                else
                {
                    Debug.Log("No grapple point found");
                }
            }
        }

        private void StartGrapple(Vector3 target)
        {
            if (isGrappling)
            {
                CancelGrapple();
            }
            grappleCoroutine = StartCoroutine(GrappleToTarget(target));
        }


        private IEnumerator GrappleToTarget(Vector3 target)
        {
            Debug.Log("Grapple started");
            canMove = false;
            isGrappling = true;
            _controller.enabled = false;

            Vector3 highPoint = new Vector3(transform.position.x, target.y, transform.position.z);

            
            while (transform.position.y < highPoint.y)
            {
                transform.position = Vector3.MoveTowards(transform.position, highPoint, grappleSpeed * Time.deltaTime);
                yield return null;
            }

            Vector3 horizontalTarget = new Vector3(target.x, transform.position.y, target.z); 
            while (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(horizontalTarget.x, 0, horizontalTarget.z)) > 0.5f)
            {
                transform.position = Vector3.MoveTowards(transform.position, horizontalTarget, grappleSpeed * Time.deltaTime);
                yield return null;
            }

            _controller.enabled = true;
            isGrappling = false;
            canMove = true;
            Debug.Log("Grapple ended");
        }

        private void CancelGrapple()
        {
            if (grappleCoroutine != null)
            {
                StopCoroutine(grappleCoroutine);
            }
            _controller.enabled = true;
            isGrappling = false;
            canMove = true;
        }


        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z), GroundedRadius);
        }
    }
}
