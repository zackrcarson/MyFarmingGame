using UnityEngine;

public class MovementAnimationParameterControl : MonoBehaviour
{
    private Animator animator;

    // Use this for initialization

    private void Awake()
    {
        animator = GetComponent<Animator>(); // Finds the animation component for the gameObject this script is attached to, and stores it in animator
    }

    private void OnEnable() // Called every time the game object is enabled!
    {
        EventHandler.MovementEvent += SetAnimationParameters; // Subscribe the SetAnimationParameters method to the MovementEvent delegate!
    }

    private void OnDisable() // Called every time the game object is disabled!
    {
        EventHandler.MovementEvent -= SetAnimationParameters; // Unsubscribe the SetAnimationParameters method to the MovementEvent delegate!
    }

    // Because SetAnimationParameters is subscribed to the EventHandler.MovementEvent delegate, this method will get triggered everytime the 
    // MovementEvent is triggered, and each of the below parameters will be set within the animator.
    private void SetAnimationParameters(float xInput, float yInput, bool isWalking, bool isRunning, bool isIdle, bool isCarrying, ToolEffect toolEffect, 
        bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown, 
        bool isLiftingToolRight, bool isLiftingToolLeft, bool isLiftingToolUp, bool isLiftingToolDown, 
        bool isPickingLeft, bool isPickingRight, bool isPickingUp, bool isPickingDown, 
        bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown, 
        bool idleUp, bool idleDown, bool idleLeft, bool idleRight)
    {
        // Set all of the floats, bools, integers, and triggers within the animator based on the parameters received from the MovementEvent delegate
        animator.SetFloat(Settings.xInput, xInput);
        animator.SetFloat(Settings.yInput, yInput);
        animator.SetBool(Settings.isWalking, isWalking);
        animator.SetBool(Settings.isRunning, isRunning);

        animator.SetInteger(Settings.toolEffect, (int)toolEffect);

        if (isUsingToolRight)
            animator.SetTrigger(Settings.isUsingToolRight);
        if (isUsingToolLeft)
            animator.SetTrigger(Settings.isUsingToolLeft);
        if (isUsingToolUp)
            animator.SetTrigger(Settings.isUsingToolUp);
        if (isUsingToolDown)
            animator.SetTrigger(Settings.isUsingToolDown);

        if (isLiftingToolRight)
            animator.SetTrigger(Settings.isLiftingToolRight);
        if (isLiftingToolLeft)
            animator.SetTrigger(Settings.isLiftingToolLeft);
        if (isLiftingToolUp)
            animator.SetTrigger(Settings.isLiftingToolUp);
        if (isLiftingToolDown)
            animator.SetTrigger(Settings.isLiftingToolDown);

        if (isSwingingToolRight)
            animator.SetTrigger(Settings.isSwingingToolRight);
        if (isSwingingToolLeft)
            animator.SetTrigger(Settings.isSwingingToolLeft);
        if (isSwingingToolUp)
            animator.SetTrigger(Settings.isSwingingToolUp);
        if (isSwingingToolDown)
            animator.SetTrigger(Settings.isSwingingToolDown);

        if (isPickingRight)
            animator.SetTrigger(Settings.isPickingRight);
        if (isPickingLeft)
            animator.SetTrigger(Settings.isPickingLeft);
        if (isPickingUp)
            animator.SetTrigger(Settings.isPickingUp);
        if (isPickingDown)
            animator.SetTrigger(Settings.isPickingDown);

        if (idleRight)
            animator.SetTrigger(Settings.idleRight);
        if (idleLeft)
            animator.SetTrigger(Settings.idleLeft);
        if (idleUp)
            animator.SetTrigger(Settings.idleUp);
        if (idleDown)
            animator.SetTrigger(Settings.idleDown);
    }

    // Triggered within the walking/running animations (directly set up in the animation controllers for the player walking!) to play a footstep sound at certain times during the animation
    // This one (little white bars on the animation timeline for the body animations) plays every time the foot hits the ground.
    // Because we are triggering this sound from the animation, it will synchronize very well with the actual footsteps!!
    private void AnimationEventPlayFootstepSound()
    {
        // Use the AudioManager to play the footstep sound everytime this method is triggered from the animation!
        AudioManager.Instance.PlaySound(SoundName.effectFootStepHardGround);
    }
}
