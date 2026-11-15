using Domain.Models;
using Domain.Ports.Driving;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEngine.UI;

namespace PlayMode.Tests
{
    // Starts a run in the real scene with its real wiring. Any error logged along the way
    // fails the test, which is how a broken reference in GameBootstrap shows up.
    // The run saves through PlayerPrefs, so the saved stats are restored afterwards.
    [TestFixture]
    public class GameSmokeTests
    {
        private const string SceneName = "SampleScene";
        private const float FirstAttackTimeout = 10f;
        private readonly Dictionary<string, int?> _savedPrefs = new Dictionary<string, int?>();

        [SetUp]
        public void SavePlayerPrefs()
        {
            _savedPrefs.Clear();
            foreach (StorageKeys key in Enum.GetValues(typeof(StorageKeys)))
            {
                string name = key.ToString();
                _savedPrefs[name] = PlayerPrefs.HasKey(name) ? PlayerPrefs.GetInt(name) : (int?)null;
            }
        }

        [UnityTearDown]
        public IEnumerator UnloadSceneAndRestorePlayerPrefs()
        {
            Scene scene = SceneManager.GetSceneByName(SceneName);
            if (scene.isLoaded)
            {
                SceneManager.SetActiveScene(SceneManager.CreateScene("Empty test scene"));
                yield return SceneManager.UnloadSceneAsync(scene);
            }

            foreach (var pair in _savedPrefs)
            {
                if (pair.Value.HasValue) PlayerPrefs.SetInt(pair.Key, pair.Value.Value);
                else PlayerPrefs.DeleteKey(pair.Key);
            }
            PlayerPrefs.Save();
        }

        private static T PrivateField<T>(object target, string field)
        {
            var info = target.GetType().GetField(field, BindingFlags.NonPublic | BindingFlags.Instance);
            Assert.IsNotNull(info, $"{target.GetType().Name} has no field {field}");
            return (T)info.GetValue(target);
        }

        [UnityTest]
        public IEnumerator StartingARun_SpawnsAttacks_WithoutErrors()
        {
            yield return SceneManager.LoadSceneAsync(SceneName);
            yield return null;

            var boot = UnityEngine.Object.FindFirstObjectByType<GameBootstrap>();
            Assert.IsNotNull(boot, "the scene has no GameBootstrap");
            var attacks = PrivateField<IAttackService>(boot, "_attackService");
            int spawned = 0;
            attacks.OnAttackAdded += attack => spawned++;

            var menu = UnityEngine.Object.FindFirstObjectByType<MainMenuUI>();
            Assert.IsNotNull(menu, "the scene has no main menu");
            PrivateField<Button>(menu, "_startButton").onClick.Invoke();

            float timeout = Time.realtimeSinceStartup + FirstAttackTimeout;
            while (spawned == 0 && Time.realtimeSinceStartup < timeout) yield return null;

            Assert.Greater(spawned, 0, $"no attack spawned within {FirstAttackTimeout} seconds of starting a run");
        }
    }
}
