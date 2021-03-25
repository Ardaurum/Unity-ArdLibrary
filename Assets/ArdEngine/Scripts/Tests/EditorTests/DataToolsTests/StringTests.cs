using System.Collections.Generic;
using System.IO;
using ArdEditor.AssetUtilities;
using ArdEngine.DataTools;
using NUnit.Framework;
using UnityEngine;

namespace EditorTests.DataToolsTests
{
    public sealed class HashTests
    {
        [Test]
        public void GetStableHash_ForAListOfEnglishWords_HasLessThan10Collisions()
        {
            var hashes = new HashSet<int>();
            var numCollisions = 0;
            string testFilePath = EditorAssetUtilities.GetPathFromAssetName("EnglishWordsForHashTest");
            using (var reader = new StreamReader(testFilePath))
            {
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    if (hashes.Add(line.ToLower().GetStableHash()) == false)
                    {
                        numCollisions++;
                    }
                }
            }
            
            Debug.Log($"Number of collisions: {numCollisions}");
            Assert.LessOrEqual(numCollisions, 10);
        }
    }
}