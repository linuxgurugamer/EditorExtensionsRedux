using System;
using System.Reflection;
using UnityEngine;


namespace NoOffsetBehaviour
{
    public static class Refl
    {
        public static FieldInfo GetField(object obj, int fieldNum)
        {
            int c = 0;
            foreach (FieldInfo FI in obj.GetType().GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                if (c == fieldNum)
                    return FI;
                c++;
            }
            throw new Exception("No such field: " + obj.GetType() + "#" + fieldNum.ToString());
        }
        public static object GetValue(object obj, int fieldNum)
        {
            return GetField(obj, fieldNum).GetValue(obj);
        }
        public static void SetValue(object obj, int fieldNum, object value)
        {
            GetField(obj, fieldNum).SetValue(obj, value);
        }


        public static MethodInfo GetMethod(object obj, int methodnum)
        {

            MethodInfo[] m = obj.GetType().GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            int c = 0;
            foreach (MethodInfo FI in m)
            {
                if (c == methodnum)
                    return FI;
                c++;
            }

            throw new Exception("No such method: " + obj.GetType() + "#" + methodnum);
        }
        public static object Invoke(object obj, int methodnum, params object[] args)
        {
            return GetMethod(obj, methodnum).Invoke(obj, args);

        }

    }
}
