using System;
using System.Collections;
using JongCo.Easing;
using UnityEngine;

public class CardAnimations
{
    public static IEnumerator HwatooAnimation(
        Transform transform,
        float duration,
        Vector2 targetPosition,
        Action<float, Vector2, Vector3> animation
    ) {
        float progress = 0;
        Vector2 initialPos = transform.position;
        Vector3 initialZPos = Vector3.forward * transform.position.z;

        while (progress < duration) {
            float progressRatio = progress / duration;

            animation(progress / duration, initialPos, initialZPos);

            progress += Time.unscaledDeltaTime;
            yield return null;
        }
        transform.position = (Vector3) targetPosition + initialZPos;
    }

    public static IEnumerator MoveAnimation(
        Transform transform,
        Vector2 targetPosition,
        EasingOption easingOption,
        float duration
    ) {
        return HwatooAnimation (
            transform,
            duration,
            targetPosition,
            (progress, initialPos, initialZPos) => {
                transform.position = Vector3.Lerp(
                    initialPos, 
                    targetPosition,
                    SingleAxisBezier.CubicBezier(easingOption, progress)
                ) + initialZPos;
            }
        );
    }

    public static IEnumerator ShuffleAnimation(
        Transform transform,
        EasingOption easing,
        Vector2 targetPosition,
        float strength = 0.5f,
        float duration = 0.5f
    )
    {
        Vector2 randomPos = (Vector2)transform.position + UnityEngine.Random.insideUnitCircle.normalized * strength;
        return HwatooAnimation (
            transform,
            duration,
            targetPosition,
            (progress, initialPos, initialZPos) => {
                if (progress < 0.5f) {
                    transform.position = Vector3.Lerp(
                        initialPos,
                        randomPos,
                        SingleAxisBezier.CubicBezier(easing, progress / 0.5f)
                    ) + initialZPos;
                } else {
                    transform.position = Vector3.Lerp(
                        randomPos,
                        targetPosition,
                        SingleAxisBezier.CubicBezier(easing, (progress - 0.5f) / 0.5f)
                    ) + initialZPos;
                }
            }
        );
    }
}
