using System.Collections;
using UnityEngine;
using UnityEngine.Events;


namespace GameUtils
{
    public class GameTimer
    {
        static WaitForSeconds waitForSeconds = new WaitForSeconds(1);
        static Coroutine countdowntimerRoutine;

        static MonoBehaviour localBehaviour;
        public static void CoutDownTimer(MonoBehaviour currentBehavior, float countdownTime, UnityAction<float> countdownEvent)
        {
            localBehaviour = currentBehavior;
            if (countdowntimerRoutine != null)
            {
                currentBehavior.StopCoroutine(countdowntimerRoutine);
            }
            countdowntimerRoutine = currentBehavior.StartCoroutine(StartCountDown(countdownTime, countdownEvent));
        }

        private static IEnumerator StartCountDown(float countDown, UnityAction<float> countdownEvent)
        {
            while (countDown >= 0)
            {
                countdownEvent?.Invoke(countDown);
                yield return waitForSeconds;
                countDown--;
            }
        }

        public static void StopTimer()
        {
            if (countdowntimerRoutine != null)
            {
                if(localBehaviour != null)
                    localBehaviour.StopCoroutine(countdowntimerRoutine);
            }
        }
    }
}

