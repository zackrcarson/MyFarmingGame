using UnityEngine;

// Execute always will be running ALWAYS, even when the game is not running in the editor!
// When we attach this to a game object, it will generate a GUID, even when the game isn't running
[ExecuteAlways]
public class GenerateGUID : MonoBehaviour
{
    [SerializeField]
    private string _gUID = "";

    public string GUID {get => _gUID; set => _gUID = value;}

    private void Awake()
    {
        // Only populate in the editor (not if it's playing in the play mode)
        if (!Application.IsPlaying(gameObject))
        {
            // Ensure that the object has a guaranteed unique ID
            if (_gUID == "")
            {
                // Assign a random unique 16 digit string GUID
                _gUID = System.Guid.NewGuid().ToString();
            }
        }
    }
}
