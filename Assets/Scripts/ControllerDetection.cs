using System.Collections;
using System.Collections.Generic;
using System.Runtime.Remoting;
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
     }
    
    // This function will be called when a controller is connected
    // You can get information about the controller that was connected via the args parameter
    void OnControllerConnected(ControllerStatusChangedEventArgs args) {
//      Debug.Log("A controller was connected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
        isConnected = true;
//        Services.GameManager.player.GetComponent<Crosshair>().ChangeUiOnControllerConnect();
        Services.GameManager.playerInput.isUsingController = true;
        Services.GameManager.playerInput.lookSensitivity = Services.GameManager.playerInput.controllerSens;
        Services.GameManager.playerInput.lookSensitivityAtStart = Services.GameManager.playerInput.controllerSens;
        Services.GameManager.playerInput.aimAssistSensitivity = Services.GameManager.playerInput.controllerSens * Services.GameManager.playerInput.aimAssistFactor;
        Services.GameManager.player.GetComponent<Crosshair>().SetUiToController();

//        if (Services.GameManager.uiControls != null)
//        {
//            Services.GameManager.player.GetComponent<Crosshair>().ChangeUIOnControllerConnect();
//            Services.GameManager.playerInput.isUsingController = true;
//            Services.GameManager.playerInput.lookSensitivity = Services.GameManager.playerInput.controllerSens;
//            Services.GameManager.playerInput.lookSensitivityAtStart = Services.GameManager.playerInput.controllerSens;
//            Services.GameManager.playerInput.aimAssistSensitivity =
//            Services.GameManager.playerInput.controllerSens * Services.GameManager.playerInput.aimAssistFactor;
//        }
    }
    
    // This function will be called when a controller is fully disconnected
    // You can get information about the controller that was disconnected via the args parameter
    void OnControllerDisconnected(ControllerStatusChangedEventArgs args) {
//        Debug.Log("A controller was disconnected! Name = " + args.name + " Id = " + args.controllerId + " Type = " + args.controllerType);
        isConnected = false;
//        Services.GameManager.player.GetComponent<Crosshair>().ChangeUiOnControllerDisconnect();
        Services.GameManager.playerInput.isUsingController = false;
        Services.GameManager.playerInput.lookSensitivity = Services.GameManager.playerInput.mouseSens;
        Services.GameManager.playerInput.lookSensitivityAtStart = Services.GameManager.playerInput.mouseSens;
        Services.GameManager.playerInput.aimAssistSensitivity = Services.GameManager.playerInput.mouseSens * Services.GameManager.playerInput.aimAssistFactor;
        Services.GameManager.player.GetComponent<Crosshair>().SetUiToMouseAndKeyboard();

//        if (Services.GameManager.uiControls != null)
//        {
//            Services.GameManager.player.GetComponent<Crosshair>().ChangeUIOnControllerDisconnect();
//            Services.GameManager.playerInput.isUsingController = false;
//            Services.GameManager.playerInput.lookSensitivity = Services.GameManager.playerInput.mouseSens;
//            Services.GameManager.playerInput.lookSensitivityAtStart = Services.GameManager.playerInput.mouseSens;
//            Services.GameManager.playerInput.aimAssistSensitivity = Services.GameManager.playerInput.mouseSens * Services.GameManager.playerInput.aimAssistFactor;
//         }
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