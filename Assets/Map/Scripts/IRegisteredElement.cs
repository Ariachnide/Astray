using System;

public enum RegElmState {
    available,
    empty,
    ended
}

public class RegElmData {
    public Int16 index;
    public string mapName;
    public RegElmState state;
}

public interface IRegisteredElement {
    public void HandleStateUpdate(RegElmState s);
}
