using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public static bool paused = false;
    public static int minigamesCompleted = 0;
    public static int obstaclesHits = 0;

    // SlideManager-specific things
    public static List<AudioClip> slideManagerAudioClips;
    public static List<Sprite> slideManagerSlides;
    public static String slideManagerTargetScene;
    public static bool slideManagerSkipFirstFadeIn = false;
}
