using UnityEngine;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;

public enum WeaponTypes
{ 
	Auto,
	Semi,
	Single,
	Melee,
	Grenade
}

[System.Serializable]
[CreateAssetMenu(menuName = "Weapons/Create Weapon", order = 0)]
public class PlayerWeapon : ScriptableObject
{

	public string weaponName;
	public WeaponTypes weaponType;

	public int damage = 10;
	public float range = 100f;
	public float fireRate = 0f;

	public int maxBullets = 20;
	[HideInInspector] public int bullets;

	public float reloadTime = 1f;
	public float explosionRange = 1f;

	public GameObject graphics;

	public List<AudioClip> UseSounds;

	// -- camera shake settings
	public float fireCameraShakeIntensity;
	public float fireCameraShakeTime;

	// -- weapon recoil ssettings
	public float verticalRecoilPerShot = .1f;
	public float maxVerticalRecoil = 10f;
	public Vector2 horizontalRecoilPerShot = new Vector2(-.1f, .1f);
	public float horizontalRecoilAfterMaxVerticalRecoil = 2f;
	public float recoilRecoveryTime = 1f;

	public PlayerWeapon()
	{
		bullets = maxBullets;
	}

	public AudioClip GetRandomShotSound() => UseSounds[Random.Range(0, UseSounds.Count - 1)];

}

[CustomEditor(typeof(PlayerWeapon))]
public class PlayerWeaponEditor : Editor
{
    public override void OnInspectorGUI()
    {
		PlayerWeapon pw = (PlayerWeapon)target;

		var centeredLabelStyle = GUI.skin.GetStyle("Label");
		centeredLabelStyle.alignment = TextAnchor.MiddleCenter;

		var boldLabelStyle = GUI.skin.GetStyle("Label");
		boldLabelStyle.fontStyle = FontStyle.Bold;

		var centeredTextFieldStyle = GUI.skin.GetStyle("TextField");
		centeredTextFieldStyle.alignment = TextAnchor.MiddleCenter;

		GUILayout.BeginVertical();
		GUILayout.Label("Weapon Name");
		pw.weaponName = GUILayout.TextField(pw.weaponName, centeredTextFieldStyle);
		GUILayout.EndVertical();

		GUILayout.Space(20f);
		GUILayout.BeginHorizontal();
		GUILayout.Label("Damage");
		pw.damage = int.Parse(EditorGUILayout.TextField(pw.damage.ToString()));
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Weapon Graphics");
		pw.graphics = EditorGUILayout.ObjectField("", pw.graphics, typeof(GameObject), false) as GameObject;
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Use Sounds");
		EditorGUILayout.PropertyField(serializedObject.FindProperty("UseSounds"), true);
		serializedObject.ApplyModifiedProperties();
		GUILayout.EndHorizontal();

		GUILayout.Space(20f);

		GUILayout.Label("CAMERA SHAKE SETTINGS");

		GUILayout.BeginHorizontal();
		GUILayout.Label("Fire Camera Shake Intensity");
		pw.fireCameraShakeIntensity = EditorGUILayout.FloatField(pw.fireCameraShakeIntensity);
		GUILayout.EndHorizontal();

		GUILayout.BeginHorizontal();
		GUILayout.Label("Fire Camera Shake Time");
		pw.fireCameraShakeTime = EditorGUILayout.FloatField(pw.fireCameraShakeTime);
		GUILayout.EndHorizontal();

		GUILayout.Space(20f);

		GUILayout.BeginHorizontal();
		GUILayout.Label("Weapon Type");
		pw.weaponType = (WeaponTypes)EditorGUILayout.EnumPopup("", pw.weaponType);
		GUILayout.EndHorizontal();


		GUILayout.Space(20f);

		if (pw.weaponType == WeaponTypes.Auto || pw.weaponType == WeaponTypes.Semi || pw.weaponType == WeaponTypes.Single)
		{

			if (pw.weaponType == WeaponTypes.Auto) {
				GUILayout.Label("WEAPON RECOIL SETTINGS");

				GUILayout.BeginHorizontal();
				GUILayout.Label("Vertical Recoil per Shot");
				pw.verticalRecoilPerShot = EditorGUILayout.FloatField(pw.verticalRecoilPerShot);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Maximum Vertical Recoil");
				pw.maxVerticalRecoil = EditorGUILayout.FloatField(pw.maxVerticalRecoil);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Horizontal Recoil per Shot");
				pw.horizontalRecoilPerShot.x = EditorGUILayout.FloatField(pw.horizontalRecoilPerShot.x);
				pw.horizontalRecoilPerShot.y = EditorGUILayout.FloatField(pw.horizontalRecoilPerShot.y);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Horizontal Recoil Multiplier After Vertical Recoil Reached");
				pw.horizontalRecoilAfterMaxVerticalRecoil = EditorGUILayout.FloatField(pw.horizontalRecoilAfterMaxVerticalRecoil);
				GUILayout.EndHorizontal();

				GUILayout.BeginHorizontal();
				GUILayout.Label("Recoil Recovery Time");
				pw.recoilRecoveryTime = EditorGUILayout.FloatField(pw.recoilRecoveryTime);
				GUILayout.EndHorizontal();

				GUILayout.Space(20f);
			}

			GUILayout.BeginHorizontal();
			GUILayout.Label("Range");
			pw.range = EditorGUILayout.FloatField(pw.range);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Fire Rate");
			pw.fireRate = EditorGUILayout.FloatField(pw.fireRate);
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Max Bullets");
			pw.maxBullets = int.Parse(EditorGUILayout.TextField(pw.maxBullets.ToString()));
			GUILayout.EndHorizontal();

			GUILayout.BeginHorizontal();
			GUILayout.Label("Reload Time");
			pw.reloadTime = EditorGUILayout.FloatField(pw.reloadTime);
			GUILayout.EndHorizontal();

		}
		else if (pw.weaponType == WeaponTypes.Melee)
		{

			GUILayout.BeginHorizontal();
			GUILayout.Label("Use Rate");
			pw.fireRate = EditorGUILayout.FloatField(pw.fireRate);
			GUILayout.EndHorizontal();

		}
		else if (pw.weaponType == WeaponTypes.Grenade)
		{

			GUILayout.BeginHorizontal();
			GUILayout.Label("Explosion Range");
			pw.explosionRange = EditorGUILayout.FloatField(pw.explosionRange);
			GUILayout.EndHorizontal();

		}


		if (GUI.changed)
		{
			EditorUtility.SetDirty(target);
		}

	}
}
