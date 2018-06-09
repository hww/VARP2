using VARP.Scheme.Data;

public class SubSystems {

    public static SubSystem[] subsystems = new SubSystem[] {
        new SubSystem()
        {
            name = "NAME",
            Init = Name.Init,
            DeInit = Name.DeInit,
            PostInit = Name.DeInit,
        },
       // new SubSystem()
       // {
       //     name = "DDRAW",
       //     Init = DebugDrawManager.Init,
       //     DeInit = DebugDrawManager.DeInit,
       //     PostInit = DebugDrawManager.DeInit,
       // }
    };


    public static void Init()
    {
        for ( var i = 0 ; i < subsystems.Length ; i++ )
            subsystems[ i ].Init ( );
        for ( var i = 0 ; i < subsystems.Length ; i++ )
            subsystems[ i ].PostInit ( );
    }

    public static void DeInit ( )
    {
        for ( var i = 0 ; i < subsystems.Length ; i++ )
            subsystems[ i ].DeInit ( );
    }

}
