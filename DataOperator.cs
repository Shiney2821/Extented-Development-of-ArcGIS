using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;

namespace MapControlApplication1
{
    class DataOperator
    {
        //保存当前地图对象
        public IMap m_map; 
        //传入当前地图对象 
        public DataOperator(IMap map)
        {
            m_map = map; //map相当于一个mxd文件，其中包含许多的shp
        }
        //获取地图图层函数
        public ILayer GetLayerByName(String sLayerName)
        {
            //判断图层名或者图层对象是否为0，如果为0，则返回空值
            if (sLayerName == "" || m_map == null)
            {
                return null;
            }
            //对于选定mxd文件中的不同图层进行遍历，直到名字相同即返回该图层
            for (int i = 0; i < m_map.LayerCount; i++)
            {
                if (m_map.get_Layer(i).Name == sLayerName)
                {
                    return m_map.get_Layer(i);
                }
            }
            return null;
        }
        public DataTable GetContinentsNames(String sLayerName)
        {
            ILayer layer=GetLayerByName(sLayerName);
            //IFeaturelayer进行访问，从而判断是否成功
            IFeatureLayer featureLayer=layer as IFeatureLayer;
            if(featureLayer==null)
            {
                return null;
            }
            //IFeature用于获取要素指针，用于遍历图层中的要素
            IFeature feature;
            IFeatureCursor featureCursor=featureLayer.Search(null,false);
            feature=featureCursor.NextFeature();
            if(feature==null)
            {
                return null;
            }
            //return dataTable
            DataTable dataTable=new DataTable();
            DataRow dataRow;
            for (int i = 0; i < feature.Fields.FieldCount; i++)
            {
                DataColumn dataColumn = new DataColumn();
                dataColumn.ColumnName = feature.Fields.Field[i].Name;
                dataColumn.DataType = (feature.Fields.Field[i].Type!=esriFieldType.esriFieldTypeGeometry)?  feature.get_Value(i).GetType() : System.Type.GetType("System.String"); 
                dataTable.Columns.Add(dataColumn);
            }
            while (feature!= null)
            {
                dataRow = dataTable.NewRow();
                for (int j = 0; j < feature.Fields.FieldCount; j++)
                {
                    dataRow[j] = (feature.Fields.Field[j].Type != esriFieldType.esriFieldTypeGeometry) ? feature.get_Value(j) : feature.Shape.GeometryType.ToString();
                }
               dataTable.Rows.Add(dataRow);
               feature = featureCursor.NextFeature();
            }  
            return dataTable;
        }
        public IFeatureClass CreateShapefile(String sParentDirectory, String sWorkspaceName, String sFileName)
        {
            IWorkspaceFactory workspaceFactory;
            IWorkspace workspace;
            IFeatureWorkspace featureWorkspace;
            IFeatureClass featureClass;
            if (System.IO.Directory.Exists(sParentDirectory + sWorkspaceName))
            {
                workspaceFactory = new ShapefileWorkspaceFactoryClass();
                workspace = workspaceFactory.OpenFromFile(sParentDirectory+sWorkspaceName, 0);
                featureWorkspace = workspace as IFeatureWorkspace;
                featureClass = featureWorkspace.OpenFeatureClass(sFileName);
            }
            else
            {
                workspaceFactory = new ShapefileWorkspaceFactoryClass();
                IWorkspaceName workspaceName = workspaceFactory.Create(sParentDirectory, sWorkspaceName, null, 0);
                ESRI.ArcGIS.esriSystem.IName name = workspaceName as ESRI.ArcGIS.esriSystem.IName;
                workspace = (IWorkspace)name.Open();
                featureWorkspace = workspace as IFeatureWorkspace;

                IFields fields = new FieldsClass();
                IFieldsEdit fieldsEdit = fields as IFieldsEdit;

                IFieldEdit fieldEdit = new FieldClass();
                fieldEdit.Name_2 = "OID";
                fieldEdit.AliasName_2 = "序号";
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeOID;
                fieldsEdit.AddField((IField)fieldEdit);

                fieldEdit = new FieldClass();
                fieldEdit.Name_2 = "Name";
                fieldEdit.AliasName_2 = "名称";
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeString;
                fieldsEdit.AddField((IField)fieldEdit);

                IGeometryDefEdit geoDefEdit = new GeometryDefClass();
                ISpatialReference spatialReference = m_map.SpatialReference;
                geoDefEdit.SpatialReference_2 = spatialReference;
                geoDefEdit.GeometryType_2 = esriGeometryType.esriGeometryPoint;

                fieldEdit = new FieldClass();
                fieldEdit.Name_2 = "Shape";
                fieldEdit.AliasName_2 = "形状";
                fieldEdit.Type_2 = esriFieldType.esriFieldTypeGeometry;
                fieldEdit.GeometryDef_2 = geoDefEdit;
                fieldsEdit.AddField((IField)fieldEdit);

                featureClass = featureWorkspace.CreateFeatureClass(sFileName, fields, null, null, esriFeatureType.esriFTSimple, "Shape", "");
                if (featureClass == null)
                {
                    return null;
                }
            }
            return featureClass;
        }
        public bool AddFeatureClassToMap(IFeatureClass featureClass, String sLayerName)
        {
            if (featureClass == null || sLayerName == "" || m_map == null)
            {
                return false;
            }
            IFeatureLayer featureLayer = new FeatureLayerClass();
            featureLayer.FeatureClass = featureClass;
            featureLayer.Name = sLayerName;

            ILayer layer = featureLayer as ILayer;
            if (layer == null)
            {
                return false;
            }
            m_map.AddLayer(layer);
            IActiveView activeView = m_map as IActiveView;
            if (activeView == null)
            {
                return false;
            }
            activeView.Refresh();
            return true;
        }
        public bool AddFeaturePointToLayer(String sLayerName, String sFeatureName, IPoint point)
        {
            if (sLayerName == "" || sFeatureName == "" || point == null || m_map == null)
            {
                return false;
            }
            ILayer layer = null;
            for (int i = 0; i < m_map.LayerCount; i++)
            {
                layer = m_map.get_Layer(i);
                if (layer.Name == sLayerName)
                {
                    break;
                }
                layer = null;
            }
            if (layer == null)
            {
                return false;
            }

            IFeatureLayer featureLayer = layer as IFeatureLayer;
            IFeatureClass featureClass = featureLayer.FeatureClass;
            IFeature feature = featureClass.CreateFeature();
            if (feature == null)
            {
                return false;
            }

            feature.Shape = point;
            int index = feature.Fields.FindField("Name");
            feature.set_Value(index, sFeatureName);
            feature.Store();
            if (feature == null)
            {
                return false;
            }
            IActiveView activeView = m_map as IActiveView;
            if (activeView == null)
            {
                return false;
            }
            activeView.Refresh();
            return true;
        }
        public bool AddFeaturePolylineToLayer(String sLayerName, String sFeatureName, IPolyline polyline)
        {
            if (sLayerName == "" || sFeatureName == "" || polyline == null || m_map == null)
            {
                return false;
            }
            ILayer layer = null;
            for (int i = 0; i < m_map.LayerCount; i++)
            {
                layer = m_map.get_Layer(i);
                if (layer.Name == sLayerName)
                {
                    break;
                }
                layer = null;
            }
            if (layer == null)
            {
                return false;
            }

            IFeatureLayer featureLayer = layer as IFeatureLayer;
            IFeatureClass featureClass = featureLayer.FeatureClass;
            IFeature feature = featureClass.CreateFeature();
            if (feature == null)
            {
                return false;
            }

            feature.Shape = polyline;
            int index = feature.Fields.FindField("Name");
            feature.set_Value(index, sFeatureName);
            feature.Store();
            if (feature == null)
            {
                return false;
            }
            IActiveView activeView = m_map as IActiveView;
            if (activeView == null)
            {
                return false;
            }
            activeView.Refresh();
            return true;
        }
        public bool AddFeaturePolygonToLayer(String sLayerName, String sFeatureName, IPolygon polygon)
        {
            if (sLayerName == "" || sFeatureName == "" || polygon == null || m_map == null)
            {
                return false;
            }
            ILayer layer = null;
            for (int i = 0; i < m_map.LayerCount; i++)
            {
                layer = m_map.get_Layer(i);
                if (layer.Name == sLayerName)
                {
                    break;
                }
                layer = null;
            }
            if (layer == null)
            {
                return false;
            }

            IFeatureLayer featureLayer = layer as IFeatureLayer;
            IFeatureClass featureClass = featureLayer.FeatureClass;
            IFeature feature = featureClass.CreateFeature();
            if (feature == null)
            {
                return false;
            }

            feature.Shape = polygon;
            int index = feature.Fields.FindField("Name");
            feature.set_Value(index, sFeatureName);
            feature.Store();
            if (feature == null)
            {
                return false;
            }
            IActiveView activeView = m_map as IActiveView;
            if (activeView == null)
            {
                return false;
            }
            activeView.Refresh();
            return true;
        }
        
    }
}

