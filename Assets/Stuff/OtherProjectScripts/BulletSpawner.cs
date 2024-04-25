using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletSpawner : MonoBehaviour
{
    public EnemyStats bossStats;
    public BossMovement bossMove;
    public GameObject bullet;
    public GameObject player;
    public GameObject boss;
    public float cd = 0;
    public float interval = 0.07f;
    public float bullAmount = 15;
    public int phase = 1;
    public float castTime = 1f;
    public bool canAttack = true;
    public bool skillInUse = false;

    private bool locChosen = false;
    private bool arrived = false;
    private bool rotChosen = false;

    private float dist = 0;
    private GameObject currBullet;
    private int speed = 30;
    private Vector3 direction;
    private Vector3 destination;
    private float autoAttackCd = 0;
    public Quaternion targetRotation;

    public GameObject shoulder1;
    public GameObject shoulder2;

    private Vector3 horizontalShift = new Vector3(3f, 0f, 0f);
    private Vector3 verticalShift = new Vector3(0f, 3f, 0f);

    private bool movedToSide = false;
    private bool movedForward = false;

    public float moveSpeed = 9.0f;
    private bool isMoving = false;
    private Vector3 start1;
    private Vector3 start2;
    private Vector3 end1;
    private Vector3 end2;
    private float journeyLength;
    private float journeyCovered;
    public bool isScaling = false;
    private Vector3 startScale;
    private Vector3 endScale;
    private float scaleJourneyLength;
    private float scaleJourneyCovered;
    public float scaleSpeed = 5.0f;

    public bool canSmash = false;
    public bool isSmashing = false;
    public float smashInterval = 0.5f;
    public bool isRetreating = false;
    public int smashAmount = 0;
    public bool isScalingBack = false;
    public bool movedBackwards = false;
    public bool movedToOrigin = false;
    public bool isMovingBackwards = false;
    float distance;

    void Start()
    {

    }

    void Update()
    {
        cd -= Time.deltaTime;
        interval -= Time.deltaTime;
        autoAttackCd -= Time.deltaTime;
        if (canAttack)
        {
            if (!skillInUse && autoAttackCd <= 0)
            {
                Vector3 newPos1 = new Vector3(0.6f, 0f, 0f);
                currBullet = Instantiate(bullet, transform.position + transform.rotation * newPos1, transform.rotation);
                currBullet.transform.rotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z);
                currBullet = Instantiate(bullet, transform.position - transform.rotation * newPos1, transform.rotation);
                currBullet.transform.rotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z);
                currBullet = Instantiate(bullet, transform.position, transform.rotation);
                currBullet.transform.rotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z);
                autoAttackCd = 1.5f;
            }
            if (cd <= 0 && interval <= 0 && bossStats.hp / bossStats.maxHp <= 0.85f && bossStats.hp / bossStats.maxHp >= 0.65f && !skillInUse)
            {
                skillInUse = true;
                bossMove.canMove = false;
                bossMove.canRotate = false;
                Vector3 newPos1 = new Vector3(0.6f, 0f, 0f);
                currBullet = Instantiate(bullet, transform.position + transform.rotation * newPos1, transform.rotation);
                currBullet.transform.rotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z);
                currBullet = Instantiate(bullet, transform.position - transform.rotation * newPos1, transform.rotation);
                currBullet.transform.rotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z);
                bullAmount--;
                interval = 0.1f;
                skillInUse = false;
                autoAttackCd = 1.5f;
                if (bullAmount <= 0)
                {
                    bossMove.canMove = true;
                    bossMove.canRotate = true;

                    cd = 4.0f;
                    bullAmount = 9;

                }
            }
            if (cd <= 0 && interval <= 0 && bossStats.hp / bossStats.maxHp < 0.65f && bossStats.hp / bossStats.maxHp >= 0.45f && phase == 1 && !skillInUse)
            {
                skillInUse = true;
                bossMove.canMove = false;
                bossMove.canRotate = false;
                Vector3 newPos1 = new Vector3(0.6f, 0f, 0f);
                currBullet = Instantiate(bullet, transform.position + transform.rotation * newPos1, transform.rotation);
                currBullet.transform.rotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z);
                currBullet = Instantiate(bullet, transform.position - transform.rotation * newPos1, transform.rotation);
                currBullet.transform.rotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z);
                currBullet = Instantiate(bullet, transform.position, transform.rotation);
                currBullet.transform.rotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z);
                bullAmount--;
                interval = 0.08f;
                skillInUse = false;
                autoAttackCd = 1.5f;
                if (bullAmount <= 0)
                {
                    //cd = 0.1f;
                    bullAmount = 8;
                    phase++;

                }
            }
            if (cd <= 0 && interval <= 0 && bossStats.hp / bossStats.maxHp < 0.65f && bossStats.hp / bossStats.maxHp >= 0.45f && phase == 2 && !skillInUse)
            {
                autoAttackCd = 1.5f;
                if (!rotChosen)
                {
                    targetRotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z - 90);
                    rotChosen = true;
                }
                boss.transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 720f * Time.deltaTime);

                if (boss.transform.rotation != targetRotation)
                {
                    return;
                }
                skillInUse = true;
                bossMove.canMove = false;
                bossMove.canRotate = false;
                Vector3 newPos1 = new Vector3(0.6f, 0f, 0f);
                currBullet = Instantiate(bullet, transform.position + transform.rotation * newPos1, transform.rotation);
                currBullet.transform.rotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z);
                currBullet = Instantiate(bullet, transform.position - transform.rotation * newPos1, transform.rotation);
                currBullet.transform.rotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z);
                currBullet = Instantiate(bullet, transform.position, transform.rotation);
                currBullet.transform.rotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z);
                bullAmount--;
                interval = 0.08f;
                skillInUse = false;

                if (bullAmount <= 0)
                {
                    //cd = 0.1f;
                    bullAmount = 8;
                    phase++;
                    rotChosen = false;

                }
            }
            if (cd <= 0 && interval <= 0 && bossStats.hp / bossStats.maxHp < 0.65f && bossStats.hp / bossStats.maxHp >= 0.45f && phase == 3 && !skillInUse)
            {
                autoAttackCd = 1.5f;
                if (!rotChosen)
                {
                    targetRotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z - 90);
                    rotChosen = true;
                }
                boss.transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 720f * Time.deltaTime);
                if (boss.transform.rotation != targetRotation)
                {
                    return;
                }
                skillInUse = true;
                bossMove.canMove = false;
                bossMove.canRotate = false;
                Vector3 newPos1 = new Vector3(0.6f, 0f, 0f);
                currBullet = Instantiate(bullet, transform.position + transform.rotation * newPos1, transform.rotation);
                currBullet.transform.rotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z);
                currBullet = Instantiate(bullet, transform.position - transform.rotation * newPos1, transform.rotation);
                currBullet.transform.rotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z);
                currBullet = Instantiate(bullet, transform.position, transform.rotation);
                currBullet.transform.rotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z);
                bullAmount--;
                interval = 0.08f;
                skillInUse = false;

                if (bullAmount <= 0)
                {
                    //cd = 0.1f;
                    bullAmount = 8;
                    phase++;
                    rotChosen = false;

                }
            }
            if (cd <= 0 && interval <= 0 && bossStats.hp / bossStats.maxHp < 0.65f && bossStats.hp / bossStats.maxHp >= 0.45f && phase == 4 && !skillInUse)
            {
                autoAttackCd = 1.5f;
                if (!rotChosen)
                {
                    targetRotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z - 90);
                    rotChosen = true;
                }
                boss.transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 720f * Time.deltaTime);

                if (boss.transform.rotation != targetRotation)
                {
                    return;
                }
                skillInUse = true;
                bossMove.canMove = false;
                bossMove.canRotate = false;
                Vector3 newPos1 = new Vector3(0.6f, 0f, 0f);
                currBullet = Instantiate(bullet, transform.position + transform.rotation * newPos1, transform.rotation);
                currBullet.transform.rotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z);
                currBullet = Instantiate(bullet, transform.position - transform.rotation * newPos1, transform.rotation);
                currBullet.transform.rotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z);
                currBullet = Instantiate(bullet, transform.position, transform.rotation);
                currBullet.transform.rotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z);
                bullAmount--;
                interval = 0.08f;
                skillInUse = false;

                if (bullAmount <= 0)
                {
                    bossMove.canMove = true;
                    bossMove.canRotate = true;
                    rotChosen = false;
                    cd = 4.0f;
                    bullAmount = 8;

                    phase = 1;
                }
            }
            if (!isMoving && cd <= 0 && interval <= 0 && bossStats.hp / bossStats.maxHp < 0.45f && bossStats.hp / bossStats.maxHp >= 0.25f && !movedToSide)
            {
                autoAttackCd = 1.5f;
                bossMove.canMove = false;
                bossMove.canRotate = false;
                skillInUse = true;
                start1 = shoulder1.transform.localPosition;
                end1 = new Vector3(start1.x - 3.0f, start1.y, start1.z);

                start2 = shoulder2.transform.localPosition;
                end2 = new Vector3(start2.x + 3.0f, start2.y, start2.z);

                journeyLength = Vector3.Distance(start1, end1);
                journeyCovered = 0;

                isMoving = true;
            }

            if (isMoving && !movedToSide)
            {
                autoAttackCd = 1.5f;
                float distanceCovered = moveSpeed * Time.deltaTime;

                journeyCovered += distanceCovered;

                float journeyProgress = journeyCovered / journeyLength;

                shoulder1.transform.localPosition = Vector3.Lerp(start1, end1, journeyProgress);
                shoulder2.transform.localPosition = Vector3.Lerp(start2, end2, journeyProgress);

                if (journeyProgress >= 1.0f)
                {
                    isMoving = false;
                    movedToSide = true;
                }
            }

            if (!isMoving && cd <= 0 && interval <= 0 && bossStats.hp / bossStats.maxHp < 0.45f && bossStats.hp / bossStats.maxHp >= 0.25f && movedToSide && !movedForward)
            {
                autoAttackCd = 1.5f;
                skillInUse = true;
                distance = Vector3.Distance(player.transform.position, boss.transform.position);
                start1 = shoulder1.transform.localPosition;
                end1 = new Vector3(start1.x, start1.y + distance, start1.z);
                start2 = shoulder2.transform.localPosition;
                end2 = new Vector3(start2.x, start2.y + distance, start2.z);

                journeyLength = Vector3.Distance(start1, end1);
                journeyCovered = 0;

                isMoving = true;
            }

            if (isMoving && !movedForward && movedToSide)
            {
                autoAttackCd = 1.5f;
                float distanceCovered = moveSpeed * Time.deltaTime;

                journeyCovered += distanceCovered;

                float journeyProgress = journeyCovered / journeyLength;

                shoulder1.transform.localPosition = Vector3.Lerp(start1, end1, journeyProgress);
                shoulder2.transform.localPosition = Vector3.Lerp(start2, end2, journeyProgress);

                if (journeyProgress >= 1.0f)
                {
                    isMoving = false;
                    movedForward = true;

                    startScale = shoulder1.transform.localScale;
                    endScale = new Vector3(1.0f, 1.0f, 1.0f);

                    scaleJourneyLength = Vector3.Distance(startScale, endScale);
                    scaleJourneyCovered = 0;

                    isScaling = true;
                }
            }
            if (isScaling)
            {
                autoAttackCd = 1.5f;
                float scaleDistanceCovered = scaleSpeed * Time.deltaTime;

                scaleJourneyCovered += scaleDistanceCovered;

                float scaleJourneyProgress = scaleJourneyCovered / scaleJourneyLength;

                shoulder1.transform.localScale = Vector3.Lerp(startScale, endScale, scaleJourneyProgress);
                shoulder2.transform.localScale = Vector3.Lerp(startScale, endScale, scaleJourneyProgress);

                if (scaleJourneyProgress >= 1.0f)
                {
                    isScaling = false;
                    canSmash = true;
                }
            }

            if (canSmash && cd <= 0 && smashAmount < 3)
            {
                autoAttackCd = 1.5f;
                bossMove.canMove = false;
                bossMove.canRotate = false;
                start1 = shoulder1.transform.localPosition;
                end1 = new Vector3(start1.x + 3.1f, start1.y, start1.z);

                start2 = shoulder2.transform.localPosition;
                end2 = new Vector3(start2.x - 3.1f, start2.y, start2.z);

                journeyLength = Vector3.Distance(start1, end1);
                journeyCovered = 0;

                isSmashing = true;
                canSmash = false;
            }
            if (isSmashing)
            {
                autoAttackCd = 1.5f;
                float distanceCovered = 3f * moveSpeed * Time.deltaTime;

                journeyCovered += distanceCovered;

                float journeyProgress = journeyCovered / journeyLength;

                shoulder1.transform.localPosition = Vector3.Lerp(start1, end1, journeyProgress);
                shoulder2.transform.localPosition = Vector3.Lerp(start2, end2, journeyProgress);

                shoulder1.GetComponent<Collider2D>().enabled = true;
                shoulder2.GetComponent<Collider2D>().enabled = true;

                if (journeyProgress >= 1.0f)
                {
                    start1 = shoulder1.transform.localPosition;
                    end1 = new Vector3(start1.x - 3.1f, start1.y, start1.z);
                    start2 = shoulder2.transform.localPosition;
                    end2 = new Vector3(start2.x + 3.1f, start2.y, start2.z);

                    isSmashing = false;
                    isRetreating = true;
                    journeyCovered = 0;
                    //cd = 0.2f;
                }
            }
            if (isRetreating && cd <= 0)
            {
                autoAttackCd = 1.5f;
                float distanceCovered = moveSpeed * Time.deltaTime;

                journeyCovered += distanceCovered;

                float journeyProgress = journeyCovered / journeyLength;

                shoulder1.transform.localPosition = Vector3.Lerp(start1, end1, journeyProgress);
                shoulder2.transform.localPosition = Vector3.Lerp(start2, end2, journeyProgress);

                if (journeyProgress >= 1.0f)
                {
                    canSmash = true;
                    isRetreating = false;
                    cd = 0.2f;
                    journeyCovered = 0;

                    smashAmount++;
                    if (smashAmount >= 3)
                    {
                        startScale = shoulder1.transform.localScale;
                        endScale = new Vector3(0.2f, 0.2f, 1.0f);

                        scaleJourneyLength = Vector3.Distance(startScale, endScale);
                        scaleJourneyCovered = 0;

                        isScalingBack = true;
                    }
                }
            }
            if (isScalingBack)
            {
                autoAttackCd = 1.5f;
                float scaleDistanceCovered = scaleSpeed * Time.deltaTime;

                scaleJourneyCovered += scaleDistanceCovered;

                float scaleJourneyProgress = scaleJourneyCovered / scaleJourneyLength;

                shoulder1.transform.localScale = Vector3.Lerp(startScale, endScale, scaleJourneyProgress);
                shoulder2.transform.localScale = Vector3.Lerp(startScale, endScale, scaleJourneyProgress);

                if (scaleJourneyProgress >= 1.0f)
                {
                    isScalingBack = false;
                    start1 = shoulder1.transform.localPosition;
                    end1 = new Vector3(start1.x, start1.y - distance, start1.z);

                    start2 = shoulder2.transform.localPosition;
                    end2 = new Vector3(start2.x, start2.y - distance, start2.z);

                    journeyLength = Vector3.Distance(start1, end1);
                    journeyCovered = 0;

                    isMovingBackwards = true;
                }
            }
            if (isMovingBackwards && !movedBackwards)
            {
                autoAttackCd = 1.5f;
                float distanceCovered = moveSpeed * Time.deltaTime;

                journeyCovered += distanceCovered;

                float journeyProgress = journeyCovered / journeyLength;

                shoulder1.transform.localPosition = Vector3.Lerp(start1, end1, journeyProgress);
                shoulder2.transform.localPosition = Vector3.Lerp(start2, end2, journeyProgress);

                if (journeyProgress >= 1.0f)
                {
                    movedBackwards = true;
                    start1 = shoulder1.transform.localPosition;
                    end1 = new Vector3(start1.x + 3.0f, start1.y, start1.z);

                    start2 = shoulder2.transform.localPosition;
                    end2 = new Vector3(start2.x - 3.0f, start2.y, start2.z);

                    journeyLength = Vector3.Distance(start1, end1);
                    journeyCovered = 0;
                }
            }
            if (isMovingBackwards && movedBackwards)
            {
                autoAttackCd = 1.5f;
                float distanceCovered = moveSpeed * Time.deltaTime;

                journeyCovered += distanceCovered;

                float journeyProgress = journeyCovered / journeyLength;

                shoulder1.transform.localPosition = Vector3.Lerp(start1, end1, journeyProgress);
                shoulder2.transform.localPosition = Vector3.Lerp(start2, end2, journeyProgress);

                if (journeyProgress >= 1.0f)
                {
                    isMovingBackwards = false;
                    bossMove.canMove = true;
                    bossMove.canRotate = true;
                    cd = 2.5f;
                    movedBackwards = false;
                    movedToSide = false;
                    movedForward = false;
                    smashAmount = 0;
                    journeyCovered = 0;
                    canSmash = false;
                    bullAmount = 25;
                    skillInUse = false;
                }
            }

            if (cd <= 0 && interval <= 0 && bossStats.hp / bossStats.maxHp < 0.25f && !skillInUse)
            {
                autoAttackCd = 1.5f;
                bossMove.rotationSpeed = 360;
                bossMove.canMove = false;
                bossMove.canRotate = false;
                Vector3 newPos1 = new Vector3(0.6f, 0f, 0f);
                currBullet = Instantiate(bullet, transform.position + transform.rotation * newPos1, transform.rotation);
                currBullet.transform.rotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z);
                currBullet = Instantiate(bullet, transform.position - transform.rotation * newPos1, transform.rotation);
                currBullet.transform.rotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z);
                currBullet = Instantiate(bullet, transform.position, transform.rotation);
                currBullet.transform.rotation = Quaternion.Euler(0, 0, boss.transform.rotation.eulerAngles.z);
                bullAmount--;
                interval = 0.08f;
                if (bullAmount <= 0)
                {
                    cd = 5f;
                    bullAmount = 25;
                    rotChosen = false;
                    skillInUse = false;
                    bossMove.canMove = true;
                    bossMove.canRotate = true;
                    bossMove.rotationSpeed = 0;
                }
            }
            //    if (cd <= 0 && interval <= 0 && bossStats.hp <= 3000 && phase == 1)
            //    {
            //        skillInUse = true;
            //        bossMove.canMove = false;
            //        bossMove.canRotate = false;
            //        if (!locChosen)
            //        {
            //            destination = player.transform.position;
            //            direction = player.transform.position - boss.transform.position;
            //            direction.Normalize();
            //            dist = Vector3.Distance(player.transform.position, boss.transform.position);    
            //            locChosen = true;
            //        }
            //        if (locChosen && castTime > 0)
            //        {
            //            bossMove.rotationSpeed = (2 - castTime) * 720;
            //            castTime -= Time.deltaTime;
            //        }
            //        if (!arrived && castTime <= 0.0f)
            //        {
            //            float distanceToDestination = Vector3.Distance(boss.transform.position, destination);

            //            if (distanceToDestination > 1.0f)
            //            {
            //                boss.transform.position += direction * speed * Time.deltaTime;
            //            }
            //            else
            //            {
            //                arrived = true;
            //            }
            //        }
            //        if (arrived)
            //        {
            //            bossMove.rotationSpeed = 120;
            //            currBullet = Instantiate(bullet, transform.position, transform.rotation);
            //            currBullet.transform.rotation = Quaternion.Euler(0, 0, 45 + boss.transform.rotation.eulerAngles.z);

            //            currBullet = Instantiate(bullet, transform.position, transform.rotation);
            //            currBullet.transform.rotation = Quaternion.Euler(0, 0, -45 + boss.transform.rotation.eulerAngles.z);

            //            currBullet = Instantiate(bullet, transform.position, transform.rotation);
            //            currBullet.transform.rotation = Quaternion.Euler(0, 0, 135 + boss.transform.rotation.eulerAngles.z);

            //            currBullet = Instantiate(bullet, transform.position, transform.rotation);
            //            currBullet.transform.rotation = Quaternion.Euler(0, 0, -135 + boss.transform.rotation.eulerAngles.z);

            //            interval = 0.07f;
            //            bullAmount--;
            //            if (bullAmount <= 0)
            //            {
            //                bossMove.rotationSpeed = 0;
            //                cd = 0.3f;
            //                bullAmount = 7;
            //                phase++;
            //            }
            //        }
            //    }
            //    if (cd <= 0 && interval <= 0 && bossStats.hp <= 3000 && phase == 2)
            //    {
            //        bossMove.rotationSpeed = -120;
            //        currBullet = Instantiate(bullet, transform.position, transform.rotation);
            //        currBullet.transform.rotation = Quaternion.Euler(0, 0, 45 + boss.transform.rotation.eulerAngles.z);

            //        currBullet = Instantiate(bullet, transform.position, transform.rotation);
            //        currBullet.transform.rotation = Quaternion.Euler(0, 0, -45 + boss.transform.rotation.eulerAngles.z);

            //        currBullet = Instantiate(bullet, transform.position, transform.rotation);
            //        currBullet.transform.rotation = Quaternion.Euler(0, 0, 135 + boss.transform.rotation.eulerAngles.z);

            //        currBullet = Instantiate(bullet, transform.position, transform.rotation);
            //        currBullet.transform.rotation = Quaternion.Euler(0, 0, -135 + boss.transform.rotation.eulerAngles.z);

            //        interval = 0.07f;
            //        bullAmount--;
            //        if (bullAmount <= 0)
            //        {
            //            phase++;
            //            bossMove.rotationSpeed = 0;
            //            cd = 0.3f;
            //            bullAmount = 5;
            //        }
            //    }
            //    if (cd <= 0 && interval <= 0 && bossStats.hp <= 3000 && phase == 3)
            //    {               
            //        bossMove.rotationSpeed = 360;
            //        currBullet = Instantiate(bullet, transform.position, transform.rotation);
            //        currBullet.transform.rotation = Quaternion.Euler(0, 0, 45 + boss.transform.rotation.eulerAngles.z);

            //        currBullet = Instantiate(bullet, transform.position, transform.rotation);
            //        currBullet.transform.rotation = Quaternion.Euler(0, 0, -45 + boss.transform.rotation.eulerAngles.z);

            //        currBullet = Instantiate(bullet, transform.position, transform.rotation);
            //        currBullet.transform.rotation = Quaternion.Euler(0, 0, 135 + boss.transform.rotation.eulerAngles.z);

            //        currBullet = Instantiate(bullet, transform.position, transform.rotation);
            //        currBullet.transform.rotation = Quaternion.Euler(0, 0, -135 + boss.transform.rotation.eulerAngles.z);

            //        interval = 0.07f;
            //        bullAmount--;
            //        if (bullAmount <= 0)
            //        {
            //            phase++;
            //            bossMove.rotationSpeed = 0;
            //            cd = 0.3f;
            //            bullAmount = 5;

            //        }
            //    }
            //    if (cd <= 0 && interval <= 0 && bossStats.hp <= 3000 && phase == 4)
            //    {               
            //        bossMove.rotationSpeed = -360;
            //        currBullet = Instantiate(bullet, transform.position, transform.rotation);
            //        currBullet.transform.rotation = Quaternion.Euler(0, 0, 45 + boss.transform.rotation.eulerAngles.z);

            //        currBullet = Instantiate(bullet, transform.position, transform.rotation);
            //        currBullet.transform.rotation = Quaternion.Euler(0, 0, -45 + boss.transform.rotation.eulerAngles.z);

            //        currBullet = Instantiate(bullet, transform.position, transform.rotation);
            //        currBullet.transform.rotation = Quaternion.Euler(0, 0, 135 + boss.transform.rotation.eulerAngles.z);

            //        currBullet = Instantiate(bullet, transform.position, transform.rotation);
            //        currBullet.transform.rotation = Quaternion.Euler(0, 0, -135 + boss.transform.rotation.eulerAngles.z);

            //        interval = 0.07f;
            //        bullAmount--;
            //        if (bullAmount <= 0)
            //        {
            //            phase = 1;
            //            bossMove.rotationSpeed = 0;
            //            cd = 4f;
            //            bullAmount = 7;
            //            bossMove.canMove = true;
            //            bossMove.canRotate = true;
            //            arrived = false;
            //            locChosen = false;
            //            skillInUse = false;
            //            castTime = 1f;
            //        }
            //    }
        }
    }
}
