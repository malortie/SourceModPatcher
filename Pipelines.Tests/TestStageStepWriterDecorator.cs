namespace Pipelines.Tests
{
    public class StageStepWriterFormatterMock : IStageStepWriterFormatter
    {
        public int FormatTotal { get; private set; } = 0;

        public string Format(string message, ReadOnlyPipelineStageData stageData, ReadOnlyPipelineStepData stepData)
        {
            ++FormatTotal;
            return string.Empty;
        }
    }

    [TestClass]
    public class TestStageStepWriterDecorator
    {
        static readonly IWriter NullWriter = new NullWriter();

        [TestMethod]
        public void Success_Calls_Writer_Success()
        {
            var writer = new WriterMock();
            var stageStepWriterDecorator = new StageStepWriterDecorator(writer);
            stageStepWriterDecorator.Success(string.Empty);
            Assert.AreEqual(1, writer.SuccessTotal);
        }

        [TestMethod]
        public void Info_Calls_Writer_Info()
        {
            var writer = new WriterMock();
            var stageStepWriterDecorator = new StageStepWriterDecorator(writer);
            stageStepWriterDecorator.Info(string.Empty);
            Assert.AreEqual(1, writer.InfoTotal);
        }

        [TestMethod]
        public void Warning_Calls_Writer_Warning()
        {
            var writer = new WriterMock();
            var stageStepWriterDecorator = new StageStepWriterDecorator(writer);
            stageStepWriterDecorator.Warning(string.Empty);
            Assert.AreEqual(1, writer.WarningTotal);
        }

        [TestMethod]
        public void Error_Calls_Writer_Error()
        {
            var writer = new WriterMock();
            var stageStepWriterDecorator = new StageStepWriterDecorator(writer);
            stageStepWriterDecorator.Error(string.Empty);
            Assert.AreEqual(1, writer.ErrorTotal);
        }

        [TestMethod]
        public void Failure_Calls_Writer_Failure()
        {
            var writer = new WriterMock();
            var stageStepWriterDecorator = new StageStepWriterDecorator(writer);
            stageStepWriterDecorator.Failure(string.Empty);
            Assert.AreEqual(1, writer.FailureTotal);
        }

        [TestMethod]
        public void Cancellation_Calls_Writer_Cancellation()
        {
            var writer = new WriterMock();
            var stageStepWriterDecorator = new StageStepWriterDecorator(writer);
            stageStepWriterDecorator.Cancellation(string.Empty);
            Assert.AreEqual(1, writer.CancellationTotal);
        }

        [TestMethod]
        public void Writer_OneOfEachType()
        {
            var writer = new WriterMock();
            var stageStepWriterDecorator = new StageStepWriterDecorator(writer);
            stageStepWriterDecorator.Success(string.Empty);
            stageStepWriterDecorator.Info(string.Empty);
            stageStepWriterDecorator.Warning(string.Empty);
            stageStepWriterDecorator.Error(string.Empty);
            stageStepWriterDecorator.Failure(string.Empty);
            stageStepWriterDecorator.Cancellation(string.Empty);
            Assert.AreEqual(1, writer.SuccessTotal);
            Assert.AreEqual(1, writer.InfoTotal);
            Assert.AreEqual(1, writer.WarningTotal);
            Assert.AreEqual(1, writer.ErrorTotal);
            Assert.AreEqual(1, writer.FailureTotal);
            Assert.AreEqual(1, writer.CancellationTotal);
        }

        [TestMethod]
        public void Success_Calls_SuccessFormatter()
        {
            var successFormatter = new StageStepWriterFormatterMock();
            var infoFormatter = new StageStepWriterFormatterMock();
            var warningFormatter = new StageStepWriterFormatterMock();
            var errorFormatter = new StageStepWriterFormatterMock();
            var failureFormatter = new StageStepWriterFormatterMock();
            var cancellationFormatter = new StageStepWriterFormatterMock();

            var stageStepWriterDecorator = new StageStepWriterDecorator(NullWriter)
            {
                SuccessFormatter = successFormatter,
                InfoFormatter = infoFormatter,
                WarningFormatter = warningFormatter,
                ErrorFormatter = errorFormatter,
                FailureFormatter = failureFormatter,
                CancellationFormatter = cancellationFormatter,
            };
            stageStepWriterDecorator.Success(string.Empty);

            Assert.AreEqual(1, successFormatter.FormatTotal);
        }

        [TestMethod]
        public void Info_Calls_InfoFormatter()
        {
            var successFormatter = new StageStepWriterFormatterMock();
            var infoFormatter = new StageStepWriterFormatterMock();
            var warningFormatter = new StageStepWriterFormatterMock();
            var errorFormatter = new StageStepWriterFormatterMock();
            var failureFormatter = new StageStepWriterFormatterMock();
            var cancellationFormatter = new StageStepWriterFormatterMock();

            var stageStepWriterDecorator = new StageStepWriterDecorator(NullWriter)
            {
                SuccessFormatter = successFormatter,
                InfoFormatter = infoFormatter,
                WarningFormatter = warningFormatter,
                ErrorFormatter = errorFormatter,
                FailureFormatter = failureFormatter,
                CancellationFormatter = cancellationFormatter,
            };
            stageStepWriterDecorator.Info(string.Empty);

            Assert.AreEqual(1, infoFormatter.FormatTotal);
        }

        [TestMethod]
        public void Warning_Calls_WarningFormatter()
        {
            var successFormatter = new StageStepWriterFormatterMock();
            var infoFormatter = new StageStepWriterFormatterMock();
            var warningFormatter = new StageStepWriterFormatterMock();
            var errorFormatter = new StageStepWriterFormatterMock();
            var failureFormatter = new StageStepWriterFormatterMock();
            var cancellationFormatter = new StageStepWriterFormatterMock();

            var stageStepWriterDecorator = new StageStepWriterDecorator(NullWriter)
            {
                SuccessFormatter = successFormatter,
                InfoFormatter = infoFormatter,
                WarningFormatter = warningFormatter,
                ErrorFormatter = errorFormatter,
                FailureFormatter = failureFormatter,
                CancellationFormatter = cancellationFormatter,
            };
            stageStepWriterDecorator.Warning(string.Empty);

            Assert.AreEqual(1, warningFormatter.FormatTotal);
        }

        [TestMethod]
        public void Error_Calls_ErrorFormatter()
        {
            var successFormatter = new StageStepWriterFormatterMock();
            var infoFormatter = new StageStepWriterFormatterMock();
            var warningFormatter = new StageStepWriterFormatterMock();
            var errorFormatter = new StageStepWriterFormatterMock();
            var failureFormatter = new StageStepWriterFormatterMock();
            var cancellationFormatter = new StageStepWriterFormatterMock();

            var stageStepWriterDecorator = new StageStepWriterDecorator(NullWriter)
            {
                SuccessFormatter = successFormatter,
                InfoFormatter = infoFormatter,
                WarningFormatter = warningFormatter,
                ErrorFormatter = errorFormatter,
                FailureFormatter = failureFormatter,
                CancellationFormatter = cancellationFormatter,
            };
            stageStepWriterDecorator.Error(string.Empty);

            Assert.AreEqual(1, errorFormatter.FormatTotal);
        }

        [TestMethod]
        public void Failure_Calls_FailureFormatter()
        {
            var successFormatter = new StageStepWriterFormatterMock();
            var infoFormatter = new StageStepWriterFormatterMock();
            var warningFormatter = new StageStepWriterFormatterMock();
            var errorFormatter = new StageStepWriterFormatterMock();
            var failureFormatter = new StageStepWriterFormatterMock();
            var cancellationFormatter = new StageStepWriterFormatterMock();

            var stageStepWriterDecorator = new StageStepWriterDecorator(NullWriter)
            {
                SuccessFormatter = successFormatter,
                InfoFormatter = infoFormatter,
                WarningFormatter = warningFormatter,
                ErrorFormatter = errorFormatter,
                FailureFormatter = failureFormatter,
                CancellationFormatter = cancellationFormatter,
            };
            stageStepWriterDecorator.Failure(string.Empty);

            Assert.AreEqual(1, failureFormatter.FormatTotal);
        }

        [TestMethod]
        public void Cancellation_Calls_CancellationFormatter()
        {
            var successFormatter = new StageStepWriterFormatterMock();
            var infoFormatter = new StageStepWriterFormatterMock();
            var warningFormatter = new StageStepWriterFormatterMock();
            var errorFormatter = new StageStepWriterFormatterMock();
            var failureFormatter = new StageStepWriterFormatterMock();
            var cancellationFormatter = new StageStepWriterFormatterMock();

            var stageStepWriterDecorator = new StageStepWriterDecorator(NullWriter)
            {
                SuccessFormatter = successFormatter,
                InfoFormatter = infoFormatter,
                WarningFormatter = warningFormatter,
                ErrorFormatter = errorFormatter,
                FailureFormatter = failureFormatter,
                CancellationFormatter = cancellationFormatter,
            };
            stageStepWriterDecorator.Cancellation(string.Empty);

            Assert.AreEqual(1, cancellationFormatter.FormatTotal);
        }

        [TestMethod]
        public void Formatter_OneOfEachType()
        {
            var successFormatter = new StageStepWriterFormatterMock();
            var infoFormatter = new StageStepWriterFormatterMock();
            var warningFormatter = new StageStepWriterFormatterMock();
            var errorFormatter = new StageStepWriterFormatterMock();
            var failureFormatter = new StageStepWriterFormatterMock();
            var cancellationFormatter = new StageStepWriterFormatterMock();

            var stageStepWriterDecorator = new StageStepWriterDecorator(NullWriter)
            {
                SuccessFormatter = successFormatter,
                InfoFormatter = infoFormatter,
                WarningFormatter = warningFormatter,
                ErrorFormatter = errorFormatter,
                FailureFormatter = failureFormatter,
                CancellationFormatter = cancellationFormatter,
            };
            stageStepWriterDecorator.Success(string.Empty);
            stageStepWriterDecorator.Info(string.Empty);
            stageStepWriterDecorator.Warning(string.Empty);
            stageStepWriterDecorator.Error(string.Empty);
            stageStepWriterDecorator.Failure(string.Empty);
            stageStepWriterDecorator.Cancellation(string.Empty);

            Assert.AreEqual(1, successFormatter.FormatTotal);
            Assert.AreEqual(1, infoFormatter.FormatTotal);
            Assert.AreEqual(1, warningFormatter.FormatTotal);
            Assert.AreEqual(1, errorFormatter.FormatTotal);
            Assert.AreEqual(1, failureFormatter.FormatTotal);
            Assert.AreEqual(1, cancellationFormatter.FormatTotal);
        }
    }
}