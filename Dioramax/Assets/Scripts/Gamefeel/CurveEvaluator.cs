using UnityEngine;
using UnityEngine.Events; 

/// <summary>
/// Wrapper class around the Evalute() function
/// </summary>
public class CurveEvaluator : MonoBehaviour
{
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField, Range(0.25f, 4)] private float curveDuration = 2f;
    public bool EvaluateCurve { get; private set; } 
    private bool routineHasBeenCalled;
    private float time = -0.2f;

    private UnityEvent gameFeelCurveEndEnvent = new UnityEvent();

    /// <summary>
    /// Evaluates the animCurve component
    /// </summary>
    /// <param name="curve"> The curve of this component </param>
    /// <param name="time"> The time at which to evaluate </param>
    /// <param name="duration"> The duration of the curve evaluation </param>
    /// <returns> The curve Y value at give time </returns>
    public float Evaluate(UnityAction call)
    {
        if (!routineHasBeenCalled)
        {
            gameFeelCurveEndEnvent.AddListener(call); 
            routineHasBeenCalled = true;
            StartCoroutine(nameof(SetEvaluationState), curveDuration); 
        }                              

        if (EvaluateCurve)
        {
            time += Time.fixedDeltaTime / curveDuration;
            return animCurve.Evaluate(time);
        }

        return default; 
    }

    private WaitForSeconds waitForSeconds;
    private System.Collections.IEnumerator SetEvaluationState(float duration)
    {
        EvaluateCurve = true;
        waitForSeconds = new WaitForSeconds(duration); 
        yield return waitForSeconds;

        EndGamefeelCurve(); 
    }    

    public void EndGamefeelCurve()
    {
        StopAllCoroutines(); 
        EvaluateCurve = routineHasBeenCalled = false;
        time = -0.2f;
        gameFeelCurveEndEnvent?.Invoke();
        gameFeelCurveEndEnvent.RemoveAllListeners(); // why remove all ? store the call instead and remove IT
    }
}
