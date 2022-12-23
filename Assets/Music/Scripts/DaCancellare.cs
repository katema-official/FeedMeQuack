using UnityEngine;

public class DaCancellare : MonoBehaviour
{
    public AudioSource _audioSource;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Music.UniversalAudio._spitBarSoundController.Spit(4f, _audioSource);
        }
        if (Input.GetKeyUp(KeyCode.Space))
            Music.UniversalAudio._spitBarSoundController.SetIsInSpittingState(false);
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            Music.UniversalAudio._eatingController.Eat(_audioSource);
        }
        if (Input.GetKeyUp(KeyCode.LeftArrow))
            Music.UniversalAudio._eatingController.SetIsInEatingState(false);
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            //Music.UniversalAudio._animalSoundController.Swim(_audioSource);
        }
        if (Input.GetKeyUp(KeyCode.RightArrow))
            Music.UniversalAudio._animalSoundController.SetIsInSwimmingState(false);
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            Music.UniversalAudio.PlayStealing("Mallard", transform);
        }
        if (Input.GetKeyUp(KeyCode.UpArrow))
            Music.UniversalAudio._animalSoundController.SetIsInStealingState(false);
    }
}
