using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlobalRegisteredElementsController : MonoBehaviour {
    [SerializeField]
    private List<RegElmData> storedRegisteredElements;

    private void Awake() {
        storedRegisteredElements = new List<RegElmData>();
    }

    public void PopulateRegistry(List<string> rawData) {
        List<RegElmData> dataList = new List<RegElmData>();
        foreach (string d in rawData)
            dataList.Add(ConvertRawRegElm(d));
        foreach (RegElmData d in dataList)
            transform
                .Find(d.mapName)
                .GetComponent<MapController>()
                .registeredElements[d.index]
                .GetComponent<IRegisteredElement>()
                .HandleStateUpdate(d.state);
    }

    private RegElmData ConvertRawRegElm(string rawElm) {
        RegElmData d = new RegElmData();
        List<string> elms = rawElm.Split("_").ToList();
        d.mapName = elms[0];
        d.index = Int16.Parse(elms[1]);
        d.state = (RegElmState)Enum.Parse(typeof(RegElmState), elms[2], true);
        return d;
    }

    public List<string> GetRawRegisteredElements() {
        List<string> rawData = new List<string>();
        foreach (RegElmData d in storedRegisteredElements)
            rawData.Add($"{d.mapName}_{d.index.ToString()}_{d.state.ToString()}");
        return rawData;
    }

    public void GetOrCreateElmInRegistry(Int16 i, string mapName, RegElmState state) {
        bool isFound = false;
        foreach (RegElmData e in storedRegisteredElements) {
            if (e.index == i && e.mapName == mapName) {
                e.state = state;
                isFound = true;
                break;
            }
        }
        if (!isFound)
            storedRegisteredElements.Add(new RegElmData() {
                index = i,
                mapName = mapName,
                state = state
            });
    }
}
