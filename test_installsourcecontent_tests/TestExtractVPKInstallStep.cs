using Pipelines;
using System.Diagnostics.CodeAnalysis;
using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Text.RegularExpressions;
using test_installsourcecontent;

namespace test_installsourcecontent_tests
{
    public class TestExtractVPKInstallStepEventHandler : IExtractVPKInstallStepEventHandler
    {
        public int NoVPKsSpecifiedTotal { get; private set; } = 0;
        public int NoOutDirSpecifiedTotal { get; private set; } = 0;
        public int BlankVPKEntryTotal { get; private set; } = 0;
        public int FailedToBuildGlobalRegexListTotal { get; private set; } = 0;
        public int FailedToBuildVPKRegexListTotal { get; private set; } = 0;
        public int VPKFileDoesNotExistTotal { get; private set; } = 0;
        public int VPKExtractionCompleteTotal { get; private set; } = 0;
        public int VPKExtractionCompleteWithErrorsTotal { get; private set; } = 0;
        public int VPKExtractionFailedTotal { get; private set; } = 0;
        public int NoVPKExtractedTotal { get; private set; } = 0;

        public void NoVPKsSpecified() => ++NoVPKsSpecifiedTotal;
        public void NoOutDirSpecified() => ++NoOutDirSpecifiedTotal;
        public void BlankVPKEntry() => ++BlankVPKEntryTotal;
        public void FailedToBuildGlobalRegexList() => ++FailedToBuildGlobalRegexListTotal;
        public void FailedToBuildVPKRegexList() => ++FailedToBuildVPKRegexListTotal;
        public void VPKFileDoesNotExist() => ++VPKFileDoesNotExistTotal;
        public void VPKExtractionComplete() => ++VPKExtractionCompleteTotal;
        public void VPKExtractionCompleteWithErrors() => ++VPKExtractionCompleteWithErrorsTotal;
        public void VPKExtractionFailed() => ++VPKExtractionFailedTotal;
        public void NoVPKExtracted() => ++NoVPKExtractedTotal;
    }

    public class BadRegexConverter : IStringToRegexConverter
    {
        public Regex StringToRegex(string input)
        {
            throw new Exception();
        }
    }

    public class AlwaysCompleteVPKExtractor : IVPKExtractor
    {
        public VPKExtractionResult Extract(IFileSystem fileSystem, IWriter writer, string vpkPath, string outputDir, IVPKFileFilter fileFilter)
        {
            return VPKExtractionResult.Complete;
        }
    }

    public class AlwaysCompleteWithErrorsVPKExtractor : IVPKExtractor
    {
        public VPKExtractionResult Extract(IFileSystem fileSystem, IWriter writer, string vpkPath, string outputDir, IVPKFileFilter fileFilter)
        {
            return VPKExtractionResult.CompleteWithErrors;
        }
    }

    public class AlwaysFailedVPKExtractor : IVPKExtractor
    {
        public VPKExtractionResult Extract(IFileSystem fileSystem, IWriter writer, string vpkPath, string outputDir, IVPKFileFilter fileFilter)
        {
            return VPKExtractionResult.Failed;
        }
    }

    [TestClass]
    public class TestExtractVPKInstallStep
    {
        static IVPKExtractor Extractor = new VPKExtractor();
        static IVPKExtractor AlwaysCompleteVPKExtractor = new AlwaysCompleteVPKExtractor();
        static IVPKExtractor AlwaysCompleteWithErrorsVPKExtractor = new AlwaysCompleteWithErrorsVPKExtractor();
        static IVPKExtractor AlwaysFailedVPKExtractor = new AlwaysFailedVPKExtractor();

        static IWriter NullWriter = new NullWriter();
        static IStringToRegexConverter DefaultStringToRegexConverter = new StringToRegexConverter();
        static IStringToRegexConverter BadRegexConverter = new BadRegexConverter();
        static IConfiguration NullConfiguration = new NullConfiguration();
        static IVPKFileResolver VPKFileResolver = new VPKFileResolver();

        [TestMethod]
        public void EmptyVPKListReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestExtractVPKInstallStepEventHandler();
            var step = new ExtractVPKInstallStep(Extractor, DefaultStringToRegexConverter, VPKFileResolver, eventHandler);
            var stepData = new ExtractVPKInstallStepData();

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(1, eventHandler.NoVPKsSpecifiedTotal);
        }

        [TestMethod]
        public void BlankVPKEntryReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestExtractVPKInstallStepEventHandler();
            var step = new ExtractVPKInstallStep(Extractor, DefaultStringToRegexConverter, VPKFileResolver, eventHandler);
            var stepData = new ExtractVPKInstallStepData()
            {
                Vpks = [
                    new ExtractVPKInstallStepDataVPK { VPKFile = string.Empty }
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(1, eventHandler.BlankVPKEntryTotal);
        }

        [TestMethod]
        public void EmptyOutDirReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestExtractVPKInstallStepEventHandler();
            var step = new ExtractVPKInstallStep(Extractor, DefaultStringToRegexConverter, VPKFileResolver, eventHandler);
            var stepData = new ExtractVPKInstallStepData()
            {
                Vpks = [
                    new ExtractVPKInstallStepDataVPK { VPKFile = "test" }
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(1, eventHandler.NoOutDirSpecifiedTotal);
        }

        [TestMethod]
        public void BadRegexInGlobalFilesToExcludeReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestExtractVPKInstallStepEventHandler();
            var step = new ExtractVPKInstallStep(Extractor, BadRegexConverter, VPKFileResolver, eventHandler);
            var stepData = new ExtractVPKInstallStepData()
            {
                OutDir = "test",
                Vpks = [
                    new ExtractVPKInstallStepDataVPK
                    {
                        VPKFile = "test"
                    }
                ],
                FilesToExclude = ["test"]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(1, eventHandler.FailedToBuildGlobalRegexListTotal);
        }

        [TestMethod]
        public void BadRegexInGlobalFilesToExtractReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestExtractVPKInstallStepEventHandler();
            var step = new ExtractVPKInstallStep(Extractor, BadRegexConverter, VPKFileResolver, eventHandler);
            var stepData = new ExtractVPKInstallStepData()
            {
                OutDir = "test",
                Vpks = [
                    new ExtractVPKInstallStepDataVPK
                    {
                        VPKFile = "test"
                    }
                ],
                FilesToExtract = ["test"]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(1, eventHandler.FailedToBuildGlobalRegexListTotal);
        }

        [TestMethod]
        public void BadRegexInGlobalFilesToExcludeOrFilesToExtractReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });
            var eventHandler = new TestExtractVPKInstallStepEventHandler();
            var step = new ExtractVPKInstallStep(Extractor, BadRegexConverter, VPKFileResolver, eventHandler);
            var stepData = new ExtractVPKInstallStepData()
            {
                OutDir = "test",
                Vpks = [
                    new ExtractVPKInstallStepDataVPK
                    {
                        VPKFile = "test"
                    }
                ],
                FilesToExclude = ["test"],
                FilesToExtract = ["test"]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(1, eventHandler.FailedToBuildGlobalRegexListTotal);
        }




        [TestMethod]
        public void BadRegexInVPKFilesToExcludeReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/vpks/simple_vpk.vpk", new MockFileData("") }
            });
            var eventHandler = new TestExtractVPKInstallStepEventHandler();
            var step = new ExtractVPKInstallStep(Extractor, BadRegexConverter, VPKFileResolver, eventHandler);
            var stepData = new ExtractVPKInstallStepData()
            {
                OutDir = "test",
                Vpks = [
                    new ExtractVPKInstallStepDataVPK
                    {
                        VPKFile = "C:/vpks/simple_vpk.vpk",
                        FilesToExclude = ["test"]
                    }
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(1, eventHandler.FailedToBuildVPKRegexListTotal);
        }

        [TestMethod]
        public void BadRegexInVPKFilesToExtractReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/vpks/simple_vpk.vpk", new MockFileData("") }
            });
            var eventHandler = new TestExtractVPKInstallStepEventHandler();
            var step = new ExtractVPKInstallStep(Extractor, BadRegexConverter, VPKFileResolver, eventHandler);
            var stepData = new ExtractVPKInstallStepData()
            {
                OutDir = "test",
                Vpks = [
                    new ExtractVPKInstallStepDataVPK
                    {
                        VPKFile = "C:/vpks/simple_vpk.vpk",
                        FilesToExtract = ["test"]
                    }
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(1, eventHandler.FailedToBuildVPKRegexListTotal);
        }

        [TestMethod]
        public void BadRegexInVPKFilesToExcludeOrFilesToExtractReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> {
                { "C:/vpks/simple_vpk.vpk", new MockFileData("") }
            });
            var eventHandler = new TestExtractVPKInstallStepEventHandler();
            var step = new ExtractVPKInstallStep(Extractor, BadRegexConverter, VPKFileResolver, eventHandler);
            var stepData = new ExtractVPKInstallStepData()
            {
                OutDir = "test",
                Vpks = [
                    new ExtractVPKInstallStepDataVPK
                    {
                        VPKFile = "C:/vpks/simple_vpk.vpk",
                        FilesToExclude = ["test"],
                        FilesToExtract = ["test"]
                    }
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(1, eventHandler.FailedToBuildVPKRegexListTotal);
        }





        class ExtractVPKInstallStepDataVPKEqualityComparer : IEqualityComparer<ExtractVPKInstallStepDataVPK>
        {
            public bool Equals(ExtractVPKInstallStepDataVPK? x, ExtractVPKInstallStepDataVPK? y)
            {
                if (null == x || null == y)
                    throw new NullReferenceException();
                if (x.VPKFile != y.VPKFile)
                    return false;
                if (!x.FilesToExclude.SequenceEqual(y.FilesToExclude))
                    return false;
                if (!x.FilesToExtract.SequenceEqual(y.FilesToExtract))
                    return false;

                return true;
            }

            public int GetHashCode([DisallowNull] ExtractVPKInstallStepDataVPK obj)
            {
                return obj.GetHashCode();
            }
        }

        [TestMethod]
        public void ResolveWildcardsToFiles_WhenWildcardsArePresent()
        {
            string[] languages = ["english", "french"];

            var pakVPKS = languages.Select(lang => ($"C:/vpks/hl2_pak_{lang}_dir.vpk")).ToList();
            var soundVPKS = languages.Select(lang => ($"C:/vpks/hl2_sound_vo_{lang}_dir.vpk")).ToList();

            var fileSystemData = new Dictionary<string, MockFileData>
            {
                { "C:/vpks/hl2_sound_misc_dir.vpk", new MockFileData("") },
                { "C:/vpks/hl2_pak_dir.vpk", new MockFileData("") },
                { "C:/vpks/hl2_textures_dir.vpk", new MockFileData("") }
            };
            pakVPKS.ForEach(vpkFile => fileSystemData.Add(vpkFile, new MockFileData("")));
            soundVPKS.ForEach(vpkFile => fileSystemData.Add(vpkFile, new MockFileData("")));

            var fileSystem = new MockFileSystem(fileSystemData);
            var vpks = new List<ExtractVPKInstallStepDataVPK> {
                new ExtractVPKInstallStepDataVPK {
                    VPKFile = "C:/vpks/hl2_pak_dir.vpk",
                    FilesToExclude = ["test1"],
                    FilesToExtract = ["test2"]
                },
                new ExtractVPKInstallStepDataVPK {
                    VPKFile = "C:/vpks/hl2_pak_*_dir.vpk", // Wildcard
                    FilesToExclude = ["test3"],
                    FilesToExtract = ["test4"]
                },
                new ExtractVPKInstallStepDataVPK {
                    VPKFile = "C:/vpks/hl2_sound_misc_dir.vpk",
                    FilesToExclude = ["test5"],
                    FilesToExtract = ["test6"]
                },
                new ExtractVPKInstallStepDataVPK {
                    VPKFile = "C:/vpks/hl2_sound_vo_*_dir.vpk",  // Wildcard
                    FilesToExclude = ["test7"],
                    FilesToExtract = ["test8"]
                },
                new ExtractVPKInstallStepDataVPK {
                    VPKFile = "C:/vpks/hl2_textures_dir.vpk",
                    FilesToExclude = ["test9"],
                    FilesToExtract = ["test10"]
                },
            };

            var expected = new ExtractVPKInstallStepDataVPK[] {
                new ExtractVPKInstallStepDataVPK {
                    VPKFile = "C:/vpks/hl2_pak_dir.vpk",
                    FilesToExclude = ["test1"],
                    FilesToExtract = ["test2"]
                },
                new ExtractVPKInstallStepDataVPK {
                    VPKFile = "C:/vpks/hl2_pak_english_dir.vpk", // Replaced
                    FilesToExclude = ["test3"],
                    FilesToExtract = ["test4"]
                },
                new ExtractVPKInstallStepDataVPK {
                    VPKFile = "C:/vpks/hl2_pak_french_dir.vpk", // Replaced
                    FilesToExclude = ["test3"],
                    FilesToExtract = ["test4"]
                },
                new ExtractVPKInstallStepDataVPK {
                    VPKFile = "C:/vpks/hl2_sound_misc_dir.vpk",
                    FilesToExclude = ["test5"],
                    FilesToExtract = ["test6"]
                },
                new ExtractVPKInstallStepDataVPK {
                    VPKFile = "C:/vpks/hl2_sound_vo_english_dir.vpk", // Replaced
                    FilesToExclude = ["test7"],
                    FilesToExtract = ["test8"]
                },
                new ExtractVPKInstallStepDataVPK {
                    VPKFile = "C:/vpks/hl2_sound_vo_french_dir.vpk", // Replaced
                    FilesToExclude = ["test7"],
                    FilesToExtract = ["test8"]
                },
                new ExtractVPKInstallStepDataVPK {
                    VPKFile = "C:/vpks/hl2_textures_dir.vpk",
                    FilesToExclude = ["test9"],
                    FilesToExtract = ["test10"]
                },
            };

            var fileResolver = new VPKFileResolver();
            fileResolver.ResolveFilePaths(fileSystem, vpks);

            Assert.AreEqual(expected.Length, vpks.Count());
            for (int i = 0; i < expected.Length; ++i)
                Assert.AreEqual(expected[i], vpks[i], new ExtractVPKInstallStepDataVPKEqualityComparer());
        }

        [TestMethod]
        public void ResolveWildcardsToFiles_WhenNoWildcardsArePresent()
        {
            var fileSystemData = new Dictionary<string, MockFileData>
            {
                { "C:/vpks/hl2_sound_misc_dir.vpk", new MockFileData("") },
                { "C:/vpks/hl2_pak_dir.vpk", new MockFileData("") },
                { "C:/vpks/hl2_textures_dir.vpk", new MockFileData("") }
            };

            var fileSystem = new MockFileSystem(fileSystemData);
            var vpks = new List<ExtractVPKInstallStepDataVPK> {
                new ExtractVPKInstallStepDataVPK {
                    VPKFile = "C:/vpks/hl2_pak_dir.vpk",
                    FilesToExclude = ["test1"],
                    FilesToExtract = ["test2"]
                },
                new ExtractVPKInstallStepDataVPK {
                    VPKFile = "C:/vpks/hl2_sound_misc_dir.vpk",
                    FilesToExclude = ["test3"],
                    FilesToExtract = ["test4"]
                },
                new ExtractVPKInstallStepDataVPK {
                    VPKFile = "C:/vpks/hl2_textures_dir.vpk",
                    FilesToExclude = ["test5"],
                    FilesToExtract = ["test6"]
                },
            };

            var expected = new ExtractVPKInstallStepDataVPK[] {
                new ExtractVPKInstallStepDataVPK {
                    VPKFile = "C:/vpks/hl2_pak_dir.vpk",
                    FilesToExclude = ["test1"],
                    FilesToExtract = ["test2"]
                },
                new ExtractVPKInstallStepDataVPK {
                    VPKFile = "C:/vpks/hl2_sound_misc_dir.vpk",
                    FilesToExclude = ["test3"],
                    FilesToExtract = ["test4"]
                },
                new ExtractVPKInstallStepDataVPK {
                    VPKFile = "C:/vpks/hl2_textures_dir.vpk",
                    FilesToExclude = ["test5"],
                    FilesToExtract = ["test6"]
                },
            };

            var fileResolver = new VPKFileResolver();
            fileResolver.ResolveFilePaths(fileSystem, vpks);

            Assert.AreEqual(expected.Length, vpks.Count());
            for (int i = 0; i < expected.Length; ++i)
                Assert.AreEqual(expected[i], vpks[i], new ExtractVPKInstallStepDataVPKEqualityComparer());
        }

        [TestMethod]
        public void AllVPKsNotFoundReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData> { });

            var eventHandler = new TestExtractVPKInstallStepEventHandler();
            var step = new ExtractVPKInstallStep(AlwaysCompleteVPKExtractor, DefaultStringToRegexConverter, VPKFileResolver, eventHandler);
            var stepData = new ExtractVPKInstallStepData()
            {
                OutDir = "test",
                Vpks = [
                    new ExtractVPKInstallStepDataVPK { VPKFile = "test1" },
                    new ExtractVPKInstallStepDataVPK { VPKFile = "test2" },
                    new ExtractVPKInstallStepDataVPK { VPKFile = "test3" },
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(3, eventHandler.VPKFileDoesNotExistTotal);
            Assert.AreEqual(0, eventHandler.VPKExtractionCompleteTotal);
            Assert.AreEqual(1, eventHandler.NoVPKExtractedTotal);
        }

        [TestMethod]
        public void AtLeastOneVPKFoundAndExtractedReturnsPartiallyCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/vpks/test1.vpk", new MockFileData("") }
            });

            var eventHandler = new TestExtractVPKInstallStepEventHandler();
            var step = new ExtractVPKInstallStep(AlwaysCompleteVPKExtractor, DefaultStringToRegexConverter, VPKFileResolver, eventHandler);
            var stepData = new ExtractVPKInstallStepData()
            {
                OutDir = "test",
                Vpks = [
                    new ExtractVPKInstallStepDataVPK { VPKFile = "C:/vpks/test1.vpk" },
                    new ExtractVPKInstallStepDataVPK { VPKFile = "test2" },
                    new ExtractVPKInstallStepDataVPK { VPKFile = "test3" },
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.PartiallyComplete, result);
            Assert.AreEqual(2, eventHandler.VPKFileDoesNotExistTotal);
            Assert.AreEqual(1, eventHandler.VPKExtractionCompleteTotal);
            Assert.AreEqual(0, eventHandler.NoVPKExtractedTotal);
        }

        [TestMethod]
        public void AllVPKsFoundAndExtractedReturnsComplete()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/vpks/test1.vpk", new MockFileData("") },
                { "C:/vpks/test2.vpk", new MockFileData("") },
                { "C:/vpks/test3.vpk", new MockFileData("") }
            });

            var eventHandler = new TestExtractVPKInstallStepEventHandler();
            var step = new ExtractVPKInstallStep(AlwaysCompleteVPKExtractor, DefaultStringToRegexConverter, VPKFileResolver, eventHandler);
            var stepData = new ExtractVPKInstallStepData()
            {
                OutDir = "test",
                Vpks = [
                    new ExtractVPKInstallStepDataVPK { VPKFile = "C:/vpks/test1.vpk" },
                    new ExtractVPKInstallStepDataVPK { VPKFile = "C:/vpks/test2.vpk" },
                    new ExtractVPKInstallStepDataVPK { VPKFile = "C:/vpks/test3.vpk" }
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(0, eventHandler.VPKFileDoesNotExistTotal);
            Assert.AreEqual(3, eventHandler.VPKExtractionCompleteTotal);
            Assert.AreEqual(0, eventHandler.NoVPKExtractedTotal);
        }

        [TestMethod]
        public void OnlyOneVPK_ExtractionCompleteReturnsCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/vpks/test1.vpk", new MockFileData("") },
            });

            var eventHandler = new TestExtractVPKInstallStepEventHandler();
            var step = new ExtractVPKInstallStep(AlwaysCompleteVPKExtractor, DefaultStringToRegexConverter, VPKFileResolver, eventHandler);
            var stepData = new ExtractVPKInstallStepData()
            {
                OutDir = "test",
                Vpks = [
                    new ExtractVPKInstallStepDataVPK { VPKFile = "C:/vpks/test1.vpk" },
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Complete, result);
            Assert.AreEqual(1, eventHandler.VPKExtractionCompleteTotal);
            Assert.AreEqual(0, eventHandler.VPKExtractionCompleteWithErrorsTotal);
            Assert.AreEqual(0, eventHandler.VPKExtractionFailedTotal);
            Assert.AreEqual(0, eventHandler.NoVPKExtractedTotal);
        }

        [TestMethod]
        public void OnlyOneVPK_ExtractionCompleteWithErrorsReturnsPartiallyCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/vpks/test1.vpk", new MockFileData("") },
            });

            var eventHandler = new TestExtractVPKInstallStepEventHandler();
            var step = new ExtractVPKInstallStep(AlwaysCompleteWithErrorsVPKExtractor, DefaultStringToRegexConverter, VPKFileResolver, eventHandler);
            var stepData = new ExtractVPKInstallStepData()
            {
                OutDir = "test",
                Vpks = [
                    new ExtractVPKInstallStepDataVPK { VPKFile = "C:/vpks/test1.vpk" },
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.PartiallyComplete, result);
            Assert.AreEqual(0, eventHandler.VPKExtractionCompleteTotal);
            Assert.AreEqual(1, eventHandler.VPKExtractionCompleteWithErrorsTotal);
            Assert.AreEqual(0, eventHandler.VPKExtractionFailedTotal);
            Assert.AreEqual(0, eventHandler.NoVPKExtractedTotal);
        }

        [TestMethod]
        public void OnlyOneVPK_ExtractionFailedReturnsFailed()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/vpks/test1.vpk", new MockFileData("") },
            });

            var eventHandler = new TestExtractVPKInstallStepEventHandler();
            var step = new ExtractVPKInstallStep(AlwaysFailedVPKExtractor, DefaultStringToRegexConverter, VPKFileResolver, eventHandler);
            var stepData = new ExtractVPKInstallStepData()
            {
                OutDir = "test",
                Vpks = [
                    new ExtractVPKInstallStepDataVPK { VPKFile = "C:/vpks/test1.vpk" },
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.Failed, result);
            Assert.AreEqual(0, eventHandler.VPKExtractionCompleteTotal);
            Assert.AreEqual(0, eventHandler.VPKExtractionCompleteWithErrorsTotal);
            Assert.AreEqual(1, eventHandler.VPKExtractionFailedTotal);
            Assert.AreEqual(1, eventHandler.NoVPKExtractedTotal);
        }

        public class MockVPKExtractor : IVPKExtractor
        {
            public List<string> CompleteOnVpks { get; set; } = [];
            public List<string> CompleteWithErrorsOnVpks { get; set; } = [];
            public List<string> FailOnVpks { get; set; } = [];

            public VPKExtractionResult Extract(IFileSystem fileSystem, IWriter writer, string vpkPath, string outputDir, IVPKFileFilter fileFilter)
            {
                if (CompleteOnVpks.Contains(vpkPath))
                    return VPKExtractionResult.Complete;
                if (CompleteWithErrorsOnVpks.Contains(vpkPath))
                    return VPKExtractionResult.CompleteWithErrors;
                if (FailOnVpks.Contains(vpkPath))
                    return VPKExtractionResult.Failed;

                throw new Exception($"{vpkPath} does not appear in any list.");
            }
        }

        [TestMethod]
        public void TwoVPKs_Extraction_Complete_CompleteWithErrors_ReturnsPartiallyCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/vpks/test1.vpk", new MockFileData("") },
                { "C:/vpks/test2.vpk", new MockFileData("") },
            });

            var eventHandler = new TestExtractVPKInstallStepEventHandler();
            var step = new ExtractVPKInstallStep(new MockVPKExtractor()
            {
                CompleteOnVpks = ["C:/vpks/test1.vpk"],
                CompleteWithErrorsOnVpks = ["C:/vpks/test2.vpk"]
            }, DefaultStringToRegexConverter, VPKFileResolver, eventHandler);
            var stepData = new ExtractVPKInstallStepData()
            {
                OutDir = "test",
                Vpks = [
                    new ExtractVPKInstallStepDataVPK { VPKFile = "C:/vpks/test1.vpk" },
                    new ExtractVPKInstallStepDataVPK { VPKFile = "C:/vpks/test2.vpk" },
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.PartiallyComplete, result);
            Assert.AreEqual(1, eventHandler.VPKExtractionCompleteTotal);
            Assert.AreEqual(1, eventHandler.VPKExtractionCompleteWithErrorsTotal);
            Assert.AreEqual(0, eventHandler.VPKExtractionFailedTotal);
            Assert.AreEqual(0, eventHandler.NoVPKExtractedTotal);
        }

        [TestMethod]
        public void TwoVPKs_Extraction_Complete_Failed_ReturnsPartiallyCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/vpks/test1.vpk", new MockFileData("") },
                { "C:/vpks/test2.vpk", new MockFileData("") },
            });

            var eventHandler = new TestExtractVPKInstallStepEventHandler();
            var step = new ExtractVPKInstallStep(new MockVPKExtractor()
            {
                CompleteOnVpks = ["C:/vpks/test1.vpk"],
                FailOnVpks = ["C:/vpks/test2.vpk"]
            }, DefaultStringToRegexConverter, VPKFileResolver, eventHandler);
            var stepData = new ExtractVPKInstallStepData()
            {
                OutDir = "test",
                Vpks = [
                    new ExtractVPKInstallStepDataVPK { VPKFile = "C:/vpks/test1.vpk" },
                    new ExtractVPKInstallStepDataVPK { VPKFile = "C:/vpks/test2.vpk" },
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.PartiallyComplete, result);
            Assert.AreEqual(1, eventHandler.VPKExtractionCompleteTotal);
            Assert.AreEqual(0, eventHandler.VPKExtractionCompleteWithErrorsTotal);
            Assert.AreEqual(1, eventHandler.VPKExtractionFailedTotal);
            Assert.AreEqual(0, eventHandler.NoVPKExtractedTotal);
        }

        [TestMethod]
        public void TwoVPKs_Extraction_CompleteWithErrors_Failed_ReturnsPartiallyCompleted()
        {
            var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>{
                { "C:/vpks/test1.vpk", new MockFileData("") },
                { "C:/vpks/test2.vpk", new MockFileData("") },
            });

            var eventHandler = new TestExtractVPKInstallStepEventHandler();
            var step = new ExtractVPKInstallStep(new MockVPKExtractor()
            {
                CompleteWithErrorsOnVpks = ["C:/vpks/test1.vpk"],
                FailOnVpks = ["C:/vpks/test2.vpk"]
            }, DefaultStringToRegexConverter, VPKFileResolver, eventHandler);
            var stepData = new ExtractVPKInstallStepData()
            {
                OutDir = "test",
                Vpks = [
                    new ExtractVPKInstallStepDataVPK { VPKFile = "C:/vpks/test1.vpk" },
                    new ExtractVPKInstallStepDataVPK { VPKFile = "C:/vpks/test2.vpk" },
                ]
            };

            var result = step.DoStep(new Context(fileSystem, NullConfiguration), stepData, NullWriter);

            Assert.AreEqual(PipelineStepStatus.PartiallyComplete, result);
            Assert.AreEqual(0, eventHandler.VPKExtractionCompleteTotal);
            Assert.AreEqual(1, eventHandler.VPKExtractionCompleteWithErrorsTotal);
            Assert.AreEqual(1, eventHandler.VPKExtractionFailedTotal);
            Assert.AreEqual(0, eventHandler.NoVPKExtractedTotal);
        }
    }
}