using System;
using System.Collections;
using System.Collections.Generic;
using JongCo.Easing;
using UnityEngine;

public class CardAnimations
{
    public static IEnumerator HwatooAnimation(
        Transform transform,
        float duration,
        Action<float, Vector2, Vector2, Vector3> animation,
        Action<Vector2, Vector3> finalAction
    ) {
        float progress = 0;
        Vector2 initialPos = transform.position;
        Vector3 initialZPos = Vector3.forward * transform.position.z;
        Vector2 initialScale = transform.localScale;

        while (progress < duration) {
            float progressRatio = progress / duration;

            animation(progressRatio, initialPos, initialScale, initialZPos);

            progress += Time.unscaledDeltaTime;
            yield return null;
        }
        finalAction(initialPos, initialZPos);
    }

    public static IEnumerator CombineHwatooAnimation(
        IEnumerator[] animations
    )
    {
        foreach (var animation in animations)
        {
            yield return animation;
        }
    }

    public static IEnumerator MoveAnimation(
        Transform transform,
        Vector2 targetPosition,
        Vector2 targetScale,
        EasingOption easingOption,
        float duration
    ) {
        return HwatooAnimation (
            transform,
            duration,
            (progress, initialPos, initialScale, initialZPos) => {

                transform.position = Vector3.Lerp(
                    initialPos, 
                    targetPosition,
                    SingleAxisBezier.CubicBezier(easingOption, progress)
                ) + initialZPos;

                transform.localScale = Vector3.Lerp(
                    initialScale,
                    targetScale,
                    SingleAxisBezier.CubicBezier(easingOption, progress)
                ) + Vector3.forward;

            },
            (initialPos, initialZPos) => {transform.position = (Vector3) targetPosition + initialZPos;}
        );
    }

    public static IEnumerator SlapAnimation(
        Transform transform,
        Vector2 targetPos,
        float duration
    ) 
    {
        float liftDurationRatio = 0.2f;
        float slapDurationRatio = 1-liftDurationRatio;
        return HwatooAnimation(
            transform, 
            duration,
            (progress, initialPos, initialScale, initialZPos) => 
            {
                if (progress < liftDurationRatio) 
                {
                    transform.localScale = Vector3.Lerp(
                        Vector3.one,
                        Vector3.one * 1.3f,
                        SingleAxisBezier.CubicBezier(
                            Preset.FastInSlowOut, progress / liftDurationRatio
                        )
                    );
                }
                else
                {
                    transform.localScale = Vector3.Lerp(
                        Vector3.one * 1.3f,
                        Vector3.one,
                        SingleAxisBezier.CubicBezier(
                            Preset.SlowInFastOut2, (progress - liftDurationRatio) / slapDurationRatio
                        )
                    );
                }
            },
            (_, _) => {transform.localScale = Vector3.one;}
        );
    }

    public static IEnumerator MoveAndSlapAnimation(
        Transform transform, 
        Vector2 targetPosition,
        EasingOption easingOption,
        float duration1,
        float duration2
    )
    {
        yield return CombineHwatooAnimation(new IEnumerator[2] {
            MoveAnimation(transform, targetPosition, Vector3.one, easingOption, duration1),
            SlapAnimation(transform, targetPosition, duration2)
        });  
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
            (progress, initialPos, initialScale, initialZPos) => {
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
            },
            (initialPos, initialZPos) => {transform.position = (Vector3) targetPosition + initialZPos;}
        );
    }
}
