using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class TestSuite
{
    [UnityTest]
    public System.Collections.IEnumerator TestsCanRun()
    {
        SceneManager.LoadScene("Levels/Testing");
        var scene = SceneManager.GetActiveScene();
        Object[] objects = Object.FindObjectsOfType<Object>();

        var rootObjects = new List<GameObject>();
        scene.GetRootGameObjects(rootObjects);

        foreach (Object object_ in rootObjects)
        {
            Debug.Log(object_);
        }
        Assert.Greater(objects.Length, 1000);

        yield return null;
    }
}
