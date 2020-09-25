using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

// This class (requires RigidBody, Animator, NPCPath, SpriteRenderer, and BoxCollider on the NPC) will take care of moving our NPC along a path specified by NPCPath, built by AStar
// It will then face the NPC in the proper direction, and play the target animation once they get there
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(NPCPath))]
[RequireComponent(typeof(SpriteRenderer))]
[RequireComponent(typeof(BoxCollider2D))]
public class NPCMovement : MonoBehaviour
{
    // The current/target scenes and gridPositions for the NPC given this Scheduled Movement Event, as well as the target position in the world space, and the direction
    // the NPC needs to face once they reach the destination
    public SceneName npcCurrentScene; // Set the initial current scene in the inspector
    [HideInInspector] public SceneName npcTargetScene;
    [HideInInspector] public Vector3Int npcCurrentGridPosition;
    [HideInInspector] public Vector3Int npcTargetGridPosition;
    [HideInInspector] public Vector3 npcTargetWorldPosition;
    public Direction npcFacingDirectionAtDestination; // Set the initial facing direction scene in the inspector

    // The scene that the previous movement step was in, and the grid/world positions of the next step in the new scene (so we can transfer the NPC between scenes!)
    private SceneName npcPreviousMovementStepScene;
    private Vector3Int npcNextGridPosition;
    private Vector3 npcNextWorldPosition;

    // The normal speed, minimum speed, and maximum speed of our NPC, to be populated in the editor
    [Header("NPC Movement")]
    public float npcNormalSpeed = 2f;
    [SerializeField] private float npcMinSpeed = 1f;
    [SerializeField] private float npcMaxSpeed = 3f;

    // Toggles on when the NPC is moving (e.g. so other things won't happen to the NPC)
    private bool npcIsMoving = false;

    // The animation clip that will be played when the NPC arrives at the target location. This is set in the NPCScheduleEvent for this movement
    [HideInInspector] public AnimationClip npcTargetAnimationClip;

    [Header("NPC Animation")]
    
    // The blank animation the NPC will play if nothing is supplied for when they get there
    [SerializeField] private AnimationClip blankAnimation = null;

    private Grid grid; // The tilemap grid that the NPC walks on
    private Rigidbody2D rigidBody2D; // the NPC's rigidBody
    private BoxCollider2D boxCollider2D; // the NPC's boxCollider
    private WaitForFixedUpdate waitForFixedUpdate; // the pause time to yield between coroutine plays of the walking event, corresponding to the time until the next fixed update
    private Animator animator; // the NPC's animator component
    private AnimatorOverrideController animatorOverrideController; // the NPC's override controller (to override the blank animation with the targetAnimation at the end of movement. EventScheduleAnimation in the animation controller currently has a blank animation - we will override this)
    private int lastMoveAnimationParameter; // the animation parameter for the target animation (i.e.digLeft, smokeRight, etc)
    private NPCPath npcPath; // The NPCPath component that defines the path the NPC will walk, and the timestamps that the NPC needs to be at each step by
    private bool npcInitialized = false; // Whether or not the NPC has been initialized or not yet 
    private SpriteRenderer spriteRenderer; // The NPC's spriteRenderer
    [HideInInspector] public bool npcActiveInScene = false; // Whether the NPC is active in the scene or not
    private bool sceneLoaded = false; // Whether or not this scene has been loaded yet

    // The coroutine for moving to the gridPosition, this way we can easily stop it
    private Coroutine moveToGridPositionRoutine;


    // subscribe the AfterSceneLoad and BeforeSceneUnloaded methods to the AfterSceneLoadEvent and BeforeSceneUnloadEvent events, so they can initialize the NPC/ set sceneLoaded
    private void OnEnable()
    {
        EventHandler.AfterSceneLoadEvent += AfterSceneLoad;
        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloaded;
    }


    private void OnDisable()
    {
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoad;
        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloaded;
    }


    // Populate all of the objects (rigidBocy, boxCollider, animator, spriteRenderer, animatorOverrideController, NPC target grid/world position/scene, and NPC path
    private void Awake()
    {
        // Populate the below objects
        rigidBody2D = GetComponent<Rigidbody2D>();
        boxCollider2D = GetComponent<BoxCollider2D>();
        animator = GetComponent<Animator>();
        npcPath = GetComponent<NPCPath>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // Set the animation override controller to swap out the blank animations for new ones
        animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
        animator.runtimeAnimatorController = animatorOverrideController;

        // Initialize the target grid position, world position, and scene to their current values, to be updated later
        npcTargetScene = npcCurrentScene;
        npcTargetGridPosition = npcCurrentGridPosition;
        npcTargetWorldPosition = transform.position;
    }


    // Populate the waitForFixedUpdate delay, and set the idle animation on the NPC while we wait for paths to walk on
    private void Start()
    {
        // This method returns a WaitForSeconds corresponding to the when the next fixed update will start. This will be used inbetween coroutine yields 
        // for moving the player accross two gridsquare over a set number of FixedUpdates
        waitForFixedUpdate = new WaitForFixedUpdate();

        // Set the animator parameter to idleDown so that the NPC stays still facing down 
        SetIdleAnimation();
    }


    // FixedUpdate will move our NPC following the NPCMovementSteps in the NPCMovementStepStack. It processes each step in turn, waiting until it's done to start on the next step.
    // It will move the NPC one step at a time via a coroutine that moves the NPC from the current gridPosition to the next one. Once that's done, this
    // fixed update method will initiate the next step
    private void FixedUpdate()
    {
        // Make sure the scene is loaded before doing anything
        if (sceneLoaded)
        {
            // Make sure the NPC isn't already moving before doing anything. Every fixed update this will be checked, and if npcIsMoving=true, it is currently moving one step.
            // Eventually, when it's false again this will be enabled and we can move the next step
            if (npcIsMoving == false)
            {
                // Set the NPC current and next grid position - to take into account the NPC might be animating
                // The current position is where the NPC currently is, and initially we will set the next grid position to the current one
                npcCurrentGridPosition = GetGridPosition(transform.position);
                npcNextGridPosition = npcCurrentGridPosition;
                
                // Only run if there are still NPCMovementSteps in the NPCMovementStepStack
                if (npcPath.npcMovementStepStack.Count > 0)
                {
                    // Populate the npcMovementStep with the npcMovementStep on the top of the stack (just peek at it, don't remove yet)
                    NPCMovementStep npcMovementStep = npcPath.npcMovementStepStack.Peek();

                    // Set the current NPC scene to the first movement step in this stack
                    npcCurrentScene = npcMovementStep.sceneName;

                    // If the NPC is about to move to a new scene, reset their position to the starting point in the new scene, and re-update the step times for the
                    // next path elements in the new scene
                    if (npcCurrentScene != npcPreviousMovementStepScene)
                    {
                        // If the current and next scenes are different, we are about to change scenes. Update the NPCs current grid position to the next movement steps grid coordinate
                        // (in the next scene), set the nextGridPosition to the current one, update the NPCs transform to immediately teleport them to the new scenes entrance point,
                        // update the NPCs current scene, and re-update the remaining times on the path (things probably got messed up between scenes because the grid coordinates changed substantially)
                        npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                        npcNextGridPosition = npcCurrentGridPosition;
                        transform.position = GetWorldPosition(npcCurrentGridPosition);
                        npcPreviousMovementStepScene = npcCurrentScene;
                        npcPath.UpdateTimesOnPath();
                    }

                    // If the NPC is in the current scene, then set the NPC to active to make it visiblie, then pop the movement step off 
                    // The top of the stack, and then call the method to actually move the NPC
                    if (npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
                    {
                        // Set the NPC to active in the scene so we can see them
                        SetNPCActiveInScene();

                        // Pop the next npcMovementStep from the top of the npcMovementStepStack
                        npcMovementStep = npcPath.npcMovementStepStack.Pop();

                        // Find the next grid position the NPC needs to walk to from the next npcMovementStep, gridCoordinate member
                        npcNextGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;

                        // This is the time this next npcMovementStep will take, from it's hour/minute/second member variables
                        TimeSpan npcMovementStepTime = new TimeSpan(npcMovementStep.hour, npcMovementStep.minute, npcMovementStep.second);

                        // This method will move the NPC to the next grid position npcNextGridPosition, and make sure they are there by the time npcMovementStepTime, starting off
                        // at the current game time
                        MoveToGridPosition(npcNextGridPosition, npcMovementStepTime, TimeManager.Instance.GetGameTime());
                    }

                    // Else, if the NPC is not in the current scene, then set the NPC to inactive to make it invisible (it's still moving but we can't see it until we move to their scene)
                    // Once the movement step time is less than the game time (i.e. in the past), then pop the movement step off the stack and set the NPC position to the 
                    // movement step position. Because we can't see the NPC, we don't need to run the walking animations - just keep popping off the next movement Step and
                    // immediately teleporting the NPC there at the proper times. This will keep happening until the player enters the NPC's current scene - and then
                    // they will start smoothly walking again.
                    else
                    {
                        // Disables the sprite renderer and box collider, and sets the npcActiveInScene to false. The NPC is still on the persistent scene and moving, but we can't see them so it looks like they arent
                        SetNPCInactiveInScene();

                        // Set the current and nextGrid position and move the NPC to the current one
                        npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                        npcNextGridPosition = npcCurrentGridPosition;
                        transform.position = GetWorldPosition(npcCurrentGridPosition);

                        // The step time needed for the next step to be completed by, and the current game time
                        TimeSpan npcMovementStepTime = new TimeSpan(npcMovementStep.hour, npcMovementStep.minute, npcMovementStep.second);
                        TimeSpan gameTime = TimeManager.Instance.GetGameTime();

                        // Whenever the stepTime < gameTime (so they should be to the next position by now), immediately teleport the NPC to that step, and then wait for the next step to occur, and then
                        // teleport them again. This will keep going until the player enters the NPC's current scene - then they will start smoothly walking again.
                        if (npcMovementStepTime < gameTime)
                        {
                            // Pop off the next Movement Step from the Stack
                            npcMovementStep = npcPath.npcMovementStepStack.Pop();

                            // Update the current and next grid positions, and immediately teleport the NPC to the next step position, rather than the smooth walking animation if the player IS in the NPCs scene.
                            npcCurrentGridPosition = (Vector3Int)npcMovementStep.gridCoordinate;
                            npcNextGridPosition = npcCurrentGridPosition;
                            transform.position = GetWorldPosition(npcCurrentGridPosition);
                        }
                    }
                }

                // Else, if there are no more movement steps in the npcMovementStepStack (we've reached our target destination!), reset the move animation parameters so the NPC stops moving, then 
                // set the proper NPCs facing direction, and then initiate the NPCs target animation
                else
                {
                    // Trigger all of the animator parameters to false for walking, so the NPC will stop the walking animation
                    ResetMoveAnimation();

                    // Using the npcFacingDirection set in the movement schedule, this will trigger the proper facing direction (i.e. idleUp, idleLeft, etc) once we're arrived at our destination
                    SetNPCFacingDirection();

                    // Set the NPCs target animation to play (i.e. smokeDirection, digDirection, etc) now that we're at the destination, as long as 
                    // one has been specified in the NPCScheduleEvent
                    SetNPCEventAnimation();
                }
            }
        }
    }


    // Called from NPCPath, after we've built the path for the NPC to follow. This Sets all of the scheduled movement event details in this class
    // including the target scene, target grid/world positions, the direction the NPC faces at destination, and the animation clip to play
    // at the destination
    public void SetScheduleEventDetails(NPCScheduleEvent npcScheduleEvent)
    {
        // Set the target scene, the target drid and world positions we're walking to, the direction the NPC faces when they get there, and the animation to play
        // when they get there. These are all set in the npcScheduleEvent passed here from NPCPath
        npcTargetScene = npcScheduleEvent.toSceneName;
        npcTargetGridPosition = (Vector3Int)npcScheduleEvent.toGridCoordinate;
        npcTargetWorldPosition = GetWorldPosition(npcTargetGridPosition);
        npcFacingDirectionAtDestination = npcScheduleEvent.npcFacingDirectionAtDestination;
        npcTargetAnimationClip = npcScheduleEvent.animationAtDestination;

        // Clear out the current NPC's event animations back to the blank one so we can start moving on the new path
        ClearNPCEventAnimation();
    }


    // This will initiate the NPC's target destination animation (i.e. smokeDirection, digDirection, etc) once they get there,
    // if the movement Event scheduled had a target animation (if not, set the blank animation). This is done with an
    // animation override controller to swap the default blank animation for the target animation
    public void SetNPCEventAnimation()
    {
        // Make sure this scheduled movement event has a target animation.
        if (npcTargetAnimationClip != null)
        {
            // First reset the idle animation parameters to false so we can play a new animation
            ResetIdleAnimation();

            // Set the animationOverrideController swap list for the blank animation to be replaced with the target animation.
            // And then set the eventAnimation animation controller trigger to true, so we can play the blankAnimation -> replaced by targetAnimation
            animatorOverrideController[blankAnimation] = npcTargetAnimationClip;
            animator.SetBool(Settings.eventAnimation, true);
        }
        else
        {
            // If no target animation was specified, keep the blankAnimation in place and set the animator eventAnimation parameter to false so nothing is triggered
            animatorOverrideController[blankAnimation] = blankAnimation;
            animator.SetBool(Settings.eventAnimation, false);
        }
    }


    // Resets the animatorOverrideController back to the blank animation, and sets the animatorController eventAnimation to false so now extra animations are played.
    public void ClearNPCEventAnimation()
    {
        // Set the override controller back to the blank animation so blank -> blank, and set the eventAnimation animation paramter to false so it stops playing
        animatorOverrideController[blankAnimation] = blankAnimation;
        animator.SetBool(Settings.eventAnimation, false);

        // Clear any rotation on NPC if it exists for any reason
        transform.rotation = Quaternion.identity;
    }


    // Given the npcFacingDirectionAtDestination setup in the scheduled movement event, this method will 
    private void SetNPCFacingDirection()
    {
        // Reset all of the idle animation parameters to false
        ResetIdleAnimation();

        // Depending on which facing direction the schedule wants, this will set the proper idleDirection animation parameter so they face in that direction
        switch (npcFacingDirectionAtDestination)
        {
            case Direction.up:
                animator.SetBool(Settings.idleUp, true);
                break;

            case Direction.down:
                animator.SetBool(Settings.idleDown, true);
                break;

            case Direction.left:
                animator.SetBool(Settings.idleLeft, true);
                break;

            case Direction.right:
                animator.SetBool(Settings.idleRight, true);
                break;

            case Direction.none:
                break;

            default:
                break;
        }
    }


    // This method will set the NPC to active in the scene, via enabling the spriteRenderer and boxCollider - so we can see them! Also set the npcActiveInScene bool to true so other methods
    // know the NPC is active
    public void SetNPCActiveInScene()
    {
        spriteRenderer.enabled = true; // Let's us see the NPC
        boxCollider2D.enabled = true;
        npcActiveInScene = true;
    }


    // This method will set the NPC to inactive in the scene, via disabling the spriteRenderer and boxCollider - so we can't see them anymore! Also set the npcActiveInScene bool to false so other methods
    // know the NPC is active
    public void SetNPCInactiveInScene()
    {
        spriteRenderer.enabled = false; // Can't see the NPC anymore, if they're in another scene currently
        boxCollider2D.enabled = false;
        npcActiveInScene = false;
    }


    // Once the scene has been fully loaded, this method is called and we can populate the TileMap grid, and initialize the NPC if it isn't already.
    private void AfterSceneLoad()
    {
        // Populate the grid now that the scene is loaded
        grid = GameObject.FindObjectOfType<Grid>();

        if (!npcInitialized)
        {
            // Initialize the NPC now that the scene is loaded
            InitializeNPC();
            npcInitialized = true;
        }

        // Set this bool so other methods know if it's been loaded or not
        sceneLoaded = true;
    }


    // Before we unload the scene, set sceneLoaded to false so other methods know there's no scene yet
    private void BeforeSceneUnloaded()
    {
        sceneLoaded = false;
    }


    /// <summary>
    /// Returns the grid tile position given the world position.
    /// </summary>
    /// <param name="worldPosition"></param>
    private Vector3Int GetGridPosition(Vector3 worldPosition)
    {
        // Get the grid position given a world position if the grid exists. If it doesn't just return a 0 vector
        if (grid != null)
        {
            return grid.WorldToCell(worldPosition);
        }
        else
        {
            return Vector3Int.zero;
        }
    }


    /// <summary>
    /// Returns the world position (in the center of the tileMap square) given the grid-based position
    /// </summary>
    public Vector3 GetWorldPosition(Vector3Int gridPosition)
    {
        Vector3 worldPosition = grid.CellToWorld(gridPosition);

        // Get the center of the grid square (how world positions are defined)
        return new Vector3(worldPosition.x + Settings.gridCellSize / 2f, worldPosition.y + Settings.gridCellSize / 2f, worldPosition.z);
    }


    // This method can be called from elsewhere (i.e. when we load the game) to cancel all of the NPCs movement - clear the path, isMoving -> false, stop the walking coroutines,
    // clear the moving, idle, and event animations, sets the NPC to idle down
    public void CancelNPCMovement()
    {
        // Clear out the build path the NPC may have, set the next Grid positions to 0, and set npcIsMoving to false so other methods can utilize it again
        npcPath.ClearPath();
        npcNextGridPosition = Vector3Int.zero;
        npcNextWorldPosition = Vector3Int.zero;
        npcIsMoving = false;

        // If we have a walking coroutine going, stop it
        if (moveToGridPositionRoutine != null)
        {
            StopCoroutine(moveToGridPositionRoutine);
        }

        // Reset the move animation the the NPC stops the moving animations
        ResetMoveAnimation();

        // Clear the event animation the NPC may be playing at the destination
        ClearNPCEventAnimation();
        npcTargetAnimationClip = null;

        // Reset the idle animations to all false
        ResetIdleAnimation();

        // Set the idle animation to idle down
        SetIdleAnimation();
    }


    // Initializes the NPC if it is in the currently active scene, by activating its SpriteRenderer and BoxCollider, and setting the npcActiveInScene bool.
    // If it's not in the correct scene, disable the same members
    private void InitializeNPC()
    {
        // If the NPC's current scene is the same as the active scene, set thae NPC to active. Else, set it to inactive
        if (npcCurrentScene.ToString() == SceneManager.GetActiveScene().name)
        {
            // This method enables the spriteRenderer and boxCollider of the NPC in the scene
            SetNPCActiveInScene();
        }
        else
        {
            // This method disables the spriteRenderer and boxCollider of the NPC in the scene
            SetNPCInactiveInScene();
        }

        // Make sure the NPCs current scene is correct because it can be changing
        npcPreviousMovementStepScene = npcCurrentScene;

        // Get the NPC's current grid position
        npcCurrentGridPosition = GetGridPosition(transform.position);

        // Set the next Grid position and the target grid position to the current grid position
        npcNextGridPosition = npcCurrentGridPosition;
        npcTargetGridPosition = npcCurrentGridPosition;
        npcTargetWorldPosition = GetWorldPosition(npcTargetGridPosition);

        // Get the NPC's next world position
        npcNextWorldPosition = GetWorldPosition(npcCurrentGridPosition);
    }


    // This method triggers the Coroutine that will move the player from their current gridPosition, to the next gridPosition in the path.
    // The NPC must be to the gridPosition by time npcMovementStepTime, starting at the current gameTime
    private void MoveToGridPosition(Vector3Int gridPosition, TimeSpan npcMovementStepTime, TimeSpan gameTime)
    {
        moveToGridPositionRoutine = StartCoroutine(MoveToGridPositionRoutine(gridPosition, npcMovementStepTime, gameTime));
    }


    // This is the coroutine that will actually move the NPC forward one grid square in the built path, to be used again later for the next step, all the way until the target position
    private IEnumerator MoveToGridPositionRoutine(Vector3Int gridPosition, TimeSpan npcMovementStepTime, TimeSpan gameTime)
    {
        // First set npcIsMoving so other methods know not to interfere
        npcIsMoving = true;

        // Change the NPCs walking animation in relation to the gridPosition we're walking to, relative to where we are now (ie walkingLeft, walkingUp, etc)
        SetMoveAnimation(gridPosition);

        // Get the world position of the grid square we want to walk to
        npcNextWorldPosition = GetWorldPosition(gridPosition);

        // If the movement step time is in the future, make the animation to step. Otherwise, if it's in the past skip it and immediately move the NPC to that position
        if (npcMovementStepTime > gameTime)
        {
            // Calculate the time difference between now and when we need to be at the next step be, in seconds
            float timeToMove = (float)(npcMovementStepTime.TotalSeconds - gameTime.TotalSeconds);

            // Calculate the speed we must move to get there in the proper timeToMove (converted into game seconds)
            // We want the speed to be whichever is greater: the NPCs minSpeed, or the speed required to get there in time as distance / timeToMove
            float npcCalculatedSpeed = Mathf.Max(npcMinSpeed, Vector3.Distance(transform.position, npcNextWorldPosition) / timeToMove / Settings.secondsPerGameSecond);

            // If the speed is less than the NPCs maximum speed, then process the move to the next grid position. Otherwise, skip it and immediately move the NPC to the next position
            if (npcCalculatedSpeed <= npcMaxSpeed)
            {
                // This loop will start to move the NPC towards the next target grid square, over the course of several FixedUpdates, yielding for the proper time waitForFixedUpdate inbetween each 
                // update. Do this until the position is within 1 pixel of the target world position
                while (Vector3.Distance(transform.position, npcNextWorldPosition) > Settings.pixelSize)
                {
                    // This is the unit vector pointing from the current location (this is moved every iteration of the loop) and the target world position for the next step
                    Vector3 unitVector = Vector3.Normalize(npcNextWorldPosition - transform.position);

                    // This is the distance the player needs to move in this fixed update, to be updated again in the next fixed update. Calculated via unitVector * speed * fixedUpdateTime
                    Vector2 move = new Vector2(unitVector.x * npcCalculatedSpeed * Time.fixedDeltaTime, unitVector.y * npcCalculatedSpeed * Time.fixedDeltaTime);

                    // Move the NPC's rigid body with the additional fixedUpdate move vector added to the current position vector
                    rigidBody2D.MovePosition(rigidBody2D.position + move);

                    // Wait for the next fixed update to do the loop again. and keep doing it over and over again each fixedUpdate until we are within 1 pixel of our target grid square world position
                    yield return waitForFixedUpdate;
                }
            }
        }

        // If either of the above two if statements are false (i.e. step time is in the past, or if the speed required is above the NPC's max speed), 
        // Directly move the NPC the the next grid position instead of smoothly animating it. 
        // If both of the if statements WERE evaluated, this will make sure we are moved exactly to the proper grid position (i.e. above we only animated to within one pixel of the target.), so
        // Now set it precisely to where it needs to be at.
        rigidBody2D.position = npcNextWorldPosition;

        // Update the NPC's current grid Position member variable so the next step will know it, set the next GridPosition to the current one (nothing to move yet, until the next step), and set isMoving 
        // back to false so other methods know
        npcCurrentGridPosition = gridPosition;
        npcNextGridPosition = npcCurrentGridPosition;
        npcIsMoving = false;
    }


    // This method will determine and set the animation parameter (walking direction) that the NPC needs to move in, corresponding to the 
    // current position of the NPC, and the position of the next gridsquare over that they are walking to (up, down, left, right)
    private void SetMoveAnimation(Vector3Int gridPosition)
    {
        // Reset the Idle animation (set all parameters to falsE)
        ResetIdleAnimation();

        // Reset the move animation (set all parameters to falsE)
        ResetMoveAnimation();

        // get the world position that we are walking to
        Vector3 toWorldPosition = GetWorldPosition(gridPosition);

        // Get the vector pointing from the NPCs current world position to the world position we want to move to
        Vector3 directionVector = toWorldPosition - transform.position;

        // First test if we are walking left/right (directionVector x > y), or up/down (directionVector y > x)
        if (Mathf.Abs(directionVector.x) >= Mathf.Abs(directionVector.y))
        {
            // Now check if we are moving left (directionVector x < 0) or right (directionVector x > 0), and set the proper walkDirection animation parameter so the animation controller will walk in the proper direction
            if (directionVector.x > 0)
            {
                animator.SetBool(Settings.walkRight, true);
            }
            else
            {
                animator.SetBool(Settings.walkLeft, true);
            }
        }
        else
        {
            // Do the same thing, checking for up and down movement
            if (directionVector.y > 0)
            {
                animator.SetBool(Settings.walkUp, true);
            }
            else
            {
                animator.SetBool(Settings.walkDown, true);
            }
        }
    }


    // This method simply sets the idleDown parameter in the NPC animator controller to be true, so that the NPC will stand still facing down
    private void SetIdleAnimation()
    {
        animator.SetBool(Settings.idleDown, true);
    }


    // This method resets all of the walk animation parameters to false so that the NPC will stop walking
    private void ResetMoveAnimation()
    {
        animator.SetBool(Settings.walkRight, false);
        animator.SetBool(Settings.walkLeft, false);
        animator.SetBool(Settings.walkUp, false);
        animator.SetBool(Settings.walkDown, false);
    }


    // This method resets all of the idle animation parameters to false
    private void ResetIdleAnimation()
    {
        animator.SetBool(Settings.idleRight, false);
        animator.SetBool(Settings.idleLeft, false);
        animator.SetBool(Settings.idleUp, false);
        animator.SetBool(Settings.idleDown, false);
    }
}
