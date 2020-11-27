using FTN.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
    public class SCADAConverter
    {

        public static void PopulateDisconnectorProperties(FTN.Disconnector cimDisconnector, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimDisconnector != null) && (rd != null))
            {
                SCADAConverter.PopulateSwitchProperties(cimDisconnector, rd, importHelper, report);
            }
        }

        public static void PopulatePowerTransformerProperties(FTN.PowerTransformer cimPowerTransformer, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimPowerTransformer != null) && (rd != null))
            {
                SCADAConverter.PopulateEquipmentProperties(cimPowerTransformer, rd, importHelper, report);
            }
        }

        public static void PopulateProtectedSwitchProperties(FTN.ProtectedSwitch cimProtectedSwitch, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimProtectedSwitch != null) && (rd != null))
            {
                SCADAConverter.PopulateSwitchProperties(cimProtectedSwitch, rd, importHelper, report);
            }
        }

        public static void PopulateRatioTapChangerProperties(FTN.RatioTapChanger cimRatioTapChanger, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimRatioTapChanger != null) && (rd != null))
            {
                SCADAConverter.PopulateTapChangerProperties(cimRatioTapChanger, rd, importHelper, report);

                if (cimRatioTapChanger.TransformerWindingHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimRatioTapChanger.TransformerWinding.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimRatioTapChanger.GetType().ToString()).Append(" rdfID = \"").Append(cimRatioTapChanger.ID);
                        report.Report.Append("\" - Failed to set reference to TransformerWinding: rdfID \"").Append(cimRatioTapChanger.TransformerWinding.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.RATIOTAPCHANGER_TRWINDING, gid));
                }
            }
        }

        public static void PopulateIdentifiedObjectProperties(FTN.IdentifiedObject cimIdentifiedObject, ResourceDescription rd)
        {
            if ((cimIdentifiedObject != null) && (rd != null))
            {
                if (cimIdentifiedObject.MRIDHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.IDOBJ_MRID, cimIdentifiedObject.MRID));
                }
                if (cimIdentifiedObject.NameHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.IDOBJ_NAME, cimIdentifiedObject.Name));
                }
                if (cimIdentifiedObject.DescriptionHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.IDOBJ_DESC, cimIdentifiedObject.Description));
                }
            }
        }

        public static void PopulatePowerSystemResourceProperties(FTN.PowerSystemResource cimPowerSystemResource, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimPowerSystemResource != null) && (rd != null))
            {
                SCADAConverter.PopulateIdentifiedObjectProperties(cimPowerSystemResource, rd);
            }
        }

        public static void PopulateEquipmentProperties(FTN.Equipment cimEquipment, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimEquipment != null) && (rd != null))
            {
                SCADAConverter.PopulatePowerSystemResourceProperties(cimEquipment, rd, importHelper, report);

                if (cimEquipment.EquipmentContainerHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimEquipment.EquipmentContainer.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimEquipment.GetType().ToString()).Append(" rdfID = \"").Append(cimEquipment.ID);
                        report.Report.Append("\" - Failed to set reference to EquipmentContainer: rdfID \"").Append(cimEquipment.EquipmentContainer.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.EQUIPMENT_EQUIPCONTAINER, gid));
                }
            }
        }

        public static void PopulateConductingEquipmentProperties(FTN.ConductingEquipment cimConductingEquipment, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimConductingEquipment != null) && (rd != null))
            {
                SCADAConverter.PopulateEquipmentProperties(cimConductingEquipment, rd, importHelper, report);
            }
        }

        public static void PopulateTerminalProperties(FTN.Terminal cimTerminal, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimTerminal != null) && (rd != null))
            {
                SCADAConverter.PopulateIdentifiedObjectProperties(cimTerminal, rd);

                if (cimTerminal.ConnectivityNodeHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimTerminal.ConnectivityNode.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimTerminal.GetType().ToString()).Append(" rdfID = \"").Append(cimTerminal.ID);
                        report.Report.Append("\" - Failed to set reference to ConnectivityNode: rdfID \"").Append(cimTerminal.ConnectivityNode.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.TERMINAL_CONNNODE, gid));
                }
                if (cimTerminal.ConductingEquipmentHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimTerminal.ConductingEquipment.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimTerminal.GetType().ToString()).Append(" rdfID = \"").Append(cimTerminal.ID);
                        report.Report.Append("\" - Failed to set reference to ConductingEquipment: rdfID \"").Append(cimTerminal.ConductingEquipment.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.TERMINAL_CONDEQUIPMENT, gid));
                }
            }
        }

        public static void PopulateConnectivityNodeProperties(FTN.ConnectivityNode cimConnectivityNode, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimConnectivityNode != null) && (rd != null))
            {
                SCADAConverter.PopulateIdentifiedObjectProperties(cimConnectivityNode, rd);

                if (cimConnectivityNode.ConnectivityNodeContainerHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimConnectivityNode.ConnectivityNodeContainer.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimConnectivityNode.GetType().ToString()).Append(" rdfID = \"").Append(cimConnectivityNode.ID);
                        report.Report.Append("\" - Failed to set reference to ConnectivityNodeContainer: rdfID \"").Append(cimConnectivityNode.ConnectivityNodeContainer.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.CONNECTIVITYNODE_CNODECONT, gid));
                }
            }
        }

        public static void PopulateRegulatingCondEqProperties(FTN.RegulatingCondEq cimRegulatingCondEq, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimRegulatingCondEq != null) && (rd != null))
            {
                SCADAConverter.PopulateConductingEquipmentProperties(cimRegulatingCondEq, rd, importHelper, report);
            }
        }

        public static void PopulateSwitchProperties(FTN.Switch cimSwitch, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimSwitch != null) && (rd != null))
            {
                SCADAConverter.PopulateConductingEquipmentProperties(cimSwitch, rd, importHelper, report);
                if (cimSwitch.ManipulationCountHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.SWITCH_MANIPULATIONCOUNT, cimSwitch.ManipulationCount));
                }
            }
        }

        public static void PopulateTapChangerProperties(FTN.TapChanger cimTapChanger, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimTapChanger != null) && (rd != null))
            {
                SCADAConverter.PopulatePowerSystemResourceProperties(cimTapChanger, rd, importHelper, report);

                if (cimTapChanger.HighStepHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.TAPCHANGER_HIGHSTEP, cimTapChanger.HighStep));
                }
                if (cimTapChanger.LowStepHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.TAPCHANGER_LOWSTEP, cimTapChanger.LowStep));
                }
                if (cimTapChanger.NormalStepHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.TAPCHANGER_NORMALSTEP, cimTapChanger.NormalStep));
                }
            }
        }

        public static void PopulateTransformerWindingProperties(FTN.TransformerWinding cimTransformerWinding, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimTransformerWinding != null) && (rd != null))
            {
                SCADAConverter.PopulateConductingEquipmentProperties(cimTransformerWinding, rd, importHelper, report);

                if (cimTransformerWinding.PowerTransformerHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimTransformerWinding.PowerTransformer.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimTransformerWinding.GetType().ToString()).Append(" rdfID = \"").Append(cimTransformerWinding.ID);
                        report.Report.Append("\" - Failed to set reference to PowerTransformer: rdfID \"").Append(cimTransformerWinding.PowerTransformer.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.TRANSFORMERWINDING_POWERTR, gid));
                }
                if (cimTransformerWinding.RatioTapChangerHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimTransformerWinding.RatioTapChanger.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimTransformerWinding.GetType().ToString()).Append(" rdfID = \"").Append(cimTransformerWinding.ID);
                        report.Report.Append("\" - Failed to set reference to RatioTapChanger: rdfID \"").Append(cimTransformerWinding.RatioTapChanger.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.TRANSFORMERWINDING_RATIOTC, gid));
                }
            }
        }

        public static void PopulateConnectivityNodeContainerProperties(FTN.ConnectivityNodeContainer cimConnectivityNodeCont, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimConnectivityNodeCont != null) && (rd != null))
            {
                SCADAConverter.PopulatePowerSystemResourceProperties(cimConnectivityNodeCont, rd, importHelper, report);
            }
        }

        public static void PopulateEquipmentContainerProperties(FTN.EquipmentContainer cimEquipmentCont, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimEquipmentCont != null) && (rd != null))
            {
                SCADAConverter.PopulateConnectivityNodeContainerProperties(cimEquipmentCont, rd, importHelper, report);
            }
        }

        public static void PopulateSubstationProperties(FTN.Substation cimSubstation, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimSubstation != null) && (rd != null))
            {
                SCADAConverter.PopulateEquipmentContainerProperties(cimSubstation, rd, importHelper, report);
            }
        }

        public static void PopulateMeasurementProperties(FTN.Measurement cimMeasurement, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimMeasurement != null) && (rd != null))
            {
                SCADAConverter.PopulateIdentifiedObjectProperties(cimMeasurement, rd);

                if (cimMeasurement.PowerSystemResourceHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimMeasurement.PowerSystemResource.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimMeasurement.GetType().ToString()).Append(" rdfID = \"").Append(cimMeasurement.ID);
                        report.Report.Append("\" - Failed to set reference to PowerSystemResource: rdfID \"").Append(cimMeasurement.PowerSystemResource.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.MEASUREMENT_PSR, gid));
                }
                if (cimMeasurement.TerminalsHasValue)
                {
                    long gid = importHelper.GetMappedGID(cimMeasurement.Terminals.ID);
                    if (gid < 0)
                    {
                        report.Report.Append("WARNING: Convert ").Append(cimMeasurement.GetType().ToString()).Append(" rdfID = \"").Append(cimMeasurement.ID);
                        report.Report.Append("\" - Failed to set reference to Terminal: rdfID \"").Append(cimMeasurement.Terminals.ID).AppendLine(" \" is not mapped to GID!");
                    }
                    rd.AddProperty(new Property(ModelCode.MEASUREMENT_TERMINAL, gid));
                }
                if (cimMeasurement.DirectionHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.MEASUREMENT_DIRECTION, (short)GetDMSSignalDirection((SignalDirection)cimMeasurement.Direction)));
                }
                if (cimMeasurement.MeasurementTypeHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.MEASUREMENT_MEASTYPE, (short)GetDMSMeasurementType((MeasurementType)cimMeasurement.MeasurementType)));
                }
                if (cimMeasurement.BaseAddressHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.MEASUREMENT_BASEADDR, cimMeasurement.BaseAddress));
                }
                if (cimMeasurement.ObjectmRIDHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.MEASUREMENT_OBJMRID, cimMeasurement.ObjectmRID));
                }
                if (cimMeasurement.TimeStampHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.MEASUREMENT_TIMESTAMP, cimMeasurement.TimeStamp));
                }
            }
        }

        public static void PopulateAnalogProperties(FTN.Analog cimAnalog, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimAnalog != null) && (rd != null))
            {
                SCADAConverter.PopulateMeasurementProperties(cimAnalog, rd, importHelper, report);

                if (cimAnalog.MaxValueHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ANALOG_MAXVALUE, cimAnalog.MaxValue));
                }
                if (cimAnalog.MinValueHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ANALOG_MINVALUE, cimAnalog.MinValue));
                }
                if (cimAnalog.NormalValueHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ANALOG_NORMALVALUE, cimAnalog.NormalValue));
                }
            }
        }

        public static void PopulateAsynchronousMachineProperties(FTN.AsynchronousMachine cimAsynchronousMachine, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimAsynchronousMachine != null) && (rd != null))
            {
                SCADAConverter.PopulateRegulatingCondEqProperties(cimAsynchronousMachine, rd, importHelper, report);

                if (cimAsynchronousMachine.CosPhiHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ASYNCMACHINE_COSPHI, cimAsynchronousMachine.CosPhi));
                }
                if (cimAsynchronousMachine.RatedPHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ASYNCMACHINE_RATEDP, cimAsynchronousMachine.RatedP));
                }
                if (cimAsynchronousMachine.MaximumTempHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ASYNCMACHINE_MAXTEMP, cimAsynchronousMachine.MaximumTemp));
                }
                if (cimAsynchronousMachine.MinimumTempHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ASYNCMACHINE_MINTEMP, cimAsynchronousMachine.MinimumTemp));
                }
                if (cimAsynchronousMachine.CurrentTempHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.ASYNCMACHINE_CURRTEMP, cimAsynchronousMachine.CurrentTemp));
                }
            }
        }

        public static void PopulateDiscreteProperties(FTN.Discrete cimDiscrete, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimDiscrete != null) && (rd != null))
            {
                SCADAConverter.PopulateMeasurementProperties(cimDiscrete, rd, importHelper, report);

                if (cimDiscrete.MaxValueHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.DISCRETE_MAXVALUE, cimDiscrete.MaxValue));
                }
                if (cimDiscrete.MinValueHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.DISCRETE_MINVALUE, cimDiscrete.MinValue));
                }
                if (cimDiscrete.NormalValueHasValue)
                {
                    rd.AddProperty(new Property(ModelCode.DISCRETE_NORMALVALUE, cimDiscrete.NormalValue));
                }
            }
        }

        public static void PopulateBreakerProperties(FTN.Breaker cimBreaker, ResourceDescription rd, ImportHelper importHelper, TransformAndLoadReport report)
        {
            if ((cimBreaker != null) && (rd != null))
            {
                SCADAConverter.PopulateProtectedSwitchProperties(cimBreaker, rd, importHelper, report);
            }
        }

        public static SignalDirection GetDMSSignalDirection(SignalDirection signalDirection)
        {
            switch (signalDirection)
            {
                case SignalDirection.Read:
                    return SignalDirection.Read;
                case SignalDirection.ReadWrite:
                    return SignalDirection.ReadWrite;
                case SignalDirection.Write:
                    return SignalDirection.Write;
                default:
                    return SignalDirection.ReadWrite;
            }
        }

        public static MeasurementType GetDMSMeasurementType(MeasurementType measurementType)
        {
            switch (measurementType)
            {
                case MeasurementType.ActiveEnergy:
                    return MeasurementType.ActiveEnergy;
                case MeasurementType.ActivePower:
                    return MeasurementType.ActivePower;
                case MeasurementType.Admittance:
                    return MeasurementType.Admittance;
                case MeasurementType.AdmittancePerLength:
                    return MeasurementType.AdmittancePerLength;
                case MeasurementType.ApparentPower:
                    return MeasurementType.ApparentPower;
                case MeasurementType.CosPhi:
                    return MeasurementType.CosPhi;
                case MeasurementType.Current:
                    return MeasurementType.Current;
                case MeasurementType.CurrentAngle:
                    return MeasurementType.CurrentAngle;
                case MeasurementType.Discrete:
                    return MeasurementType.Discrete;
                case MeasurementType.Flow:
                    return MeasurementType.Flow;
                case MeasurementType.FluidLevel:
                    return MeasurementType.FluidLevel;
                case MeasurementType.Frequency:
                    return MeasurementType.Frequency;
                case MeasurementType.Impedance:
                    return MeasurementType.Impedance;
                case MeasurementType.Length:
                    return MeasurementType.Length;
                case MeasurementType.Percent:
                    return MeasurementType.Percent;
                case MeasurementType.ReactiveEnergy:
                    return MeasurementType.ReactiveEnergy;
                case MeasurementType.ReactivePower:
                    return MeasurementType.ReactivePower;
                case MeasurementType.RelativeVoltage:
                    return MeasurementType.RelativeVoltage;
                case MeasurementType.RotationSpeed:
                    return MeasurementType.RotationSpeed;
                case MeasurementType.Status:
                    return MeasurementType.Status;
                case MeasurementType.SwitchStatus:
                    return MeasurementType.SwitchStatus;
                case MeasurementType.Temperature:
                    return MeasurementType.Temperature;
                case MeasurementType.Time:
                    return MeasurementType.Time;
                case MeasurementType.Voltage:
                    return MeasurementType.Voltage;
                case MeasurementType.VoltageAngle:
                    return MeasurementType.VoltageAngle;
                default:
                    return MeasurementType.Voltage;
            }
        }

    }
}
