using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Medallion.CommandLine.Tests
{
    public class NamespaceTest
    {
        [Test]
        public void TestNamespaces()
        {
            var usedNamespaces = typeof(CommandLineGrammar).Assembly.GetTypes()
                .Select(t => t.Namespace)
                .Where(n => n != null && !n.StartsWith("Microsoft.") && !n.StartsWith("System."))
                .Distinct();
            CollectionAssert.AreEquivalent(new[] { "Medallion.CommandLine" }, usedNamespaces);
        }
    }
}
