using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDamageEffect : MonoBehaviour
{
	private const int WIDTH = 313;

	private const int HEIGHT = 43;

	[SerializeField]
	private float _height;

	[SerializeField]
	private float _width;

	[SerializeField]
	private float _duration;

	[SerializeField]
	private MeshRenderer _renderer;

	private Vector2[] FONT_METRICS = new Vector2[10]
	{
		new Vector2(0f, 42f),
		new Vector2(42f, 21f),
		new Vector2(63f, 29f),
		new Vector2(92f, 28f),
		new Vector2(120f, 34f),
		new Vector2(154f, 28f),
		new Vector2(182f, 33f),
		new Vector2(215f, 31f),
		new Vector2(246f, 34f),
		new Vector2(280f, 33f)
	};

	public float _speed;

	private float _offset;

	private bool _show;

	private float _time;

	private Vector3 _start;

	private Vector3 _direction;

	private Transform _transform;

	private void Awake()
	{
		_transform = base.transform;
		_start = _transform.position;
	}

	private void Update()
	{
		if (!_show)
		{
			return;
		}
		float num = _time * _speed - _offset;
		Vector3 vector = _direction * _time;
		vector.y = _height - num * num * _width;
		_time += Time.deltaTime;
		_transform.position = _start + vector;
		UpdateTransform();
		if (_time > _duration)
		{
			Color color = _renderer.material.GetColor("_Color");
			color.a = Mathf.Lerp(color.a, 0f, Time.deltaTime * 3f);
			_renderer.material.SetColor("_Color", color);
			if (color.a < 0.2f)
			{
				Object.Destroy(base.gameObject);
			}
		}
	}

	public void Show(DamageInfo shot)
	{
		if (_width == 0f)
		{
			_width = 1f;
		}
		MeshFilter meshFilter = base.gameObject.AddComponent<MeshFilter>();
		if ((bool)meshFilter)
		{
			meshFilter.mesh = CreateCharacterMesh(shot.Damage, FONT_METRICS, 313, 43);
		}
		UpdateTransform();
		_show = true;
		_offset = Mathf.Sqrt(_height / _width);
		_direction = Random.onUnitSphere;
		_renderer.material = new Material(_renderer.material);
		StartCoroutine(StartEnableRenderer());
	}

	private IEnumerator StartEnableRenderer()
	{
		yield return new WaitForSeconds(0.1f);
		_renderer.enabled = true;
	}

	private void UpdateTransform()
	{
		float num = Vector3.Distance(_transform.position, GameState.Current.PlayerData.Position);
		float num2 = 0.003f + 0.0005f * num * LevelCamera.FieldOfView / 60f;
		_transform.localScale = new Vector3(num2, num2, num2);
		_transform.rotation = GameState.Current.PlayerData.HorizontalRotation;
	}

	private Mesh CreateCharacterMesh(int number, Vector2[] metrics, int width, int height)
	{
		Mesh mesh = new Mesh();
		string text = Mathf.Abs(number).ToString();
		List<Vector3> list = new List<Vector3>();
		List<Vector2> list2 = new List<Vector2>();
		List<int> list3 = new List<int>();
		Vector3[] array = new Vector3[4];
		Vector2[] array2 = new Vector2[4];
		int[] array3 = new int[6] { 0, 1, 2, 0, 2, 3 };
		float num = 0f;
		float num2 = 0f;
		for (int i = 0; i < text.Length; i++)
		{
			int num3 = text[i] - 48;
			if (num3 >= 0 && num3 < 10)
			{
				for (int j = 0; j < 6; j++)
				{
					list3.Add(array3[j] + list.Count);
				}
				array[0] = new Vector3(metrics[num3].x, 0f, 0f);
				array[1] = new Vector3(metrics[num3].x + metrics[num3].y, 0f, 0f);
				array[2] = new Vector3(metrics[num3].x + metrics[num3].y, height, 0f);
				array[3] = new Vector3(metrics[num3].x, height, 0f);
				array2[0] = new Vector2(array[0].x / (float)width, array[0].y / (float)height);
				array2[1] = new Vector2(array[1].x / (float)width, array[1].y / (float)height);
				array2[2] = new Vector2(array[2].x / (float)width, array[2].y / (float)height);
				array2[3] = new Vector2(array[3].x / (float)width, array[3].y / (float)height);
				list.AddRange(array);
				list2.AddRange(array2);
				num += metrics[num3].y;
			}
		}
		for (int k = 0; k < list.Count / 4; k++)
		{
			List<Vector3> list5;
			List<Vector3> list4 = (list5 = list);
			int index2;
			int index = (index2 = k * 4 + 1);
			Vector3 vector = list5[index2];
			list4[index] = vector - new Vector3(list[k * 4].x + num / 2f - num2, height / 2);
			List<Vector3> list7;
			List<Vector3> list6 = (list7 = list);
			int index3 = (index2 = k * 4 + 2);
			vector = list7[index2];
			list6[index3] = vector - new Vector3(list[k * 4 + 3].x + num / 2f - num2, height / 2);
			List<Vector3> list9;
			List<Vector3> list8 = (list9 = list);
			int index4 = (index2 = k * 4 + 3);
			vector = list9[index2];
			list8[index4] = vector - new Vector3(list[k * 4 + 3].x + num / 2f - num2, height / 2);
			List<Vector3> list11;
			List<Vector3> list10 = (list11 = list);
			int index5 = (index2 = k * 4);
			vector = list11[index2];
			list10[index5] = vector - new Vector3(list[k * 4].x + num / 2f - num2, height / 2);
			num2 += list[k * 4 + 1].x - list[k * 4].x;
		}
		mesh.vertices = list.ToArray();
		mesh.uv = list2.ToArray();
		mesh.triangles = list3.ToArray();
		return mesh;
	}
}
