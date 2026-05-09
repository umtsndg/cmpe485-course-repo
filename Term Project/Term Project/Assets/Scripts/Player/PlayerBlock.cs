using UnityEngine;

public class PlayerBlock : MonoBehaviour
{
    [Header("References")]
    public SwordMovement swordAttack;
    public Transform playerView;

    [Header("Block Settings")]
    public int maxBlockHp = 3;
    public float blockAngle = 60f;
    public float parryWindowDuration = 1f;
    public float hpRegenInterval = 5f;

    private int currentBlockHp;
    private float currentBlockStartTime = -999f;
    private float regenTimer = 0f;
    private bool wasBlockingLastFrame = false;

    void Start()
    {
        currentBlockHp = maxBlockHp;
    }

    void Update()
    {
        TrackBlockStart();
        HandleRegen();
        if (Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("Blocking: " + IsBlocking() +
                      " | Parry: " + IsParryWindowActive() +
                      " | HP: " + currentBlockHp + "/" + maxBlockHp);
        }

    }

    void TrackBlockStart()
    {
        bool isBlockingNow = IsBlocking();

        if (isBlockingNow && !wasBlockingLastFrame)
        {
            currentBlockStartTime = Time.time;
            regenTimer = 0f;
        }

        wasBlockingLastFrame = isBlockingNow;
    }

    void HandleRegen()
    {
        if (IsBlocking()) return;
        if (currentBlockHp >= maxBlockHp) return;

        regenTimer += Time.deltaTime;

        if (regenTimer >= hpRegenInterval)
        {
            currentBlockHp += 1;

            if (currentBlockHp > maxBlockHp)
            {
                currentBlockHp = maxBlockHp;
            }

            regenTimer = 0f;
        }
    }

    public bool IsBlocking()
    {
        if (swordAttack == null) return false;
        return swordAttack.IsBlocking();
    }

    public bool IsParryWindowActive()
    {
        return IsBlocking() && Time.time <= currentBlockStartTime + parryWindowDuration;
    }

    public bool CanBlockFromDirection(Vector3 incomingDirection)
    {
        if (!IsBlocking()) return false;
        if (playerView == null) return false;

        Vector3 flatForward = playerView.forward;
        flatForward.y = 0f;
        flatForward.Normalize();

        Vector3 flatIncoming = -incomingDirection;
        flatIncoming.y = 0f;
        flatIncoming.Normalize();

        float angle = Vector3.Angle(flatForward, flatIncoming);

        Debug.Log("Block angle: " + angle);
        return angle <= blockAngle;
    }

    public BlockResult TryHandleIncomingHit(int damage, Vector3 incomingDirection)
    {
        BlockResult result = new BlockResult();

        result.wasBlocked = false;
        result.wasParried = false;
        result.blockBroke = false;

        if (!IsBlocking()) return result;
        if (currentBlockHp <= 0) return result;
        if (!CanBlockFromDirection(incomingDirection)) return result;

        result.wasBlocked = true;
        result.wasParried = IsParryWindowActive();


        if (result.wasParried)
        {
            regenTimer = 0f;
            return result;
        }

        if (damage >= currentBlockHp)
        {
            currentBlockHp = 0;
            result.blockBroke = true;

            if (swordAttack != null)
            {
                swordAttack.ForceStopBlocking();
            }
        }
        else
        {
            currentBlockHp -= damage;
        }

        regenTimer = 0f;
        return result;
    }

    public int GetCurrentBlockHp()
    {
        return currentBlockHp;
    }

    public int GetMaxBlockHp()
    {
        return maxBlockHp;
    }

    public bool CanStartBlock()
    {
        return currentBlockHp > 0;
    }
}

public struct BlockResult
{
    public bool wasBlocked;
    public bool wasParried;
    public bool blockBroke;
}