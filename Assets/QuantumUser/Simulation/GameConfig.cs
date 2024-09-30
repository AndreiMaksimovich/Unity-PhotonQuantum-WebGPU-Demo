using UnityEngine.Serialization;

namespace Quantum 
{
  using Photon.Deterministic;
  using UnityEngine;
  
  public class GameConfig: AssetObject
  {

    [Header("Player Input")] 
    [Tooltip("Relative to DPI")]
    public float minDistanceToPointer = 0.25f;
    
    [Header("Player")]
    public FP playerSpeed;
    public FP playerSpeedBoost = FP._1_25;
    public int playerShootPeriodicity = 15;
    
    [Header("Player Projectile")]
    public EntityPrototype playerPrototype;
    public FP playerProjectileSpeed;
    public int playerProjectileLifeTime = 90;
    public EntityPrototype playerProjectilePrototype;

    [Header("Dots")] 
    public int dotCount = 10000;
    public EntityPrototype dotPrototype;
    public int dotsToUpdatePerFrame = 100;
    public float spawnDispawnAnimationDuration = 0.5f;
    
    [Tooltip("In frames")]
    public int dotRespawnDuration = 90;
    
    [Header("Map configuration")]
    public FPVector2 mapSize = new FPVector2(25, 25);
    
  }
}