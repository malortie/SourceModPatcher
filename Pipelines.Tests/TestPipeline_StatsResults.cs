namespace Pipelines.Tests
{
    [TestClass]
    public class TestPipeline_StatsResults
    {
        static NullContext NullContext = new NullContext();

        [TestMethod]
        public void StatsResults_NoStages()
        {
            var pipeline = new Pipeline<NullContext>(Array.Empty<IPipelineStage<NullContext>>());

            pipeline.Execute(NullContext);

            Assert.AreEqual(0, pipeline.StatsResults.NumStagesTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCancelled);

            Assert.AreEqual(0, pipeline.StatsResults.NumStepsTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsCancelled);
        }

        [TestMethod]
        public void StatsResults_OneStage_With_NoSteps_ReturnsCompleted()
        {
            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = []
                }
            });

            pipeline.Execute(NullContext);

            Assert.AreEqual(1, pipeline.StatsResults.NumStagesTotal);
            Assert.AreEqual(1, pipeline.StatsResults.NumStagesCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCancelled);

            Assert.AreEqual(0, pipeline.StatsResults.NumStepsTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsCancelled);
        }

        [TestMethod]
        public void StatsResults_OneStage_With_OneStepCompleted_ReturnsCompleted()
        {
            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataComplete()
                    ]
                }
            });

            pipeline.Execute(NullContext);

            Assert.AreEqual(1, pipeline.StatsResults.NumStagesTotal);
            Assert.AreEqual(1, pipeline.StatsResults.NumStagesCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCancelled);

            Assert.AreEqual(1, pipeline.StatsResults.NumStepsTotal);
            Assert.AreEqual(1, pipeline.StatsResults.NumStepsCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsCancelled);
        }

        [TestMethod]
        public void StatsResults_OneStage_With_OneStepPartiallyCompleted_ReturnsPartiallyCompleted()
        {
            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataPartiallyComplete()
                    ]
                }
            });

            pipeline.Execute(NullContext);

            Assert.AreEqual(1, pipeline.StatsResults.NumStagesTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCompleted);
            Assert.AreEqual(1, pipeline.StatsResults.NumStagesPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCancelled);

            Assert.AreEqual(1, pipeline.StatsResults.NumStepsTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsCompleted);
            Assert.AreEqual(1, pipeline.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsCancelled);
        }

        [TestMethod]
        public void StatsResults_OneStage_With_OneStepFailed_ReturnsFailed()
        {
            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataFailed()
                    ]
                }
            });

            pipeline.Execute(NullContext);

            Assert.AreEqual(1, pipeline.StatsResults.NumStagesTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesPartiallyCompleted);
            Assert.AreEqual(1, pipeline.StatsResults.NumStagesFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCancelled);

            Assert.AreEqual(1, pipeline.StatsResults.NumStepsTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(1, pipeline.StatsResults.NumStepsFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsCancelled);
        }

        [TestMethod]
        public void StatsResults_OneStage_With_OneStepCancelled_ReturnsCancelled()
        {
            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataCancelled()
                    ]
                }
            });

            pipeline.Execute(NullContext);

            Assert.AreEqual(1, pipeline.StatsResults.NumStagesTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesFailed);
            Assert.AreEqual(1, pipeline.StatsResults.NumStagesCancelled);

            Assert.AreEqual(1, pipeline.StatsResults.NumStepsTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsFailed);
            Assert.AreEqual(1, pipeline.StatsResults.NumStepsCancelled);
        }

        [TestMethod]
        public void StatsResults_OneStage_With_AllStepsCompleted_ReturnsCompleted()
        {
            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataComplete(),
                        new NullStepDataComplete()
                    ]
                }
            });

            pipeline.Execute(NullContext);

            Assert.AreEqual(1, pipeline.StatsResults.NumStagesTotal);
            Assert.AreEqual(1, pipeline.StatsResults.NumStagesCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCancelled);

            Assert.AreEqual(2, pipeline.StatsResults.NumStepsTotal);
            Assert.AreEqual(2, pipeline.StatsResults.NumStepsCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsCancelled);
        }

        [TestMethod]
        public void StatsResults_OneStage_With_OneStepCompleted_OneStepPartiallyCompleted_ReturnsPartiallyCompleted()
        {
            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataComplete(),
                        new NullStepDataPartiallyComplete()
                    ]
                }
            });

            pipeline.Execute(NullContext);

            Assert.AreEqual(1, pipeline.StatsResults.NumStagesTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCompleted);
            Assert.AreEqual(1, pipeline.StatsResults.NumStagesPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCancelled);

            Assert.AreEqual(2, pipeline.StatsResults.NumStepsTotal);
            Assert.AreEqual(1, pipeline.StatsResults.NumStepsCompleted);
            Assert.AreEqual(1, pipeline.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsCancelled);
        }

        [TestMethod]
        public void StatsResults_OneStage_With_OneStepPartiallyCompleted_OneStepFailed_OneStepCancelled_ReturnsPartiallyCompleted()
        {
            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataPartiallyComplete(),
                        new NullStepDataFailed(),
                        new NullStepDataCancelled()
                    ]
                }
            });

            pipeline.Execute(NullContext);

            Assert.AreEqual(1, pipeline.StatsResults.NumStagesTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCompleted);
            Assert.AreEqual(1, pipeline.StatsResults.NumStagesPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCancelled);

            Assert.AreEqual(3, pipeline.StatsResults.NumStepsTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsCompleted);
            Assert.AreEqual(1, pipeline.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(1, pipeline.StatsResults.NumStepsFailed);
            Assert.AreEqual(1, pipeline.StatsResults.NumStepsCancelled);
        }

        [TestMethod]
        public void StatsResults_OneStage_With_OneStepFailed_OneStepCancelled_ReturnsFailed()
        {
            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataFailed(),
                        new NullStepDataCancelled()
                    ]
                }
            });

            pipeline.Execute(NullContext);

            Assert.AreEqual(1, pipeline.StatsResults.NumStagesTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesPartiallyCompleted);
            Assert.AreEqual(1, pipeline.StatsResults.NumStagesFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCancelled);

            Assert.AreEqual(2, pipeline.StatsResults.NumStepsTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(1, pipeline.StatsResults.NumStepsFailed);
            Assert.AreEqual(1, pipeline.StatsResults.NumStepsCancelled);
        }

        [TestMethod]
        public void StatsResults_OneStage_With_TwoStepsCancelled_ReturnsCancelled()
        {
            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataCancelled(),
                        new NullStepDataCancelled()
                    ]
                }
            });

            pipeline.Execute(NullContext);

            Assert.AreEqual(1, pipeline.StatsResults.NumStagesTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesFailed);
            Assert.AreEqual(1, pipeline.StatsResults.NumStagesCancelled);

            Assert.AreEqual(2, pipeline.StatsResults.NumStepsTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsFailed);
            Assert.AreEqual(2, pipeline.StatsResults.NumStepsCancelled);
        }

        [TestMethod]
        public void StatsResults_3_Stages_Complete()
        {
            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataComplete()
                    ]
                },
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataComplete()
                    ]
                },
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataComplete()
                    ]
                }
            });

            pipeline.Execute(NullContext);

            Assert.AreEqual(3, pipeline.StatsResults.NumStagesTotal);
            Assert.AreEqual(3, pipeline.StatsResults.NumStagesCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCancelled);

            Assert.AreEqual(3, pipeline.StatsResults.NumStepsTotal);
            Assert.AreEqual(3, pipeline.StatsResults.NumStepsCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsCancelled);
        }

        [TestMethod]
        public void StatsResults_3_Stages_PartiallyComplete()
        {
            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataPartiallyComplete()
                    ]
                },
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataPartiallyComplete()
                    ]
                },
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataPartiallyComplete()
                    ]
                }
            });

            pipeline.Execute(NullContext);

            Assert.AreEqual(3, pipeline.StatsResults.NumStagesTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCompleted);
            Assert.AreEqual(3, pipeline.StatsResults.NumStagesPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCancelled);

            Assert.AreEqual(3, pipeline.StatsResults.NumStepsTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsCompleted);
            Assert.AreEqual(3, pipeline.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsCancelled);
        }

        [TestMethod]
        public void StatsResults_3_Stages_Failed()
        {
            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataFailed()
                    ]
                },
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataFailed()
                    ]
                },
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataFailed()
                    ]
                }
            });

            pipeline.Execute(NullContext);

            Assert.AreEqual(3, pipeline.StatsResults.NumStagesTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesPartiallyCompleted);
            Assert.AreEqual(3, pipeline.StatsResults.NumStagesFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCancelled);

            Assert.AreEqual(3, pipeline.StatsResults.NumStepsTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(3, pipeline.StatsResults.NumStepsFailed);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsCancelled);
        }

        [TestMethod]
        public void StatsResults_3_Stages_Cancelled()
        {
            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataCancelled()
                    ]
                },
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataCancelled()
                    ]
                },
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataCancelled()
                    ]
                }
            });

            pipeline.Execute(NullContext);

            Assert.AreEqual(3, pipeline.StatsResults.NumStagesTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStagesFailed);
            Assert.AreEqual(3, pipeline.StatsResults.NumStagesCancelled);

            Assert.AreEqual(3, pipeline.StatsResults.NumStepsTotal);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(0, pipeline.StatsResults.NumStepsFailed);
            Assert.AreEqual(3, pipeline.StatsResults.NumStepsCancelled);
        }

        [TestMethod]
        public void StatsResults_Stages_OneOfEachType()
        {
            var pipeline = new Pipeline<NullContext>(new[] {
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataComplete()
                    ]
                },
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataPartiallyComplete()
                    ]
                },
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataFailed()
                    ]
                },
                new NullStage
                {
                    StepsDatas = [
                        new NullStepDataCancelled()
                    ]
                }
            });

            pipeline.Execute(NullContext);

            Assert.AreEqual(4, pipeline.StatsResults.NumStagesTotal);
            Assert.AreEqual(1, pipeline.StatsResults.NumStagesCompleted);
            Assert.AreEqual(1, pipeline.StatsResults.NumStagesPartiallyCompleted);
            Assert.AreEqual(1, pipeline.StatsResults.NumStagesFailed);
            Assert.AreEqual(1, pipeline.StatsResults.NumStagesCancelled);

            Assert.AreEqual(4, pipeline.StatsResults.NumStepsTotal);
            Assert.AreEqual(1, pipeline.StatsResults.NumStepsCompleted);
            Assert.AreEqual(1, pipeline.StatsResults.NumStepsPartiallyCompleted);
            Assert.AreEqual(1, pipeline.StatsResults.NumStepsFailed);
            Assert.AreEqual(1, pipeline.StatsResults.NumStepsCancelled);
        }
    }
}

