namespace Pipelines.Tests
{
    public class SuccessWriterStepData : AbstractNullStepData { }
    public class InfoWriterStepData : AbstractNullStepData { }
    public class WarningWriterStepData : AbstractNullStepData { }
    public class ErrorWriterStepData : AbstractNullStepData { }
    public class FailureWriterStepData : AbstractNullStepData { }
    public class CancellationWriterStepData : AbstractNullStepData { }

    public class WriterStep : IPipelineStep<NullContext>
    {
        public PipelineStepStatus DoStep(NullContext context, IPipelineStepData stepData, IWriter writer)
        {
            if (stepData is SuccessWriterStepData)
                writer.Success(string.Empty);
            else if (stepData is InfoWriterStepData)
                writer.Info(string.Empty);
            else if (stepData is WarningWriterStepData)
                writer.Warning(string.Empty);
            else if (stepData is ErrorWriterStepData)
                writer.Error(string.Empty);
            else if (stepData is FailureWriterStepData)
                writer.Failure(string.Empty);
            else if (stepData is CancellationWriterStepData)
                writer.Cancellation(string.Empty);
            else
                throw new Exception($"Invalid stepData type passed : {stepData.GetType()}");

            return PipelineStepStatus.Complete;
        }
    }

    public class WriterStage : PipelineStage<NullContext>
    {
        static readonly Dictionary<Type, IPipelineStep<NullContext>> _stepsDataToStepMap = new()
        {
            { typeof(SuccessWriterStepData), new WriterStep() },
            { typeof(InfoWriterStepData), new WriterStep() },
            { typeof(WarningWriterStepData), new WriterStep() },
            { typeof(ErrorWriterStepData), new WriterStep() },
            { typeof(FailureWriterStepData), new WriterStep() },
            { typeof(CancellationWriterStepData), new WriterStep() },
        };

        public override IPipelineStep<NullContext> GetStepForStepData(IPipelineStepData stepData)
        {
            return _stepsDataToStepMap[stepData.GetType()];
        }
    }

    [TestClass]
    public class TestPipelineStage_Writer
    {
        static readonly NullContext NullContext = new();

        [TestMethod]
        public void Writer_Success()
        {
            var writer = new WriterMock();
            var stage = new WriterStage
            {
                Writer = writer,
                StepsDatas = [
                    new SuccessWriterStepData(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(1, writer.SuccessTotal);
        }


        [TestMethod]
        public void Writer_Info()
        {
            var writer = new WriterMock();
            var stage = new WriterStage
            {
                Writer = writer,
                StepsDatas = [
                    new InfoWriterStepData(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(1, writer.InfoTotal);
        }

        [TestMethod]
        public void Writer_Warning()
        {
            var writer = new WriterMock();
            var stage = new WriterStage
            {
                Writer = writer,
                StepsDatas = [
                    new WarningWriterStepData(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(1, writer.WarningTotal);
        }

        [TestMethod]
        public void Writer_Error()
        {
            var writer = new WriterMock();
            var stage = new WriterStage
            {
                Writer = writer,
                StepsDatas = [
                    new ErrorWriterStepData(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(1, writer.ErrorTotal);
        }

        [TestMethod]
        public void Writer_Failure()
        {
            var writer = new WriterMock();
            var stage = new WriterStage
            {
                Writer = writer,
                StepsDatas = [
                    new FailureWriterStepData(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(1, writer.FailureTotal);
        }

        [TestMethod]
        public void Writer_Cancellation()
        {
            var writer = new WriterMock();
            var stage = new WriterStage
            {
                Writer = writer,
                StepsDatas = [
                    new CancellationWriterStepData(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(1, writer.CancellationTotal);
        }

        [TestMethod]
        public void Writer_OneOfEachType()
        {
            var writer = new WriterMock();
            var stage = new WriterStage
            {
                Writer = writer,
                StepsDatas = [
                    new SuccessWriterStepData(),
                    new InfoWriterStepData(),
                    new WarningWriterStepData(),
                    new ErrorWriterStepData(),
                    new FailureWriterStepData(),
                    new CancellationWriterStepData(),
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(1, writer.SuccessTotal);
            Assert.AreEqual(1, writer.InfoTotal);
            Assert.AreEqual(1, writer.WarningTotal);
            Assert.AreEqual(1, writer.ErrorTotal);
            Assert.AreEqual(1, writer.FailureTotal);
            Assert.AreEqual(1, writer.CancellationTotal);
        }
    }
}