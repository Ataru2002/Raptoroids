using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon Data")]
public class WeaponData : ScriptableObject
{
    public bool isPlayer;
    public ShotType shotType;
    public PlayerProjectileType playerProjectileType;
    public EnemyProjectileType enemyProjectileType;
    public float fireRate;
    public AudioClip shotSound;

    public int projectileCount;
    public float coneAngle;

    public float range;
    public float laserBreadth;
}

#if UNITY_EDITOR
[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        #region PROPERTIES
        SerializedProperty isPlayer = serializedObject.FindProperty("isPlayer");
        SerializedProperty shotType = serializedObject.FindProperty("shotType");
        SerializedProperty playerProjectileType = serializedObject.FindProperty("playerProjectileType");
        SerializedProperty enemyProjectileType = serializedObject.FindProperty("enemyProjectileType");
        SerializedProperty fireRate = serializedObject.FindProperty("fireRate");
        SerializedProperty shotSound = serializedObject.FindProperty("shotSound");

        SerializedProperty projectileCount = serializedObject.FindProperty("projectileCount");
        SerializedProperty coneAngle = serializedObject.FindProperty("coneAngle");

        SerializedProperty range = serializedObject.FindProperty("range");
        SerializedProperty laserBreadth = serializedObject.FindProperty("laserBreadth");

        #endregion

        ShotType shotTypeProperty = (ShotType)shotType.enumValueIndex;

        // Draw these by default because they are common to all weapons
        EditorGUILayout.PropertyField(isPlayer);
        EditorGUILayout.PropertyField(shotType);

        if(shotTypeProperty != ShotType.Laser)
        {
            if (isPlayer.boolValue)
            {
                EditorGUILayout.PropertyField(playerProjectileType);
            }
            else
            {
                EditorGUILayout.PropertyField(enemyProjectileType);
            }
        }

        EditorGUILayout.PropertyField(fireRate);
        EditorGUILayout.PropertyField(shotSound);

        switch (shotTypeProperty)
        {
            case ShotType.Cone:
                EditorGUILayout.PropertyField(projectileCount);
                EditorGUILayout.PropertyField(coneAngle);
                break;
            case ShotType.Laser:
                EditorGUILayout.PropertyField(range);
                EditorGUILayout.PropertyField(laserBreadth);
                break;
            case ShotType.ProjectileSequence:
                EditorGUILayout.PropertyField(projectileCount);
                break;
            default:
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}
#endif

public enum ShotType
{
    Single,
    Cone,
    Laser,
    ProjectileSequence
}