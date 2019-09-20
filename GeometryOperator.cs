using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using ESRI.ArcGIS.Geometry;
using ESRI.ArcGIS.esriSystem;
using ESRI.ArcGIS.Carto;
using ESRI.ArcGIS.Controls;
using ESRI.ArcGIS.ADF;
using ESRI.ArcGIS.SystemUI;
using ESRI.ArcGIS.Display;
using ESRI.ArcGIS.Output;
using ESRI.ArcGIS.Geodatabase;
using ESRI.ArcGIS.DataSourcesFile;
using ESRI.ArcGIS.DataSourcesGDB;
using ESRI.ArcGIS.Analyst3D;
using CSProject;

namespace MapControlApplication1
{
    public class GeometryOperator
    {
       //===成员变量的声明================
       public IMap m_map;
       public MainForm m_form; 
       IPointCollection pointCollection;
       IGraphicsContainer pGraphicsContainer;
        
        //传入当前地图对象 
        public GeometryOperator(IMap map,MainForm form)
        {
            m_map = map; //map相当于一个mxd文件，其中包含许多的shp
            m_form = form;
            
        }
      
        public void ShowGeometry(IGeometry geometry)
        {
           //强制转换，将Geometry对象转化为pointCollection
            pointCollection = (Polygon)geometry;
            DataTable dataTable = new DataTable();
            DataColumn dataColumn = new DataColumn();

            //===创建一个有三列的表格====================
            dataColumn = new DataColumn();
            dataColumn.ColumnName = "number";//设置第一列，表示用户指定的列名
            dataColumn.DataType = System.Type.GetType("System.String");
            dataColumn.ReadOnly = true; //第一列无法修改
            dataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "X";//设置第二列为在目标图层类作为分类标准的属性
            dataColumn.DataType = System.Type.GetType("System.String");
            dataTable.Columns.Add(dataColumn);

            dataColumn = new DataColumn();
            dataColumn.ColumnName = "Y";//设置第三列为在目标图层类作为分类标准的属性
            dataColumn.DataType = System.Type.GetType("System.String");
            dataTable.Columns.Add(dataColumn);

            //提供一个图形的容器
            pGraphicsContainer = m_map as IGraphicsContainer;//只在本函数
            DataRow dataRow;
            for (int i = 0; i < pointCollection.PointCount; i++)
            {
                dataRow = dataTable.NewRow();
                dataRow[0] = i+1;
                dataRow[1] = Math.Round(pointCollection.Point[i].X,3);
                dataRow[2] = Math.Round(pointCollection.Point[i].Y,3);//将统计结果添加到第三列
                dataTable.Rows.Add(dataRow);
                /*IElement pElement = new MarkerElementClass();
                pElement.Geometry = pointCollection.Point[i];
                pGraphicsContainer.AddElement(pElement, 0);//0 represents z-order */

                //确定Element对象，并将对象加载到pGraphicsContainer中
                IMarkerSymbol markerSymbol = new SimpleMarker3DSymbolClass();
                //markerSymbol的类型和分辨率
                ((ISimpleMarker3DSymbol)markerSymbol).Style = esriSimple3DMarkerStyle.esriS3DMSSphere;
                ((ISimpleMarker3DSymbol)markerSymbol).ResolutionQuality = 1.0;

                //markerSymbol的颜色和大小
                IRgbColor color = new RgbColorClass();
                color.Red = 255;
                color.Green = 0;
                color.Blue = 0;

                markerSymbol.Size = 5;
                markerSymbol.Color = color as IColor;

                IElement pElement = new MarkerElementClass();
                ((IMarkerElement)pElement).Symbol = markerSymbol;
                pElement.Geometry = pointCollection.Point[i];
                pGraphicsContainer.AddElement(pElement, 0);
               
            }
            DataBoard dataBoard = new DataBoard("Position", dataTable,this,pointCollection.PointCount);
            dataBoard.Show();
            m_form.setButton_miShowTime(false);

        }
        public void HighLight(int row)
        {
            DataOperator dataoperator = new DataOperator(m_map);
            IMarkerSymbol markerSymbol = new SimpleMarker3DSymbolClass();

            //markerSymbol的类型和分辨率
            ((ISimpleMarker3DSymbol)markerSymbol).Style = esriSimple3DMarkerStyle.esriS3DMSSphere; 
            ((ISimpleMarker3DSymbol)markerSymbol).ResolutionQuality = 1.0;

            //markerSymbol的颜色和大小
            IRgbColor color = new RgbColorClass();
            color.Red = 250;
            color.Green = 230;
            color.Blue = 20;

            markerSymbol.Size = 8;
            markerSymbol.Color = color as IColor;

            // 将showElement对象赋予markerSymbol的属性
            IElement showElement = new MarkerElementClass();
            ((IMarkerElement)showElement).Symbol = markerSymbol;
            showElement.Geometry = pointCollection.Point[row];

            //在绘制前，清楚图像容器内的所有Elements
            pGraphicsContainer.DeleteAllElements();//通过name删除

            //为了使点在上面，先显示polygon
            IGeometry polygon = pointCollection as IGeometry;
            IPolygonElement polygonElement = new PolygonElementClass();
            IElement polyElement = polygonElement as IElement;
            polyElement.Geometry = polygon;
            pGraphicsContainer.AddElement((IElement)polygonElement, 0);
            //绘制高亮显示的点
            pGraphicsContainer.AddElement((IElement)showElement, 0);
            //重新绘制除了高亮点外的构造点
            for (int i = 0; (i < pointCollection.PointCount); i++)
            {
                if (i != row)
                {
                    IRgbColor color3 = new RgbColorClass();      //如果出现重复 将函数独立出来
                    color3.Red = 255;
                    color3.Green = 0;
                    color3.Blue = 0;

                    markerSymbol.Size = 5;         
                    markerSymbol.Color = color3 ;
                    IElement pElement = new MarkerElementClass();
                    ((IMarkerElement)pElement).Symbol = markerSymbol;
                    pElement.Geometry = pointCollection.Point[i];
                    pGraphicsContainer.AddElement(pElement, 0);
                    /*IElement pElement=new MarkerElementClass();
                    pElement.Geometry = pointCollection.Point[i];
                    pGraphicsContainer.AddElement(pElement, 0);*/
                }   
            }
            m_form.axMapControl1.ActiveView.Refresh();//partialrefresh
        }

        public void Update(double newvalue, int row, int column)//x,y一起修改
        {
            
            IPoint ipoint = (IPoint)pointCollection.Point[row];
            switch (column)
            {
                case 0:
                    break;
                case 1:
                    ipoint.X = newvalue;
                    break;
                case 2:
                    ipoint.Y = newvalue;
                    break;
            }
            //对于pointCollection进行修改
            pointCollection.UpdatePoint(row, ipoint);
            //清空图像容器
            pGraphicsContainer.DeleteAllElements();

            IGeometry polygon = pointCollection as IGeometry;
            IPolygonElement polygonElement = new PolygonElementClass();
            IElement polyElement = polygonElement as IElement;
            polyElement.Geometry = polygon;
            pGraphicsContainer.AddElement((IElement)polygonElement, 0);
            m_form.axMapControl1.ActiveView.Refresh();

            for (int i = 0; i < pointCollection.PointCount; i++)
            {
                IMarkerSymbol markerSymbol = new SimpleMarker3DSymbolClass();
                //markerSymbol的类型和分辨率
                ((ISimpleMarker3DSymbol)markerSymbol).Style = esriSimple3DMarkerStyle.esriS3DMSSphere;
                ((ISimpleMarker3DSymbol)markerSymbol).ResolutionQuality = 1.0;

                //markerSymbol的颜色和大小
                IRgbColor color = new RgbColorClass();
                color.Red = 255;
                color.Green = 0;
                color.Blue = 0;

                markerSymbol.Size = 5;
                markerSymbol.Color = color as IColor;

                IElement pElement = new MarkerElementClass();
                ((IMarkerElement)pElement).Symbol = markerSymbol;
                pElement.Geometry = pointCollection.Point[i];
                pGraphicsContainer.AddElement(pElement, 0);
                /*IElement pElement=new MarkerElementClass();
                pElement.Geometry = pointCollection.Point[i];
                pGraphicsContainer.AddElement(pElement, 0);*/
            }
            m_form.axMapControl1.ActiveView.Refresh();
        }

        public void EndShow()
        {
            pGraphicsContainer.DeleteAllElements();

            IGeometry polygon = pointCollection as IGeometry;
            IPolygonElement polygonElement = new PolygonElementClass();
            IElement polyElement = polygonElement as IElement;
            polyElement.Geometry = polygon;
            pGraphicsContainer.AddElement((IElement)polygonElement, 0);
            m_form.axMapControl1.ActiveView.Refresh();
            
        }
    }
}
