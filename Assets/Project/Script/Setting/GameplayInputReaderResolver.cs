using UnityEngine;

public static class GameplayInputReaderResolver
{
    public static IGameplayInputReader Resolve(
        MonoBehaviour preferredBehaviour,
        Component searchRoot,
        out MonoBehaviour resolvedBehaviour)
    {
        if (preferredBehaviour is IGameplayInputReader preferredInputReader)
        {
            resolvedBehaviour = preferredBehaviour;
            return preferredInputReader;
        }

        resolvedBehaviour = null;

        if (searchRoot == null)
        {
            return null;
        }

        MonoBehaviour[] behaviours = searchRoot.GetComponents<MonoBehaviour>();

        for (int i = 0; i < behaviours.Length; i++)
        {
            if (behaviours[i] is IGameplayInputReader discoveredInputReader)
            {
                resolvedBehaviour = behaviours[i];
                return discoveredInputReader;
            }
        }

        return null;
    }
}
