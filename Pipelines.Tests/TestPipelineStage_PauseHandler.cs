namespace Pipelines.Tests
{
    public class PauseHandlerMock : IPauseHandler
    {
        public int PauseTotal { get; private set; } = 0;

        public void Pause()
        {
            ++PauseTotal;
        }
    }

    [TestClass]
    public class TestPipelineStage_PauseHandler
    {
        static NullContext NullContext = new NullContext();

        [TestMethod]
        public void PauseHandler_CalledAfterEachStep_WhenPauseAfterEachStepIsTrue()
        {
            var pauseHandler = new PauseHandlerMock();
            var stage = new NullStage
            {
                PauseAfterEachStep = true,
                PauseHandler = pauseHandler,
                StepsDatas = [
                    new NullStepDataComplete(),
                    new NullStepDataComplete(),
                    new NullStepDataComplete()
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(3, pauseHandler.PauseTotal);
        }

        [TestMethod]
        public void PauseHandler_NotCalledAfterEachStep_WhenPauseAfterEachStepIsFalse()
        {
            var pauseHandler = new PauseHandlerMock();
            var stage = new NullStage
            {
                PauseAfterEachStep = false,
                PauseHandler = pauseHandler,
                StepsDatas = [
                    new NullStepDataComplete(),
                    new NullStepDataComplete(),
                    new NullStepDataComplete()
                ]
            };

            stage.DoStage(NullContext);
            Assert.AreEqual(0, pauseHandler.PauseTotal);
        }
    }
}