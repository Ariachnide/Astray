using System;
using System.Collections;
using UnityEngine;

public class PlayTime : MonoBehaviour {
    bool isRecording = true;
    private TimeSpan ts;
    private WaitForSecondsRealtime wait = new WaitForSecondsRealtime(1);

    public void StartRecording(string strTime) {
        ts = TimeSpan.Parse(strTime);
        StartCoroutine(RecordTimeRoutine());
    }

    public string GetTime() {
        return ts.ToString(@"hh\:mm\:ss");
    }

    public IEnumerator RecordTimeRoutine() {
        TimeSpan second = TimeSpan.FromSeconds(1);
        while(true) {
            yield return wait;
            if (isRecording) ts = ts.Add(second);
        }
    }
}
