using UnityEngine;
using UnityEngine.Events; 

/// <summary>
/// Wrapper class around the Evalute() function
/// </summary>
public class CurveEvaluator : MonoBehaviour
{
    [SerializeField] private AnimationCurve animCurve;
    [SerializeField, Range(0f, 50)] private float curveMaxDuration = 2f;
    public bool EvaluateCurve { get; private set; } 
    private bool routineHasBeenCalled;
    private float time = -0.2f;

    private UnityEvent gameFeelCurveEndEnvent = new UnityEvent();
    private float _curveMaxDuration;

    private void Awake()
    {
        _curveMaxDuration = curveMaxDuration;
    }

    /// <summary>
    /// Evaluates the animCurve component
    /// </summary>
    /// <param name="curve"> The curve of this component </param>
    /// <param name="time"> The time at which to evaluate </param>
    /// <param name="duration"> The duration of the curve evaluation </param>
    /// <returns> The curve Y value at give time </returns>
    public float DoXYRotationCurve(UnityAction call, float multiplier = 1f)
    {
        _curveMaxDuration = curveMaxDuration * multiplier;

        if (!routineHasBeenCalled)
        {
            gameFeelCurveEndEnvent.AddListener(call); 
            routineHasBeenCalled = true;
            StartCoroutine(nameof(SetEvaluationState), _curveMaxDuration); 
        }                              

        if (EvaluateCurve)
        {
            time += Time.fixedDeltaTime / _curveMaxDuration;
            return animCurve.Evaluate(time);
        }

        return default; 
    }

    public float DoZRotationCurve(UnityAction call, float multiplier = 1f)
    {
        _curveMaxDuration = curveMaxDuration * multiplier;

        if (!routineHasBeenCalled)
        {
            gameFeelCurveEndEnvent.AddListener(call);
            routineHasBeenCalled = true;
            StartCoroutine(nameof(SetEvaluationState), _curveMaxDuration);
            time = 0f; 
        }

        if (EvaluateCurve)
        {
            time += Time.fixedDeltaTime / _curveMaxDuration;
            return animCurve.Evaluate(time);
        }

        return default;
    }

    public float DoZoomCurve(UnityAction call, float multiplier = 1f)
    {
        _curveMaxDuration = curveMaxDuration * multiplier;

        if (!routineHasBeenCalled)
        {
            gameFeelCurveEndEnvent.AddListener(call);
            routineHasBeenCalled = true;
            StartCoroutine(nameof(SetEvaluationState), _curveMaxDuration);
            time = 0f;
        }

        if (EvaluateCurve)
        {
            time += Time.fixedDeltaTime / _curveMaxDuration;

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
