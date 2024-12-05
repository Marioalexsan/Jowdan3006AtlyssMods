using System.Reflection;

namespace MoreBankTabs
{
    public static class Extensions
    {
        public static T GetPrivateField<T>(this object instance, string name)
        {
            FieldInfo field = instance.GetType().GetField(name, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)field.GetValue(instance);
        }

        public static void InvokePrivateMethod(this object instance, string name, params object[] args)
        {
            MethodInfo method = instance.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);
            method.Invoke(instance, args);
        }

        public static T InvokePrivateMethod<T>(this object instance, string name, params object[] args)
        {
            MethodInfo method = instance.GetType().GetMethod(name, BindingFlags.NonPublic | BindingFlags.Instance);
            return (T)method.Invoke(instance, args);
        }
    }
}
