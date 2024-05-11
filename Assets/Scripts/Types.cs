using UnityEngine;

public static class Types
{
    // Type that determines the action executed with an instruction
    public enum InstructionType
    {
        LookAt,
        Video,
        ShowObject,
        HideObject,
        TeleportObject,
        MoveObject,
        RotateObject,
        DisableButton,
        EnableButton
    }

    // The type of an instruction
    [System.Serializable]
    public struct Instruction
    {
        public InstructionType instructionType;
        public AudioClip audioClip;
        public GameObject target;
        public Vector3 optPosition;     // ToDo: only show when instructionType == MoveObject (custom inspector)
    }

    // Enum for all available Functions for the conditions
    public enum ConditionType
    {
        A_ButtonPress,
        RotateLeft,
        RotateRight,
        Teleport,
        ContinuousMove,
        SelectObject,
        RaySelect,
        TakePhoto,
        X_ButtonPress,
        CompareValue
    };

    // The type of a condition
    [System.Serializable]
    public struct Condition
    {
        public ConditionType selectedFunction;
        public GameObject[] objects;
        public string comparison;
    };
}
