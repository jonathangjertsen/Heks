using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.SceneManagement;

public class TestSuite
{
    [UnityTest]
    public IEnumerator TestsCanRun()
    {
        SceneManager.LoadScene("Levels/Testing");
        yield return null;
    }
}
