using UnityEngine;
public class GunBarrel : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _cooldown = 0.1f;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Transform _fireDirection;
    [SerializeField] private string[] _tagsToIgnore;
    
    private float _cooldownTimer = 0.0f;
    
    public void TryFire()
    {
        if (_cooldownTimer <= 0)
        {
            Fire();
            _cooldownTimer = _cooldown;
        }
        _cooldownTimer -= Time.deltaTime;
        _cooldownTimer = Mathf.Max(0, _cooldownTimer);
    }
    
    private void Fire()
    {
        Vector3 direction = _fireDirection.position - _firePoint.position;
        GameObject projectile = Instantiate(_projectilePrefab, _firePoint.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().Initialize(direction, _tagsToIgnore);
    }
}
