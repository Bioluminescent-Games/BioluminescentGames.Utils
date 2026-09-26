using System;
using System.Collections;
using System.Linq;
using System.Reflection;
using System.Text;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace BioluminescentGames.Utils.Runtime
{
    // Some parts are based off Editor Attributes's ReflectionUtils.
    public static class ReflectionUtils
    {
        public const BindingFlags BINDING_FLAGS = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.FlattenHierarchy;
        
        /// <summary>
        /// Finds a field inside a serialized object
        /// </summary>
        /// <param name="fieldName">The name of the field to search</param>
        /// <param name="property">The serialized property</param>
        /// <returns>The field info of the desired field</returns>
#if UNITY_EDITOR
        public static FieldInfo FindField(string fieldName, SerializedProperty property)
        {
            if (fieldName.Contains('.'))
                return GetStaticMemberInfoFromPath(fieldName, MemberTypes.Field) as FieldInfo;

            Type serializedObjectType = GetNestedObjectType(property, out _);
            FieldInfo fieldInfo = serializedObjectType != null ? serializedObjectType.GetField(fieldName, BINDING_FLAGS) : FindField(fieldName, property.serializedObject.targetObject);

            return fieldInfo;
        }
#endif
        
        internal static FieldInfo FindField(string fieldName, object targetObject) => FindMember(fieldName, targetObject?.GetType(), BINDING_FLAGS, MemberTypes.Field) as FieldInfo;
        internal static PropertyInfo FindProperty(string propertyName, object targetObject) => FindMember(propertyName, targetObject?.GetType(), BINDING_FLAGS, MemberTypes.Property) as PropertyInfo;
        
        /// <summary>
        /// Finds a property inside a serialized object
        /// </summary>
        /// <param name="propertyName">The name of the property to search</param>
        /// <param name="property">The serialized property</param>
        /// <returns>The property info of the desired property</returns>
#if UNITY_EDITOR
        public static PropertyInfo FindProperty(string propertyName, SerializedProperty property)
        {
            if (propertyName.Contains('.'))
                return GetStaticMemberInfoFromPath(propertyName, MemberTypes.Property) as PropertyInfo;

            Type serializedObjectType = GetNestedObjectType(property, out _);
            PropertyInfo propertyInfo = serializedObjectType != null ? serializedObjectType.GetProperty(propertyName, BINDING_FLAGS) : FindProperty(propertyName, property.serializedObject.targetObject);

            return propertyInfo;
        }
#endif
        
        /// <summary>
        /// Finds a member inside a serialzied object
        /// </summary>
        /// <param name="memberName">The name of the member to look for</param>
        /// <param name="serializedProperty">The serialized property</param>
        /// <returns>The member info of the member</returns>
#if UNITY_EDITOR
        public static MemberInfo GetValidMemberInfo(string memberName, SerializedProperty serializedProperty)
        {
            MemberInfo memberInfo;
        
            memberInfo = FindField(memberName, serializedProperty);
        
            memberInfo ??= FindProperty(memberName, serializedProperty);
            memberInfo ??= FindFunction(memberName, serializedProperty);
        
            return memberInfo;
        }
#endif
        
        internal static MemberInfo GetValidMemberInfo(string memberName, object targetObject)
        {
            MemberInfo memberInfo;
        
            memberInfo = FindField(memberName, targetObject);
        
            memberInfo ??= FindProperty(memberName, targetObject);
            memberInfo ??= FindFunction(memberName, targetObject);
        
            return memberInfo;
        }
        
        /// <summary>
        /// Finds a funciton inside a serialized object
        /// </summary>
        /// <param name="functionName">The name of the function to search</param>
        /// <param name="property">The serialized property</param>
        /// <returns>The method info of the desired function</returns>
#if UNITY_EDITOR
        public static MethodInfo FindFunction(string functionName, SerializedProperty property)
        {
            if (functionName.Contains('.'))
                return GetStaticMemberInfoFromPath(functionName, MemberTypes.Method) as MethodInfo;

            MethodInfo methodInfo = null;
            Type serializedObjectType = GetNestedObjectType(property, out _);

            if (serializedObjectType != null)
            {
                try
                {
                    methodInfo = serializedObjectType.GetMethod(functionName, BINDING_FLAGS);
                }
                catch (AmbiguousMatchException)
                {
                    MethodInfo[] functions = serializedObjectType.GetMethods();

                    foreach (var function in functions)
                    {
                        if (function.Name == functionName)
                            methodInfo = function;
                    }
                }
            }
            else
            {
                methodInfo = FindFunction(functionName, property.serializedObject.targetObject);
            }

            return methodInfo;
        }
#endif
        
        internal static MethodInfo FindFunction(string functionName, object targetObject)
        {
            try
            {
                return FindMember(functionName, targetObject?.GetType(), BINDING_FLAGS, MemberTypes.Method) as MethodInfo;
            }
            catch (AmbiguousMatchException)
            {
                MethodInfo[] functions = targetObject?.GetType().GetMethods();

                foreach (var function in functions)
                {
                    if (function.Name == functionName)
                        return function;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the type of a nested serialized object
        /// </summary>
        /// <param name="property">The serialized property</param>
        /// <param name="nestedObject">Outputs the serialized nested object</param>
        /// <returns>The nested object type</returns>
#if UNITY_EDITOR
        public static Type GetNestedObjectType(SerializedProperty property, out object nestedObject)
        {
            try
            {
                nestedObject = property.serializedObject.targetObject;
                int cutPathIndex = property.propertyPath.LastIndexOf('.');

                if (cutPathIndex == -1) // If the cutPathIndex is -1 it means that the member is not nested and we return null
                    return null;

                string path = property.propertyPath[..cutPathIndex].Replace(".Array.data[", "[");
                string[] elements = path.Split('.');

                foreach (var element in elements)
                {
                    if (element.Contains("["))
                    {
                        var elementName = element[..element.IndexOf("[")];
                        var index = Convert.ToInt32(element[element.IndexOf("[")..].Replace("[", "").Replace("]", ""));

                        nestedObject = GetValue(nestedObject, elementName, index);
                    }
                    else
                    {
                        nestedObject = GetValue(nestedObject, element);
                    }
                }

                return nestedObject?.GetType();
            }
            catch (ObjectDisposedException)
            {
                nestedObject = null;
                return null;
            }
        }
#endif

        /// <summary>
        /// Finds a member from the target and it's inherited types
        /// </summary>
        /// <param name="memberName">The name of the member to look for</param>
        /// <param name="targetType">The type to get the member from</param>
        /// <param name="bindingFlags">The binding flags</param>
        /// <param name="memberType">The type of the member to look for. Only Field, Property and Method types are supported</param>
        /// <returns>The member info of the specified member type</returns>
        public static MemberInfo FindMember(string memberName, Type targetType, BindingFlags bindingFlags, MemberTypes memberType)
        {
            MemberInfo memberInfo = null;

            while (targetType != null)
            {
                switch (memberType)
                {
                    case MemberTypes.Field:
                        memberInfo = targetType.GetField(memberName, bindingFlags);
                        break;

                    case MemberTypes.Property:
                        memberInfo = targetType.GetProperty(memberName, bindingFlags);
                        break;

                    case MemberTypes.Method:
                        memberInfo = targetType.GetMethod(memberName, bindingFlags);
                        break;
                }

                if (memberInfo != null)
                    return memberInfo;

                targetType = targetType.BaseType;
            }

            return null;
        }

        /// <summary>
        /// Gets the info of a const or static member from the type specified in the path
        /// </summary>
        /// <param name="memberPath">The path on which to locate the member</param>
        /// <param name="memberTypes">The type of the member to look for. Only Field, Property and Method types are supported</param>
        /// <returns>The member info of the specified member type</returns>
#if UNITY_EDITOR
        public static MemberInfo GetStaticMemberInfoFromPath(string memberPath, MemberTypes memberTypes)
        {
            MemberInfo memberInfo = null;

            string[] splitPath = memberPath.Split('.');

            string typeNamespace = GetNamespaceString(splitPath);
            string typeName = splitPath[^2];
            string actualFieldName = splitPath[^1];

            var matchingTypes = TypeCache.GetTypesDerivedFrom<object>().Where((type) => type.Name == typeName && type.Namespace == typeNamespace);

            foreach (var type in matchingTypes)
            {
                memberInfo = FindMember(actualFieldName, type, BINDING_FLAGS ^ BindingFlags.Instance, memberTypes);

                if (memberInfo == null)
                {
                    continue;
                }
                else
                {
                    break;
                }
            }

            return memberInfo;
        }
#endif

        private static string GetNamespaceString(string[] splitMemberPath)
        {
            StringBuilder stringBuilder = new();

            string[] namespacePath = splitMemberPath[..^2];

            for (int i = 0; i < namespacePath.Length; i++)
            {
                stringBuilder.Append(namespacePath[i]);

                if (i != namespacePath.Length - 1)
                    stringBuilder.Append('.');
            }

            return stringBuilder.Length == 0 ? null : stringBuilder.ToString();
        }
        
        private static object GetValue(object source, string name, int index)
        {
            if (GetValue(source, name) is not IEnumerable enumerable)
                return null;

            IEnumerator enumerator = enumerable.GetEnumerator();

            for (int i = 0; i <= index; i++)
            {
                if (!enumerator.MoveNext())
                    return null;
            }

            return enumerator.Current;
        }

        private static object GetValue(object source, string name)
        {
            if (source == null)
                return null;

            Type type = source.GetType();

            while (type != null)
            {
                var field = FindMember(name, type, BINDING_FLAGS, MemberTypes.Field) as FieldInfo;

                if (field != null)
                    return field.GetValue(source);

                type = type.BaseType;
            }

            return null;
        }
        
        /// <summary>
        /// Gets the value of a member
        /// </summary>
        /// <param name="memberInfo">The member to get the value from</param>
        /// <param name="property">The serialized property</param>
        /// <param name="methodParameters">Optional parameter data to pass through if the member is a method</param>
        /// <returns>The value of the member</returns>
#if UNITY_EDITOR
        public static object GetMemberInfoValue(MemberInfo memberInfo, SerializedProperty property, params object[] methodParameters)
        {
            Object targetObject = property.serializedObject.targetObject;

            if (targetObject == null)
                return null;

            try
            {
                if (memberInfo is FieldInfo fieldInfo)
                {
                    return fieldInfo.GetValue(targetObject);
                }
                else if (memberInfo is PropertyInfo propertyInfo)
                {
                    return propertyInfo.GetValue(targetObject);
                }
                else if (memberInfo is MethodInfo methodInfo)
                {
                    return methodInfo.Invoke(targetObject, methodParameters);
                }
            }
            catch (Exception exception)
            {
                if (exception is ArgumentException or TargetException or TargetInvocationException) // If these expections are thrown it means that the member we try to get the value from is inside a different target
                {
                    GetNestedObjectType(property, out object serializedObjectTarget);

                    if (serializedObjectTarget != null)
                    {
                        if (memberInfo is FieldInfo fieldInfo)
                        {
                            return fieldInfo.GetValue(serializedObjectTarget);
                        }
                        else if (memberInfo is PropertyInfo propertyInfo)
                        {
                            return propertyInfo.GetValue(serializedObjectTarget);
                        }
                        else if (memberInfo is MethodInfo methodInfo)
                        {
                            return methodInfo.Invoke(serializedObjectTarget, methodParameters);
                        }
                    }
                }
                else
                {
                    throw;
                }
            }

            return null;
        }
#endif
    }
}
