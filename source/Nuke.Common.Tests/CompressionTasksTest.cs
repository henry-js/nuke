// Copyright 2021 Maintainers of NUKE.
// Distributed under the MIT License.
// https://github.com/nuke-build/nuke/blob/master/LICENSE

using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using Nuke.Common.IO;
using Xunit;
using Xunit.Abstractions;

namespace Nuke.Common.Tests
{
    public class CompressionTasksTest : FileSystemDependentTest
    {
        private AbsolutePath RootFile => TestTempDirectory / "root-file";
        private AbsolutePath NestedFile => TestTempDirectory / "a" / "b" / "c" / "nested-file";

        public CompressionTasksTest(ITestOutputHelper testOutputHelper)
            : base(testOutputHelper)
        {
        }

        [Theory]
        [InlineData("archive.zip")]
        [InlineData("archive.tar.gz")]
        [InlineData("archive.tar.bz2")]
        public void Test(string archiveFile)
        {
            RootFile.WriteAllText("root");
            NestedFile.WriteAllText("nested");

            var archive = Path.Combine(TestTempDirectory, archiveFile);
            CompressionTasks.Compress(TestTempDirectory, archive);
            File.Exists(archive).Should().BeTrue();

            File.Delete(RootFile);
            File.Delete(NestedFile);
            Directory.GetFiles(TestTempDirectory, "*").Should().HaveCount(1);

            CompressionTasks.Uncompress(archive, TestTempDirectory);
            File.Exists(RootFile).Should().BeTrue();
            File.ReadAllText(RootFile).Should().Be("root");
            File.Exists(NestedFile).Should().BeTrue();
            File.ReadAllText(NestedFile).Should().Be("nested");
        }
    }
}
