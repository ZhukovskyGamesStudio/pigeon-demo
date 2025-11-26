using UnityEngine;

public class BulletCameraFollow : MonoBehaviour {
    [SerializeField] private Transform bullet;
    [SerializeField] private Vector3 offset = new Vector3(0f, 3f, -6f);
    [SerializeField] private float positionLerpSpeed = 5f;

    public void SetBullet(Transform target) {
        bullet = target;
    }

    private void LateUpdate() {
        FollowBullet();
    }

    public void FollowBullet() {
        if (bullet == null) {
            return;
        }

        var desiredPosition = bullet.position + offset;
        transform.position = Vector3.Lerp(transform.position, desiredPosition, positionLerpSpeed * Time.deltaTime);
    }
}

