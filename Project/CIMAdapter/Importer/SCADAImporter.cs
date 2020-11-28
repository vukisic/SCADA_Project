using CIM.Model;
using FTN.Common;
using FTN.ESI.SIMES.CIM.CIMAdapter.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FTN.ESI.SIMES.CIM.CIMAdapter.Importer
{
    public class SCADAImporter
    {
        private static SCADAImporter importer = null;
        private static object singletoneLock = new object();

        private ConcreteModel concreteModel;
        private Delta delta;
        private ImportHelper importHelper;
        private TransformAndLoadReport report;

        public void Reset()
        {
            concreteModel = null;
            delta = new Delta();
            importHelper = new ImportHelper();
            report = null;
        }

        public static SCADAImporter Instance
        {
            get
            {
                if (importer == null)
                {
                    lock (singletoneLock)
                    {
                        if(importer == null)
                        {
                            importer = new SCADAImporter();
                            importer.Reset();
                        }
                    }
                }
                return importer;
            }
        }

        public Delta NMSDelta
        {
            get
            {
                return delta;
            }
        }

        public TransformAndLoadReport CreateNMSDelta(ConcreteModel cimConcreteModel)
        {
            LogManager.Log("Importing SCADA Elements...", LogLevel.Info);
            report = new TransformAndLoadReport();
            concreteModel = cimConcreteModel;
            delta.ClearDeltaOperations();

            if ((concreteModel != null) && (concreteModel.ModelMap != null))
            {
                try
                {
                    // convert into DMS elements
                    ConvertModelAndPopulateDelta();
                }
                catch (Exception ex)
                {
                    string message = string.Format("{0} - ERROR in data import - {1}", DateTime.Now, ex.Message);
                    LogManager.Log(message);
                    report.Report.AppendLine(ex.Message);
                    report.Success = false;
                }
            }
            LogManager.Log("Importing PowerTransformer Elements - END.", LogLevel.Info);
            return report;
        }

        private void ConvertModelAndPopulateDelta()
        {
            LogManager.Log("Loading elements and creating delta...", LogLevel.Info);

            //// import all concrete model types (DMSType enum)
            ImportSubstation();
            ImportDisconnector();
            ImportBreaker();
            ImportAsynchronousMachine();
            ImportPowerTransformer();
            ImportTransformerWinding();
            ImportRatioTapChanger();
            ImportConnectivityNode();
            ImportTerminal();
            ImportAnalog();
            ImportDiscrete();

            LogManager.Log("Loading elements and creating delta completed.", LogLevel.Info);
        }

        private void ImportSubstation()
        {
            SortedDictionary<string, object> cimSubstations = concreteModel.GetAllObjectsOfType("FTN.Substation");
            if(cimSubstations != null)
            {
                foreach(KeyValuePair<string, object> item in cimSubstations)
                {
                    Substation cimSubstation = item.Value as Substation;

                    ResourceDescription rd = CreateSubstationResourceDescription(cimSubstation);
                    if(rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("Substation ID = ").Append(cimSubstation.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("Substation ID = ").Append(cimSubstation.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateSubstationResourceDescription(Substation cimSubstation)
        {
            ResourceDescription rd = null;
            if(cimSubstation != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.SUBSTATION, importHelper.CheckOutIndexForDMSType(DMSType.SUBSTATION));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimSubstation.ID, gid);

                SCADAConverter.PopulateSubstationProperties(cimSubstation, rd, importHelper, report);
            }
            return rd;
        }
        
        private void ImportDisconnector()
        {
            SortedDictionary<string, object> cimDisconnectors = concreteModel.GetAllObjectsOfType("FTN.Disconnector");
            if(cimDisconnectors != null)
            {
                foreach(KeyValuePair<string, object> item in cimDisconnectors)
                {
                    Disconnector cimDisconnector = item.Value as Disconnector;

                    ResourceDescription rd = CreateDisconnectorResourceDescription(cimDisconnector);
                    if(rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("Disconnector ID = ").Append(cimDisconnector.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("Disconnector ID = ").Append(cimDisconnector.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateDisconnectorResourceDescription(Disconnector cimDisconnector)
        {
            ResourceDescription rd = null;
            if(cimDisconnector != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.DISCONNECTOR, importHelper.CheckOutIndexForDMSType(DMSType.DISCONNECTOR));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimDisconnector.ID, gid);

                SCADAConverter.PopulateDisconnectorProperties(cimDisconnector, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportBreaker()
        {
            SortedDictionary<string, object> cimBreakers = concreteModel.GetAllObjectsOfType("FTN.Breaker");
            if (cimBreakers != null)
            {
                foreach (KeyValuePair<string, object> item in cimBreakers)
                {
                    Breaker cimBreaker = item.Value as Breaker;

                    ResourceDescription rd = CreateBreakerResourceDescription(cimBreaker);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("Breaker ID = ").Append(cimBreaker.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("Breaker ID = ").Append(cimBreaker.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateBreakerResourceDescription(Breaker cimBreaker)
        {
            ResourceDescription rd = null;
            if (cimBreaker != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.BREAKER, importHelper.CheckOutIndexForDMSType(DMSType.BREAKER));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimBreaker.ID, gid);

                SCADAConverter.PopulateBreakerProperties(cimBreaker, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportAsynchronousMachine()
        {
            SortedDictionary<string, object> cimAsynchronousMachines = concreteModel.GetAllObjectsOfType("FTN.AsynchronousMachine");
            if (cimAsynchronousMachines != null)
            {
                foreach (KeyValuePair<string, object> item in cimAsynchronousMachines)
                {
                    AsynchronousMachine cimAsynchronousMachine = item.Value as AsynchronousMachine;

                    ResourceDescription rd = CreateAsynchronousMachineResourceDescription(cimAsynchronousMachine);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("AsynchronousMachine ID = ").Append(cimAsynchronousMachine.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("AsynchronousMachine ID = ").Append(cimAsynchronousMachine.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateAsynchronousMachineResourceDescription(AsynchronousMachine cimAsynchronousMachine)
        {
            ResourceDescription rd = null;
            if (cimAsynchronousMachine != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.ASYNCHRONOUSMACHINE, importHelper.CheckOutIndexForDMSType(DMSType.ASYNCHRONOUSMACHINE));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimAsynchronousMachine.ID, gid);

                SCADAConverter.PopulateAsynchronousMachineProperties(cimAsynchronousMachine, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportPowerTransformer()
        {
            SortedDictionary<string, object> cimPowerTransformers = concreteModel.GetAllObjectsOfType("FTN.PowerTransformer");
            if (cimPowerTransformers != null)
            {
                foreach (KeyValuePair<string, object> item in cimPowerTransformers)
                {
                    PowerTransformer cimPowerTransformer = item.Value as PowerTransformer;

                    ResourceDescription rd = CreatePowerTransformerResourceDescription(cimPowerTransformer);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("PowerTransformer ID = ").Append(cimPowerTransformer.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("PowerTransformer ID = ").Append(cimPowerTransformer.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreatePowerTransformerResourceDescription(PowerTransformer cimPowerTransformer)
        {
            ResourceDescription rd = null;
            if (cimPowerTransformer != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.POWERTRANSFORMER, importHelper.CheckOutIndexForDMSType(DMSType.POWERTRANSFORMER));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimPowerTransformer.ID, gid);

                SCADAConverter.PopulatePowerTransformerProperties(cimPowerTransformer, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportTransformerWinding()
        {
            SortedDictionary<string, object> cimTransformerWindings = concreteModel.GetAllObjectsOfType("FTN.TransformerWinding");
            if (cimTransformerWindings != null)
            {
                foreach (KeyValuePair<string, object> item in cimTransformerWindings)
                {
                    TransformerWinding cimTransformerWinding = item.Value as TransformerWinding;

                    ResourceDescription rd = CreateTransformerWindingResourceDescription(cimTransformerWinding);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("TransformerWinding ID = ").Append(cimTransformerWinding.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("TransformerWinding ID = ").Append(cimTransformerWinding.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateTransformerWindingResourceDescription(TransformerWinding cimTransformerWinding)
        {
            ResourceDescription rd = null;
            if (cimTransformerWinding != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.TRANSFORMERWINDING, importHelper.CheckOutIndexForDMSType(DMSType.TRANSFORMERWINDING));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimTransformerWinding.ID, gid);

                SCADAConverter.PopulateTransformerWindingProperties(cimTransformerWinding, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportRatioTapChanger()
        {
            SortedDictionary<string, object> cimRatioTapChangers = concreteModel.GetAllObjectsOfType("FTN.RatioTapChanger");
            if (cimRatioTapChangers != null)
            {
                foreach (KeyValuePair<string, object> item in cimRatioTapChangers)
                {
                    RatioTapChanger cimRatioTapChanger = item.Value as RatioTapChanger;

                    ResourceDescription rd = CreateRatioTapChangerResourceDescription(cimRatioTapChanger);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("RatioTapChanger ID = ").Append(cimRatioTapChanger.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("RatioTapChanger ID = ").Append(cimRatioTapChanger.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateRatioTapChangerResourceDescription(RatioTapChanger cimRatioTapChanger)
        {
            ResourceDescription rd = null;
            if (cimRatioTapChanger != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.RATIOTAPCHANGER, importHelper.CheckOutIndexForDMSType(DMSType.RATIOTAPCHANGER));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimRatioTapChanger.ID, gid);

                SCADAConverter.PopulateRatioTapChangerProperties(cimRatioTapChanger, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportConnectivityNode()
        {
            SortedDictionary<string, object> cimConnectivityNodes = concreteModel.GetAllObjectsOfType("FTN.ConnectivityNode");
            if (cimConnectivityNodes != null)
            {
                foreach (KeyValuePair<string, object> item in cimConnectivityNodes)
                {
                    ConnectivityNode cimConnectivityNode = item.Value as ConnectivityNode;

                    ResourceDescription rd = CreateConnectivityNodeDescription(cimConnectivityNode);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("ConnectivityNode ID = ").Append(cimConnectivityNode.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("ConnectivityNode ID = ").Append(cimConnectivityNode.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateConnectivityNodeDescription(ConnectivityNode cimConnectivityNode)
        {
            ResourceDescription rd = null;
            if (cimConnectivityNode != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.CONNECTIVITYNODE, importHelper.CheckOutIndexForDMSType(DMSType.CONNECTIVITYNODE));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimConnectivityNode.ID, gid);

                SCADAConverter.PopulateConnectivityNodeProperties(cimConnectivityNode, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportTerminal()
        {
            SortedDictionary<string, object> cimTerminals = concreteModel.GetAllObjectsOfType("FTN.Terminal");
            if (cimTerminals != null)
            {
                foreach (KeyValuePair<string, object> item in cimTerminals)
                {
                    Terminal cimTerminal = item.Value as Terminal;

                    ResourceDescription rd = CreateTerminalResourceDescription(cimTerminal);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("Terminal ID = ").Append(cimTerminal.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("Terminal ID = ").Append(cimTerminal.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateTerminalResourceDescription(Terminal cimTerminal)
        {
            ResourceDescription rd = null;
            if (cimTerminal != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.TERMINAL, importHelper.CheckOutIndexForDMSType(DMSType.TERMINAL));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimTerminal.ID, gid);

                SCADAConverter.PopulateTerminalProperties(cimTerminal, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportAnalog()
        {
            SortedDictionary<string, object> cimAnalogs = concreteModel.GetAllObjectsOfType("FTN.Analog");
            if (cimAnalogs != null)
            {
                foreach (KeyValuePair<string, object> item in cimAnalogs)
                {
                    Analog cimAnalog = item.Value as Analog;

                    ResourceDescription rd = CreateAnalogResourceDescription(cimAnalog);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("Analog ID = ").Append(cimAnalog.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("Analog ID = ").Append(cimAnalog.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateAnalogResourceDescription(Analog cimAnalog)
        {
            ResourceDescription rd = null;
            if (cimAnalog != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.ANALOG, importHelper.CheckOutIndexForDMSType(DMSType.ANALOG));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimAnalog.ID, gid);

                SCADAConverter.PopulateAnalogProperties(cimAnalog, rd, importHelper, report);
            }
            return rd;
        }

        private void ImportDiscrete()
        {
            SortedDictionary<string, object> cimDiscretes = concreteModel.GetAllObjectsOfType("FTN.Discrete");
            if (cimDiscretes != null)
            {
                foreach (KeyValuePair<string, object> item in cimDiscretes)
                {
                    Discrete cimDiscrete = item.Value as Discrete;

                    ResourceDescription rd = CreateDiscreteResourceDescription(cimDiscrete);
                    if (rd != null)
                    {
                        delta.AddDeltaOperation(DeltaOpType.Insert, rd, true);
                        report.Report.Append("Discrete ID = ").Append(cimDiscrete.ID).Append(" SUCCESSFULLY converted to GID = ").AppendLine(rd.Id.ToString());
                    }
                    else
                    {
                        report.Report.Append("Discrete ID = ").Append(cimDiscrete.ID).AppendLine(" FAILED to be converted");
                    }
                }
                report.Report.AppendLine();
            }
        }

        private ResourceDescription CreateDiscreteResourceDescription(Discrete cimDiscrete)
        {
            ResourceDescription rd = null;
            if (cimDiscrete != null)
            {
                long gid = ModelCodeHelper.CreateGlobalId(0, (short)DMSType.DISCRETE, importHelper.CheckOutIndexForDMSType(DMSType.DISCRETE));
                rd = new ResourceDescription(gid);
                importHelper.DefineIDMapping(cimDiscrete.ID, gid);

                SCADAConverter.PopulateDiscreteProperties(cimDiscrete, rd, importHelper, report);
            }
            return rd;
        }
    }
}
