using System;

// Code from : https://forum.unity.com/threads/sendmessage-cannot-be-called-during-awake-checkconsistency-or-onvalidate-can-we-suppress.537265/
public static class ValidationBugFix 
{
    public static void SafeValidate(Action onValidateAction)
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.delayCall += validate;


        void validate()
        {
            UnityEditor.EditorApplication.delayCall -= validate;

            onValidateAction();
        }
#endif
    }
}
