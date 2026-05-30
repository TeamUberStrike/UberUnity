using System;
using System.Collections.Generic;
using UberStrike.Core.Models;
using UberStrike.Core.Types;
using UnityEngine;

public class HUDDesktopEventStream : MonoBehaviour
{
	[Serializable]
	public class ItemGfx
	{
		public UILabel Label1;

		public UILabel Label2;

		public UILabel Label3;

		public UIHorizontalAligner Aligner;
	}

	[Serializable]
	public class Item
	{
		public string Label1 = string.Empty;

		public Color Label1EffectColor;

		public string Label2 = string.Empty;

		public string Label3 = string.Empty;

		public Color Label3EffectColor;

		public float TimeEnd;
	}

	[SerializeField]
	private UIHorizontalAligner template;

	[SerializeField]
	private GameObject container;

	[SerializeField]
	private float ySpace = -17f;

	[SerializeField]
	private int maxItems = 8;

	[SerializeField]
	private float displayTime = 5f;

	private List<ItemGfx> itemsGfx = new List<ItemGfx>();

	private Queue<Item> items = new Queue<Item>();

	private void Start()
	{
		GameData.Instance.OnHUDStreamMessage.AddEvent(AddMessage, this);
		GameData.Instance.OnHUDStreamClear.AddEvent(ClearLog, this);
		GameData.Instance.OnPlayerKilled.AddEvent(HandleKilledMessage, this);
		foreach (Transform item in template.transform.parent)
		{
			if (item != template.transform)
			{
				UnityEngine.Object.Destroy(item.gameObject);
			}
		}
		for (int i = 0; i < maxItems; i++)
		{
			GameObject gameObject = GameObjectHelper.Instantiate(template.gameObject, template.transform.parent, new Vector3(0f, (float)i * ySpace, 0f));
			gameObject.name = "Item " + i;
			itemsGfx.Add(new ItemGfx
			{
				Label1 = gameObject.transform.Find("1_label").GetComponent<UILabel>(),
				Label2 = gameObject.transform.Find("2_label").GetComponent<UILabel>(),
				Label3 = gameObject.transform.Find("3_label").GetComponent<UILabel>(),
				Aligner = gameObject.GetComponent<UIHorizontalAligner>()
			});
		}
		template.gameObject.SetActive(false);
		ApplyChanges();
	}

	private void ApplyChanges()
	{
		int i = 0;
		foreach (Item item in items)
		{
			ItemGfx itemGfx = itemsGfx[i];
			itemGfx.Aligner.gameObject.SetActive(true);
			itemGfx.Label1.text = item.Label1;
			itemGfx.Label1.effectColor = item.Label1EffectColor;
			itemGfx.Label2.text = item.Label2;
			itemGfx.Label3.text = item.Label3;
			itemGfx.Label3.effectColor = item.Label3EffectColor;
			itemGfx.Label1.gameObject.SetActive(itemGfx.Label1.text != string.Empty);
			itemGfx.Label2.gameObject.SetActive(itemGfx.Label2.text != string.Empty);
			itemGfx.Label3.gameObject.SetActive(itemGfx.Label3.text != string.Empty);
			itemGfx.Aligner.Reposition();
			i++;
		}
		for (; i < maxItems; i++)
		{
			itemsGfx[i].Aligner.gameObject.SetActive(false);
		}
	}

	public void AddMessage(GameActorInfo player1, string actionString, GameActorInfo player2)
	{
		items.Enqueue(new Item
		{
			Label1 = ((!string.IsNullOrEmpty(player1.ClanTag)) ? ("[" + player1.ClanTag + "] " + player1.PlayerName) : player1.PlayerName),
			Label1EffectColor = GetPlayerColor(player1),
			Label2 = actionString,
			Label3 = ((player2 == null) ? string.Empty : ((!string.IsNullOrEmpty(player2.ClanTag)) ? ("[" + player2.ClanTag + "] " + player2.PlayerName) : player2.PlayerName)),
			Label3EffectColor = GetPlayerColor(player2),
			TimeEnd = Time.time + displayTime
		});
		if (items.Count > maxItems)
		{
			items.Dequeue();
		}
		ApplyChanges();
	}

	public void ClearLog()
	{
		items.Clear();
		ApplyChanges();
	}

	public void DoAnimateDown(bool down)
	{
		SpringPosition.Begin(container.gameObject, new Vector3(0f, (!down) ? 0f : (-60f), 0f), 10f).onFinished = delegate(SpringPosition el)
		{
			el.enabled = false;
		};
	}

	private void Update()
	{
		while (items.Count > 0 && Time.time >= items.Peek().TimeEnd)
		{
			items.Dequeue();
			ApplyChanges();
		}
	}

	public static Color GetPlayerColor(GameActorInfo player)
	{
		if (player == null)
		{
			return Color.white;
		}
		if (player.Cmid == PlayerDataManager.Cmid)
		{
			return Color.green.SetAlpha(28f / 51f);
		}
		if (GameState.Current.GameMode == GameModeType.DeathMatch)
		{
			return new Color(0.5019608f, 0.5019608f, 0.5019608f, 28f / 51f);
		}
		if (player.TeamID == TeamID.BLUE)
		{
			return GUIUtils.ColorBlue.SetAlpha(28f / 51f);
		}
		if (player.TeamID == TeamID.RED)
		{
			return GUIUtils.ColorRed.SetAlpha(28f / 51f);
		}
		return Color.black;
	}

	public static void HandleKilledMessage(GameActorInfo shooter, GameActorInfo target, UberstrikeItemClass weapon, BodyPart bodyPart)
	{
		if (GameState.Current.GameMode == GameModeType.None || target == null)
		{
			return;
		}
		if (shooter == null || shooter == target)
		{
			GameData.Instance.OnHUDStreamMessage.Fire(target, LocalizedStrings.NKilledThemself, null);
			return;
		}
		string empty = string.Empty;
		if (weapon == UberstrikeItemClass.WeaponMelee)
		{
			empty = "smacked";
		}
		else
		{
			switch (bodyPart)
			{
			case BodyPart.Head:
				empty = "headshot";
				break;
			case BodyPart.Nuts:
				empty = "nutshot";
				break;
			default:
				empty = "killed";
				break;
			}
		}
		GameData.Instance.OnHUDStreamMessage.Fire(shooter, empty, target);
	}
}
