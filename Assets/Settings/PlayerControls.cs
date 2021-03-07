// GENERATED AUTOMATICALLY FROM 'Assets/Settings/PlayerControls.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @PlayerControls : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @PlayerControls()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""PlayerControls"",
    ""maps"": [
        {
            ""name"": ""PlayerInput"",
            ""id"": ""aac3fb2f-a702-4458-9437-0b73339e0daf"",
            ""actions"": [
                {
                    ""name"": ""Movement"",
                    ""type"": ""PassThrough"",
                    ""id"": ""81e007f6-d20b-4188-9520-6cc721cfecf5"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Jump"",
                    ""type"": ""Button"",
                    ""id"": ""4286a67d-00c6-400d-b289-33099741f99d"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MouseX"",
                    ""type"": ""PassThrough"",
                    ""id"": ""aba0d4d3-ec16-4e01-b394-7a2a411cf34c"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""MouseY"",
                    ""type"": ""PassThrough"",
                    ""id"": ""0671e677-93c9-48bd-aa12-83e2ae34e0fe"",
                    ""expectedControlType"": ""Axis"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Crouch"",
                    ""type"": ""Button"",
                    ""id"": ""5c7e26fe-ce4e-480f-90e9-ed6415aa1739"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""ShiftWalk"",
                    ""type"": ""Button"",
                    ""id"": ""4f1d32b9-5460-4cb6-ba01-8cecb43f9ec4"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Aim"",
                    ""type"": ""Button"",
                    ""id"": ""66b52b31-95fb-4cfd-9a40-ee10abda2d6b"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Shoot"",
                    ""type"": ""Button"",
                    ""id"": ""efec4be8-00f0-422f-ba03-9f443a9d3946"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""DebugMonitor"",
                    ""type"": ""Button"",
                    ""id"": ""7eacb172-d519-40ee-bc51-039c028ba365"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""EscapeMenu"",
                    ""type"": ""Button"",
                    ""id"": ""62a482b4-7aff-4bf6-a17a-728b83235548"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Reload"",
                    ""type"": ""Button"",
                    ""id"": ""a816c5b8-fc6c-4cd1-a06a-a1a5f2b75d39"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Suicide"",
                    ""type"": ""Button"",
                    ""id"": ""6cd20dca-2e19-4bde-9944-73dcc6ec1ce1"",
                    ""expectedControlType"": ""Button"",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""WASD"",
                    ""id"": ""6e8aa65f-7cb2-44e4-8413-ec5bba369a38"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""9fedb33a-4a1b-4809-a663-912d2e9fd1dd"",
                    ""path"": ""<Keyboard>/w"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""19673417-5348-40c2-a78e-c5860a224688"",
                    ""path"": ""<Keyboard>/s"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""1a10a13e-1515-4256-b80b-0662fc1d09a4"",
                    ""path"": ""<Keyboard>/a"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""06f73591-577a-41a7-bcd1-ef07edb343a2"",
                    ""path"": ""<Keyboard>/d"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Movement"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""b19ddb12-6dac-4f1e-bacf-4a30069fd6bb"",
                    ""path"": ""<Keyboard>/space"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Jump"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""42153324-3557-4c8f-aca2-f8ce85172b2d"",
                    ""path"": ""<Mouse>/delta/x"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseX"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""cea62f44-fc47-45f7-9513-d57cad94ea96"",
                    ""path"": ""<Mouse>/delta/y"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""MouseY"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""19ba4541-9fd5-4b84-90f2-799e7785ba71"",
                    ""path"": ""<Keyboard>/leftCtrl"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Crouch"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""1b2dfc3f-7ea8-4c31-942c-3af1e5282b2b"",
                    ""path"": ""<Keyboard>/leftShift"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""ShiftWalk"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""4d7faddf-c255-4ef2-ad0a-5e9f9a976dd0"",
                    ""path"": ""<Mouse>/rightButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Aim"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""07fcb0f4-2ecc-4ba7-bf16-1bcdf09cf78c"",
                    ""path"": ""<Mouse>/leftButton"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Shoot"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""513cf210-38ae-43aa-a383-0fef840ec991"",
                    ""path"": ""<Keyboard>/tab"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""DebugMonitor"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""3d08ff27-33b9-490b-aa3d-b41c2304e06e"",
                    ""path"": ""<Keyboard>/escape"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""EscapeMenu"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""f865104c-9a84-41d9-8031-0afc06b78c16"",
                    ""path"": ""<Keyboard>/r"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Reload"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": """",
                    ""id"": ""01f44fed-fb7f-4f94-b886-ef465c9a9659"",
                    ""path"": ""<Keyboard>/numpad9"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Suicide"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": []
}");
        // PlayerInput
        m_PlayerInput = asset.FindActionMap("PlayerInput", throwIfNotFound: true);
        m_PlayerInput_Movement = m_PlayerInput.FindAction("Movement", throwIfNotFound: true);
        m_PlayerInput_Jump = m_PlayerInput.FindAction("Jump", throwIfNotFound: true);
        m_PlayerInput_MouseX = m_PlayerInput.FindAction("MouseX", throwIfNotFound: true);
        m_PlayerInput_MouseY = m_PlayerInput.FindAction("MouseY", throwIfNotFound: true);
        m_PlayerInput_Crouch = m_PlayerInput.FindAction("Crouch", throwIfNotFound: true);
        m_PlayerInput_ShiftWalk = m_PlayerInput.FindAction("ShiftWalk", throwIfNotFound: true);
        m_PlayerInput_Aim = m_PlayerInput.FindAction("Aim", throwIfNotFound: true);
        m_PlayerInput_Shoot = m_PlayerInput.FindAction("Shoot", throwIfNotFound: true);
        m_PlayerInput_DebugMonitor = m_PlayerInput.FindAction("DebugMonitor", throwIfNotFound: true);
        m_PlayerInput_EscapeMenu = m_PlayerInput.FindAction("EscapeMenu", throwIfNotFound: true);
        m_PlayerInput_Reload = m_PlayerInput.FindAction("Reload", throwIfNotFound: true);
        m_PlayerInput_Suicide = m_PlayerInput.FindAction("Suicide", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // PlayerInput
    private readonly InputActionMap m_PlayerInput;
    private IPlayerInputActions m_PlayerInputActionsCallbackInterface;
    private readonly InputAction m_PlayerInput_Movement;
    private readonly InputAction m_PlayerInput_Jump;
    private readonly InputAction m_PlayerInput_MouseX;
    private readonly InputAction m_PlayerInput_MouseY;
    private readonly InputAction m_PlayerInput_Crouch;
    private readonly InputAction m_PlayerInput_ShiftWalk;
    private readonly InputAction m_PlayerInput_Aim;
    private readonly InputAction m_PlayerInput_Shoot;
    private readonly InputAction m_PlayerInput_DebugMonitor;
    private readonly InputAction m_PlayerInput_EscapeMenu;
    private readonly InputAction m_PlayerInput_Reload;
    private readonly InputAction m_PlayerInput_Suicide;
    public struct PlayerInputActions
    {
        private @PlayerControls m_Wrapper;
        public PlayerInputActions(@PlayerControls wrapper) { m_Wrapper = wrapper; }
        public InputAction @Movement => m_Wrapper.m_PlayerInput_Movement;
        public InputAction @Jump => m_Wrapper.m_PlayerInput_Jump;
        public InputAction @MouseX => m_Wrapper.m_PlayerInput_MouseX;
        public InputAction @MouseY => m_Wrapper.m_PlayerInput_MouseY;
        public InputAction @Crouch => m_Wrapper.m_PlayerInput_Crouch;
        public InputAction @ShiftWalk => m_Wrapper.m_PlayerInput_ShiftWalk;
        public InputAction @Aim => m_Wrapper.m_PlayerInput_Aim;
        public InputAction @Shoot => m_Wrapper.m_PlayerInput_Shoot;
        public InputAction @DebugMonitor => m_Wrapper.m_PlayerInput_DebugMonitor;
        public InputAction @EscapeMenu => m_Wrapper.m_PlayerInput_EscapeMenu;
        public InputAction @Reload => m_Wrapper.m_PlayerInput_Reload;
        public InputAction @Suicide => m_Wrapper.m_PlayerInput_Suicide;
        public InputActionMap Get() { return m_Wrapper.m_PlayerInput; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(PlayerInputActions set) { return set.Get(); }
        public void SetCallbacks(IPlayerInputActions instance)
        {
            if (m_Wrapper.m_PlayerInputActionsCallbackInterface != null)
            {
                @Movement.started -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnMovement;
                @Movement.performed -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnMovement;
                @Movement.canceled -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnMovement;
                @Jump.started -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnJump;
                @Jump.performed -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnJump;
                @Jump.canceled -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnJump;
                @MouseX.started -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnMouseX;
                @MouseX.performed -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnMouseX;
                @MouseX.canceled -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnMouseX;
                @MouseY.started -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnMouseY;
                @MouseY.performed -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnMouseY;
                @MouseY.canceled -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnMouseY;
                @Crouch.started -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnCrouch;
                @Crouch.performed -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnCrouch;
                @Crouch.canceled -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnCrouch;
                @ShiftWalk.started -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnShiftWalk;
                @ShiftWalk.performed -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnShiftWalk;
                @ShiftWalk.canceled -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnShiftWalk;
                @Aim.started -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnAim;
                @Aim.performed -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnAim;
                @Aim.canceled -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnAim;
                @Shoot.started -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnShoot;
                @Shoot.performed -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnShoot;
                @Shoot.canceled -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnShoot;
                @DebugMonitor.started -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnDebugMonitor;
                @DebugMonitor.performed -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnDebugMonitor;
                @DebugMonitor.canceled -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnDebugMonitor;
                @EscapeMenu.started -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnEscapeMenu;
                @EscapeMenu.performed -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnEscapeMenu;
                @EscapeMenu.canceled -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnEscapeMenu;
                @Reload.started -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnReload;
                @Reload.performed -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnReload;
                @Reload.canceled -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnReload;
                @Suicide.started -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnSuicide;
                @Suicide.performed -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnSuicide;
                @Suicide.canceled -= m_Wrapper.m_PlayerInputActionsCallbackInterface.OnSuicide;
            }
            m_Wrapper.m_PlayerInputActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Movement.started += instance.OnMovement;
                @Movement.performed += instance.OnMovement;
                @Movement.canceled += instance.OnMovement;
                @Jump.started += instance.OnJump;
                @Jump.performed += instance.OnJump;
                @Jump.canceled += instance.OnJump;
                @MouseX.started += instance.OnMouseX;
                @MouseX.performed += instance.OnMouseX;
                @MouseX.canceled += instance.OnMouseX;
                @MouseY.started += instance.OnMouseY;
                @MouseY.performed += instance.OnMouseY;
                @MouseY.canceled += instance.OnMouseY;
                @Crouch.started += instance.OnCrouch;
                @Crouch.performed += instance.OnCrouch;
                @Crouch.canceled += instance.OnCrouch;
                @ShiftWalk.started += instance.OnShiftWalk;
                @ShiftWalk.performed += instance.OnShiftWalk;
                @ShiftWalk.canceled += instance.OnShiftWalk;
                @Aim.started += instance.OnAim;
                @Aim.performed += instance.OnAim;
                @Aim.canceled += instance.OnAim;
                @Shoot.started += instance.OnShoot;
                @Shoot.performed += instance.OnShoot;
                @Shoot.canceled += instance.OnShoot;
                @DebugMonitor.started += instance.OnDebugMonitor;
                @DebugMonitor.performed += instance.OnDebugMonitor;
                @DebugMonitor.canceled += instance.OnDebugMonitor;
                @EscapeMenu.started += instance.OnEscapeMenu;
                @EscapeMenu.performed += instance.OnEscapeMenu;
                @EscapeMenu.canceled += instance.OnEscapeMenu;
                @Reload.started += instance.OnReload;
                @Reload.performed += instance.OnReload;
                @Reload.canceled += instance.OnReload;
                @Suicide.started += instance.OnSuicide;
                @Suicide.performed += instance.OnSuicide;
                @Suicide.canceled += instance.OnSuicide;
            }
        }
    }
    public PlayerInputActions @PlayerInput => new PlayerInputActions(this);
    public interface IPlayerInputActions
    {
        void OnMovement(InputAction.CallbackContext context);
        void OnJump(InputAction.CallbackContext context);
        void OnMouseX(InputAction.CallbackContext context);
        void OnMouseY(InputAction.CallbackContext context);
        void OnCrouch(InputAction.CallbackContext context);
        void OnShiftWalk(InputAction.CallbackContext context);
        void OnAim(InputAction.CallbackContext context);
        void OnShoot(InputAction.CallbackContext context);
        void OnDebugMonitor(InputAction.CallbackContext context);
        void OnEscapeMenu(InputAction.CallbackContext context);
        void OnReload(InputAction.CallbackContext context);
        void OnSuicide(InputAction.CallbackContext context);
    }
}
