using UnityEngine;
public class GunBarrel : MonoBehaviour
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private float _cooldown = 0.1f;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private Transform _fireDirection;
    [SerializeField] private string[] _tagsToIgnore;
    
    private GunComponent _parentGunComponent;
    private float _cooldownTimer = 0.0f;
    public void Initialize(GunComponent parentGunComponent)
    {
        _parentGunComponent = parentGunComponent;
    }
    
    private void Update()
    {
        _cooldownTimer = Mathf.Max(0, _cooldownTimer - Time.deltaTime);
    }
    public void TryFire()
    {
        if (_cooldownTimer <= 0)
        {
            Fire();
            _cooldownTimer = _cooldown;
        }
    }
    
    private void Fire()
    {
        print("Firing");
        Vector3 direction = _fireDirection.position - _firePoint.position;
        Vector3 sourceVelocity = _parentGunComponent.ParentShip.Rb2d.velocity;
        GameObject projectile = Instantiate(_projectilePrefab, _firePoint.position, Quaternion.identity);
        projectile.GetComponent<Projectile>().Initialize(direction, sourceVelocity, _tagsToIgnore);
    }
}
