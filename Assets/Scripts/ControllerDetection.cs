using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class ControllerDetection
{

    public bool isConnected;
    
    public void Start() {
        // Subscribe to events
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
        ReInput.ControllerPreDisconnectEvent += OnControllerPreDisconnect;
        Debug.Log("Controller detection initialized!");
    }
    
    // This function will be called when a controller is connected
    // You can get information about the controller that was connected via the args parameter
    void OnControllerConnected(ControllerStatusChangedEventArgs args) {
        Debug.Log("A controller was connected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
        isConnected = true;
        if (Services.GameManager.uiControls != null)
        {
            Services.GameManager.uiControls.ChangeUIOnControllerConnect();
            Services.GameManager.playerInput.isUsingController = true;
            Services.GameManager.playerInput.lookSensitivity = Services.GameManager.playerInput.controllerSens;
            Debug.Log("Changed to mouse sensitivity! " + Services.GameManager.playerInput.controllerSens);
        }
    }
    
    // This function will be called when a controller is fully disconnected
    // You can get information about the controller that was disconnected via the args parameter
    void OnControllerDisconnected(ControllerStatusChangedEventArgs args) {
        Debug.Log("A controller was disconnected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
        isConnected = false;
        if (Services.GameManager.uiControls != null)
        {
            Services.GameManager.uiControls.ChangeUIOnControllerDisconnect();
            Services.GameManager.playerInput.isUsingController = false;
            Services.GameManager.playerInput.lookSensitivity = Services.GameManager.playerInput.mouseSens;
            Debug.Log("Changed to mouse sensitivity! " + Services.GameManager.playerInput.mouseSens);
        }
    }
    
    // This function will be called when a controller is about to be disconnected
    // You can get information about the controller that is being disconnected via the args parameter
    // You can use this event to save the controller's maps before it's disconnected
    void OnControllerPreDisconnect(ControllerStatusChangedEventArgs args) {
        Debug.Log("A controller is being disconnected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
    }
    
    void OnDestroy() {
        // Unsubscribe from events
        ReInput.ControllerConnectedEvent -= OnControllerConnected;
        ReInput.ControllerDisconnectedEvent -= OnControllerDisconnected;
        ReInput.ControllerPreDisconnectEvent -= OnControllerPreDisconnect;
    }
}