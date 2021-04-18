using System.IO;
using ArdEditor.AssetUtilities;
using EditorTests.TestUtilities;
using NUnit.Framework;
using UnityEditor;
using UnityEngine;

namespace EditorTests.ArdEditorTests.AssetUtilities
{
    public sealed class EditorAssetUtilitiesTests
    {
        private const string TEST_FOLDER = "TempTests";

        [SetUp]
        public static void SetUp()
        {
            AssetDatabase.CreateFolder(EditorConstants.ASSET_PATH, TEST_FOLDER);
        }

        [TearDown]
        public static void TearDown()
        {
            AssetDatabase.DeleteAsset(Path.Combine(EditorConstants.ASSET_PATH, TEST_FOLDER));
        }

        private static void CreateTestAsset(string name, int version, bool overwrite)
        {
            var testObject = ScriptableObject.CreateInstance<TestScriptable>();
            testObject.Version = version;
            EditorAssetUtilities.CreateAsset(testObject, GetTestAssetPath(name), overwrite, true);
        }

        private static void TestAsset(string name, int version)
        {
            var asset = AssetDatabase.LoadAssetAtPath<TestScriptable>(GetTestAssetPath(name));
            Assert.AreEqual(version, asset.Version);
        }

        private static string GetTestAssetPath(string name, string extension = "asset")
        {
            return Path.Combine(EditorConstants.ASSET_PATH, TEST_FOLDER, $"{name}.{extension}");
        }

        [Test]
        public void CreateAsset_NoAssetExists_CreateANewAsset()
        {
            CreateTestAsset("Test", 1, false);
            TestAsset("Test", 1);
        }

        [Test]
        public void CreateAsset_AssetExistsButOverwrite_ReplaceExistingAsset()
        {
            CreateTestAsset("Test", 1, false);
            TestAsset("Test", 1);
            CreateTestAsset("Test", 2, true);
            TestAsset("Test", 2);
        }

        [Test]
        public void CreateAsset_AssetExists_DoNothing()
        {
            CreateTestAsset("Test", 1, false);
            TestAsset("Test", 1);
            CreateTestAsset("Test", 2, false);
            TestAsset("Test", 1);
        }

        [Test]
        public void CreateAssetAtSelection_AssetSelected_CreatesAssetNextToSelectedOne()
        {
            CreateTestAsset("Test", 1, false);
            Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(GetTestAssetPath("Test"));
            var testAsset = ScriptableObject.CreateInstance<TestScriptable>();
            testAsset.Version = 1;
            EditorAssetUtilities.CreateAssetAtSelection(testAsset, "Test2.asset", false);
            TestAsset("Test2", 1);
        }

        [Test]
        public void CreateAssetAtSelection_NothingSelected_CreatesInAssetsFolder()
        {
            Selection.activeObject = null;
            var testAsset = ScriptableObject.CreateInstance<TestScriptable>();
            testAsset.Version = 1;
            EditorAssetUtilities.CreateAssetAtSelection(testAsset, "Test2.asset", false);
            Assert.IsNotNull(AssetDatabase.LoadAssetAtPath<TestScriptable>("Assets/Test2.asset"));
            AssetDatabase.DeleteAsset("Assets/Test2.asset");
        }

        [Test]
        public void EnsurePathExist_NoDirectoryExists_CreatesANewOne()
        {
            TearDown();
            string testFolderPath = Path.Combine(EditorConstants.ASSET_PATH, TEST_FOLDER);
            Assert.IsFalse(Directory.Exists(testFolderPath));
            EditorAssetUtilities.EnsurePathExists(GetTestAssetPath("Test"));
            Assert.IsTrue(Directory.Exists(testFolderPath));
        }

        [Test]
        public void CreateFile_NoFileExists_CreatesANewOne()
        {
            const string testContent = "test content";
            Assert.IsFalse(File.Exists(GetTestAssetPath("Test", "txt")));
            EditorAssetUtilities.CreateFile(testContent, GetTestAssetPath("Test", "txt"), false);
            Assert.AreEqual(testContent, File.ReadAllText(GetTestAssetPath("Test", "txt")));
        }

        [Test]
        public void GetPathFromAssetName_ExistingScriptName_ReturnsNonEmptyPath()
        {
            string path = EditorAssetUtilities.GetPathFromAssetName("TestScriptable");
            Assert.AreNotEqual(string.Empty, path);
        }

        [Test]
        public void FindAssetsOfType_AssetOfTypeExists_ReturnsIt()
        {
            CreateTestAsset("Test", 1, false);
            TestScriptable[] testScriptables = EditorAssetUtilities.FindAssetsOfType<TestScriptable>();
            Assert.GreaterOrEqual(1, testScriptables.Length);
        }
    }
}