using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeBossBehavior : BossBehavior
{
    [SerializeField] StrafeEnemyBehavior strafeBehavior;

    const int legPairCount = 5;
    [SerializeField] CentipedeLegBehavior[] centipedeLeftLegs;
    [SerializeField] CentipedeLegBehavior[] centipedeRightLegs;

    [SerializeField] CentipedePincerBehavior leftPincer;
    [SerializeField] CentipedePincerBehavior rightPincer;

    [SerializeField] CentipedeMandibleBehavior[] mandibles;
    [SerializeField] LaserBeamSource headLaser;
    [SerializeField] ParticleSystem laserChargeParticles;
    [SerializeField] EnemyLaserBall laserBall;

    StatusEffectHandler mainStatusHandler;

    bool firstHintDisplayed = false;

    bool legBarrageStarted = false;
    bool pincersFiring = false;
    bool laserSequenceStarted = false;
    bool laserFiring = false;

    public bool strafing = false;
    float remainingHealthRatio = 1f;

    new void Awake()
    {
        transitionConditions = new List<System.Func<bool>>
        {
            StateTransition1,
            StateTransition2
        };

        stateExecute = State1Execute;

        CombatStageManager.Instance.SetBossHintTable("CentipedeHints");

        foreach (CentipedeLegBehavior l in centipedeLeftLegs)
        {
            CombatStageManager.Instance.RegisterEnemyTransform(l.transform);
        }

        foreach (CentipedeLegBehavior r in centipedeRightLegs)
        {
            CombatStageManager.Instance.RegisterEnemyTransform(r.transform);
        }

        mainStatusHandler = GetComponent<StatusEffectHandler>();
    }

    public void UpdateHealthRatio(float val)
    {
        remainingHealthRatio = val;
        TryTransition();
    }

    public void UpdateStrafeStatus(bool val)
    {
        strafing = val;
        TryTransition();
    }

    #region TRANSITIONS

    bool StateTransition1()
    {
        if (remainingHealthRatio > 0.5f)
        {
            return false;
        }

        stateExecute = State2Execute;

        strafeBehavior.enabled = true;

        Transform leftPincerHitTransform = leftPincer.transform.Find("Homing Target");
        Transform rightPincerHitTransform = rightPincer.transform.Find("Homing Target");
        CombatStageManager.Instance.RegisterEnemyTransform(leftPincerHitTransform);
        CombatStageManager.Instance.RegisterEnemyTransform(rightPincerHitTransform);

        CombatStageManager.Instance.DisplayBossHint("hint02");

        return true;
    }

    bool StateTransition2()
    {
        if (remainingHealthRatio > 0.1f || strafing)
        {
            return false;
        }

        stateExecute = State3Execute;

        strafeBehavior.enabled = false;

        CombatStageManager.Instance.DisplayBossHint("hint03");

        CombatStageManager.Instance.RegisterEnemyTransform(mandibles[0].transform.Find("Homing Target"));
        CombatStageManager.Instance.RegisterEnemyTransform(mandibles[1].transform.Find("Homing Target"));

        return true;
    }

    #endregion

    #region EXECUTION

    void State1Execute()
    {
        if (!FinalPositionReached)
        {
            return;
        }

        if (!firstHintDisplayed)
        {
            DisplayFirstHint();
        }

        if (!legBarrageStarted)
        {
            StartCoroutine(LegBarrage());
        }
    }

    void DisplayFirstHint()
    {
        CombatStageManager.Instance.DisplayBossHint("hint01");
        firstHintDisplayed = true;
    }

    IEnumerator LegBarrage()
    {
        legBarrageStarted = true;

        for (int i = 0; i < legPairCount; i++)
        {
            CentipedeLegBehavior leftLeg = centipedeLeftLegs[i];
            CentipedeLegBehavior rightLeg = centipedeRightLegs[i];

            if (leftLeg != null)
            {
                leftLeg.TryStartAttack();
            }

            if (rightLeg != null)
            {
                rightLeg.TryStartAttack();
            }

            yield return new WaitForSeconds(1.2f);
        }

        legBarrageStarted = false;
    }

    void State2Execute()
    {
        if (!pincersFiring)
        {
            StartCoroutine(PincerFireSequence());
        }
    }

    IEnumerator PincerFireSequence()
    {
        pincersFiring = true;

        if (leftPincer != null)
        {
            leftPincer.PlayAttackSequence();
        }
        yield return new WaitForSeconds(0.6f);

        if (rightPincer != null)
        {
            rightPincer.PlayAttackSequence();
        }
        yield return new WaitForSeconds(0.6f);

        pincersFiring = false;
    }

    void State3Execute()
    {   
        if (!laserSequenceStarted)
        {
            StartCoroutine(LaserFireSequence());
        }

        if (laserFiring)
        {
            headLaser.TryShoot();
        }
    }

    IEnumerator LaserFireSequence()
    {
        trackPlayer = true;
        laserSequenceStarted = true;

        yield return new WaitForSeconds(0.6f);
        foreach (CentipedeMandibleBehavior mandible in mandibles)
        {
            if (mandible != null)
            {
                mandible.SetState(1);
            }
        }

        // Wait until the stun status ends
        yield return new WaitUntil(() => !mainStatusHandler.HasStatusCondition(StatusEffect.Stun));

        laserChargeParticles.Play();
        laserBall.gameObject.SetActive(true);
        CombatSFXManager.PlaySoundAtLocation("LaserChargeUp", laserBall.transform.position);

        yield return new WaitForSeconds(0.9f);
        trackPlayer = false;

        yield return new WaitForSeconds(0.4f);
        foreach (CentipedeMandibleBehavior mandible in mandibles)
        {
            if (mandible != null)
            {
                mandible.SetState(2);
            }
        }
        laserChargeParticles.Stop();

        // Wait until the stun status ends
        yield return new WaitUntil(() => !mainStatusHandler.HasStatusCondition(StatusEffect.Stun));

        laserBall.ResetGrowth();
        laserBall.gameObject.SetActive(false);
        laserFiring = true;
        headLaser.ToggleLaserBeam(true);
        CombatSFXManager.PlaySoundAtLocation("CentipedeLaser", headLaser.transform.position);

        yield return new WaitForSeconds(1);
        foreach (CentipedeMandibleBehavior mandible in mandibles)
        {
            if (mandible != null)
            {
                mandible.SetState(1);
            }
        }
        laserFiring = false;
        headLaser.ToggleLaserBeam(false);

        yield return new WaitForSeconds(0.6f);

        // Wait until the stun status ends
        yield return new WaitUntil(() => !mainStatusHandler.HasStatusCondition(StatusEffect.Stun));

        foreach (CentipedeMandibleBehavior mandible in mandibles)
        {
            if (mandible != null)
            {
                mandible.SetState(0);
            }
        }
        laserSequenceStarted = false;
    }

    #endregion
}
