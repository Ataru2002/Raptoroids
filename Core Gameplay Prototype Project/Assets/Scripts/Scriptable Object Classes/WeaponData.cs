using UnityEngine;
using UnityEditor;

[CreateAssetMenu(fileName = "New Weapon Data", menuName = "Weapon Data")]
public class WeaponData : ScriptableObject
{
    public bool isPlayer;
    public ShotType shotType;
    public float fireRate;
    public AudioClip shotSound;

    public int projectileCount;
    public float coneAngle;
}

[CustomEditor(typeof(WeaponData))]
public class WeaponDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        #region PROPERTIES
        SerializedProperty isPlayer = serializedObject.FindProperty("isPlayer");
        SerializedProperty shotType = serializedObject.FindProperty("shotType");
        SerializedProperty fireRate = serializedObject.FindProperty("fireRate");
        SerializedProperty projectileCount = serializedObject.FindProperty("projectileCount");
        SerializedProperty coneAngle = serializedObject.FindProperty("coneAngle");
        SerializedProperty shotSound = serializedObject.FindProperty("shotSound");
        #endregion

        ShotType shotTypeProperty = (ShotType)shotType.enumValueIndex;

        // Draw these by default because they are common to all weapons
        EditorGUILayout.PropertyField(isPlayer);
        EditorGUILayout.PropertyField(shotType);
        EditorGUILayout.PropertyField(fireRate);
        EditorGUILayout.PropertyField(shotSound);

        switch (shotTypeProperty)
        {
            case ShotType.Cone:
                EditorGUILayout.PropertyField(projectileCount);
                EditorGUILayout.PropertyField(coneAngle);
                break;
            default:
                break;
        }

        serializedObject.ApplyModifiedProperties();
    }
}