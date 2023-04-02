public class BehaviorTreeEnums
{
    public enum TreeEnemyValues
    {
        Rigidbody,
        Animator,
        Transform,
        MeshRenderer,
        EnemyManager
    }

    public enum TreeExternValues
    {
        PlayerTransform,
        EnvironmentGridManager,
        TickManager,
        PlayerHandlerMovement,
        PoolService,
        PlayerHealth,
        HypeService
    }

    public enum NodeState
    {
        SUCCESS,
        FAILURE
    }

    public enum InternValueType
    {
        VECTOR3,
        INT,
        FLOAT,
        NONE,
        CALLBACK,
        BOOL
    }

    public enum InternValuePropertyType
    {
        GET,
        SET,
        REMOVE
    }
}