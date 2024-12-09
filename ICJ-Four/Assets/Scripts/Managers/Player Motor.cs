using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMotor : MonoBehaviour
{
    public static PlayerMotor Instance;
    InputSystem_Actions playerInput;
    [Header("Input")]
    [SerializeField] public Vector2 MovementInput;
    //[SerializeField] public Vector3 Movement; This is for 3D games
    [SerializeField] public Vector2 Movement; //This is for 2D games in this case its basically a 3d game so yeah
    [SerializeField] public Vector2 MouseInput;
    [SerializeField] public bool isMovementPressed;
    [SerializeField] public bool InteractPressed;

    void Awake()
    {
        if (Instance == null)
            Instance = this;

        playerInput = new InputSystem_Actions();

        playerInput.CharactherControls.Move.started += OnMovementInput;
        playerInput.CharactherControls.Move.canceled += OnMovementInput;
        playerInput.CharactherControls.Move.performed += OnMovementInput;

        playerInput.CharactherControls.Look.started += OnMouseInput;
        playerInput.CharactherControls.Look.canceled += OnMouseInput;
        playerInput.CharactherControls.Look.performed += OnMouseInput;

        playerInput.CharactherControls.Interact.started += OnInteractInput;
        playerInput.CharactherControls.Interact.performed += OnInteractInput;
        playerInput.CharactherControls.Interact.canceled += OnInteractInput;
    }
    void OnMovementInput(InputAction.CallbackContext ctx)
    {
        MovementInput = ctx.ReadValue<Vector2>();
        //Movement.x = MovementInput.x; // 3D games
        //Movement.z = MovementInput.y;
        Movement.x = MovementInput.x; // 2D games
        Movement.y = MovementInput.y;
        isMovementPressed = MovementInput.x != 0 || MovementInput.y != 0;
    }
    void OnMouseInput(InputAction.CallbackContext ctx) => MouseInput = ctx.ReadValue<Vector2>();
    void OnInteractInput(InputAction.CallbackContext ctx) => InteractPressed = ctx.ReadValueAsButton();

    void Update()
    {
        
    }

    private void OnEnable()
    {
        playerInput.CharactherControls.Enable();
    }
    private void OnDisable()
    {
        playerInput.CharactherControls.Disable();   
    }
}
