using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EditorExtensionsRedux.StripSymmetry
{
    internal abstract class Enums<U> where U : class
    {
        /// <summary>
        /// Converts the string representation of the name or numeric value of one or more enumerated constants to an equivalent enumerated object.
        /// </summary>
        /// <typeparam name="TEnumType">The type of the enumeration.</typeparam>
        /// <param name="value">A string containing the name or value to convert.</param>
        /// <exception cref="System.ArgumentNullException">value is null</exception>
        /// <exception cref="System.ArgumentException">value is either an empty string or only contains white space.-or- value is a name, but not one of the named constants defined for the enumeration.</exception>
        /// <exception cref="System.OverflowException">value is outside the range of the underlying type of EnumType</exception>
        /// <returns>An object of type enumType whose value is represented by value.</returns>
        static public TEnumType Parse<TEnumType>(string value) where TEnumType : U
        {
            return (TEnumType)Enum.Parse(typeof(TEnumType), value);
        }
    }

    abstract class Enums : Enums<Enum>
    {
    }
}
