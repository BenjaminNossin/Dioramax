using UnityEngine;

/// <summary>
/// Wrapper class around the Evalute() function
/// </summary>
public class CurveEvaluator : MonoBehaviour
{
    [SerializeField] private AnimationCurve animCurve;
    private bool evaluate;
    private bool routineHasBeenCalled;

    /// <summary>
    /// Evaluates the animCurve component
    /// </summary>
    /// <param name="curve"> The curve of this component </param>
    /// <param name="time"> The time at which to evaluate </param>
    /// <param name="duration"> The duration of the curve evaluation </param>
    /// <returns> The curve Y value at give time </returns>
    public float Evaluate(AnimationCurve curve, float time, float duration)
    {
        if (!routineHasBeenCalled)
        {
            routineHasBeenCalled = true;
            StartCoroutine(nameof(SetEvaluationState)); 
        }                              

        if (evaluate)
        {
            return curve.Evaluate(time);
        }

        return default; 
    }

    private WaitForSeconds waitForSeconds;
    private System.Collections.IEnumerator SetEvaluationState(float duration = 0.5f)
    {
        evaluate = true;
        waitForSeconds = new WaitForSeconds(duration); 
        yield return waitForSeconds;
        evaluate = routineHasBeenCalled = false; 
    }
    
}
