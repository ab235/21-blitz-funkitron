using System;
using UnityEngine;

// A static class to manage actions and provide extension methods to run actions
public static class ActionsManager
{
    // Run an action if it is not null
    public static void RunAction(this Action action)
    {
        if (action != null)
        {
            // Invoke the action
            action();
        }
    }

    // Run an action with a single argument if it is not null
    public static void RunAction<T>(this Action<T> action, T argument)
    {
        if (action != null)
        {
            // Invoke the action with the argument
            action(argument);
        }
    }

    // Run an action with two arguments if it is not null
    public static void RunAction<T,U>(this Action<T,U> action, T argument1, U argument2)
    {
        if (action != null)
        {
            // Invoke the action with the two arguments
            action(argument1,argument2);
        }
    }

    // Convert a string to an enumeration value
    public static T ToEnum<T>(this string value, T defaultValue, bool ignoreCase = true)
    {
        if (string.IsNullOrEmpty(value))
        {
            // Return the default value if the string is null or empty
            return defaultValue;
        }

        // Parse the string as an enumeration value
        return (T)Enum.Parse(typeof(T), value, ignoreCase);
    }
}
