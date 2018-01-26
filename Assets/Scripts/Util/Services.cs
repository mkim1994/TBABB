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

}
