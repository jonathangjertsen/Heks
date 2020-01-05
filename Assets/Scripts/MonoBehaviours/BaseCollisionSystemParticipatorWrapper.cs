using UnityEngine;

public abstract class BaseCollisionSystemParticipatorWrapper : MonoBehaviour
{
    [Header("Debug")]
    public bool logCollisions;

    public abstract ICollisionSystemParticipator GetCollisionSystemParticipator();

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (GetOtherParticipator(collision.collider, out var other))
        {
            GetCollisionSystemParticipator().CollidedWith(other);
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (GetOtherParticipator(collision.collider, out var other))
        {
            GetCollisionSystemParticipator().ExitedCollisionWith(other);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (GetOtherParticipator(collision, out var other))
        {
            GetCollisionSystemParticipator().TriggeredWith(other);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (GetOtherParticipator(collision, out var other))
        {
            GetCollisionSystemParticipator().ExitedTriggerWith(other);
        }
    }

    private bool GetOtherParticipator(Collider2D coll, out ICollisionSystemParticipator participator)
    {
        var wrap = coll.gameObject.GetComponent<ICollisionSystemParticipatorWrapper>();
        if (wrap != null)
        {
            var other = wrap.GetCollisionSystemParticipator();
            if (other != null)
            {
                participator = other;
                Log($"Event between {this} and {other}");
                return true;
            }
        }

        Log($"No event between {this} and {coll}");
        participator = default;
        return false;
    }

    private void Log(object obj)
    {
        if (logCollisions)
        {
            Debug.Log(obj);
        }
    }
}
