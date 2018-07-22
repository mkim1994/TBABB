using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Services {
    public static GameManager GameManager { get; set; }
    public static EventManager EventManager { get; set; }
    public static PrefabDB Prefabs { get; set; }
    public static MaterialDB Materials { get; set; }
    public static SceneStackManager<TransitionData> SceneStackManager { get; set; }
    public static TweenManager TweenManager { get; set; }    
    public static DrinkDictionary DrinkDictionary { get; set; }
    public static MixerDictionary MixerDictionary { get; set; }
    public static CustomerDictionary CustomerDictionary { get; set; }
    public static ControllerDetection ControllerDetection { get; set; }
    public static AudioLoopScript AudioLoopScript { get; set; }
    public static HandManager HandManager { get; set; }
    public static CoasterManager CoasterManager { get; set; }
 
}
