using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerAnimationAtlas", menuName = "Greed/PlayerAnimationAtlas", order = 0)]
public class PlayerAnimationAtlas : ScriptableObject {
    [SerializeField] private List<AnimatorOverrideController> animationControllers;
    private static int index = 0;

    public AnimatorOverrideController NextAnimator() {
        int i = index;
        index = (index + 1) % animationControllers.Count;
        return animationControllers[i];
    }
}
