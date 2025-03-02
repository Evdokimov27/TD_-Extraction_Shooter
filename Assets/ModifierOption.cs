using Inventory.Scripts.Core.Items;
using Inventory.Scripts.Core.ScriptableObjects;
using Inventory.Scripts.Core.ScriptableObjects.Configuration.Events;
using UnityEngine;

namespace Inventory.Scripts.Options
{
	public class ModifierOption : MonoBehaviour
	{
		[Header("Supplier")][SerializeField] private InventorySupplierSo inventorySupplierSo;

		[Header("Listening on...")]
		[SerializeField]
		private OnItemExecuteOptionEventChannelSo onItemExecuteOptionEventChannelSo;

		[Header("Broadcasting on...")]
		[SerializeField]
		private WindowContainerChangeStateEventChannelSo windowContainerChangeStateEventChannelSo;

		private void OnEnable()
		{
			onItemExecuteOptionEventChannelSo.OnEventRaised += HandleModifierOption;
		}

		private void OnDisable()
		{
			onItemExecuteOptionEventChannelSo.OnEventRaised -= HandleModifierOption;
		}
		private void HandleModifierOption(AbstractItem inventoryItem)
		{
			// Add your action in here...
			Debug.Log("Executing option...");
		}
	}
}