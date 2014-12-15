using UnityEngine;
using System.Collections;

public class UIHelper {

	/// <summary>
	/// Disable the widget
	/// </summary>
    public static void DisableWidget(GameObject button)
    {
        DisableWidget(button, 0.0f);
    }

	public static void DisableWidget(GameObject button, float alpha)
	{
		UIWidget[] children = button.GetComponentsInChildren<UIWidget>();
		foreach (var child in children)
		{
			child.alpha = alpha;
		}

		Collider[] colliders = button.GetComponentsInChildren<Collider>();
		foreach (var collider in colliders)
		{
			collider.enabled = false;
		}
	}

	/// <summary>
	/// Disable the widget
	/// </summary>
    public static void EnableWidget(GameObject button)
    {
        EnableWidget(button, 1.0f);
    }
    
    public static void EnableWidget(GameObject button, float alpha)
	{
		UIWidget[] children = button.GetComponentsInChildren<UIWidget>();
		foreach (var child in children)
		{
			child.alpha = alpha;
		}

		Collider[] colliders = button.GetComponentsInChildren<Collider>();
		foreach (var collider in colliders)
		{
			collider.enabled = true;
		}
	}

	/// <summary>
	/// Determines if is widget enabled the specified button.
	/// </summary>
	/// <returns><c>true</c> if is widget enabled the specified button; otherwise, <c>false</c>.</returns>
	/// <param name="button">Button.</param>
	public static bool IsWidgetEnabled(GameObject button)
	{
		return button.collider.enabled;
	}

    /// <summary>
    /// Disable colliders
    /// </summary>
    public static void DisableCollider(GameObject button)
    {
        Collider[] colliders = button.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = false;
        }
    }

    /// <summary>
    /// Enable colliders
    /// </summary>
    public static void EnableCollider(GameObject button)
    {
        Collider[] colliders = button.GetComponentsInChildren<Collider>();
        foreach (var collider in colliders)
        {
            collider.enabled = true;
        }
    }
}