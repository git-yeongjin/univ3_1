using UnityEngine;

public class ItemObj_Doll : MonoBehaviour
{
    [Header("아이템 설정")]
    public DollItemType myItemType;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerInventory player = other.GetComponent<PlayerInventory>();
            if (player == null) return;

            if (myItemType == DollItemType.Button)
            {
                player.ButtonCount++;
                Debug.Log($"플레이어가 인형 아이템 {myItemType} 습득");
            }
            else if (myItemType == DollItemType.Ribbon)
            {
                player.RibbonCount++;
                Debug.Log($"플레이어가 인형 아이템 {myItemType} 습득");
            }

            Destroy(gameObject);
        }
    }
}
