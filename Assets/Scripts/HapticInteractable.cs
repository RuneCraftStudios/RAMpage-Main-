using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class HapticInteractable : MonoBehaviour
{
    public AnimationCurve intensityCurve;
    public float duration;

    // Start is called before the first frame update
    void Start()
    {
        XRBaseInteractable interactable = GetComponent<XRBaseInteractable>();
        interactable.activated.AddListener(TriggerHaptic);
    }

    // Trigger haptic feedback
    public void TriggerHaptic(BaseInteractionEventArgs eventArgs)
    {
        if (eventArgs.interactorObject is XRBaseControllerInteractor controllerInteractor)
        {
            TriggerHaptic(controllerInteractor.xrController);
        }
    }

    // Trigger haptic feedback with intensity evaluated from the curve
    public void TriggerHaptic(XRBaseController controller)
    {
        StartCoroutine(HapticCoroutine(controller));
    }

    IEnumerator HapticCoroutine(XRBaseController controller)
    {
        float timer = 0f;

        while (timer < duration)
        {
            float normalizedTime = timer / duration;
            float curveValue = intensityCurve.Evaluate(normalizedTime);
            controller.SendHapticImpulse(curveValue, 0.05f); // You might want to adjust the duration here

            timer += Time.deltaTime;
            yield return null;
        }
    }
}
