using UnityEngine;

public class DecelerationCurve : MonoBehaviour
{
    [SerializeField] private AnimationCurve animCurve;
    private bool evaluate;
    private bool routineHasBeenCalled;

    private float Evaluate(AnimationCurve curve, float time)
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

    private readonly WaitForSeconds waitForSeconds = new(0.5f);
    private System.Collections.IEnumerator SetEvaluationState()
    {
        evaluate = true; 
        yield return waitForSeconds;
        evaluate = routineHasBeenCalled = false; 
    }
    
}
