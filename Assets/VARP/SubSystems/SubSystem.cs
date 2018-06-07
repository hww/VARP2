using System;

public class SubSystem
{
    public string name;
    public Action Init;
    public Action PostInit;
    public Action DeInit;
    public Action<Entry> OnLoad;
    public Action<Entry> OnFree;

    public SubSystem()
    {
        Init = VoidMethod;
        PostInit = VoidMethod;
        DeInit = VoidMethod;
        OnLoad = VoidMethod;
        OnFree = VoidMethod;
    }

    private void VoidMethod ( ) { }
    private void VoidMethod ( Entry entry ) { }
}
