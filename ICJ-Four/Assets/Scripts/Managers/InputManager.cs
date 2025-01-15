using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using static UnityEditor.Experimental.GraphView.GraphView;

public class InputManager : MonoBehaviour
{
    public static InputManager Instance { get; private set; }
	InputSystem_Actions playerInput;

	[Layer] int layerMask;
    public bool MovementFor3D;
    [Header("Input")]
    [SerializeField] public Vector2 movementInput, mouseInput, pointerPos;
    [SerializeField] public Vector3 movement;
    [SerializeField] public bool isMovementPressed, interactPressed;


	[Header("Singleton")]
	public bool dontDestroyOnLoad = true;

	void Awake()
    {
		if (Instance == null)
			Instance = this;
		else
			Destroy(this.gameObject);

		if (dontDestroyOnLoad)
			DontDestroyOnLoad(this.gameObject);

		playerInput = new InputSystem_Actions();

        playerInput.Controls.Move.started += OnMovementInput;
        playerInput.Controls.Move.canceled += OnMovementInput;
        playerInput.Controls.Move.performed += OnMovementInput;

        playerInput.Controls.Look.started += OnmouseInput;
        playerInput.Controls.Look.canceled += OnmouseInput;
        playerInput.Controls.Look.performed += OnmouseInput;

        playerInput.Controls.Interact.started += OnInteractInput;
        playerInput.Controls.Interact.performed += OnInteractInput;
        playerInput.Controls.Interact.canceled += OnInteractInput;

		playerInput.UI.Point.started += OnPointerInput;
		playerInput.UI.Point.performed += OnPointerInput;
		playerInput.UI.Point.canceled += OnPointerInput;
    }
    void OnMovementInput(InputAction.CallbackContext ctx)
    {
        movementInput = ctx.ReadValue<Vector2>();
        movement.x = movementInput.x;
        if (MovementFor3D) 
            movement.z = movementInput.y;
        else
            movement.y = movementInput.y;

        isMovementPressed = movementInput.x != 0 || movementInput.y != 0;
    }
    void OnmouseInput(InputAction.CallbackContext ctx) => mouseInput = ctx.ReadValue<Vector2>();
    void OnPointerInput(InputAction.CallbackContext ctx) => pointerPos = ctx.ReadValue<Vector2>();
    void OnInteractInput(InputAction.CallbackContext ctx) => interactPressed = ctx.ReadValueAsButton();
   
	private void OnEnable()
    {
        playerInput.Controls.Enable();
    }
    private void OnDisable()
    {
        playerInput.Controls.Disable();
    }

	// holding bools demo
	/*
	public void OnFire(InputAction.CallbackContext context)
	{
		switch (context.phase)
		{
			case InputActionPhase.Started:
				if (context.interaction is SlowTapInteraction)
					m_Charging = true;
				break;

			case InputActionPhase.Canceled:
				m_Charging = false;
				break;
		}
	}
    */

}
