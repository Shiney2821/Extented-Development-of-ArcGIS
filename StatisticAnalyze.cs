using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Data;


using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Geoprocessor;
using ESRI.ArcGIS.AnalysisTools;

namespace CSProject
{
    class StatisticAnalyze
    {


        public bool cilpOperator(IMap iMap, string inLayerName, string clipLayerName, string outpath)
        {
 
            IFeatureLayer inLayer = (IFeatureLayer)GetLayerByName(inLayerName);
            IFeatureLayer clipLayer = (IFeatureLayer)GetLayerByName(clipLayerName);

            //调用GeoProcessing工具
            Geoprocessor geoprocessor = new Geoprocessor();
            geoprocessor.OverwriteOutput = true;

            IFeatureClass inLayerFeaCls = inLayer.FeatureClass;
            IFeatureClass clipLayerFeaCls = clipLayer.FeatureClass;

            //调用clip工具
            ESRI.ArcGIS.AnalysisTools.Clip clip = new ESRI.ArcGIS.AnalysisTools.Clip(
                inLayerFeaCls, clipLayerFeaCls, outpath + "\\" + inLayerName + "_Clip");

            geoprocessor.Execute(clip, null);

            //输出切割后图层
            IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactoryClass();
            IFeatureWorkspace featureWorkspace = (IFeatureWorkspace)workspaceFactory.OpenFromFile(outpath, 0);
            IFeatureClass outFClass = featureWorkspace.OpenFeatureClass(inLayerName + "_Clip");
            IFeatureLayer outLayer = new FeatureLayerClass();
            outLayer.FeatureClass = outFClass;
            outLayer.Name = inLayerName + "_Clip";
            iMap.AddLayer(outLayer);

            IActiveView activeView = iMap as IActiveView;
            activeView.Refresh();

            if (inLayerName == "DLTB")
            {
                int raIndex = outFClass.FindField("SHAPE_Area");
                IFeatureCursor featureCursor = outLayer.FeatureClass.Search(null, false);
                IFeature feature = featureCursor.NextFeature();

                while (feature != null)
                {
                    if (feature.Shape.GeometryType == esriGeometryType.esriGeometryPolygon)
                    {
                        //使用IArea接口计算面积
                        IArea pArea = feature.Shape as IArea;
                        feature.set_Value(raIndex, pArea.Area);
                    }
                    feature.Store();
                    feature = featureCursor.NextFeature();
                }
            }
            else if (inLayerName == "XZDW")
            {
                //计算切割后图层中线状地物的长度
                IFieldsEdit pFieldsEdit = outFClass.Fields as IFieldsEdit;
                IField pField = new FieldClass();
                IFieldEdit pFieldEdit = pField as IFieldEdit;
                pFieldEdit.Name_2 = "Real_Leng";
                pFieldEdit.Type_2 = esriFieldType.esriFieldTypeDouble;
                outFClass.AddField(pField);

                int rlIndex = outFClass.FindField("Real_Leng");
                int slIndex = outFClass.FindField("SHAPE_Leng");
                int xzdwmjIndex = outFClass.FindField("XZDWMJ");
                IFeatureCursor featureCursor = outLayer.FeatureClass.Search(null, false);
                IFeature feature = featureCursor.NextFeature();

                while (feature != null)
                {
                    if (feature.Shape.GeometryType == esriGeometryType.esriGeometryPolyline)
                    {
                        //使用ICure接口计算长度
                        ICurve pCurve = feature.Shape as ICurve;
                        feature.set_Value(rlIndex, pCurve.Length);
                        //计算切割后线状地物面积
                        Double real_mj = Double.Parse(feature.get_Value(rlIndex).ToString()) / Double.Parse(feature.get_Value(slIndex).ToString())
                            * Double.Parse(feature.get_Value(xzdwmjIndex).ToString());
                        feature.set_Value(xzdwmjIndex, real_mj.ToString());
                    }
                    feature.Store();
                    feature = featureCursor.NextFeature();
                }
            }

            return true;
        }//切割

        public bool DLTBquitXZandLX(string DLTB, string XZDW, string LXDW)
        {

            IFeature DLTBfeature;
            IFeature XZDWfeature;
            IFeature LXDWfeature;
            IGeometry DLTBGeom;

            /*****************************************************************/
            IFeatureLayer DLTBLayer = (IFeatureLayer)GetLayerByName(DLTB);
            IFeatureLayer XZDWLayer = (IFeatureLayer)GetLayerByName(XZDW);
            IFeatureLayer LXDWLayer = (IFeatureLayer)GetLayerByName(LXDW);

            /*****************************************************************/

            //将DLTB.shp文件中的XZDWMJ，LXDWMJ设置为0
            IFeatureCursor DLTBSetCursor = DLTBLayer.Search(null, false);
            DLTBfeature = DLTBSetCursor.NextFeature();
            if (DLTBfeature == null)
                return false;
            while (DLTBfeature != null)
            {
                DLTBfeature.set_Value(19, 0);
                DLTBfeature.set_Value(20, 0);
                DLTBfeature.Store();
                DLTBfeature = DLTBSetCursor.NextFeature();
            }

            /****************************************************************************************************************************
             *****************************遍历每一个DLTB的要素****************************************************************************/

            IFeatureCursor PolygonCursor = DLTBLayer.Search(null, false);
            DLTBfeature = PolygonCursor.NextFeature();
            while (DLTBfeature != null)
            {
                DLTBGeom = DLTBfeature.Shape;


                /*****************************************************************/



                //定义一个空间过滤器，选出在地类图斑内的零星地物
                ISpatialFilter spatialFilterForPoint = new SpatialFilter();
                spatialFilterForPoint.Geometry = DLTBGeom;
                spatialFilterForPoint.SpatialRel = esriSpatialRelEnum.esriSpatialRelContains;

              
                IFeatureCursor PolyLineCursor1 = XZDWLayer.Search(null, false);
                XZDWfeature = PolyLineCursor1.NextFeature();

                while (XZDWfeature != null)
                {

                    ITopologicalOperator ipTO = (ITopologicalOperator)DLTBGeom;
                    IGeometry iGeomBuffer = ipTO.Buffer(0.05);

                    IRelationalOperator pRelOpt = (IRelationalOperator)iGeomBuffer;

                    if (pRelOpt.Contains(XZDWfeature.Shape))
                    {
                        {
                            double KCBL = (double)XZDWfeature.get_Value(21);
                            double Value = (double)XZDWfeature.get_Value(10);
                            double AddValue = KCBL * Value;
                            double NowValue = (double)DLTBfeature.get_Value(19);
                            DLTBfeature.set_Value(19, AddValue + NowValue);
                            DLTBfeature.Store();

                        }
                    }

                    XZDWfeature = PolyLineCursor1.NextFeature();

                }
                /*********************找出DLTB,LXDW符合Contain，然后更新DLTB的LXDWMJ的值********************************************/

                IFeatureCursor PointCursor = LXDWLayer.Search(spatialFilterForPoint, false);
                LXDWfeature = PointCursor.NextFeature();

                while (LXDWfeature != null)
                {
                    double AddValue = (double)LXDWfeature.get_Value(4);
                    double NowValue = (double)DLTBfeature.get_Value(20);
                    DLTBfeature.set_Value(20, AddValue + NowValue);
                    DLTBfeature.Store();
                    LXDWfeature = PointCursor.NextFeature();
                }

                DLTBfeature = PolygonCursor.NextFeature();

            }

            MessageBox.Show("Analyze complete!");
            return true;
        }//扣除面积计算

        public DataTable GetSt()
        {

            IFeatureLayer DLTBfeatLayer = (IFeatureLayer)GetLayerByName("DLTB_Clip");
            IFeatureLayer LXDWfeatLayer = (IFeatureLayer)GetLayerByName("LXDW_Clip");
            IFeatureLayer XZDWfeatLayer = (IFeatureLayer)GetLayerByName("XZDW_Clip");
           
            string[] a;
            a = new string[10000];
            string[] c;
            c = new string[10000];
            double[] b;
            b = new double[10000];

            DataTable dataTable = new DataTable();

            DataColumn dataColumn = new DataColumn();
            dataColumn.ColumnName = "地类编号";
            dataColumn.DataType = System.Type.GetType("System.String");
            dataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "地类名称";
            dataColumn.DataType = System.Type.GetType("System.String");
            dataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "面积";
            dataColumn.DataType = System.Type.GetType("System.Double");
            dataTable.Columns.Add(dataColumn);


            DataRow dataRow;

            IFeature feature1;
            IFeatureCursor featureCursor1 = DLTBfeatLayer.Search(null, false);
            feature1 = featureCursor1.NextFeature();

            IFeature feature2;
            IFeatureCursor featureCursor2 = LXDWfeatLayer.Search(null, false);
            feature2 = featureCursor2.NextFeature();

            IFeature feature3;
            IFeatureCursor featureCursor3 = XZDWfeatLayer.Search(null, false);
            feature3 = featureCursor3.NextFeature();


            int i;
            int k = 0;
            int flag = 0;
            int j = 0;

            if (feature1 != null)
            {
                a[0] = feature1.get_Value(6).ToString();
                c[0] = feature1.get_Value(7).ToString();
                b[0] = 0;
            }

            while (feature1 != null)
            {
                for (k = 0; k <= j; k++)
                {
                    if (feature1.get_Value(6).ToString() == a[k])
                    {
                        b[k] = b[k] + (Convert.ToDouble(feature1.get_Value(18))
                            - Convert.ToDouble(feature1.get_Value(19))
                            - Convert.ToDouble(feature1.get_Value(20)));
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    j++;
                    a[j] = feature1.get_Value(6).ToString();
                    c[j] = feature1.get_Value(7).ToString();
                    b[j] = Double.Parse(feature1.get_Value(18).ToString());
                }
                flag = 0;
                feature1 = featureCursor1.NextFeature();

            }

            while (feature2 != null)
            {
                for (k = 0; k <= j; k++)
                {
                    if (feature2.get_Value(5).ToString() == a[k])
                    {
                        b[k] = b[k] + Double.Parse(feature2.get_Value(4).ToString());
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    j++;
                    a[j] = feature2.get_Value(5).ToString();
                    c[j] = feature2.get_Value(6).ToString();
                    b[j] = Double.Parse(feature2.get_Value(4).ToString());
                }
                flag = 0;
                feature2 = featureCursor2.NextFeature();

            }

            while (feature3 != null)
            {
                for (k = 0; k <= j; k++)
                {
                    if (feature3.get_Value(4).ToString() == a[k])
                    {
                        b[k] = b[k] + Double.Parse(feature3.get_Value(10).ToString());
                        flag = 1;
                        break;
                    }
                }
                if (flag == 0)
                {
                    j++;
                    a[j] = feature3.get_Value(4).ToString();
                    c[j] = feature3.get_Value(5).ToString();
                    b[j] = Double.Parse(feature3.get_Value(10).ToString());
                }
                flag = 0;
                feature3 = featureCursor3.NextFeature();

            }

            for (i = 0; i <= j; i++)
            {
                dataRow = dataTable.NewRow();
                dataRow[0] = a[i];
                dataRow[1] = c[i];
                dataRow[2] = b[i];
                dataTable.Rows.Add(dataRow);
            }

            return dataTable;

        }//DLBM统计




        public IMap m_map;

        public StatisticAnalyze(IMap map)
        {
            if (null != map)
                m_map = map;
        }

        public ILayer GetLayerByName(string sLayerName)
        {
            if (sLayerName == " ")
                return null;
            if (m_map == null)//判断图层名或地理对象是否为空
            {
                return null;
            }
            for (int i = 0; i < m_map.LayerCount; i++)
            {
                if (m_map.get_Layer(i).Name == sLayerName)
                {
                    return m_map.get_Layer(i);
                }
            }
            return null;
        }

        public IFeatureClass CreateShapefile(string sParentDirectory, string sWorkspaceName,string sFileName)
        {
            if (System.IO.Directory.Exists(sParentDirectory + sWorkspaceName))
                System.IO.Directory.Delete(sParentDirectory + sWorkspaceName, true);

            IWorkspaceFactory workspaceFactory = new ShapefileWorkspaceFactoryClass();
            IWorkspaceName workspaceName = workspaceFactory.Create(sParentDirectory, sWorkspaceName, null, 0);
            ESRI.ArcGIS.esriSystem.IName name = workspaceName as ESRI.ArcGIS.esriSystem.IName;
            IWorkspace workspace = (IWorkspace)name.Open();



            IFeatureWorkspace featureWorkspace = workspace as IFeatureWorkspace;
            IFields fields = new FieldsClass();
            IFieldsEdit fieldsEdit = fields as IFieldsEdit;
            IFieldEdit fieldEdit = new FieldClass();
            fieldEdit.Name_2 = "OID";
            fieldEdit.AliasName_2 = "序号";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;
            fieldsEdit.AddField((IField)fieldEdit);


            IGeometryDefEdit geoDefEdit = new GeometryDefClass();
            ISpatialReference spatialReference = m_map.SpatialReference;
            geoDefEdit.SpatialReference_2 = spatialReference;
            geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPolygon;

            fieldEdit = new FieldClass();
            string sShapeFieldName = "Shape";
            fieldEdit.Name_2 = sShapeFieldName;
            fieldEdit.AliasName_2 = "形状";
            fieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
            fieldEdit.GeometryDef_2 = geoDefEdit;
            fieldsEdit.AddField((IField)fieldEdit);

            IFeatureClass featureClass =
                featureWorkspace.CreateFeatureClass(sFileName, fields, null, null, esriFeatureType.esriFTSimple, "Shape", "");
            if (featureClass == null) { return null; }

            return featureClass;

        }

        public bool AddFeatureClassToMap(IFeatureClass featureClass,String sLayerName)
        {
            if (featureClass == null || sLayerName == "" || m_map == null)
                return false;

            IFeatureLayer featureLayer = new FeatureLayerClass();
            featureLayer.FeatureClass = featureClass;
            featureLayer.Name = sLayerName;

            ILayer layer = featureLayer as ILayer;
            if (layer == null)
                return false;

            m_map.AddLayer(layer);
            IActiveView activeView = m_map as IActiveView;
            if (activeView == null)
                return false;
            activeView.Refresh();
            return true;
        }


        public bool AddPolygonToLayer(IPolygon m_polygon)
        {
            IFeatureLayer featLayer = (IFeatureLayer)GetLayerByName("Clip_Polygon");
            if (featLayer == null)
                return false;
            IFeatureClass featureClass = featLayer.FeatureClass;
            IFeature feature = featureClass.CreateFeature();
            if (feature == null)
                return false;
            feature.Shape = m_polygon;

            int index = featureClass.FindField("Shape");
            feature.set_Value(index, (IGeometry)m_polygon);
            feature.Store();
            if (feature == null)
                return false;

            IActiveView ActiveView = m_map as IActiveView;
            if (ActiveView == null)
                return false;
            ActiveView.Refresh();
            return true;

        }


        public void DeleteByName(IFeatureWorkspace pFeaWorkspace, IDatasetName pDatasetName)
        {
            IFeatureWorkspaceManage pWorkspaceManager = pFeaWorkspace as IFeatureWorkspaceManage;
            pWorkspaceManager.DeleteByName(pDatasetName);
        }
        public bool DeleteFeatureDataset(String sParentDirectory, String sWorkspaceName, String sFileName)
        {
            IWorkspaceFactory workspaceFactory;
            IWorkspace workspace;
          

            workspaceFactory = new ShapefileWorkspaceFactoryClass();
            workspace = workspaceFactory.OpenFromFile(sParentDirectory + sWorkspaceName, 0);

            if (workspace == null || string.IsNullOrEmpty(sFileName))
            {
                MessageBox.Show("工作空间或要素类名称不能为空!");

                return false;
            }
            IFeatureDataset pFeaDataSet;
            IEnumDatasetName pEnumDatasetName;
            IFeatureWorkspace pFeaWorkspace;
            IDatasetName pDatasetName;
            try
            {
                pFeaWorkspace = workspace as IFeatureWorkspace;
                pEnumDatasetName = workspace.get_DatasetNames(esriDatasetType.esriDTFeatureClass ^ esriDatasetType.esriDTFeatureDataset);
                pEnumDatasetName.Reset();
                pDatasetName = pEnumDatasetName.Next();
                while (pDatasetName != null)
                {
                    if (pDatasetName.Type == esriDatasetType.esriDTFeatureDataset)
                    {
                        //如果是要素集，则对要素集内的要素类进行查找  
                        IEnumDatasetName pEnumFcName = (pDatasetName as IFeatureDatasetName).FeatureClassNames;
                        IDatasetName pFcName = pEnumFcName.Next();
                        while (pFcName != null)
                        {
                            if (pFcName.Name.IndexOf(sFileName) >= 0)
                            {
                                DeleteByName(pFeaWorkspace, pFcName);
                                return true;
                            }
                            pFcName = pEnumFcName.Next();
                        }
                    }
                    else
                    {
                        if (pDatasetName.Name.IndexOf(sFileName) >= 0)
                        {
                            DeleteByName(pFeaWorkspace, pDatasetName);
                            return true;
                        }
                    }
                    pDatasetName = pEnumDatasetName.Next();
                }
                return false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }  
    }
}
