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

    bool firstHintDisplayed = false;

    bool legBarrageStarted = false;
    bool pincersFiring = false;
    bool laserSequenceStarted = false;
    bool laserFiring = false;

    public bool strafing = false;
    float remainingHealthRatio = 1f;

    void Awake()
    {
        transitionConditions = new List<System.Func<bool>>
        {
            StateTransition1,
            StateTransition2
        };

        stateExecute = State1Execute;
    }

    public void UpdateHealthRatio(float val)
    {
        remainingHealthRatio = val;
        print("Received health ratio update");
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

        CombatStageManager.Instance.DisplayBossHint("Shoot at the claws while they're open!");

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

        CombatStageManager.Instance.DisplayBossHint("Strike the mandibles while they're hot!");

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
        CombatStageManager.Instance.DisplayBossHint("Break its legs before they break you!");
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
        laserChargeParticles.Play();
        laserBall.gameObject.SetActive(true);

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
        laserBall.ResetGrowth();
        laserBall.gameObject.SetActive(false);
        laserFiring = true;
        headLaser.ToggleLaserBeam(true);

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
