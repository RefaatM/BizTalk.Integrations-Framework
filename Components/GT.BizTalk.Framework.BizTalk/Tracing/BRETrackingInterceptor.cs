using DE.DAXFSA.Framework.BizTalk.Properties;
using DE.DAXFSA.Framework.Core.Tracing;
using Microsoft.RuleEngine;
using System;
using System.Globalization;

namespace DE.DAXFSA.Framework.BizTalk.Tracing
{
    public sealed class BRETrackingInterceptor : IRuleSetTrackingInterceptor, IDisposable
    {
        #region Private members

        // Preload all string resource for better performance.
        private static string traceHeaderTrace = BRETrackingResources.Header;

        private static string ruleEngineInstanceTrace = BRETrackingResources.RuleEngineInstance;
        private static string rulesetNameTrace = BRETrackingResources.RulesetName;
        private static string ruleFiredTrace = BRETrackingResources.RuleFired;
        private static string ruleNameTrace = BRETrackingResources.RuleName;
        private static string conflictResolutionCriteriaTrace = BRETrackingResources.ConflictResolutionCriteria;
        private static string workingMemoryUpdateTrace = BRETrackingResources.WorkingMemoryUpdate;
        private static string operationTypeTrace = BRETrackingResources.OperationType;
        private static string assertOperationTrace = BRETrackingResources.AssertOperation;
        private static string retractOperationTrace = BRETrackingResources.RetractOperation;
        private static string updateOperationTrace = BRETrackingResources.UpdateOperation;
        private static string assertUnrecognizedOperationTrace = BRETrackingResources.AssertUnrecognizedOperation;
        private static string retractUnrecognizedOperationTrace = BRETrackingResources.RetractUnrecognizedOperation;
        private static string updateUnrecognizedOperationTrace = BRETrackingResources.UpdateUnrecognizedOperation;
        private static string retractNotPresentOperationTrace = BRETrackingResources.RetractNotPresentOperation;
        private static string updateNotPresentOperationTrace = BRETrackingResources.UpdateNotPresentOperation;
        private static string unrecognizedOperationTrace = BRETrackingResources.UnrecognizedOperation;
        private static string objectTypeTrace = BRETrackingResources.ObjectType;
        private static string objectInstanceTrace = BRETrackingResources.ObjectInstance;
        private static string conditionEvaluationTrace = BRETrackingResources.ConditionEvaluation;
        private static string testExpressionTrace = BRETrackingResources.TestExpression;
        private static string leftOperandValueTrace = BRETrackingResources.LeftOperandValue;
        private static string rightOperandValueTrace = BRETrackingResources.RightOperandValue;
        private static string testResultTrace = BRETrackingResources.TestResult;
        private static string agendaUpdateTrace = BRETrackingResources.AgendaUpdate;
        private static string addOperationTrace = BRETrackingResources.AddOperation;
        private static string removeOperationTrace = BRETrackingResources.RemoveOperation;

        private const string TraceTemplateOneParam = "RULE TRACE: {0} null";
        private const string TraceTemplateTwoParam = "RULE TRACE: {0} {1}";
        private const string TraceTemplateThreeParam = "RULE TRACE: {0} {1} {2}";
        private const string TraceTemplateObjTypeEval = "RULE TRACE: {0} {1} {2} ({3})";
        private const string TraceTemplateValueTypeEval = "RULE TRACE: {0} {1} ({2})";

        private TrackingConfiguration trackingConfig;
        private string ruleSetName;
        private string ruleEngineGuid;

        #endregion Private members

        #region IRuleSetTrackingInterceptor members

        public void SetTrackingConfig(TrackingConfiguration trackingConfig)
        {
            this.trackingConfig = trackingConfig;
        }

        public void TrackAgendaUpdate(bool isAddition, string ruleName, object conflictResolutionCriteria)
        {
            PrintHeader(agendaUpdateTrace);

            if (isAddition)
            {
                TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, operationTypeTrace, addOperationTrace);
            }
            else
            {
                TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, operationTypeTrace, removeOperationTrace);
            }

            TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, ruleNameTrace, ruleName);

            if (conflictResolutionCriteria == null)
            {
                TraceProvider.Logger.TraceInfo(TraceTemplateOneParam, conflictResolutionCriteriaTrace);
            }
            else
            {
                TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, conflictResolutionCriteriaTrace, conflictResolutionCriteria);
            }
        }

        public void TrackConditionEvaluation(string testExpression, string leftClassType, int leftClassInstanceId, object leftValue, string rightClassType, int rightClassInstanceId, object rightValue, bool result)
        {
            PrintHeader(conditionEvaluationTrace);

            TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, testExpressionTrace, testExpression);

            if (leftValue == null)
            {
                TraceProvider.Logger.TraceInfo(TraceTemplateOneParam, leftOperandValueTrace);
            }
            else
            {
                Type leftValueType = leftValue.GetType();

                if (leftValueType.IsClass && (Type.GetTypeCode(leftValueType) != TypeCode.String))
                {
                    TraceProvider.Logger.TraceInfo(TraceTemplateObjTypeEval, leftOperandValueTrace, objectInstanceTrace, leftValue.GetHashCode().ToString(CultureInfo.CurrentCulture), leftValueType.FullName);
                }
                else
                {
                    TraceProvider.Logger.TraceInfo(TraceTemplateValueTypeEval, leftOperandValueTrace, leftValue, leftValueType.FullName);
                }
            }

            if (rightValue == null)
            {
                TraceProvider.Logger.TraceInfo(TraceTemplateOneParam, rightOperandValueTrace);
            }
            else
            {
                Type rightValueType = rightValue.GetType();

                if (rightValueType.IsClass && (Type.GetTypeCode(rightValueType) != TypeCode.String))
                {
                    TraceProvider.Logger.TraceInfo(TraceTemplateObjTypeEval, rightOperandValueTrace, objectInstanceTrace, rightValue.GetHashCode().ToString(CultureInfo.CurrentCulture), rightValueType.FullName);
                }
                else
                {
                    TraceProvider.Logger.TraceInfo(TraceTemplateValueTypeEval, rightOperandValueTrace, rightValue, rightValueType.FullName);
                }
            }

            TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, testResultTrace, result);
        }

        public void TrackFactActivity(FactActivityType activityType, string classType, int classInstanceId)
        {
            PrintHeader(workingMemoryUpdateTrace);

            switch (activityType)
            {
                case FactActivityType.Assert:
                    TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, operationTypeTrace, assertOperationTrace);
                    break;

                case FactActivityType.Retract:
                    TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, operationTypeTrace, retractOperationTrace);
                    break;

                case FactActivityType.Update:
                    TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, operationTypeTrace, updateOperationTrace);
                    break;

                case FactActivityType.AssertUnrecognized:
                    TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, operationTypeTrace, assertUnrecognizedOperationTrace);
                    break;

                case FactActivityType.RetractUnrecognized:
                    TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, operationTypeTrace, retractUnrecognizedOperationTrace);
                    break;

                case FactActivityType.UpdateUnrecognized:
                    TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, operationTypeTrace, updateUnrecognizedOperationTrace);
                    break;

                case FactActivityType.RetractNotPresent:
                    TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, operationTypeTrace, retractNotPresentOperationTrace);
                    break;

                case FactActivityType.UpdateNotPresent:
                    TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, operationTypeTrace, updateNotPresentOperationTrace);
                    break;

                default:
                    TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, operationTypeTrace, unrecognizedOperationTrace);
                    break;
            }

            TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, objectTypeTrace, classType);
            TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, objectInstanceTrace, classInstanceId.ToString(CultureInfo.CurrentCulture));
        }

        public void TrackRuleFiring(string ruleName, object conflictResolutionCriteria)
        {
            PrintHeader(ruleFiredTrace);

            TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, ruleNameTrace, ruleName);

            if (conflictResolutionCriteria == null)
            {
                TraceProvider.Logger.TraceInfo(TraceTemplateOneParam, conflictResolutionCriteriaTrace);
            }
            else
            {
                TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, conflictResolutionCriteriaTrace, conflictResolutionCriteria);
            }
        }

        public void TrackRuleSetEngineAssociation(RuleSetInfo ruleSetInfo, Guid ruleEngineGuid)
        {
            this.ruleSetName = ruleSetInfo.Name;
            this.ruleEngineGuid = ruleEngineGuid.ToString();

            TraceProvider.Logger.TraceInfo(TraceTemplateThreeParam, traceHeaderTrace, this.ruleSetName, DateTime.Now.ToString(CultureInfo.CurrentCulture));
        }

        #endregion IRuleSetTrackingInterceptor members

        #region IDisposable members

        public void Dispose()
        {
            // We don't really need to do anything here.
        }

        #endregion IDisposable members

        #region Private members

        private void PrintHeader(string hdr)
        {
            TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, hdr, DateTime.Now.ToString(CultureInfo.CurrentCulture));
            TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, ruleEngineInstanceTrace, this.ruleEngineGuid);
            TraceProvider.Logger.TraceInfo(TraceTemplateTwoParam, rulesetNameTrace, this.ruleSetName);
        }

        #endregion Private members
    }
}