using System;
using System.Collections.Generic;
using UnityEngine;

public enum ConditionalLoot {
    none,
    ifPlayerIsHurt,
    ifPlayerLacksMana,
    ifPlayerLackBomb
}

public class LootController : MonoBehaviour {
    [Serializable]
    public class LootElement {
        public GameObject lootGO;
        public int tickets;
        public ConditionalLoot condition;
    }
    [SerializeField]
    private int emptyTickets = 0;
    [SerializeField]
    private GameObject player;
    [SerializeField]
    private List<LootElement> lootTable = new List<LootElement>();

    private GameObject AccessPlayerValue() {
        if (player == null) player = GameObject.FindWithTag("Player");
        return player;
    }

    public GameObject HandleLoot() {
        if (lootTable.Count == 0) return null;
        // RunTests();
        int ticketAccount = 0;
        List<LootElement> filteredLootTable = new List<LootElement> ();
        foreach (LootElement e in lootTable) {
            switch (e.condition) {
                case ConditionalLoot.none:
                    break;
                case ConditionalLoot.ifPlayerIsHurt:
                    if (AccessPlayerValue().GetComponent<PlayerMain>().hitPoints < AccessPlayerValue().GetComponent<PlayerMain>().maxHP)
                        break;
                    continue;
                case ConditionalLoot.ifPlayerLacksMana:
                    if (AccessPlayerValue().GetComponent<PlayerSpell>().manaPoints < AccessPlayerValue().GetComponent<PlayerSpell>().manaPoints)
                        break;
                    continue;
                case ConditionalLoot.ifPlayerLackBomb:
                    if (AccessPlayerValue().GetComponent<PlayerInventory>().bombs < AccessPlayerValue().GetComponent<PlayerInventory>().maxBomb)
                        break;
                    continue;
            }
            filteredLootTable.Add(e);
            ticketAccount += e.tickets;
        }
        ticketAccount += emptyTickets;
        int chosenTicket = UnityEngine.Random.Range(0, ticketAccount + 1);
        int draw = 0, currentPile;
        // Debug.Log(ticketAccount);
        foreach (LootElement e in filteredLootTable) {
            currentPile = 0;
            while (currentPile < e.tickets) {
                if (chosenTicket == draw) return e.lootGO;
                currentPile++;
                draw++;
            }
        }
        return null;
    }

    /*
    private void RunTests() {
        int ticketAccount = 0;
        List<LootElement> filteredLootTable = new List<LootElement>();
        foreach (LootElement e in lootTable) {
            switch (e.condition) {
                case ConditionalLoot.none:
                    break;
                case ConditionalLoot.ifPlayerIsHurt:
                    if (player.GetComponent<PlayerMain>().hitPoints < player.GetComponent<PlayerMain>().maxHP)
                        break;
                    continue;
                case ConditionalLoot.ifPlayerLacksMana:
                    if (player.GetComponent<PlayerSpell>().GetManaValue() < player.GetComponent<PlayerSpell>().GetManaMaxValue())
                        break;
                    continue;
                case ConditionalLoot.ifPlayerLackBomb:
                    if (player.GetComponent<PlayerInventory>().bombs < player.GetComponent<PlayerInventory>().maxBomb)
                        break;
                    continue;
            }
            filteredLootTable.Add(e);
            ticketAccount += e.tickets;
        }
        ticketAccount += emptyTickets;
        int count = 0;
        List<int> choices = new List<int>() { 0 };
        foreach (LootElement e in filteredLootTable) choices.Add(0);
        Int16 choicesCount;
        bool found;
        while (count < 1500) {
            int chosenTicket = UnityEngine.Random.Range(0, ticketAccount + 1);
            int draw = 0, currentPile;
            choicesCount = 0;
            found = false;
            foreach (LootElement e in filteredLootTable) {
                if (found) break;
                currentPile = 0;
                while (currentPile < e.tickets) {
                    if (chosenTicket == draw) {
                        choices[choicesCount]++;
                        found = true;
                        break;
                    }
                    currentPile++;
                    draw++;
                }
                choicesCount++;
            }
            count++;
            if (found) continue;
            choices[choices.Count - 1]++;
        }
        for (int i = 0; i < choices.Count - 1; i++) {
            Debug.Log($"Item {filteredLootTable[i].lootGO.elementName} dropped {choices[i]} times");
        }
        Debug.Log($"No item dropped {choices[choices.Count - 1]} times");
    }
    */
}
